import Foundation

typealias CurrentDay = Day16

let startTime = Date.now

print("--- Sample ---")
let testDay = CurrentDay(input: load("samples"))
print(testDay.partOne())
print(testDay.partTwo())

print("\n--- Input---")
let day = CurrentDay(input: load("inputs"))
print(day.partOne())
print(day.partTwo())

let endTime = Date.now
let executionTime = Measurement(value: endTime.timeIntervalSince(startTime), unit: UnitDuration.seconds)
print("\nExecution Time: \(executionTime.formatted(.measurement(width: .wide, usage: .asProvided)))")

func load(_ dir: String) -> String {
    let typeName = String(describing: CurrentDay.self)
    guard let url = Bundle.module.url(forResource: "\(dir)/\(typeName)", withExtension: "txt") else {
        fatalError("Could not find \(dir)/\(typeName).txt")
    }
    let txt = try! String(contentsOf: url, encoding: .utf8)
    return txt.trim()
}
