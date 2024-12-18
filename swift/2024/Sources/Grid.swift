import Foundation

class Grid<T> {
    var contents: [[T]]
    let def: T

    init(contents: [[T]], def: T) {
        self.contents = contents
        self.def = def
    }

    var width: Int { contents.first?.count ?? 0 }
    var height: Int { contents.count }
    
    subscript(_ p: Point) -> T {
        get {
            if contents.indices.contains(p.y)
                && contents[p.y].indices.contains(p.x)
            {
                return contents[p.y][p.x]
            }
            return def
        }
        set {
            contents[p.y][p.x] = newValue
        }
    }

    func forEach(_ f: (Point, T) throws -> Void) rethrows {
        for y in contents.indices {
            for x in contents[y].indices {
                try f(Point(x, y), contents[y][x])
            }
        }
    }
    
    func show() {
        for y in contents.indices {
            for x in contents[y].indices {
                print(contents[y][x], terminator: "")
            }
            print()
        }
    }
}

extension Grid<Character> {
    convenience init(_ s: String) {
        self.init(contents: s.split(separator: "\n").map(Array.init), def: " ")
    }
}

extension Grid<Int> {
    convenience init(_ s: String) {
        self.init(
            contents: s.split(separator: "\n").map({
                $0.map({ Int(String($0))! })
            }), def: -1)
    }
}
