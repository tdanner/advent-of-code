import Collections
import Foundation

struct Day16: Day {
    let grid: Grid<Character>

    init(input: String) {
        grid = Grid(input)
    }

    enum Move {
        case TurnLeft
        case TurnRight
        case Forward

        var score: Int {
            switch self {
            case .Forward: return 1
            default: return 1000
            }
        }
    }

    struct State: Hashable {
        let loc: Point
        let facing: Point

        func options(_ grid: Grid<Character>) -> [(State, Int)] {
            var options: [(State, Int)] = []
            if grid[loc + facing] != "#" {
                let location = self.loc + facing
                options.append((State(loc: location, facing: facing), 1))
            }
            options.append((right(), 1000))
            options.append((left(), 1000))
            return options
        }

        func right() -> State {
            switch facing {
            case .north: return State(loc: loc, facing: .east)
            case .east: return State(loc: loc, facing: .south)
            case .south: return State(loc: loc, facing: .west)
            case .west: return State(loc: loc, facing: .north)
            default: fatalError()
            }
        }

        func left() -> State {
            switch facing {
            case .north: return State(loc: loc, facing: .west)
            case .east: return State(loc: loc, facing: .north)
            case .south: return State(loc: loc, facing: .east)
            case .west: return State(loc: loc, facing: .south)
            default: fatalError()
            }
        }
    }

    struct StateWithFScore: Comparable {
        let state: State
        let fScore: Int

        static func < (lhs: StateWithFScore, rhs: StateWithFScore) -> Bool {
            lhs.fScore < rhs.fScore
        }
    }

    func partOne() -> Int {
        var startLoc = Point.zero
        var end = Point.zero
        grid.forEach {
            if $1 == "S" { startLoc = $0 }
            if $1 == "E" { end = $0 }
        }
        let start = State(loc: startLoc, facing: .east)
        var openSet = Heap<StateWithFScore>([
            StateWithFScore(state: start, fScore: hScore(start, end))
        ])
        var cameFrom: [State: State] = [:]
        var gScore: [State: Int] = [start: 0]

        while !openSet.isEmpty {
            let current = openSet.removeMin()
            if current.state.loc == end {
                return current.fScore
            }
            for (opt, cost) in current.state.options(grid) {
                let g = gScore[current.state]! + cost
                if g < gScore[opt] ?? Int.max {
                    cameFrom[opt] = current.state
                    gScore[opt] = g
                    let f = g + hScore(opt, end)
                    let neighbor = StateWithFScore(state: opt, fScore: f)
                    openSet.insert(neighbor)
                }
            }
        }
        return 0
    }

    func hScore(_ start: State, _ end: Point) -> Int {
        // we need the taxicab distance from state.location to end.location
        // plus 1000 times the minimum number of turns
        let dist = abs(start.loc.x - end.x) + abs(start.loc.y - end.y)
        // going to take the shortcut that end is always in the max northeast
        var turns = 0
        if start.loc.x < end.x && start.loc.y > end.y {
            switch start.facing {
            case .north: turns = 1
            case .east: turns = 1
            case .south: turns = 2
            case .west: turns = 2
            default: fatalError()
            }
        } else if start.loc.x < end.x {  // we are already at the correct y
            switch start.facing {
            case .north: turns = 1
            case .east: turns = 0
            case .south: turns = 1
            case .west: turns = 2
            default: fatalError()
            }
        } else if start.loc.y > end.y {  // we are already at the correct x
            switch start.facing {
            case .north: turns = 0
            case .east: turns = 1
            case .south: turns = 2
            case .west: turns = 1
            default: fatalError()
            }
        } else if start.loc.x == end.x && start.loc.y == end.y {
            return 0
        } else {
            fatalError()
        }
        return dist + turns * 1000
    }

    func partTwo() -> Int {
        return 0
    }
}
