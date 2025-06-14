#include "trade.h"
#include <sstream>

Trade::Trade(const std::string& name, const std::string& date, const std::string& type, const std::string& amount)
    : name_(name), date_(date), type_(type), amount_(amount) {}

const std::string& Trade::getName() const {
    return name_;
}

const std::string& Trade::getDate() const {
    return date_;
}

const std::string& Trade::getType() const {
    return type_;
}

const std::string& Trade::getAmount() const {
    return amount_;
}

void Trade::setName(const std::string& name) {
    name_ = name;
}

void Trade::setDate(const std::string& date) {
    date_ = date;
}

void Trade::setType(const std::string& type) {
    type_ = type;
}

void Trade::setAmount(const std::string& amount) {
    amount_ = amount;
}

void Trade::setCompany(const std::string& company) {
    company_ = company;
}

void Trade::setTicker(const std::string& ticker) {
    ticker_ = ticker;
}

std::string Trade::toJSONObj() const {
    std::ostringstream oss;
    oss << "  {\n"
        << "    \"name\": \"" << name_ << "\",\n"
        << "    \"company\": \"" << company_ << "\",\n"
        << "    \"ticker\": \"" << ticker_ << "\",\n"
        << "    \"date\": \"" << date_ << "\",\n"
        << "    \"type\": \"" << type_ << "\",\n"
        << "    \"amount\": \"" << amount_ << "\"\n"
        << "  }";
    return oss.str();
}
 
std::string Trade::toString() const {
    std::ostringstream oss;
    oss << "Name: " << name_ << "\n"
        << "Date: " << date_ << "\n"
        << "Type: " << type_ << "\n"
        << "Amount: " << amount_ << "\n"
        << "Company: " << company_ << "\n"
        << "Ticker: " << ticker_ << "\n"
        << "--------------------------";
    return oss.str();
}