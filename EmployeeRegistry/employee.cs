using System;

public class Employee
{
    public string emplid;
    public string name;
    public Job job;
    public Wage wage;
    public DateTime startDate;
    public DateTime terminationDate;
    public string status;

    public Employee(string _emplid, string _name)
    {
        this.emplid = _emplid;
        this.name = _name;
    }

    public Employee(string _emplid, string _name, DateTime _startDate, Job _job, Wage _wage, DateTime _terminationDate, string _status)
    {
        this.emplid = _emplid;
        this.name = _name;
        this.job = _job;
        this.wage = _wage;
        this.startDate = _startDate;
        this.terminatedDate = _terminationDate;
        this.status = _status;
    }

    public void Onboard(Job _job, decimal _salary, DateTime _startDate)
    {
        this.job = _job;
        this.salary = _salary;
        this.startDate = _startDate;
        this.status = "ACTIVE"";
    }

    public void Terminate(DateTime _terminationDate)
    {
        this.terminationDate = _terminationDate;
        this.status = "TERMINATED";
    }

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine($"ID: {this.emplid}");
        sb.AppendLine($"Name: {this.name}");
        sb.AppendLine($"Job: {this.job.title}");
        sb.AppendLine($"Status: {this.status}");
        sb.AppendLine($"Wage: ${this.wage.ToString()}");
        sb.AppendLine($"Start Date: {this.startDate}");

        if (this.status == "TERMINATED")
            sb.AppendLine($"Termination Date: {this.terminationDate.Value}");

        return sb.ToString();
    }
}