import Foundation

struct Day12: Day {
    let grid: Grid<Character>

    class Region {
        let plant: Character
        var points: Set<Point> = []

        required init(plant: Character) {
            self.plant = plant
        }

        var cost: Int { area * perimeter }
        var area: Int { points.count }
        var perimeter: Int {
            var edges = 0
            for p in points {
                for n in p.neighbors() {
                    if !points.contains(n) { edges += 1 }
                }
            }
            return edges
        }
    }

    class Region2: Region {
        func boundingBox() -> (topLeft: Point, bottomRight: Point) {
            var top = Int.max
            var bottom = 0
            var left = Int.max
            var right = 0
            for p in points {
                top = min(top, p.y)
                bottom = max(bottom, p.y)
                left = min(left, p.x)
                right = max(right, p.x)
            }
            return (Point(left, top), Point(right, bottom))
        }

        override var perimeter: Int {
            var count = 0
            let (topLeft, bottomRight) = boundingBox()
            for y in topLeft.y...bottomRight.y {
                for direction in [Point.north, Point.south] {
                    var inFence = false
                    for x in topLeft.x...(bottomRight.x + 1) {
                        // we are in a fence as long as the current point is in the region and the north point is not
                        let p = Point(x, y)
                        let exterior = p + direction
                        let fenceHere =
                            points.contains(p) && !points.contains(exterior)
                        if inFence && !fenceHere {
                            count += 1
                        }
                        inFence = fenceHere
                    }
                }
            }

            for x in topLeft.x...bottomRight.x {
                for direction in [Point.west, Point.east] {
                    var inFence = false
                    for y in topLeft.y...(bottomRight.y + 1) {
                        // we are in a fence as long as the current point is in the region and the north point is not
                        let p = Point(x, y)
                        let exterior = p + direction
                        let fenceHere =
                            points.contains(p) && !points.contains(exterior)
                        if inFence && !fenceHere {
                            count += 1
                        }
                        inFence = fenceHere
                    }
                }
            }

            return count
        }
    }

    init(input: String) {
        grid = Grid<Character>(input)
    }

    func partOne() -> Int {
        calculate(dummy: Region(plant: "#"))
    }

    func partTwo() -> Int {
        calculate(dummy: Region2(plant: "#"))
    }

    func calculate<TRegion: Region>(dummy: TRegion) -> Int {
        var regionMap: [Point: TRegion] = [:]

        func flood(_ region: TRegion, _ p: Point) {
            region.points.insert(p)
            regionMap[p] = region
            for next in p.neighbors() {
                if grid[next] == grid[p] && !region.points.contains(next) {
                    flood(region, next)
                }
            }
        }

        var regions: [TRegion] = []
        grid.forEach { p, c in
            if regionMap[p] == nil {
                let region = TRegion(plant: c)
                regions.append(region)
                flood(region, p)
            }
        }

//        regions.sorted(using: KeyPathComparator(\.plant)).forEach {
//            print("\($0.plant): \($0.area) * \($0.perimeter) = \($0.cost)")
//        }

        return regions.map(\.cost).reduce(0, +)
    }
}
