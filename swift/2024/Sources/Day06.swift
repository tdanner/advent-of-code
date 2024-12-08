import Foundation

struct Day06: Day {
    let grid: [[Character]]
    let startDir = Point.north
    let startPos: Point

    init(input: String) {
        self.grid = input.split(separator: "\n").map(Array.init)
        var startPos: Point?
        yloop: for y in 0..<grid.count {
            for x in 0..<grid[y].count {
                if grid[y][x] == "^" {
                    startPos = .init(x, y)
                    break yloop
                }
            }
        }
        self.startPos = startPos ?? .zero
    }

    func inBounds(_ p: Point) -> Bool {
        return p.x >= 0 && p.y >= 0 && p.x < grid.count && p.y < grid[0].count
    }

    func obstacle(_ p: Point) -> Bool {
        return inBounds(p) && grid[p.y][p.x] == "#"
    }

    func turnRight(_ dir: Point) -> Point {
        if dir == .north { return .east }
        if dir == .east { return .south }
        if dir == .south { return .west }
        if dir == .west { return .north }
        fatalError("Invalid direction \(dir)")
    }

    func partOne() -> Int {
        return findVisited().count
    }

    func findVisited() -> Set<Point> {
        var visited: Set<Point> = []
        var pos = startPos
        var dir = startDir
        while true {
            visited.insert(pos)
            var next = pos + dir
            if !inBounds(next) {
                break
            }
            while obstacle(next) {
                dir = turnRight(dir)
                next = pos + dir
            }
            pos = next
        }
        return visited
    }

    func createsLoop(_ extraObstacle: Point) -> Bool {
        var turns: Set<PointPair> = []
        var pos = startPos
        var dir = startDir
        while true {
            var next = pos + dir
            if !inBounds(next) {
                return false
            }
            while obstacle(next) || next == extraObstacle {
                if turns.contains(PointPair(pos, dir)) {
                    return true
                }
                turns.insert(PointPair(pos, dir))
                dir = turnRight(dir)
                next = pos + dir
            }
            pos = next
        }

    }

    func partTwo() -> Int {
        return findVisited().filter(createsLoop).count
    }
}
