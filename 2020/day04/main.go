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
	valids := 0
	for _, v := range strings.Split(string(data), "\n\n") {
		if isValid(parsePassport(v)) {
			valids++
		}
	}
	fmt.Println(valids)
}

func parsePassport(passport string) (values map[string]string) {
	values = make(map[string]string)
	for _, v := range strings.Fields(passport) {
		parts := strings.SplitN(v, ":", 2)
		values[parts[0]] = parts[1]
	}
	return
}

var requiredFields = [...]string{"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"}

func isValid(passport map[string]string) bool {
	for _, v := range requiredFields {
		if _, ok := passport[v]; !ok {
			return false
		}
	}
	return true
}
