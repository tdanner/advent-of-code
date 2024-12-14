import Foundation

struct Day13: Day {
    struct Game {
        let a, b, prize: Point

        func tokens() -> Int {
            let bTokens =
                Double(a.x * prize.y - a.y * prize.x)
                / Double(a.x * b.y - a.y * b.x)
            if bTokens.truncatingRemainder(dividingBy: 1) != 0 {
                return 0
            }
            let aTokens = Double(prize.x - Int(bTokens) * b.x) / Double(a.x)
            if aTokens.truncatingRemainder(dividingBy: 1) != 0 {
                return 0
            }
            return Int(aTokens) * 3 + Int(bTokens)
        }
    }

    let games: [Game]

    init(input: String) {
        let lines = input.split(separator: "\n")
        let buttonRegex = /Button [AB]: X\+(\d+), Y\+(\d+)/
        let prizeRegex = /Prize: X=(\d+), Y=(\d+)/
        var games: [Game] = []
        for l in stride(from: 0, to: lines.count, by: 3) {
            let buttonA = lines[l].firstMatch(of: buttonRegex)!
            let buttonB = lines[l + 1].firstMatch(of: buttonRegex)!
            let prize = lines[l + 2].firstMatch(of: prizeRegex)!
            let game = Game(
                a: Point(Int(buttonA.1)!, Int(buttonA.2)!),
                b: Point(Int(buttonB.1)!, Int(buttonB.2)!),
                prize: Point(Int(prize.1)!, Int(prize.2)!))
            games.append(game)
        }
        self.games = games
    }

    func partOne() -> Int {
        return games.map({ $0.tokens() }).reduce(0, +)
    }

    func partTwo() -> Int {
        let offsetSize = 10000000000000
        let offset = Point(offsetSize, offsetSize)
        let games2 = games.map { game in
            Game(a: game.a, b: game.b, prize: game.prize + offset)
        }
        return games2.map({ $0.tokens() }).reduce(0, +)
    }
}
