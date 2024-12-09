import Foundation

struct Day09: Day {
    let disk = Disk()

    class Disk {
        var sectorFiles: [Int] = []
        var freeSectors: [Int] = []
    }
    
    init(input: String) {
        var id = 0
        var free = false
        for c in input {
            let n = Int(String(c))!
            for _ in 0..<n {
                if free {
                    disk.freeSectors.append(disk.sectorFiles.count)
                }
                disk.sectorFiles.append(free ? -1 : id)
            }
            if !free {
                id += 1
            }
            free.toggle()
        }
        disk.freeSectors.reverse()
    }

    func show(_ sectors: [Int]) {
        var s: String = ""
        for i in 0..<sectors.count {
            if sectors[i] == -1 {
                s += "."
            } else {
                s += String(sectors[i])
            }
        }
        print(s)
    }

    func partOne() -> Int {
        while !disk.freeSectors.isEmpty {
//            show(disk.sectorFiles)
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
//        show(disk.sectorFiles)
        let checksum = disk.sectorFiles.enumerated().map({ $0.offset * $0.element }).reduce(0, +)
        return checksum
    }

    func partTwo() -> Int {
        return 0
    }
}
