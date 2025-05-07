public struct Wage
{
    public decimal amount;
    public string payType; // H - Hourly, S - Salary
    public string currency; // USD, EUR, etc

    public Wage(decimal _amount, string _payType, string _currency)
    {
        this.amount = _amount;
        this.payType = _payType;
        this.currency = _currency;
    }

    public Wage(string _wage)
    {
        // _wage will look like "50000.00 USD (S)"

        string[] wageParts = _wage.Split(" ");
        this.amount = Decimal.Parse(wageParts[0]);
        this.currency = wageParts[1];
        this.payType = wageParts[2].Substring(1,1); //this will look like (S) for salary, so need to pull middle char
    }

    public override string ToString()
    {
        return $"{this.amount} {this.currency} ({this.payType})";
    }
}