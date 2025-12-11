use std::collections::{BinaryHeap, HashMap};
use std::fmt::{Debug, Formatter};
use std::str::FromStr;

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

fn fmt_state(s: u16, n: usize) -> String {
    (0..n)
        .map(|i| if s & (1 << i) == 0 { '.' } else { '#' })
        .collect()
}

impl Debug for Machine {
    fn fmt(&self, f: &mut Formatter<'_>) -> std::fmt::Result {
        write!(f, "[{}]", fmt_state(self.target, self.num_lights))?;
        for button in &self.buttons {
            write!(f, " (")?;
            let mut first = true;
            for n in 0..self.num_lights {
                if (button & 1 << n) != 0 {
                    if !first {
                        write!(f, ",")?;
                    }
                    first = false;
                    write!(f, "{n}")?;
                }
            }
            write!(f, ")")?;
        }
        write!(f, " {{")?;
        let mut first = true;
        for joltage in &self.joltages {
            if !first {
                write!(f, ",")?;
            }
            first = false;
            write!(f, "{joltage}")?;
        }
        write!(f, "}}")
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
        // println!(
        //     "{} {}",
        //     fmt_state(here.1, machine.num_lights),
        //     dist[&here.1]
        // );
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
    let mut total = 0;
    for machine in &machines {
        println!("{machine:?}");
        let steps = num_buttons_needed(machine);
        println!("{steps}");
        total += steps;
    }

    Some(total)

    // Some(
    //     machines
    //         .iter()
    //         .map(|machine| num_buttons_needed(machine))
    //         .sum(),
    // )
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
        assert_eq!(result, Some(7));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, None);
    }
}
