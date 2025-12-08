use std::collections::HashSet;
use std::fmt::{Debug, Display, Formatter};

advent_of_code::solution!(8);

#[derive(Clone, Copy, PartialEq, Eq, Hash)]
struct Point3 {
    x: u64,
    y: u64,
    z: u64,
}

fn diff_squared(a: u64, b: u64) -> f64 {
    let diff = a as f64 - b as f64;
    diff * diff
}

impl Point3 {
    fn dist(&self, other: &Point3) -> f64 {
        let dist2 = diff_squared(self.x, other.x)
            + diff_squared(self.y, other.y)
            + diff_squared(self.z, other.z);
        dist2.sqrt()
    }

    fn fmt_impl(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "({}, {}, {})", self.x, self.y, self.z)
    }
}

impl Debug for Point3 {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        self.fmt_impl(f)
    }
}

impl Display for Point3 {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        self.fmt_impl(f)
    }
}

fn parse(input: &str) -> Vec<Point3> {
    input
        .lines()
        .map(|line| {
            let (x, rest) = line.split_once(',').unwrap();
            let (y, z) = rest.split_once(',').unwrap();
            Point3 {
                x: x.parse().unwrap(),
                y: y.parse().unwrap(),
                z: z.parse().unwrap(),
            }
        })
        .collect()
}

pub fn part_one(input: &str) -> Option<u64> {
    let points = parse(input);
    let mut circuits: Vec<HashSet<Point3>> = Vec::new();
    let mut connections: HashSet<(Point3, Point3)> = HashSet::new();

    let iterations = if points.len() < 100 { 10 } else { 1000 };

    for _ in 0..iterations {
        let mut closest_dist: f64 = f64::INFINITY;
        let mut closest_pair: Option<(Point3, Point3)> = None;
        for p1 in points.iter().copied() {
            for p2 in points.iter().copied() {
                if p1 == p2 {
                    continue;
                }
                if connections.contains(&(p1, p2)) {
                    continue;
                }
                let dist = p1.dist(&p2);
                if dist < closest_dist {
                    closest_dist = dist;
                    closest_pair = Some((p1, p2));
                }
            }
        }

        let (p1, p2) = closest_pair.unwrap();
        // println!("connecting {p1} to {p2}");
        connections.insert((p1, p2));
        connections.insert((p2, p1));

        let circ1idx = circuits.iter().position(|circ| circ.contains(&p1));
        let circ2idx = circuits.iter().position(|circ| circ.contains(&p2));
        match (circ1idx, circ2idx) {
            (None, None) => {
                // println!("new circuit for {p1} and {p2}");
                let mut circ: HashSet<Point3> = HashSet::new();
                circ.insert(p1);
                circ.insert(p2);
                circuits.push(circ);
            }
            (Some(c1idx), None) => {
                let c1 = &mut circuits[c1idx];
                // println!("adding {p2} to circuit {c1:?}");
                c1.insert(p2);
            }
            (None, Some(c2idx)) => {
                let c2 = &mut circuits[c2idx];
                // println!("adding {p1} to circuit {c2:?}");
                c2.insert(p1);
            }
            (Some(c1idx), Some(c2idx)) if c1idx == c2idx => {} // already same circuit
            (Some(c1idx), Some(c2idx)) => {
                let (first, second) = if c1idx < c2idx {
                    (c1idx, c2idx)
                } else {
                    (c2idx, c1idx)
                };
                let c2 = circuits.remove(second);
                let c1 = &mut circuits[first];
                // println!("merging {c1:?} and {c2:?}");
                c1.extend(c2.iter());
            }
        }
    }

    // for circuit in circuits.iter() {
    //     println!("{circuit:?}");
    // }

    // let unconnected_points = points.len() - circuits.iter().map(|c| c.len()).sum::<usize>();
    // println!("unconnected_points: {unconnected_points}");

    circuits.sort_by_key(|circ| circ.len());
    circuits.reverse();

    Some(
        circuits
            .iter()
            .take(3)
            .map(|circ| circ.len() as u64)
            .product(),
    )
}

pub fn part_two(_input: &str) -> Option<u64> {
    None
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(40));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
