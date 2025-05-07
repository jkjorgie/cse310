using System;

public class Job
{
    public string jobcode;
    public string title;
    public string descr;
    public decimal payRangeStart;
    public decimal payRangeEnd;

    public Job(string _jobcode, string _title, string _descr, decimal _payRangeStart, decimal _payRangeEnd)
    {
        this.jobcode = _jobcode;
        this.title = _title;
        this.descr = _descr;
        this.payRangeStart = _payRangeStart;
        this.payRangeEnd = _payRangeEnd;
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"Jobcode: {this.jobcode}");
        sb.AppendLine($"Title: {this.title}");
        sb.AppendLine($"Description: {this.descr}");
        sb.AppendLine($"Salary Range: ${this.payRangeStart} - ${this.payRangeEnd}");

        return sb.ToString();
    }
}