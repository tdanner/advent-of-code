use rayon::prelude::*;

advent_of_code::solution!(2);

fn parse(input: &str) -> impl Iterator<Item = (u64, u64)> + '_ {
    input.split(',').map(|r| {
        let (start_str, end_str) = r.split_once('-').unwrap();
        let start: u64 = start_str.parse().expect("not an int");
        let end: u64 = end_str.parse().expect("not an int");
        (start, end)
    })
}

fn digit_len(mut n: u64) -> u32 {
    if n == 0 {
        return 1;
    }
    let mut digits = 0;
    while n > 0 {
        digits += 1;
        n /= 10;
    }
    digits
}

fn take_block(n: &mut u64, block_len: u32) -> u64 {
    let mut block = 0u64;
    let mut place = 1u64;

    for _ in 0..block_len {
        let digit = *n % 10;
        *n /= 10;
        block += digit * place;
        place *= 10;
    }

    block as u64
}

fn repeats(value: u64, block_len: u32) -> bool {
    if block_len == 0 {
        return false;
    }

    let digits = digit_len(value);
    if digits % block_len != 0 {
        return false;
    }

    let mut n = value;
    let pattern = take_block(&mut n, block_len);
    while n > 0 {
        if take_block(&mut n, block_len) != pattern {
            return false;
        }
    }
    true
}

fn repeats_twice(id: &u64) -> bool {
    let len = digit_len(*id);
    len % 2 == 0 && repeats(*id, len / 2)
}

fn has_repeat(id: &u64) -> bool {
    (1..=digit_len(*id) / 2).any(|len| repeats(*id, len))
}

fn sum_invalids<F>(input: &str, test: F) -> u64
where
    F: Fn(&u64) -> bool + Send + Sync,
{
    parse(input)
        .flat_map(|(s, e)| s..=e)
        .collect::<Vec<_>>()
        .par_iter()
        .cloned()
        .filter(test)
        .sum()
}

pub fn part_one(input: &str) -> Option<u64> {
    Some(sum_invalids(input, repeats_twice))
}

pub fn part_two(input: &str) -> Option<u64> {
    Some(sum_invalids(input, has_repeat))
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(1227775554));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(4174379265));
    }
}
