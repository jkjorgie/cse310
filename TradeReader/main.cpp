#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <regex>
#include <filesystem>
#include <curl/curl.h>
#include "trade.h"
#include <chrono>
#include <thread>
#include <ctime>
#include <iomanip>
#include "json.hpp" // credit: Niels Lohmann

// globals
std::vector<Trade> trades;
std::vector<std::string> politicians;
std::string url;
int pageSize;

using json = nlohmann::json;

// outputs log with timestamp
void Log(std::string text_)
{
    auto now = std::chrono::system_clock::now();
    std::time_t now_c = std::chrono::system_clock::to_time_t(now);
    std::cout << std::put_time(std::localtime(&now_c), "%Y-%m-%d %H:%M:%S") << " " << text_ << std::endl;
}

size_t WriteCallback(void *contents, size_t size, size_t nmemb, void *userp)
{
    ((std::string *)userp)->append((char *)contents, size * nmemb);
    return size * nmemb;
}

// reads config file so variables aren't hardcoded
void ReadConfig() {

    Log("Reading config...");

    std::ifstream infile("data/config.json");
    if (!infile) {
        Log("Failed to open data/trade-registry.json");
        return;
    }

    json j;
    infile >> j;

    // read general properties
    url = j["targetUrl"];
    pageSize = j["pageSize"];
    Log("Target URL: " + url);
    Log("Page Size: " + std::to_string(pageSize));

    politicians.clear();

    // read all politician IDs to watch
    for (const auto& entry : j["politicians"]) {
        politicians.push_back(entry["politician"]);
    }
}

std::string ReadHtml(std::string url_)
{
    CURL *curl;
    CURLcode res;
    std::string respHtml;

    // define curl obj
    curl = curl_easy_init();

    // set curl params
    curl_easy_setopt(curl, CURLOPT_URL, url_.c_str()); // nancy pelosi's trade manifest
    curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, WriteCallback);
    curl_easy_setopt(curl, CURLOPT_WRITEDATA, &respHtml);

    // call curl action
    res = curl_easy_perform(curl);
    curl_easy_cleanup(curl);

    if (res == CURLE_OK)
    {
        // std::cout << "HTML content:\n" << respHtml << std::endl;
        // Log("Curl successful...");
    }
    else
    {
        std::ostringstream oss;
        oss << "Curl error: " << curl_easy_strerror(res);
        Log(oss.str());
    }

    return respHtml;
}

void ReadTrades(std::string html_)
{

    // define regex to pull data from each html row
    std::regex row_regex(R"(<tr.*?>.*?</tr>)", std::regex::icase);
    std::regex name_regex(R"(<a[^>]*href="\/politicians\/[^"]+">([^<]+)<\/a>)");
    std::regex trade_date_regex(R"(<div class="text-size-3 font-medium">(\d{1,2} \w+)<\/div>\s*<div class="text-size-2 text-txt-dimmer">(\d{4})<\/div>)");
    std::regex type_regex(R"(<span class="q-field tx-type[^>]*>(buy|sell)<\/span>)");
    std::regex amount_regex(R"(<span class="mt-1[^>]*">([^<]+)<\/span>)");
    std::regex company_regex(R"(<h3 class="q-fieldset issuer-name"><a[^>]*>([^<]+)</a></h3>)");
    std::regex ticker_regex(R"(<span class="q-field issuer-ticker">([^<]+)</span>)");

    auto rows_begin = std::sregex_iterator(html_.begin(), html_.end(), row_regex);
    auto rows_end = std::sregex_iterator();

    // loop through every row in the html body
    for (auto i = rows_begin; i != rows_end; i++)
    {
        std::string row = i->str();
        std::smatch match;

        std::string name, trade_date, trade_type, amount, company, ticker;

        // finding matching target data based on regex strings
        if (std::regex_search(row, match, name_regex))
            name = match[1];
        if (std::regex_search(row, match, trade_date_regex))
            trade_date = match[1].str() + " " + match[2].str();
        if (std::regex_search(row, match, type_regex))
            trade_type = match[1];
        if (std::regex_search(row, match, amount_regex))
            amount = match[1];
        if (std::regex_search(row, match, company_regex))
            company = match[1];
        if (std::regex_search(row, match, ticker_regex))
            ticker = match[1];

        // if the data is found, i.e. the row isn't empty (header row), create a new Trade and add it to the trades vector
        if (!name.empty())
        {
            Trade trade;
            trade.setName(name);
            trade.setDate(trade_date);
            trade.setType(trade_type);
            trade.setAmount(amount);
            trade.setCompany(company);
            trade.setTicker(ticker);
            trades.push_back(trade);

            // std::cout << trade.toString() << "\n";
        }
    }
}

// Write trades vector to json
void SaveTradesToJson()
{
    std::ofstream outfile("data/trade-registry.json");
    outfile << "[\n";
    for (size_t i = 0; i < trades.size(); i++)
    {
        outfile << trades[i].toJSONObj();
        if (i < trades.size() - 1)
            outfile << ",";
        outfile << "\n";
    }
    outfile << "]\n";
    outfile.close();
}

// generate base url
std::string GetBaseUrl() {

    std::string poliParams = "";
    for (size_t i = 0; i < politicians.size(); i++)
    {
        if (i > 0) {
            poliParams += "&";
        }
        poliParams += "politician=" + politicians[i];
    }

    std::string fullUrl = url + "?" + poliParams + "&pageSize=" + std::to_string(pageSize);

    Log("Base url: " + fullUrl);

    return fullUrl;
}

int main()
{
    // loop every 5 min until app is killed with ctrl+c in terminal
    while (true)
    {
        // read app config json
        ReadConfig();

        Log("Starting trade reads");

        // read html
        std::string baseUrl = GetBaseUrl();

        // pull trade details from html and load into trades vector
        int count = 1;
        std::string pageUrl;
        while (true)
        {
            pageUrl = baseUrl + "&page=" + std::to_string(count);
            Log("Pinging/reading page: " + pageUrl);
            std::string respHtml = ReadHtml(pageUrl);
            ReadTrades(respHtml);

            // if the trades vector remainder after dividing by the pageSize is 0, probably there's another page to look at
            // if not, we can safely assume we're done
            if (trades.size() % pageSize != 0)
            {
                break;
            }

            count++;
        }

        // write trades to json file
        SaveTradesToJson();

        Log("Finished trade reads... waiting for next cycle");

        // wait for 12 hours before pinging again
        std::this_thread::sleep_for(std::chrono::hours(12));
    }

    return 0;
}