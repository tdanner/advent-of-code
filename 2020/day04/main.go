package main

import (
	"fmt"
	"io"
	"log"
	"os"
	"regexp"
	"strconv"
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
		passport := parsePassport(v)
		delete(passport, "cid")
		valid := isValid(passport)
		if valid {
			valids++
			fmt.Println(passport)
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

var requiredFields = []string{"byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"}
var eyeColors = []string{"amb", "blu", "brn", "gry", "grn", "hzl", "oth"}

func isValid(passport map[string]string) bool {
	for _, v := range requiredFields {
		if _, ok := passport[v]; !ok {
			return false
		}
	}

	byr, err := strconv.Atoi(passport["byr"])
	if err != nil || byr < 1920 || byr > 2002 {
		return false
	}

	iyr, err := strconv.Atoi(passport["iyr"])
	if err != nil || iyr < 2010 || iyr > 2020 {
		return false
	}

	eyr, err := strconv.Atoi(passport["eyr"])
	if err != nil || eyr < 2020 || eyr > 2030 {
		return false
	}

	hgt := passport["hgt"]
	height, err := strconv.Atoi(hgt[:len(hgt)-2])
	if err != nil {
		return false
	}
	hUnit := hgt[len(hgt)-2:]
	if hUnit == "cm" {
		if height < 150 || height > 193 {
			return false
		}
	} else if hUnit == "in" {
		if height < 59 || height > 76 {
			return false
		}
	} else {
		return false
	}

	hcl := passport["hcl"]
	if matched, err := regexp.MatchString(`^#[0-9a-f]{6}$`, hcl); !matched || err != nil {
		return false
	}

	ecl := passport["ecl"]
	if !contains(eyeColors, ecl) {
		return false
	}

	pid := passport["pid"]
	if matched, err := regexp.MatchString(`^[0-9]{9}$`, pid); !matched || err != nil {
		return false
	}

	return true
}

func contains(arr []string, str string) bool {
	for _, v := range arr {
		if v == str {
			return true
		}
	}
	return false
}
