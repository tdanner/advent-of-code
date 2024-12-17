import Foundation

struct Day15: Day {
    let grid: String
    let moves: [Point]

    class Warehouse {
        var grid: Grid<Character>
        var robot = Point.zero

        init(_ grid: String) {
            self.grid = Grid<Character>(grid)
            self.grid.forEach {
                if $1 == "@" {
                    robot = $0
                }
            }
        }

        func move(_ start: Point, _ direction: Point) -> Bool {
            let dest = start + direction
            let destContents = self.grid[dest]
            switch destContents {
            case "#": return false
            case ".":
                swap(&self.grid[start], &self.grid[dest])
                return true
            case "O":
                return move(dest, direction) && move(start, direction)
            default:
                fatalError(
                    "unexpected grid content at \(dest): \(destContents)")
            }
        }
    }

    class DoubleWide {
        var grid: Grid<Character>
        var robot = Point.zero

        init(_ input: String) {
            let single = Grid<Character>(input)
            var rows: [[Character]] = []
            for y in 0..<single.height {
                var row: [Character] = []
                for x in 0..<single.width {
                    switch single[Point(x, y)] {
                    case "#": row.append(contentsOf: "##")
                    case ".": row.append(contentsOf: "..")
                    case "O": row.append(contentsOf: "[]")
                    case "@": row.append(contentsOf: "@.")
                    default:
                        fatalError("?! (\(x),\(y)): \(single[Point(x, y)])")
                    }
                }
                rows.append(row)
            }
            grid = Grid(contents: rows, def: " ")
            grid.forEach {
                if $1 == "@" {
                    robot = $0
                }
            }
        }

        func canMove(_ start: Point, _ direction: Point) -> Bool {
            let dest = start + direction
            let destContents = grid[dest]
            switch destContents {
            case "#": return false
            case ".": return true
            case "[":
                switch direction {
                case .north, .south:  // if moving north or south, must move the companion piece as well
                    return canMove(dest, direction)
                        && canMove(dest + .east, direction)
                case .east:  // if moving east, only the companion piece needs to be able to move
                    return canMove(dest + .east, direction)
                case .west:  // if moving west, only this piece needs to be able to move
                    return canMove(dest, direction)
                default: fatalError("?! \(direction)")
                }
            case "]":
                switch direction {
                case .north, .south:  // if moving north or south, must move the companion piece as well
                    return canMove(dest, direction)
                        && canMove(dest + .west, direction)
                case .west:  // if moving west, only the companion piece needs to be able to move
                    return canMove(dest + .west, direction)
                case .east:  // if moving east, only this piece needs to be able to move
                    return canMove(dest, direction)
                default: fatalError("?! \(direction)")
                }
            default: fatalError("?! \(destContents)")
            }
        }

        func move(_ start: Point, _ direction: Point) {
            if !canMove(start, direction) { return }
            let dest = start + direction
            let destContents = grid[dest]
            switch destContents {
            case ".": swap(&grid[start], &grid[dest])
            case "[":
                move(dest + Point.east, direction)
                move(dest, direction)
                swap(&grid[start], &grid[dest])
            case "]":
                move(dest + Point.west, direction)
                move(dest, direction)
                swap(&grid[start], &grid[dest])
            default: fatalError("?! \(destContents)")
            }
        }
    }

    init(input: String) {
        let parts = input.split(separator: "\n\n")
        grid = String(parts[0])
        var moves: [Point] = []
        for c in parts[1] {
            switch c {
            case "<": moves.append(.west)
            case ">": moves.append(.east)
            case "^": moves.append(.north)
            case "v": moves.append(.south)
            default: break
            }
        }
        self.moves = moves
    }

    func gps(_ p: Point) -> Int { return p.y * 100 + p.x }

    func partOne() -> Int {
        let warehouse = Warehouse(self.grid)

        for move in moves {
            if warehouse.move(warehouse.robot, move) {
                warehouse.robot = warehouse.robot + move
            }
            //            warehouse.show()
            //            print()
        }
        //        warehouse.show()
        var total = 0
        warehouse.grid.forEach {
            if $1 == "O" {
                total += gps($0)
            }
        }
        return total
    }

    func partTwo() -> Int {
        let warehouse = DoubleWide(self.grid)
        for move in moves {
            if warehouse.canMove(warehouse.robot, move) {
                warehouse.move(warehouse.robot, move)
                warehouse.robot = warehouse.robot + move
            }
//            warehouse.grid.show()
//            print()
        }
//        warehouse.grid.show()
        var total = 0
        warehouse.grid.forEach {
            if $1 == "[" {
                total += gps($0)
            }
        }
        return total
    }
}
