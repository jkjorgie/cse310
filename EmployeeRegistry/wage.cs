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
        
    }

    public override string ToString()
    {
        return $"Wage: {Amount} {Currency} ({PayType})";
    }
}