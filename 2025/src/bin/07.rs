advent_of_code::solution!(7);

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum Tile {
    Empty,
    Splitter,
}

fn parse(input: &str) -> (Vec<Vec<Tile>>, usize) {
    let mut lines = input.lines();
    let first = lines.next().unwrap();
    let start = first.find('S').unwrap();
    let tiles = lines
        .map(|line| {
            line.chars()
                .map(|c| match c {
                    '.' => Tile::Empty,
                    '^' => Tile::Splitter,
                    _ => panic!("unexpected char {c}"),
                })
                .collect()
        })
        .collect();
    (tiles, start)
}

pub fn part_one(input: &str) -> Option<u64> {
    let (tiles, start) = parse(input);
    let width = tiles[0].len();
    let mut beams = vec![false; width];
    beams[start] = true;

    let mut splits: u64 = 0;
    for row in tiles {
        let mut next = vec![false; width];
        for (x, tile) in row.iter().enumerate() {
            match tile {
                Tile::Empty => next[x] |= beams[x],
                Tile::Splitter if beams[x] => {
                    splits += 1;
                    next[x - 1] = true;
                    next[x + 1] = true;
                }
                Tile::Splitter => {}
            }
        }
        beams = next;
    }

    Some(splits)
}

pub fn part_two(input: &str) -> Option<u64> {
    let (tiles, start) = parse(input);
    let width = tiles[0].len();
    let mut futures = vec![1u64; width];

    for row in tiles.iter().rev() {
        let mut next = vec![0u64; width];
        for (x, tile) in row.iter().enumerate() {
            next[x] = match tile {
                Tile::Empty => futures[x],
                Tile::Splitter => futures[x - 1] + futures[x + 1],
            };
        }
        futures = next;
    }

    Some(futures[start])
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(21));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(40));
    }
}
