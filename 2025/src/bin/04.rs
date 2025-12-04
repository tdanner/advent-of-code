advent_of_code::solution!(4);

#[derive(Clone, Copy, PartialEq)]
enum Contents {
    Empty,
    Paper,
}

#[derive(Clone, Copy, PartialEq)]
struct Point {
    x: i32,
    y: i32,
}

struct Map {
    tiles: Vec<Vec<Contents>>,
}

impl Map {
    pub fn at(&self, p: &Point) -> Contents {
        if p.y >= 0 && p.y < self.tiles.len() as i32 {
            let row = &self.tiles[p.y as usize];
            if p.x >= 0 && p.x < row.len() as i32 {
                row[p.x as usize]
            } else {
                Contents::Empty
            }
        } else {
            Contents::Empty
        }
    }

    pub fn y_range(&self) -> std::ops::Range<usize> {
        0..self.tiles.len()
    }

    pub fn x_range(&self) -> std::ops::Range<usize> {
        0..self.tiles[0].len()
    }

    pub fn set(&mut self, p: &Point, to: &Contents) {
        self.tiles[p.y as usize][p.x as usize] = *to;
    }
}

fn neighbors_of(p: &Point) -> Vec<Point> {
    vec![
        Point {
            x: p.x - 1,
            y: p.y - 1,
        },
        Point { x: p.x, y: p.y - 1 },
        Point {
            x: p.x + 1,
            y: p.y - 1,
        },
        Point { x: p.x - 1, y: p.y },
        Point { x: p.x + 1, y: p.y },
        Point {
            x: p.x - 1,
            y: p.y + 1,
        },
        Point { x: p.x, y: p.y + 1 },
        Point {
            x: p.x + 1,
            y: p.y + 1,
        },
    ]
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
                        _ => panic!(),
                    })
                    .collect()
            })
            .collect(),
    }
}

pub fn part_one(input: &str) -> Option<u64> {
    let map = parse(input);
    let mut accessible = 0u64;
    for y in map.y_range() {
        for x in map.x_range() {
            let p = Point {
                x: x as i32,
                y: y as i32,
            };
            if map.at(&p) == Contents::Paper {
                let paper_count = neighbors_of(&p)
                    .iter()
                    .filter(|p| map.at(*p) == Contents::Paper)
                    .count();
                if paper_count < 4 {
                    accessible += 1;
                }
            }
        }
    }
    Some(accessible)
}

pub fn part_two(input: &str) -> Option<u64> {
    let mut map = parse(input);
    let mut removed = 0u64;
    loop {
        let mut this_round = 0u64;

        for y in map.y_range() {
            for x in map.x_range() {
                let p = Point {
                    x: x as i32,
                    y: y as i32,
                };
                if map.at(&p) == Contents::Paper {
                    let paper_count = neighbors_of(&p)
                        .iter()
                        .filter(|p| map.at(*p) == Contents::Paper)
                        .count();
                    if paper_count < 4 {
                        this_round += 1;
                        map.set(&p, &Contents::Empty);
                    }
                }
            }
        }

        if this_round > 0 {
            removed += this_round;
        } else {
            break;
        }
    }

    Some(removed)
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
