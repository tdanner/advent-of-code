advent_of_code::solution!(2);

fn parse(input: &str) -> impl Iterator<Item = (u64, u64)> + '_ {
    input.split(',').map(|r| {
        let (start_str, end_str) = r.split_once('-').unwrap();
        let start: u64 = start_str.parse().expect("not an int");
        let end: u64 = end_str.parse().expect("not an int");
        (start, end)
    })
}

fn repeats(bytes: &[u8], len: usize) -> bool {
    let head = &bytes[..len];
    bytes.chunks(len).all(|chunk| chunk == head)
}

fn repeats_twice(id: &u64) -> bool {
    let bytes = id.to_string().into_bytes();
    let len = bytes.len();
    len % 2 == 0 && repeats(&bytes, bytes.len() / 2)
}

fn has_repeat(id: &u64) -> bool {
    let bytes = id.to_string().into_bytes();
    (1..=bytes.len() / 2).any(|len| repeats(&bytes, len))
}

fn sum_invalids<F>(input: &str, test: F) -> u64
where
    F: FnMut(&u64) -> bool,
{
    parse(input).flat_map(|(s, e)| s..=e).filter(test).sum()
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
