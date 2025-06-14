# Overview

This application reads the stock trades made by politicians by scraping the capitoltrades.com site and storing the trade data in a json file. Information telling the application how to run is stored in a config file which designates information like which politicians' trades to scrape, which tickers/issuers to scrape, and the number of trades to pull in each batch. The application is smart enough to know when it has pulled a full batch of trades based on the specified search parameters.

I built this application as a start towards verifying whether it's true that US politicians trade stock at a significantly higher margin compared to market averages, and, if so, potentially integrate with an API that makes trades in my portfolio that duplicate high-performing politicians.

# Development Environment

I used VS Code and Microsoft's C++ plugin to develop the application.

I leveraged the following languages/libaries:
* C++
* Homebrew
* curl
* json.hpp (open source library by Niels Lohmann that facilitates easy json reading/writing)
* Regex

# Useful Websites

- [curl Install for MacOS](https://everything.curl.dev/install/macos.html)
- [Json.hpp by Niels Lohmann](https://github.com/nlohmann/json)
- [Get Current Date/Time in C++](https://www.w3schools.com/cpp/cpp_date.asp)
- [Regex in C++](https://www.softwaretestinghelp.com/regex-in-cpp/)

# Future Work

- Build algorithms that identify high trade performers
- Integrate with Alpaca API to automate parallel trades
- Implement notification engine when key tickers are traded and/or large amounts are traded