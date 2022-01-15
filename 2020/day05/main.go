package main

import (
	"fmt"
	"io"
	"log"
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
	ids := make([]int, 0)
	max := 0
	for _, v := range strings.Split(string(data), "\n") {
		id := 0
		for _, c := range v {
			id <<= 1
			if c == 'B' || c == 'R' {
				id |= 1
			}
		}
		ids = append(ids, id)
		if id > max {
			max = id
		}
	}
	fmt.Println(max)
}
