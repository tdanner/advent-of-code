import Foundation

struct Point: Hashable {
    let x: Int
    let y: Int

    init(_ x: Int, _ y: Int) {
        self.x = x
        self.y = y
    }
    
    static let north = Point(0, -1)
    static let northEast = Point(1, -1)
    static let east = Point(1, 0)
    static let southEast = Point(1, 1)
    static let south = Point(0, 1)
    static let southWest = Point(-1, 1)
    static let west = Point(-1, 0)
    static let northWest = Point(-1, -1)
    
    static let zero = Point(0, 0)

    static let directions = [north, northEast, east, southEast, south, southWest, west, northWest]
}

struct PointPair: Hashable {
    let point1: Point
    let point2: Point
    
    init(_ point1: Point, _ point2: Point) {
        self.point1 = point1
        self.point2 = point2
    }
}

extension Point {
    static func + (lhs: Point, rhs: Point) -> Point {
        Point(lhs.x + rhs.x, lhs.y + rhs.y)
    }

    static func * (lhs: Point, rhs: Int) -> Point {
        Point(lhs.x * rhs, lhs.y * rhs)
    }
}
