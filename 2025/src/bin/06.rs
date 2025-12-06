advent_of_code::solution!(6);

fn parse(input: &str) -> (Vec<char>, Vec<Vec<u64>>) {
    let mut lines_rev = input.lines().rev();
    let operators: Vec<char> = lines_rev
        .next()
        .unwrap()
        .split_ascii_whitespace()
        .map(|o| o.chars().next().unwrap())
        .collect();
    let numbers = lines_rev
        .map(|l| {
            l.split_ascii_whitespace()
                .map(|s| s.parse().unwrap())
                .collect()
        })
        .collect();
    (operators, numbers)
}

pub fn part_one(input: &str) -> Option<u64> {
    let (operators, numbers) = parse(input);
    let total = operators
        .iter()
        .enumerate()
        .map(|(i, op)| {
            let operands = numbers.iter().map(|row| row[i]);
            match op {
                '+' => operands.sum::<u64>(),
                '*' => operands.product(),
                _ => panic!("unknown op"),
            }
        })
        .sum();
    Some(total)
}

pub fn part_two(_: &str) -> Option<u64> {
    None
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(4277556));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
