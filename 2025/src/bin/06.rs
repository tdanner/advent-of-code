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

pub fn part_two(input: &str) -> Option<u64> {
    let text: Vec<Vec<char>> = input.lines().map(|l| l.chars().collect()).collect();

    // logic assumes all lines are exactly the same length
    let cols = text[0].len();
    assert!(text.iter().all(|l| l.len() == cols));

    let mut operands = Vec::new();
    let mut total = 0u64;
    for col in (0..cols).rev() {
        let mut operand: Option<u64> = None;
        let mut operator: Option<char> = None;
        for line in text.iter() {
            let c = line[col];
            match c {
                ' ' => continue,
                '0'..='9' => {
                    let digit = (c as u8 - '0' as u8) as u64;
                    operand = match operand {
                        None => Some(digit),
                        Some(value) => Some(value * 10 + digit),
                    };
                }
                '+' | '*' => operator = Some(c),
                _ => panic!("unexpected char {c}"),
            }
        }
        if let Some(operand) = operand {
            operands.push(operand);
        }
        match operator {
            None => {}
            Some(op) => {
                let value: u64 = match op {
                    '+' => operands.iter().sum(),
                    '*' => operands.iter().product(),
                    _ => panic!("unknown operator {op}"),
                };
                total += value;
                operands.clear();
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
        assert_eq!(result, Some(4277556));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(3263827));
    }
}
