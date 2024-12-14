import Foundation

struct Day14: Day {
    let robots: [Robot]
    let width: Int
    let height: Int

    struct Robot {
        let initial: Point
        let velocity: Point

        init(line: any StringProtocol) {
            let m = String(line).wholeMatch(
                of: /p=(\d+),(\d+) v=(-?\d+),(-?\d+)/)!
            initial = Point(Int(m.1)!, Int(m.2)!)
            velocity = Point(Int(m.3)!, Int(m.4)!)
        }

        func position(after time: Int, width: Int, height: Int) -> Point {
            let p = initial + velocity * time
            var x = p.x
            var y = p.y
            while x < 0 { x += width }
            while y < 0 { y += height }
            return Point(x % width, y % height)
        }

        func quadrant(after time: Int, width: Int, height: Int) -> Int {
            let p = position(after: time, width: width, height: height)
            return Day14.quadrant(p: p, width: width, height: height)
        }
    }

    static func quadrant(p: Point, width: Int, height: Int) -> Int {
        let xh = (p.x - width / 2).signum()
        let yh = (p.y - height / 2).signum()
        if xh == 0 || yh == 0 {
            return 0
        }
        return (xh + 1) / 2 + yh + 2
    }

    init(input: String) {
        robots = input.split(separator: "\n").map(Robot.init)
        if robots.count < 20 {
            width = 11
            height = 7
        } else {
            width = 101
            height = 103
        }
    }

    func partOne() -> Int {
        var quadrants = [0, 0, 0, 0, 0]
        robots.forEach {
            quadrants[$0.quadrant(after: 100, width: width, height: height)] +=
                1
        }
        var grid: [[Int]] = Array(
            repeating: Array(repeating: 0, count: width), count: height)
        robots.forEach {
            let p = $0.position(after: 100, width: width, height: height)
            grid[p.y][p.x] += 1
        }
        return quadrants[1...4].reduce(1, *)
    }

    func partTwo() -> Int {
        if width < 20 { return 0 } // no sample for part 2 today
        var t = 0
        while true {
            t += 1
            var grid: [[Int]] = Array(
                repeating: Array(repeating: 0, count: width), count: height)
            robots.forEach {
                let p = $0.position(after: t, width: width, height: height)
                grid[p.y][p.x] += 1
            }

            for y in 0..<height {
                var span = 0
                for x in 0..<width {
                    if grid[y][x] == 0 {
                        span = 0
                    } else {
                        span += 1
                    }
                    if span > 10 {
//                        show(grid)
                        return t
                    }
                }
            }
        }
    }

    func show(_ grid: [[Int]]) {
        for y in 0..<height {
            for x in 0..<width {
                print(grid[y][x] == 0 ? " " : "*", terminator: "")
            }
            print()
        }
    }
}
