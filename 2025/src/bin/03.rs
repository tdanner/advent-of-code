advent_of_code::solution!(3);

fn parse(input: &str) -> Vec<Vec<u8>> {
    input
        .lines()
        .map(|l| l.bytes().map(|b| b - b'0').collect())
        .collect()
}

fn max_index(bank: &[u8]) -> usize {
    let max = bank.iter().max().unwrap();
    bank.iter().position(|i| i == max).unwrap()
}

fn max_num(bank: &[u8], digits: u32) -> u64 {
    if digits == 1 {
        let max = bank.iter().max().unwrap();
        return (*max).into();
    }
    let upper = bank.len() - (digits - 1) as usize;
    let first_index = max_index(&bank[..upper]);
    let first = bank[first_index];
    let rest = max_num(&bank[first_index + 1..], digits - 1);
    10u64.pow(digits - 1) * u64::from(first) + rest
}

fn total_joltage(input: &str, battery_count: u32) -> Option<u64> {
    Some(
        parse(input)
            .iter()
            .map(|bank| max_num(bank, battery_count))
            .sum(),
    )
}

pub fn part_one(input: &str) -> Option<u64> {
    total_joltage(input, 2)
}

pub fn part_two(input: &str) -> Option<u64> {
    total_joltage(input, 12)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(357));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(3121910778619));
    }
}
