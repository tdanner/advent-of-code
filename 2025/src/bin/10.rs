use itertools::Itertools;
use std::collections::{BinaryHeap, HashMap};
use std::fmt::{Debug, Formatter};
use std::str::FromStr;
use std::u16;

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
            // println!("{machine:?} -> {}", (here.0 as i8).unsigned_abs());
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
    for combo in 1..(1 << v.len()) as u16 {
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

fn joltage_parity(joltages: &Vec<u16>) -> u16 {
    let mut parity: u16 = 0;
    for (pos, joltage) in joltages.iter().enumerate() {
        if !joltage.is_multiple_of(2) {
            parity |= 1 << pos;
        }
    }
    parity
}

fn lights_for_buttons(buttons: &Vec<u16>) -> u16 {
    let mut lights = 0;
    for button in buttons {
        lights ^= button;
    }
    lights
}

fn buttons_needed_memo(memo: &mut HashMap<Vec<u16>, u64>, machine: &Machine) -> u64 {
    if machine.joltages.iter().all(|j| *j == 0) {
        return 0;
    }
    if let Some(&needed) = memo.get(&machine.joltages) {
        return needed;
    }
    let parity = joltage_parity(&machine.joltages);

    // now we need to consider the powerset of buttons
    let mut least_presses = u64::MAX;
    // let mut winning_set = vec![];
    // let all_button_sets = powerset(&machine.buttons);
    // println!(
    //     "there are {} subsets of {}",
    //     all_button_sets.len(),
    //     fmt_buttons(&machine.buttons, machine.num_lights),
    // );
    for button_set in powerset(&machine.buttons) {
        let lights = lights_for_buttons(&button_set);
        // skip if this button combo does not satisfy the even/odd pattern of the joltages
        if lights != parity {
            continue;
        }
        // println!(
        //     "{} -> {} for {}",
        //     fmt_buttons(&button_set, machine.num_lights),
        //     fmt_state(lights, machine.num_lights),
        //     fmt_joltages(&machine.joltages)
        // );
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
        // can't push these buttons - joltage would go negative
        if went_negative {
            continue;
        }

        if reduced_joltages.iter().all(|&j| j == 0) {
            let buttons_needed = button_set.len() as u64;
            // println!(
            //     "{machine:?} => {buttons_needed} via {}",
            //     fmt_buttons(&button_set, machine.num_lights)
            // );

            if buttons_needed < least_presses {
                least_presses = buttons_needed;
                // winning_set = button_set;
            }

            continue;
        }

        let mut factor = 1;
        while reduced_joltages
            .iter()
            .all(|j| j.is_multiple_of(factor * 2))
        {
            factor *= 2;
        }

        if factor > 8 {
            println!("!!!! factor {factor}");
        }

        // println!("trying {}", fmt_buttons(&button_set, machine.num_lights));
        let factored_machine = Machine {
            target: 0,
            num_lights: machine.num_lights,
            buttons: machine.buttons.clone(),
            joltages: reduced_joltages.iter().map(|j| j / factor).collect(),
        };
        let partial_buttons_needed = buttons_needed_memo(memo, &factored_machine);
        let buttons_needed = button_set.len() as u64 + (factor as u64 * partial_buttons_needed);

        // println!(
        //     "{machine:?} => {buttons_needed} via {}",
        //     fmt_buttons(&button_set, machine.num_lights)
        // );

        if buttons_needed < least_presses {
            least_presses = buttons_needed;
            // winning_set = button_set;
        }
        // least_presses = min(least_presses, buttons_needed);
    }

    if least_presses == u64::MAX {
        panic!("no solution for {machine:?}");
    }
    // println!(
    //     "{machine:?} => {least_presses} via {}",
    //     fmt_buttons(&winning_set, machine.num_lights)
    // );
    memo.insert(machine.joltages.clone(), least_presses);
    least_presses
}

pub fn part_two(input: &str) -> Option<u64> {
    let machines = parse(input);
    let mut total = 0;
    for machine in &machines {
        println!("{machine:?}");
        let mut memo = HashMap::new();
        let steps = buttons_needed_memo(&mut memo, machine);
        println!("{steps}");
        total += steps;
    }

    Some(total)

    //Some(machines.iter().map(joltage_buttons_needed).sum())
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
