use std::{cmp::max, u64};

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
                Self { p2: *p2, p1: *p1 }
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
    // we are going to cast a ray from p to (+Inf, p.y) and count intersections
    let p2 = Point {
        x: u64::MAX,
        y: p.y,
    };
    let ray = Edge::new(p, &p2);
    let intersections: Vec<Edge> = verts
        .iter()
        .filter(|e| e.intersects(&ray))
        .copied()
        .collect();
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
    let edges = points
        .iter()
        .enumerate()
        .map(|(i, p1)| Edge::new(p1, &points[(i + 1) % points.len()]));
    let (verticals, horizontals): (Vec<Edge>, Vec<Edge>) =
        edges.partition(|e| e.orientation() == Orientation::Vertical);

    // test that the polygon is regular (not self-intersecting)
    for hor in horizontals.iter() {
        for vert in verticals.iter() {
            if hor.intersects(vert) {
                panic!("polygon intersects self: {hor:?} crosses {vert:?}");
            }
        }
    }

    // To find the largest rectangle that is inside the polygon, we are going to
    // consider all rectangles formed by pairs of vertices, checking whether the
    // opposite corners are also inside the polygon. Basically the same
    // algorithm from part 1, but with the "point_inside" check.
    let mut max_area: u64 = 0;
    for p1 in points.iter() {
        for p2 in points.iter() {
            let p3 = Point { x: p1.x, y: p2.y };
            let p4 = Point { x: p2.x, y: p1.y };
            if point_inside(&p3, &verticals) && point_inside(&p4, &verticals) {
                // max_area = max(max_area, area(p1, p2));
                let a = area(p1, p2);
                if a > max_area {
                    // println!("rect {p1:?} / {p2:?} has area {a}");
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

    #[test]
    fn test_point_inside() {
        let input = &advent_of_code::template::read_file("examples", DAY);
        let points = parse(input);

        // make a list of horizontal and vertical edges
        let edges = points
            .iter()
            .enumerate()
            .map(|(i, p1)| Edge::new(p1, &points[(i + 1) % points.len()]));
        let (verticals, _horizontals): (Vec<Edge>, Vec<Edge>) =
            edges.partition(|e| e.orientation() == Orientation::Vertical);

        let result = point_inside(&Point { x: 2, y: 5 }, &verticals);
        assert_eq!(result, true);
    }
}
