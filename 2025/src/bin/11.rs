use std::collections::HashMap;

advent_of_code::solution!(11);

type Rack = HashMap<String, Vec<String>>;

fn parse(input: &str) -> Rack {
    let mut rack = Rack::new();
    for line in input.lines() {
        let (device, outputs) = line.split_once(": ").unwrap();
        rack.insert(
            device.to_string(),
            outputs.split(' ').map(String::from).collect(),
        );
    }
    rack
}

fn dfs(rack: &Rack, from: &str, to: &str) -> u64 {
    if from == to {
        return 1;
    }
    if let Some(outputs) = rack.get(from) {
        return outputs.iter().map(|next| dfs(rack, next, to)).sum();
    }
    0
}

pub fn part_one(input: &str) -> Option<u64> {
    let rack = parse(input);
    Some(dfs(&rack, "you", "out"))
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
        assert_eq!(result, Some(5));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
