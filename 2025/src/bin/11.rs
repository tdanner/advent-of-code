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

fn dfs_memo(memo: &mut HashMap<String, u64>, rack: &Rack, from: &str, to: &str) -> u64 {
    if let Some(&ways) = memo.get(from) {
        return ways;
    }
    if from == to {
        memo.insert(from.to_string(), 1);
        return 1;
    }
    if let Some(outputs) = rack.get(from) {
        let ways = outputs
            .iter()
            .map(|next| dfs_memo(memo, rack, next, to))
            .sum();
        memo.insert(from.to_string(), ways);
        return ways;
    }
    0
}

fn dfs(rack: &Rack, from: &str, to: &str) -> u64 {
    let mut memo = HashMap::new();
    dfs_memo(&mut memo, rack, from, to)
}

pub fn part_one(input: &str) -> Option<u64> {
    let rack = parse(input);
    Some(dfs(&rack, "you", "out"))
}

pub fn part_two(input: &str) -> Option<u64> {
    let rack = parse(input);
    let dac2fft = dfs(&rack, "dac", "fft");
    let fft2dac = dfs(&rack, "fft", "dac");
    if dac2fft > 0 {
        let svr2dac = dfs(&rack, "svr", "dac");
        let fft2out = dfs(&rack, "fft", "out");
        return Some(svr2dac * dac2fft * fft2out);
    } else {
        let svr2fft = dfs(&rack, "svr", "fft");
        let dac2out = dfs(&rack, "dac", "out");
        return Some(svr2fft * fft2dac * dac2out);
    }
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
        let result = part_two(&advent_of_code::template::read_file_part(
            "examples", DAY, 2,
        ));
        assert_eq!(result, Some(2));
    }
}
