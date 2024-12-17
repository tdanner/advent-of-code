// swift-tools-version: 6.0
// The swift-tools-version declares the minimum version of Swift required to build this package.

import PackageDescription

let package = Package(
    name: "2024",
    platforms: [
        .macOS(.v15)
    ],
    products: [
        .executable(name: "2024", targets: ["2024"]),
    ],
    dependencies: [
        .package(url: "https://github.com/apple/swift-collections", from: "1.1.4"),
    ],
    targets: [
        // Targets are the basic building blocks of a package, defining a module or a test suite.
        // Targets can depend on other targets in this package and products from dependencies.
        .executableTarget(
            name: "2024",
            dependencies: [
                .product(name: "Collections", package: "swift-collections"),
            ],
            resources: [
                .copy("samples"),
                .copy("inputs")
            ]
//           , swiftSettings: [.unsafeFlags(["-O"])]
        ),
    ],
    swiftLanguageModes: [.version("6")]
)
