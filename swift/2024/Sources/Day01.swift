import Foundation

struct Day01: Day {
    var first: [Int]
    var second: [Int]

    init(input: String) {
        let pairs = input.split(separator: "\n").map({
            $0.split(separator: "  ").map({ Int($0.trim())! })
        })
        self.first = pairs.map({ $0.first! }).sorted(by: <)
        self.second = pairs.map({ $0.last! }).sorted(by: <)
    }

    func partOne() -> Int {
        return zip(self.first, self.second).map({ abs($0 - $1) }).reduce(0, +)
    }

    func partTwo() -> Int {
        let occurrences = Dictionary(
            self.second.map({ ($0, 1) }), uniquingKeysWith: +)
        return self.first.map({ (occurrences[$0] ?? 0) * $0 }).reduce(0, +)
    }
}
