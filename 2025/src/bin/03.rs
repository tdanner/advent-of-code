advent_of_code::solution!(3);

fn parse(input: &str) -> Vec<Vec<u8>> {
    input
        .lines()
        .map(|l| l.bytes().map(|b| b - b'0').collect())
        .collect()
}

fn largest(bank: Vec<u8>) -> u64 {
    let mut index_of_first_digit: usize = 0;
    for i in 0..bank.len() - 1 {
        if bank[i] > bank[index_of_first_digit] {
            index_of_first_digit = i;
        }
    }
    let second_digit = bank[index_of_first_digit + 1..].iter().max().unwrap();
    let joltage = bank[index_of_first_digit] * 10 + second_digit;
    // println!("{bank:?} -> {joltage}");
    joltage.into()
}

pub fn part_one(input: &str) -> Option<u64> {
    let banks = parse(input);
    Some(banks.into_iter().map(largest).sum())
}

pub fn part_two(input: &str) -> Option<u64> {
    None
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
        assert_eq!(result, None);
    }
}
