advent_of_code::solution!(1);

const DIAL_SIZE: i32 = 100;

fn parse(input: &str) -> Vec<i32> {
    input
        .lines()
        .map(|l| {
            let (dir, digits) = l.split_at(1);
            let dist: i32 = digits.parse().expect("not an integer");
            dist * match dir {
                "L" => -1,
                "R" => 1,
                _ => panic!(),
            }
        })
        .collect()
}

pub fn part_one(input: &str) -> Option<u64> {
    let rotations = parse(input);
    let mut position: i32 = 50;
    let mut zero_stops: u64 = 0;
    for rot in rotations {
        position += rot;
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
    let mut zero_visits: u64 = 0;
    for rot in rotations {
        for _ in 0..rot.abs() {
            position += rot.signum();
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
