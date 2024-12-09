import Foundation

struct Day08: Day {
    let grid: [[Character]]
    let antennae: [Character: [Point]]

    init(input: String) {
        self.grid = input.split(separator: "\n").map(Array.init)
        var antennae: [Character: [Point]] = [:]
        for y in 0..<grid.count {
            for x in 0..<grid[y].count {
                if grid[y][x] != "." {
                    antennae[grid[y][x], default: []].append(Point(x, y))
                }
            }
        }
        self.antennae = antennae
    }
    
    func inBounds(_ p: Point) -> Bool {
        return p.x >= 0 && p.y >= 0 && p.x < grid.count && p.y < grid[0].count
    }

    func partOne() -> Int {
        var antinodes: Set<Point> = []
        for (_, channelAntennae) in antennae {
            for (index, firstAntenna) in channelAntennae.enumerated() {
                for secondAntenna in channelAntennae.dropFirst(index + 1) {
                    let vector = secondAntenna - firstAntenna
                    antinodes.insert(firstAntenna - vector)
                    antinodes.insert(secondAntenna + vector)
                }
            }
        }
        return antinodes.filter(inBounds).count
    }

    func partTwo() -> Int {
        var antinodes: Set<Point> = []
        for (_, channelAntennae) in antennae {
            for (index, firstAntenna) in channelAntennae.enumerated() {
                for secondAntenna in channelAntennae.dropFirst(index + 1) {
                    let vector = reduce(secondAntenna - firstAntenna)
                    var n = 0
                    while inBounds(firstAntenna - vector*n) {
                        antinodes.insert(firstAntenna - vector*n)
                        n += 1
                    }
                    n = 0
                    while inBounds(firstAntenna + vector*n) {
                        antinodes.insert(firstAntenna + vector*n)
                        n += 1
                    }
                }
            }
        }
        return antinodes.count
    }
    
    func reduce(_ vector: Point) -> Point {
        if vector.x > 1 && vector.y > 1 {
            for factor in 2...min(vector.x, vector.y) {
                if vector.x.isMultiple(of: factor) && vector.y.isMultiple(of: factor) {
                    return reduce(vector / factor)
                }
            }
        }
        return vector
    }
}

