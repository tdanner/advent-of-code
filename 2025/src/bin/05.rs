advent_of_code::solution!(5);

use std::ops::RangeInclusive;

struct IngredientDB {
    fresh_list: Vec<RangeInclusive<u64>>,
    available: Vec<u64>,
}

fn parse(input: &str) -> IngredientDB {
    let mut fresh_list = Vec::new();
    let mut available = Vec::new();
    for line in input.lines() {
        if let Some((a, b)) = line.split_once('-') {
            let start: u64 = a.parse().unwrap();
            let end: u64 = b.parse().unwrap();
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
    let fresh_count = db
        .available
        .into_iter()
        .filter(|&ingredient| db.fresh_list.iter().any(|r| r.contains(&ingredient)))
        .count() as u64;
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
    let mut bounds: Vec<RangeBound> = db
        .fresh_list
        .into_iter()
        .flat_map(|range| {
            let start = *range.start();
            let end = *range.end();
            [
                RangeBound {
                    id: start,
                    kind: RangeBoundKind::Start,
                },
                RangeBound {
                    id: end,
                    kind: RangeBoundKind::End,
                },
            ]
        })
        .collect();

    bounds.sort_by_key(|e| {
        (
            e.id,
            match e.kind {
                RangeBoundKind::Start => 0,
                RangeBoundKind::End => 1,
            },
        )
    });

    let mut iter = bounds.into_iter();
    let first_bound = iter.next()?;
    assert!(matches!(first_bound.kind, RangeBoundKind::Start));

    let mut fresh_count = 0u64;
    let mut active_ranges = 1i64;
    let mut current_start = first_bound.id;

    for bound in iter {
        match bound.kind {
            RangeBoundKind::End => {
                active_ranges -= 1;
                assert!(active_ranges >= 0);
                if active_ranges == 0 {
                    fresh_count += bound.id - current_start + 1;
                }
            }
            RangeBoundKind::Start => {
                if active_ranges == 0 {
                    current_start = bound.id;
                }
                active_ranges += 1;
            }
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
