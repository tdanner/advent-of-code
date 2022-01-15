package main

import (
	"fmt"
	"io"
	"log"
	"math"
	"os"
	"strings"
)

func main() {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	data, err := io.ReadAll(file)
	if err != nil {
		log.Fatal(err)
	}
	minRow, maxRow := 99999, 0
	seats := make(map[int][]int)
	for _, v := range strings.Split(string(data), "\n") {
		if v == "" {
			continue
		}
		row, col := 0, 0
		for _, c := range v[:7] {
			row <<= 1
			if c == 'B' {
				row |= 1
			}
		}
		for _, c := range v[7:] {
			col <<= 1
			if c == 'R' {
				col |= 1
			}
		}
		seats[row] = append(seats[row], col)
		minRow = int(math.Min(float64(row), float64(minRow)))
		maxRow = int(math.Max(float64(row), float64(maxRow)))
	}
	for row := minRow + 1; row < maxRow; row++ {
		for col := 0; col < 8; col++ {
			if !contains(seats[row], col) {
				fmt.Println(map[string]int{"row": row, "col": col, "id": row*8 + col})
			}
		}
	}
	fmt.Println(seats)
}

func contains(arr []int, seeking int) bool {
	for _, v := range arr {
		if v == seeking {
			return true
		}
	}
	return false
}
