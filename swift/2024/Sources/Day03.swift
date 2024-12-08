import Foundation

struct Day03: Day {
    let input: String

    init(input: String) {
        self.input = input
    }

    func partOne() -> Int {
        let matcher = /mul\((\d{1,3}),(\d{1,3})\)/
        return self.input.matches(of: matcher)
            .map({ Int($0.1)! * Int($0.2)! })
            .reduce(0, +)
    }

    func partTwo() -> Int {
        let matcher = /mul\((\d{1,3}),(\d{1,3})\)|do\(\)|don't\(\)/
        var total = 0
        var enabled = true
        self.input.matches(of: matcher).forEach { match in
            switch match.0 {
            case "do()":
                enabled = true
            case "don't()":
                enabled = false
            default:
                if enabled {
                    total += Int(match.1!)! * Int(match.2!)!
                }
            }
        }
        return total
    }
}
