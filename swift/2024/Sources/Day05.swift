import Foundation

struct Day05: Day {
    let rules: [Int: [Int]]
    let updates: [[Int]]
    
    init(input: String) {
        let lines = input.split(
            separator: "\n", omittingEmptySubsequences: false)
        let blank = lines.firstIndex(of: "")!
        self.rules = Dictionary(
            lines[..<blank].map(Self.parseRule),
            uniquingKeysWith: { $0 + $1 })
        self.updates = lines[(blank + 1)...].map({
            $0.split(separator: ",").map({ Int($0)! })
        })
    }
    
    static func parseRule(_ rule: Substring) -> (Int, [Int]) {
        let parts = rule.split(separator: "|", maxSplits: 2)
        return (Int(parts[0])!, [Int(parts[1])!])
    }
    
    func partOne() -> Int {
        return self.updates.filter(inRightOrder).map(middle).reduce(0, +)
    }
    
    func inRightOrder(_ update: [Int]) -> Bool {
        for firstPosition in 0..<update.count {
            let page = update[firstPosition]
            for mustPreceed in self.rules[page] ?? [] {
                if let secondPosition = update.firstIndex(of: mustPreceed) {
                    if secondPosition < firstPosition {
                        return false
                    }
                }
            }
        }
        return true
    }
    
    func middle(_ update: [Int]) -> Int {
        return update[update.count / 2]
    }
    
    func ruleCompare(_ lhs: Int, _ rhs: Int) -> Bool {
        return self.rules[lhs]?.contains(rhs) ?? false
    }
    
    func fixOrder(_ update: [Int]) -> [Int] {
        return update.sorted(by: ruleCompare)
    }
    
    func partTwo() -> Int {
        self.updates.filter({ !inRightOrder($0) }).map(fixOrder).map(middle).reduce(0, +)
    }
}
