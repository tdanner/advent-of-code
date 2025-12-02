advent_of_code::solution!(2);

fn parse(input: &str) -> Vec<(u64, u64)> {
    input
        .split(',')
        .map(|r| {
            let (start_str, end_str) = r.split_once('-').unwrap();
            let start: u64 = start_str.parse().expect("not an int");
            let end: u64 = end_str.parse().expect("not an int");
            (start, end)
        })
        .collect()
}

fn repeats(s: &str, len: usize) -> bool {
    if s.len() % len != 0 {
        return false;
    }
    let b = s.as_bytes();
    for i in 0..len {
        let c = b[i];
        for r in 1..b.len() / len {
            if b[r * len + i] != c {
                return false;
            }
        }
    }
    true
}

pub fn part_one(input: &str) -> Option<u64> {
    let ranges = parse(input);
    let mut total: u64 = 0;
    for (start, end) in ranges {
        for id in start..=end {
            let id_str = format!("{id}");
            if id_str.len() % 2 == 0 && repeats(&id_str, id_str.len() / 2) {
                total += id;
            }
        }
    }
    Some(total)
}

pub fn part_two(input: &str) -> Option<u64> {
    let ranges = parse(input);
    let mut total: u64 = 0;
    for (start, end) in ranges {
        for id in start..=end {
            let id_str = format!("{id}");
            for len in 1..=id_str.len() / 2 {
                if repeats(&id_str, len) {
                    total += id;
                    break;
                }
            }
        }
    }
    Some(total)
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
