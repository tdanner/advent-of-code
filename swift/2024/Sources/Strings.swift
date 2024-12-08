//
//  Strings.swift
//  2024
//
//  Created by Tim Danner on 12/7/24.
//

extension String {
    /// Removes leading and trailing whitespace and newline characters
    func trim() -> String {
        return self[self.startIndex..<self.endIndex].trimmingCharacters(in: .whitespacesAndNewlines)
    }
}

extension Substring {
    /// Removes leading and trailing whitespace and newline characters
    func trim() -> String {
        return self.trimmingCharacters(in: .whitespacesAndNewlines)
    }
}
