advent_of_code::solution!(4);

#[derive(Clone, Copy, PartialEq, Eq, Debug)]
enum Contents {
    Empty,
    Paper,
}

#[derive(Clone, Copy, PartialEq, Eq, Debug)]
struct Point {
    x: i32,
    y: i32,
}

struct Map {
    tiles: Vec<Vec<Contents>>,
}

impl Map {
    pub fn at(&self, p: Point) -> Contents {
        self.tiles
            .get(p.y as usize)
            .and_then(|row| row.get(p.x as usize))
            .copied()
            .unwrap_or(Contents::Empty)
    }

    pub fn points(&self) -> impl Iterator<Item = Point> + '_ {
        let h = self.tiles.len();
        let w = self.tiles[0].len();
        (0..h).flat_map(move |y| {
            (0..w).map(move |x| Point {
                x: x as i32,
                y: y as i32,
            })
        })
    }

    pub fn is_accessible(&self, p: Point) -> bool {
        neighbors_of(p)
            .filter(|&p| self.at(p) == Contents::Paper)
            .count()
            < 4
    }

    pub fn accessible_papers(&self) -> impl Iterator<Item = Point> + '_ {
        self.points()
            .filter(|&p| self.at(p) == Contents::Paper && self.is_accessible(p))
    }

    pub fn set(&mut self, p: Point, to: Contents) {
        self.tiles[p.y as usize][p.x as usize] = to;
    }
}

fn neighbors_of(p: Point) -> impl Iterator<Item = Point> {
    const OFFSETS: [(i32, i32); 8] = [
        (-1, -1),
        (0, -1),
        (1, -1),
        (-1, 0),
        (1, 0),
        (-1, 1),
        (0, 1),
        (1, 1),
    ];

    OFFSETS.into_iter().map(move |(dx, dy)| Point {
        x: p.x + dx,
        y: p.y + dy,
    })
}

fn parse(input: &str) -> Map {
    Map {
        tiles: input
            .lines()
            .map(|l| {
                l.chars()
                    .map(|c| match c {
                        '.' => Contents::Empty,
                        '@' => Contents::Paper,
                        other => panic!("unexpected char in map: {other}"),
                    })
                    .collect()
            })
            .collect(),
    }
}

pub fn part_one(input: &str) -> Option<u64> {
    let map = parse(input);
    Some(map.accessible_papers().count() as u64)
}

pub fn part_two(input: &str) -> Option<u64> {
    let mut map = parse(input);
    let mut removed = 0usize;
    loop {
        let to_remove: Vec<Point> = map.accessible_papers().collect();
        if to_remove.is_empty() {
            break;
        }

        removed += to_remove.len();
        for p in to_remove {
            map.set(p, Contents::Empty);
        }
    }

    Some(removed as u64)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(13));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(43));
    }
}
