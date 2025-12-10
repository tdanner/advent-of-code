use std::{cmp::max, cmp::min, u64};

advent_of_code::solution!(9);

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord, Copy, Clone)]
struct Point {
    x: u64,
    y: u64,
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord, Copy, Clone)]
struct Edge {
    p1: Point,
    p2: Point,
}

#[derive(Debug, PartialEq, Eq, PartialOrd, Ord, Copy, Clone)]
enum Orientation {
    Horizontal,
    Vertical,
}

impl Edge {
    fn new(p1: &Point, p2: &Point) -> Self {
        if p1.x == p2.x {
            if p1.y < p2.y {
                Self { p1: *p1, p2: *p2 }
            } else {
                Self { p1: *p2, p2: *p1 }
            }
        } else {
            if p1.x < p2.x {
                Self { p1: *p1, p2: *p2 }
            } else {
                Self { p1: *p2, p2: *p1 }
            }
        }
    }

    fn orientation(&self) -> Orientation {
        if self.p1.x == self.p2.x {
            Orientation::Vertical
        } else {
            Orientation::Horizontal
        }
    }

    fn crosses_rect(&self, xmin: u64, ymin: u64, xmax: u64, ymax: u64) -> bool {
        match self.orientation() {
            Orientation::Vertical => {
                self.p1.x > xmin && self.p1.x < xmax && self.p1.y < ymax && self.p2.y > ymin
            }
            Orientation::Horizontal => {
                self.p1.y > ymin && self.p1.y < ymax && self.p1.x < xmax && self.p2.x > xmin
            }
        }
    }

    fn intersects(&self, other: &Edge) -> bool {
        match (self.orientation(), other.orientation()) {
            (Orientation::Horizontal, Orientation::Horizontal) => {
                (self.p1.y == other.p1.y)
                    && ((self.p1.x < other.p1.x && self.p2.x > other.p1.x)
                        || (other.p1.x < self.p1.x && other.p2.x > self.p1.x))
            }

            (Orientation::Horizontal, Orientation::Vertical) => {
                self.p1.x < other.p1.x
                    && self.p2.x > other.p1.x
                    && other.p1.y < self.p1.y
                    && other.p2.y > self.p1.y
            }

            (Orientation::Vertical, Orientation::Horizontal) => {
                self.p1.y < other.p1.y
                    && self.p2.y > other.p1.y
                    && other.p1.x < self.p1.x
                    && other.p2.x > self.p1.x
            }

            (Orientation::Vertical, Orientation::Vertical) => {
                (self.p1.x == other.p1.x)
                    && ((self.p1.y < other.p1.y && self.p2.y > other.p1.y)
                        || (other.p1.y < self.p1.y && other.p2.y > self.p1.y))
            }
        }
    }
}

fn parse(input: &str) -> Vec<Point> {
    input
        .lines()
        .map(|line| {
            let (x, y) = line.split_once(',').unwrap();
            Point {
                x: x.parse().unwrap(),
                y: y.parse().unwrap(),
            }
        })
        .collect()
}

fn area(p1: &Point, p2: &Point) -> u64 {
    (p1.x.abs_diff(p2.x) + 1) * (p1.y.abs_diff(p2.y) + 1)
}

fn point_inside(p: &Point, verts: &Vec<Edge>) -> bool {
    // we are going to cast a ray from p to (0, p.y) and count intersections
    let p2 = Point { x: 0, y: p.y };
    let ray = Edge::new(p, &p2);
    let intersections: Vec<Edge> = verts
        .iter()
        .filter(|e| e.intersects(&ray))
        .copied()
        .collect();
    // println!("ray {ray:?} has intersections {intersections:?}");
    let inside = !intersections.len().is_multiple_of(2);
    // let label = if inside { "inside" } else { "outside" };
    // println!("{p:?} is {label}");
    inside
}

pub fn part_one(input: &str) -> Option<u64> {
    let points = parse(input);

    let mut max_area: u64 = 0;
    for p1 in points.iter() {
        for p2 in points.iter() {
            max_area = max(max_area, area(p1, p2))
        }
    }

    Some(max_area)
}

pub fn part_two(input: &str) -> Option<u64> {
    let points = parse(input);

    // make a list of horizontal and vertical edges
    let edges: Vec<Edge> = points
        .iter()
        .enumerate()
        .map(|(i, p1)| Edge::new(p1, &points[(i + 1) % points.len()]))
        .collect();
    let (verticals, _horizontals): (Vec<Edge>, Vec<Edge>) = edges
        .iter()
        .partition(|e| e.orientation() == Orientation::Vertical);

    // To find the largest rectangle that is inside the polygon, we are going to
    // consider all rectangles formed by pairs of vertices, checking whether the
    // center is inside the polygon and no edge of the polygon crosses the
    // interior of the rectangle.
    let mut max_area: u64 = 0;
    // let p1 = &Point { x: 9, y: 5 };
    // let p2 = &Point { x: 2, y: 3 };
    for p1 in points.iter() {
        for p2 in points.iter() {
            let a = area(p1, p2);
            if a > max_area {
                // no point in testing a rectangle that isn't bigger
                let (xmin, xmax) = (min(p1.x, p2.x), max(p1.x, p2.x));
                let (ymin, ymax) = (min(p1.y, p2.y), max(p1.y, p2.y));
                let center = Point {
                    x: (p1.x + p2.x) / 2,
                    y: (p1.y + p2.y) / 2,
                };
                if point_inside(&center, &verticals)
                    && !edges.iter().any(|e| e.crosses_rect(xmin, ymin, xmax, ymax))
                {
                    max_area = a;
                }
            }
        }
    }

    Some(max_area)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(50));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(24));
    }
}
