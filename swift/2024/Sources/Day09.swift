import Foundation

struct Day09: Day {
    let input: String

    init(input: String) {
        self.input = input
    }

    class Disk1 {
        var sectorFiles: [Int] = []
        var freeSectors: [Int] = []

        init(_ input: String) {
            var id = 0
            var free = false
            for c in input {
                let n = Int(String(c))!
                for _ in 0..<n {
                    if free {
                        freeSectors.append(sectorFiles.count)
                    }
                    sectorFiles.append(free ? -1 : id)
                }
                if !free {
                    id += 1
                }
                free.toggle()
            }
            freeSectors.reverse()
        }

        func show() {
            var s: String = ""
            for i in 0..<sectorFiles.count {
                if sectorFiles[i] == -1 {
                    s += "."
                } else {
                    s += String(sectorFiles[i])
                }
            }
            print(s)
        }
    }

    func partOne() -> Int {
        let disk = Disk1(input)
        while !disk.freeSectors.isEmpty {
            //            disk.show()
            let free = disk.freeSectors.removeLast()
            if free >= disk.sectorFiles.count {
                break
            }
            var last: Int
            repeat {
                last = disk.sectorFiles.removeLast()
            } while last == -1
            disk.sectorFiles[free] = last
        }
        let checksum = disk.sectorFiles.enumerated().map({
            $0.offset * $0.element
        }).reduce(0, +)
        return checksum
    }

    class Disk2 {
        class Span {
            var id: Int
            var position: Int
            var length: Int

            init(id: Int, position: Int, length: Int) {
                self.id = id
                self.position = position
                self.length = length
            }

            var description: String {
                "\(id) \(position) \(length)"
            }
        }

        var freeSpans: [Span] = []
        var files: [Span] = []
        var sectors: [Int] = []  // file ID in a given sector, or 0 if free

        init(_ input: String) {
            var id = 0
            var free = false
            for c in input {
                let n = Int(String(c))!
                if free {
                    if n > 0 {
                        freeSpans.append(
                            Span(id: -1, position: sectors.count, length: n))
                    }
                } else {
                    files.append(
                        Span(id: id, position: sectors.count, length: n))
                }
                for _ in 0..<n {
                    sectors.append(free ? 0 : id)
                }
                if !free {
                    id += 1
                }
                free.toggle()
            }
            freeSpans.reverse()
            files.reverse()
        }

        func checksum() -> Int {
            sectors.enumerated().map({ $0.offset * $0.element }).reduce(0, +)
        }
    }

    func partTwo() -> Int {
        let disk = Disk2(input)
        for file in disk.files {
            while !disk.freeSpans.isEmpty
                && (disk.freeSpans.last!.position >= file.position
                    || disk.freeSpans.last!.length == 0)
            {
                disk.freeSpans.removeLast()
            }
            let target = disk.freeSpans.last(where: {
                $0.length >= file.length && $0.position < file.position
            })
            if let target {
                for p in 0..<file.length {
                    disk.sectors[file.position + p] = 0
                }
                file.position = target.position
                target.position += file.length
                target.length -= file.length
                for p in 0..<file.length {
                    disk.sectors[file.position + p] = file.id
                }
            }
        }
        return disk.checksum()
    }
}
