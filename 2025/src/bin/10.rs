use itertools::Itertools;
use std::collections::{BinaryHeap, HashMap};
use std::fmt::{Debug, Formatter};
use std::str::FromStr;
use std::{u16, u64};

advent_of_code::solution!(10);

struct Machine {
    target: u16,
    num_lights: usize,
    buttons: Vec<u16>,
    joltages: Vec<u16>,
}

#[derive(Debug)]
enum InvalidMachineError {
    InvalidTarget,
    InvalidButton,
    InvalidJoltage,
}

impl Machine {
    fn default() -> Self {
        Machine {
            target: 0,
            num_lights: 0,
            buttons: vec![],
            joltages: vec![],
        }
    }

    fn parse_target(t: &str) -> Result<(u16, usize), InvalidMachineError> {
        let mut target: u16 = 0;
        for c in t.chars().rev() {
            target <<= 1;
            match c {
                '.' => {}
                '#' => target |= 1,
                _ => return Err(InvalidMachineError::InvalidTarget),
            }
        }
        Ok((target, t.len()))
    }

    fn parse_button(t: &str) -> Result<u16, InvalidMachineError> {
        let mut button: u16 = 0;
        for n in t.split(',') {
            let p: u16 = n.parse().map_err(|_| InvalidMachineError::InvalidButton)?;
            button |= 1 << p;
        }
        Ok(button)
    }

    fn parse_joltages(t: &str) -> Result<Vec<u16>, InvalidMachineError> {
        t.split(',')
            .map(|n| n.parse().map_err(|_| InvalidMachineError::InvalidJoltage))
            .collect()
    }
}

fn fmt_state(state: u16, n: usize) -> String {
    let mut s = String::new();
    s.push('[');
    let inside: String = (0..n)
        .map(|i| if state & (1 << i) == 0 { '.' } else { '#' })
        .collect();
    s.push_str(&inside);
    s.push(']');
    s
}

fn fmt_button(b: u16, n: usize) -> String {
    let mut s = String::new();
    s.push('(');
    let inside = (0..n)
        .filter(|&pos| b & (1 << pos) != 0)
        .map(|pos| pos.to_string())
        .join(",");
    s.push_str(&inside);
    s.push(')');
    s
}

fn fmt_buttons(bs: &Vec<u16>, n: usize) -> String {
    bs.iter().map(|&b| fmt_button(b, n)).join(" ")
}

fn fmt_joltages(js: &Vec<u16>) -> String {
    let mut s = String::new();
    s.push('{');
    s.push_str(&js.iter().join(","));
    s.push('}');
    s
}

impl Debug for Machine {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(
            f,
            "{} {} {}",
            fmt_state(self.target, self.num_lights),
            fmt_buttons(&self.buttons, self.num_lights),
            fmt_joltages(&self.joltages)
        )
    }
}

impl FromStr for Machine {
    type Err = InvalidMachineError;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut me = Machine::default();
        for part in s.split(' ') {
            let inner = &part[1..part.len() - 1];
            match part.chars().nth(0) {
                Some('[') => (me.target, me.num_lights) = Machine::parse_target(inner)?,
                Some('(') => me.buttons.push(Machine::parse_button(inner)?),
                Some('{') => me.joltages = Machine::parse_joltages(inner)?,
                _ => panic!("unexpected part {part}"),
            }
        }

        Ok(me)
    }
}

fn parse(input: &str) -> Vec<Machine> {
    input
        .lines()
        .map(|line| line.parse())
        .collect::<Result<Vec<Machine>, InvalidMachineError>>()
        .unwrap()
}

fn num_buttons_needed(machine: &Machine) -> u64 {
    let mut dist: HashMap<u16, i8> = HashMap::new();
    let mut prev: HashMap<u16, u16> = HashMap::new();
    let mut todo = BinaryHeap::new();
    todo.push((0, 0));
    dist.insert(0, 0);

    while let Some(here) = todo.pop() {
        if here.1 == machine.target {
            return (here.0 as i8).unsigned_abs() as u64;
        }
        for button in &machine.buttons {
            let next = here.1 ^ button;
            if dist.contains_key(&next) {
                continue;
            }
            let alt = here.0 - 1;
            dist.insert(next, alt);
            prev.insert(next, here.1);
            todo.push((alt, next));
        }
    }

    100
}

pub fn part_one(input: &str) -> Option<u64> {
    let machines = parse(input);
    Some(
        machines
            .iter()
            .map(|machine| num_buttons_needed(machine))
            .sum(),
    )
}

fn powerset<T>(v: &Vec<T>) -> Vec<Vec<T>>
where
    T: Clone,
{
    let mut sets = vec![];
    for combo in 0..(1 << v.len()) as u16 {
        let mut set = vec![];
        for (pos, item) in v.iter().enumerate() {
            if (1 << pos) & combo != 0 {
                set.push(item.clone());
            }
        }
        sets.push(set);
    }
    sets
}

fn buttons_needed_memo(
    indent: &str,
    memo: &mut HashMap<Vec<u16>, u64>,
    machine: &Machine,
) -> Option<u64> {
    if machine.joltages.iter().all(|j| *j == 0) {
        return Some(0);
    }
    if let Some(&needed) = memo.get(&machine.joltages) {
        return Some(needed);
    }

    let mut least_presses = None;
    for button_set in powerset(&machine.buttons) {
        let mut reduced_joltages = machine.joltages.clone();
        let mut went_negative = false;
        for button in button_set.iter() {
            for pos in 0..machine.num_lights {
                if button & (1 << pos) != 0 {
                    if reduced_joltages[pos] == 0 {
                        went_negative = true;
                        break;
                    }
                    reduced_joltages[pos] -= 1;
                }
            }
        }
        // not a viable button set
        if went_negative || reduced_joltages.iter().any(|j| !j.is_multiple_of(2)) {
            continue;
        }

        reduced_joltages = reduced_joltages.iter().map(|j| j / 2).collect();
        let factored_machine = Machine {
            target: 0,
            num_lights: machine.num_lights,
            buttons: machine.buttons.clone(),
            joltages: reduced_joltages,
        };
        let mut deeper = indent.to_string();
        deeper.push_str("  ");
        let partial_buttons_needed = buttons_needed_memo(&deeper, memo, &factored_machine);
        if let Some(count) = partial_buttons_needed {
            let buttons_needed = button_set.len() as u64 + 2 * count;
            if least_presses.unwrap_or(u64::MAX) > buttons_needed {
                least_presses = Some(buttons_needed);
            }
        }
    }

    if let Some(least) = least_presses {
        memo.insert(machine.joltages.clone(), least);
    }
    least_presses
}

pub fn part_two(input: &str) -> Option<u64> {
    let machines = parse(input);
    let mut total = 0;
    for machine in &machines {
        let mut memo = HashMap::new();
        let steps = buttons_needed_memo("", &mut memo, machine);
        if let Some(steps) = steps {
            if machine.joltages.iter().any(|j| *j as u64 > steps) {
                panic!("not enough steps!");
            }
            total += steps;
        } else {
            panic!("no solution for {machine:?}");
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
        assert_eq!(result, Some(7));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(33));
    }
}
