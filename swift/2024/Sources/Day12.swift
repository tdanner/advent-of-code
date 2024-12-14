import Foundation

struct Day12: Day {
    let grid: Grid<Character>

    class Region {
        let plant: Character
        var points: Set<Point> = []

        init(plant: Character) {
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

    init(input: String) {
        grid = Grid<Character>(input)
    }

    func partOne() -> Int {
        var regionMap: [Point: Region] = [:]

        func flood(_ region: Region, _ p: Point) {
            region.points.insert(p)
            regionMap[p] = region
            for next in p.neighbors() {
                if grid[next] == grid[p] && !region.points.contains(next) {
                    flood(region, next)
                }
            }
        }

        var regions: [Region] = []
        grid.forEach { p, c in
            if regionMap[p] == nil {
                let region = Region(plant: c)
                regions.append(region)
                flood(region, p)
            }
        }

//        regions.sorted(using: KeyPathComparator(\.plant)).forEach {
//            print(
//                "\($0.plant): area: \($0.area), perimeter: \($0.perimeter), cost: \($0.cost)"
//            )
//        }

        return regions.map(\.cost).reduce(0, +)
    }

    func partTwo() -> Int {
        return 0
    }
}
