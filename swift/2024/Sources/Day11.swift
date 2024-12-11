import Foundation

struct Day11: Day {
    let initialStones: [Int]

    struct Stone: Hashable {
        let initial: Int
        let blinks: Int
    }

    var cache: [Stone: Int] = [:]  // (stone, blinks) -> count

    init(input: String) {
        initialStones = input.split(separator: " ").map({ Int($0)! })
    }

    func partOne() -> Int {
        return solve(25)
    }

    func partTwo() -> Int {
        return solve(75)
    }

    func solve(_ blinks: Int) -> Int {
        var cache: [Stone: Int] = [:]

        func stonesAfterBlinks(_ stone: Int, _ blinks: Int) -> Int {
            let key = Stone(initial: stone, blinks: blinks)
            if let count = cache[key] {
                return count
            }
            let stones = blink(stone)
            let count =
                blinks > 1
                ? stones.map({ stonesAfterBlinks($0, blinks - 1) }).reduce(0, +)
                : stones.count
            cache[key] = count
            return count
        }

        return initialStones.map({ stonesAfterBlinks($0, blinks) })
            .reduce(0, +)
    }

    func blink(_ stone: Int) -> [Int] {
        if stone == 0 {
            return [1]
        }
        let (left, right, wasSplit) = split(stone)
        if wasSplit {
            return [left, right]
        }
        return [stone * 2024]
    }

    func split(_ n: Int) -> (Int, Int, Bool) {
        var x = 10
        var y = 10
        while true {
            if n < y {
                return (0, 0, false)
            }
            y *= 10
            if n < y {
                return (n / x, n % x, true)
            }
            x *= 10
            y *= 10
        }
    }
}
