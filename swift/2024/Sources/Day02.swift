import Foundation

struct Day02: Day {
    let reports: [[Int]]

    init(input: String) {
        self.reports = input.split(separator: "\n").map({
            $0.split(separator: " ").map({ Int($0)! })
        })
    }

    func partOne() -> Int {
        return reports.filter(isSafe).count
    }

    func isSafe(_ report: [Int]) -> Bool {
        var diffs: [Int] = []
        for i in 1..<report.count {
            diffs.append(report[i] - report[i - 1])
        }
        return diffs.allSatisfy({ $0 >= 1 && $0 <= 3 })
            || diffs.allSatisfy({ $0 >= -3 && $0 <= -1 })
    }

    func partTwo() -> Int {
        return reports.filter(isSafeExceptOne).count
    }

    func isSafeExceptOne(_ report: [Int]) -> Bool {
        for skip in 0..<report.count {
            var skipped = report
            skipped.remove(at: skip)
            if isSafe(skipped) {
                return true
            }
        }
        return false
    }
}
