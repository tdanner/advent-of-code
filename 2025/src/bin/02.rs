advent_of_code::solution!(2);

fn is_invalid(id: u64) -> bool {
    let id_str = format!("{id}");
    if id_str.len() % 2 > 0 {
        false
    } else {
        let (a, b) = id_str.split_at(id_str.len() / 2);
        a == b
    }
}

pub fn part_one(input: &str) -> Option<u64> {
    let ranges = input.split(',').map(|r| {
        let (start_str, end_str) = r.split_once('-').unwrap();
        let start: u64 = start_str.parse().expect("not an int");
        let end: u64 = end_str.parse().expect("not an int");
        (start, end)
    });
    let mut total: u64 = 0;
    for (start, end) in ranges {
        for id in start..=end {
            if is_invalid(id) {
                total += id;
            }
        }
    }
    Some(total)
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
        assert_eq!(result, Some(1227775554));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
