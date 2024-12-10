import Foundation

struct Day10: Day {
    let grid: [[Int]]

    init(input: String) {
        self.grid = input.split(separator: "\n").map({
            $0.map({ Int(String($0))! })
        })
    }

    func partOne() -> Int {
        trailheadTotal(trailheadScore)
    }

    func trailheadTotal(_ eval: (Point) -> Int) -> Int {
        var score: Int = 0
        for y in 0..<grid.count {
            for x in 0..<grid[y].count {
                score += eval(Point(x, y))
            }
        }
        return score
    }

    func at(_ p: Point) -> Int {
        if p.x < 0 || p.y < 0 || p.y >= grid.count || p.x >= grid[p.y].count {
            return -1
        }
        return grid[p.y][p.x]
    }

    func canHike(from: Point, heading: Point) -> Bool {
        return at(from + heading) == at(from) + 1
    }

    func trailheadScore(_ p: Point) -> Int {
        if at(p) != 0 {
            return 0
        }
        var reached: Set<Point> = []

        func hike(_ p: Point) {
            if at(p) == 9 {
                reached.insert(p)
                return
            }
            for next in Point.cardinalDirections {
                if canHike(from: p, heading: next) {
                    hike(p + next)
                }
            }
        }

        hike(p)
        return reached.count
    }

    func partTwo() -> Int {
        trailheadTotal(trailheadRating)
    }

    func trailheadRating(_ p: Point) -> Int {
        if at(p) != 0 {
            return 0
        }
        var count = 0

        func hike(_ p: Point) {
            if at(p) == 9 {
                count += 1
                return
            }
            for next in Point.cardinalDirections {
                if canHike(from: p, heading: next) {
                    hike(p + next)
                }
            }
        }

        hike(p)
        return count
    }
}
