advent_of_code::solution!(1);

struct Rotation {
    direction: i32,
    distance: i32,
}

fn parse(input: &str) -> Vec<Rotation> {
    input
        .split('\n')
        .map(|l| {
            let mut iter = l.chars();
            let first = iter.next();
            let rest: String = iter.collect();
            Rotation {
                direction: match first {
                    Some('L') => -1,
                    Some('R') => 1,
                    _ => panic!(),
                },
                distance: rest.parse().expect("not an integer"),
            }
        })
        .collect()
}

pub fn part_one(input: &str) -> Option<u64> {
    let rotations = parse(input);
    let mut position: i32 = 50;
    const DIAL_SIZE: i32 = 100;
    let mut zero_stops: u64 = 0;
    for rot in rotations {
        position += rot.direction * rot.distance;
        while position < 0 {
            position += DIAL_SIZE;
        }
        position %= DIAL_SIZE;
        if position == 0 {
            zero_stops += 1;
        }
    }

    Some(zero_stops)
}

pub fn part_two(input: &str) -> Option<u64> {
    let rotations = parse(input);
    let mut position: i32 = 50;
    const DIAL_SIZE: i32 = 100;
    let mut zero_visits: u64 = 0;
    for rot in rotations {
        for _ in 0..rot.distance {
            position += rot.direction;
            if position < 0 {
                position += DIAL_SIZE;
            }
            if position >= DIAL_SIZE {
                position -= DIAL_SIZE;
            }
            if position == 0 {
                zero_visits += 1;
            }
        }
    }
    Some(zero_visits)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(3));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(6));
    }
}
