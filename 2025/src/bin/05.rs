advent_of_code::solution!(5);

use std::ops::RangeInclusive;

struct IngredientDB {
    fresh_list: Vec<RangeInclusive<u64>>,
    available: Vec<u64>,
}

fn parse(input: &str) -> IngredientDB {
    let mut fresh_list: Vec<RangeInclusive<u64>> = vec![];
    let mut available: Vec<u64> = vec![];
    for line in input.lines() {
        if let Some((a, b)) = line.split_once('-') {
            let start: u64 = a.parse().expect("not an int");
            let end: u64 = b.parse().expect("not an int");
            fresh_list.push(start..=end);
        } else if let Ok(ingredient) = line.parse() {
            available.push(ingredient);
        }
    }
    IngredientDB {
        fresh_list,
        available,
    }
}

pub fn part_one(input: &str) -> Option<u64> {
    let db = parse(input);
    let mut fresh_count = 0u64;
    for ingredient in db.available {
        if db.fresh_list.iter().any(|l| l.contains(&ingredient)) {
            fresh_count += 1;
        }
    }
    Some(fresh_count)
}

#[derive(Debug, PartialEq, Eq)]
enum RangeBoundKind {
    Start,
    End,
}

#[derive(Debug, PartialEq, Eq)]
struct RangeBound {
    id: u64,
    kind: RangeBoundKind,
}

pub fn part_two(input: &str) -> Option<u64> {
    let db = parse(input);
    let mut bounds: Vec<RangeBound> = vec![];
    for range in db.fresh_list {
        bounds.push(RangeBound {
            kind: RangeBoundKind::Start,
            id: *range.start(),
        });
        bounds.push(RangeBound {
            kind: RangeBoundKind::End,
            id: *range.end(),
        });
    }
    bounds.sort_by_key(|e| {
        (
            e.id,
            match e.kind {
                RangeBoundKind::Start => 0,
                RangeBoundKind::End => 1,
            },
        )
    });
    let mut bounds_iter = bounds.iter();
    let first_bound = bounds_iter.next().unwrap();
    assert!(first_bound.kind == RangeBoundKind::Start);
    let mut fresh_count = 0u64;
    let mut current_start = Some(first_bound.id);
    let mut active_ranges = 1i64;
    for bound in bounds_iter {
        println!("Active ranges {active_ranges}. Considering {bound:?}.");
        if bound.kind == RangeBoundKind::End {
            active_ranges -= 1;
            assert!(active_ranges >= 0);
            if active_ranges == 0
                && let Some(start) = current_start
            {
                let end = bound.id;

                let new_fresh = end - start + 1;
                println!("Closing range {start}..={end}. Adding {new_fresh}.");
                fresh_count += new_fresh;
                current_start = None;
            }
        } else if bound.kind == RangeBoundKind::Start {
            if active_ranges == 0 {
                let start = bound.id;
                println!("Opening range at {start}.");
                current_start = Some(start);
            }
            active_ranges += 1;
        }
    }
    Some(fresh_count)
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_part_one() {
        let result = part_one(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(3));
    }

    #[test]
    fn test_part_two() {
        let result = part_two(&advent_of_code::template::read_file("examples", DAY));
        assert_eq!(result, Some(14));
    }
}
