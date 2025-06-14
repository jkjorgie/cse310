#ifndef TRADE_H
#define TRADE_H

#include <string>

class Trade {
public:
    Trade() = default;
    Trade(const std::string& name, const std::string& date, const std::string& type, const std::string& amount);

    const std::string& getName() const;
    const std::string& getDate() const;
    const std::string& getType() const;
    const std::string& getAmount() const;
    const std::string& getCompany() const;
    const std::string& getTicker() const;

    void setName(const std::string& name);
    void setDate(const std::string& date);
    void setType(const std::string& type);
    void setAmount(const std::string& amount);
    void setCompany(const std::string& company);
    void setTicker(const std::string& ticket);

    std::string toString() const;
    std::string toJSONObj() const;

private:
    std::string name_;
    std::string date_;
    std::string type_;
    std::string amount_;
    std::string company_;
    std::string ticker_;
};
 
#endif // TRADE_H