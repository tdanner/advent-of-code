import Foundation

struct Day04: Day {
    let lines: [[Character]]

    init(input: String) {
        self.lines = input.split(separator: "\n").map(Array.init)
    }

    func get(_ pt: Point) -> Character {
        if 0..<lines.count ~= pt.y && 0..<lines[pt.y].count ~= pt.x {
            return lines[pt.y][pt.x]
        }
        return " "
    }

    func partOne() -> Int {
        let word = Array("XMAS")
        var found = 0
        for y in 0..<lines.count {
            for x in 0..<lines[y].count {
                let o = Point(x, y)
                for dir in Point.directions {
                    if (0..<word.count).allSatisfy({ dist -> Bool in
                        return word[dist] == get(o + dir * dist)
                    }) {
                        found += 1
                    }
                }
            }
        }
        return found
    }

    func mas(_ a: Character, _ b: Character) -> Bool {
        return a == "M" && b == "S" || a == "S" && b == "M"
    }

    func partTwo() -> Int {
        var found = 0
        for y in 0..<lines.count {
            for x in 0..<lines[y].count {
                let o = Point(x, y)
                if get(o) == "A" {
                    let nw = get(o + .northWest)
                    let ne = get(o + .northEast)
                    let sw = get(o + .southWest)
                    let se = get(o + .southEast)
                    if mas(nw, se) && mas(ne, sw) {
                        found += 1
                    }
                }
            }
        }
        return found
    }
}
