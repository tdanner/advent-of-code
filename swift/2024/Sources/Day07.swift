import Foundation

struct Day07: Day {
    var lines: [String]
    
    init(input: String) {
        self.lines = input.split(separator: "\n").map(String.init)
    }

    func partOne() -> Int {
        return puzzle(canCompute1)
    }
    
    func partTwo() -> Int {
        return puzzle(canCompute2)
    }
    
    func puzzle(_ canCompute: (Int, ArraySlice<Int>) -> Bool) -> Int {
        var total = 0
        for line in lines {
            let parts = line.split(separator: ":")
            let target = Int(parts[0])!
            let nums = parts[1].split(separator: " ").map({Int($0)!})
            if canCompute(target, nums[...]) {
                total += target
            }
        }
        return total
    }
    
    func canCompute1(_ target: Int, _ nums: ArraySlice<Int>) -> Bool {
        return nums.count == 1 && nums.first == target
            || nums.count > 1 && (
                canCompute1(target - nums.last!, nums.dropLast()) ||
                target % nums.last! == 0 && canCompute1(target / nums.last!, nums.dropLast())
            )
    }

    
    func canCompute2(_ target: Int, _ nums: ArraySlice<Int>) -> Bool {
        return nums.count == 1 && nums.first == target
            || nums.count > 1 && (
                canCompute2(target - nums.last!, nums.dropLast()) ||
                target % nums.last! == 0 && canCompute2(target / nums.last!, nums.dropLast()) ||
                target % mag(nums.last!) == nums.last! && canCompute2(target / mag(nums.last!), nums.dropLast())
            )
    }

    func mag(_ n: Int) -> Int {
        var m = 10
        while n >= m {
            m *= 10
        }
        return m
    }
}
