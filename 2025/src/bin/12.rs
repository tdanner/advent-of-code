use std::fmt::{Display, Write};

advent_of_code::solution!(12);

struct Present {
    layout: Vec<u64>,
}

struct Region {
    width: u64,
    length: u64,
    contents: Vec<u64>,
}

struct Problem {
    region: Region,
    counts: Vec<u64>,
}

impl Present {
    fn parse(s: &str) -> Self {
        let lines: Vec<&str> = s.lines().collect();
        assert_eq!(4, lines.len());
        let layout = lines[1..3]
            .iter()
            .map(|l| {
                l.chars()
                    .enumerate()
                    .map(|(pos, c)| if c == '#' { 1 << pos } else { 0 })
                    .fold(0, |acc, item| acc | item)
            })
            .collect();

        Self { layout }
    }
}

impl Display for Present {
    fn fmt(&self, f: &mut std::fmt::Formatter<'_>) -> std::fmt::Result {
        for row in self.layout.iter() {
            for pos in 0..3 {
                if row & (1 << pos) != 0 {
                    f.write_char('#')?;
                } else {
                    f.write_char('.')?;
                }
            }
            f.write_char('\n')?;
        }
        std::fmt::Result::Ok(())
    }
}

impl Region {
    fn parse(dimensions: &str) -> Self {
        let (width, length) = dimensions.split_once("x").unwrap();
        Self {
            width: width.parse().unwrap(),
            length: length.parse().unwrap(),
            contents: vec![0; length.parse().unwrap()],
        }
    }

    fn area(&self) -> u64 {
        self.length * self.width
    }
}

impl Problem {
    fn parse(line: &str) -> Self {
        let (dimensions, counts) = line.split_once(": ").unwrap();
        let region = Region::parse(dimensions);
        Self {
            region,
            counts: counts.split(' ').map(|c| c.parse().unwrap()).collect(),
        }
    }
}

fn parse(input: &str) -> (Vec<Present>, Vec<Problem>) {
    let chunks = input.split("\n\n");
    let (problems_chunks, present_chunks): (Vec<&str>, Vec<&str>) =
        chunks.partition(|c| c.contains('x'));

    let presents = present_chunks.iter().map(|c| Present::parse(c)).collect();

    let problems = problems_chunks
        .iter()
        .flat_map(|c| c.lines())
        .map(Problem::parse)
        .collect();

    (presents, problems)
}

pub fn part_one(input: &str) -> Option<u64> {
    let (_presents, problems) = parse(input);
    let mut easy = 0;
    for problem in problems {
        let count: u64 = problem.counts.iter().sum();
        if 9 * count <= problem.region.area() {
            easy += 1;
        }
    }
    Some(easy)
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
        assert_eq!(result, Some(2));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
