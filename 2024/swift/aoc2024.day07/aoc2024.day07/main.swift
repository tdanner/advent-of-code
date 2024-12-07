import Foundation

let startTime = CFAbsoluteTimeGetCurrent()

let lines = try! String(contentsOf: URL(fileURLWithPath: #file)
    .deletingLastPathComponent()
    .appendingPathComponent("input.txt"), encoding: .utf8)
    .split(separator: "\n")

var part1 = 0
var part2 = 0
for line in lines {
    let parts = line.split(separator: ":")
    let target = Int(parts[0])!
    let nums = parts[1].split(separator: " ").map({Int($0)!})
    if CanCompute1(target, nums[...]) {
        part1 += target
    }
    if CanCompute2(target, nums[...]) {
        part2 += target
    }
}
print("Part 1: \(part1)")
print("Part 2: \(part2)")

let elapsedTime = CFAbsoluteTimeGetCurrent() - startTime
print("Runtime: \(elapsedTime*1000)ms")

func CanCompute1(_ target: Int, _ nums: ArraySlice<Int>) -> Bool {
    return nums.count == 1 && nums.first == target
        || nums.count > 1 && (
            CanCompute1(target - nums.last!, nums.dropLast()) ||
            target % nums.last! == 0 && CanCompute1(target / nums.last!, nums.dropLast())
        )
}

func CanCompute2(_ target: Int, _ nums: ArraySlice<Int>) -> Bool {
    return nums.count == 1 && nums.first == target
        || nums.count > 1 && (
            CanCompute2(target - nums.last!, nums.dropLast()) ||
            target % nums.last! == 0 && CanCompute2(target / nums.last!, nums.dropLast()) ||
            target % Mag(nums.last!) == nums.last! && CanCompute2(target / Mag(nums.last!), nums.dropLast())
        )
}

func Mag(_ n: Int) -> Int {
    var m = 10
    while (n >= m) {
        m *= 10
    }
    return m
}
