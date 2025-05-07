using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

class Program
{
    static List<Employee> employees;
    static List<Job> jobs;
    static void Main(string[] args)
    {
        ReadJobDataFromFile();
        ReadEmployeeRegistryFromFile();
        WelcomeUser();

        int resp = -1;
        while (resp != 0)
        {
            resp = PromptForAction();
            HandleAction(resp);
        }

        Save();

        Console.WriteLine("");
        Console.WriteLine("Your work has been saved. Press any key to exit.");
        Console.ReadLine();
    }

    static void WelcomeUser()
    {
        Console.WriteLine("Welcome!");
    }
    static int PromptForAction()
    {
        Console.WriteLine("What would you like to do?");
        Console.WriteLine("  [1] - View Employee Registry");
        Console.WriteLine("  [2] - Hire New Employee");
        Console.WriteLine("  [3] - Terminate Employee");
        Console.WriteLine("  [4] - View Employee Info");
        Console.WriteLine("  [5] - Edit Employee");
        Console.WriteLine("  [6] - View Job Registry");
        //Console.WriteLine("  [7] - Create New Job");
        Console.WriteLine("  [0] - Save & Quit");

        string strResp = Console.ReadLine();
        int resp = 0;
        if (!int.TryParse(strResp, out resp))
        {
            Console.WriteLine($"Invalid Response {strResp}.");
            return PromptForAction();
        }

        return resp;
    }

    static void HandleAction(int _action)
    {
        switch (_action)
        {
            case 0: // Quit
                break;
            case 1: // view employee registry
                ViewEmployeeRegistry();
                break;
            case 2: // hire new employee
                HireNewEmployee();
                break;
            case 3: // terminate employee
                TerminateEmployee();
                break;
            case 4: // view employee info
                ViewEmployeeInfo();
                break;
            case 5: // edit employee
                EditEmployee();
                break;
            case 6: // view job registry
                ViewJobRegistry();
                break;
            // case 7: // create new job
            //     Console.WriteLine("Seven");
            //     break;
            default: // other
                Console.WriteLine("Invalid action indicated...");
                break;
        }

        Console.WriteLine();
    }

    static void ReadJobDataFromFile()
    {
        jobs = new List<Job>();

        string binPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".."); //get relative path to bin
        string jobDataFilePath = Path.Combine(binPath, "job-data.csv"); //get job data file path

        try
        {
            string[] jobLines = File.ReadAllLines(jobDataFilePath);
            foreach (string line in jobLines)
            {
                string[] jobParts = line.Split(",");
                string jobcode = jobParts[0];
                string title = jobParts[1];
                string descr = jobParts[2];
                decimal payRangeStart = decimal.Parse(jobParts[3]);
                decimal payRangeEnd = decimal.Parse(jobParts[4]);

                Job job = new Job(jobcode, title, descr, payRangeStart, payRangeEnd);
                jobs.Add(job);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred reading job data: {ex.Message}");
        }
    }

    static void ReadEmployeeRegistryFromFile()
    {
        employees = new List<Employee>();

        string binPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".."); //get relative path to bin
        string empRegFilePath = Path.Combine(binPath, "employee-registry.csv"); //get employee registry file path

        try
        {
            string[] employeeLines = File.ReadAllLines(empRegFilePath);
            foreach (string line in employeeLines)
            {
                string[] empParts = line.Split(",");
                string emplid = empParts[0];
                string name = empParts[1];
                string jobcode = empParts[2];
                string strWage = empParts[3];
                DateTime startDate = DateTime.Parse(empParts[4]);
                string status = empParts[6];

                Wage wage = new Wage(strWage);
                Job job = GetJobFromRegistry(jobcode);

                DateTime termDate = DateTime.MinValue;
                if (empParts[5] != "")
                {
                    termDate = DateTime.Parse(empParts[5]);
                }
                Employee emp = new Employee(emplid, name, startDate, job, wage, termDate, status);
                employees.Add(emp);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    static Job GetJobFromRegistry(string _jobcode)
    {
        for (int i = 0; i < jobs.Count; i++)
        {
            if (jobs[i].jobcode == _jobcode)
            {
                return jobs[i];
            }
        }

        return null;
    }

    static Employee GetEmployeeFromRegistry(string _emplid)
    {
        for (int i = 0; i < employees.Count; i++)
        {
            if (employees[i].emplid == _emplid)
            {
                return employees[i];
            }
        }

        return null;
    }

    static void ViewEmployeeRegistry()
    {
        Console.WriteLine("Employee Registry:");
        for (int i = 0; i < employees.Count; i++)
        {
            Console.WriteLine(employees[i].ToString());
        }
    }

    static void ViewJobRegistry()
    {
        Console.WriteLine("View Job Registry:");
        for (int i = 0; i < jobs.Count; i++)
        {
            Console.WriteLine(jobs[i].ToString());
        }
    }

    static void HireNewEmployee()
    {
        //collect jobcode
        Console.WriteLine("What is the employee's jobcode (enter '?' to see job registry)?");
        string jobcode = Console.ReadLine();
        if (jobcode == "?")
        {
            ViewJobRegistry();
            HireNewEmployee();
        }
        Job job = GetJobFromRegistry(jobcode);

        //collect name
        Console.WriteLine("What is the employee's name?");
        string name = Console.ReadLine();

        //collect salary
        Console.WriteLine("What will the employee's wage be? [Format = ### USD (S/H)]");
        string strWage = Console.ReadLine();
        Wage wage = new Wage(strWage);

        //collect start date
        Console.WriteLine("What is the employee's start date? [Format = YYYY-MM-DD]");
        DateTime startDate = DateTime.Parse(Console.ReadLine());

        //calculate next emplid
        int maxEmplid = 0;
        int curEmplid = 0;
        for (int i = 0; i < employees.Count; i++)
        {
            curEmplid = int.Parse(employees[i].emplid.Substring(1,4));
            if (curEmplid > maxEmplid)
            {
                maxEmplid = curEmplid;
            }
        }

        Employee emp = new Employee($"E{maxEmplid + 1}", name);
        emp.Onboard(job, wage, startDate);
        employees.Add(emp);
    }

    static void TerminateEmployee()
    {
        Console.WriteLine("Enter the emplid of the employee you with to terminate, or enter '?' if you'd like to see the employee registry.");
        
        string resp = Console.ReadLine();
        if (resp == "?") //see registry
        {
            ViewEmployeeRegistry();
            TerminateEmployee();
        }
        else
        {
            Console.WriteLine("What is the final day of the employee's employment?");
            DateTime termDate = DateTime.Parse(Console.ReadLine());
            GetEmployeeFromRegistry(resp).Terminate(termDate);
            Console.WriteLine("Employee successfully terminated");
            return;
        }
    }

    static void ViewEmployeeInfo()
    {
        Console.WriteLine("Enter the emplid of the employee you wish to view, or enter '?' if you'd like to see the employee registry.");
        string resp = Console.ReadLine();
        if (resp == "?") //see registry
        {
            ViewEmployeeRegistry();
            ViewEmployeeInfo();
        }
        else
        {
            Console.WriteLine(GetEmployeeFromRegistry(resp).ToString());
            return;
        }

        Console.WriteLine("Bad emplid entered...");
    }

    static void EditEmployee()
    {
        Console.WriteLine("Enter the emplid of the employee you with to edit, or enter '?' if you'd like to see the employee registry.");
        
        string resp = Console.ReadLine();
        if (resp == "?") //see registry
        {
            ViewEmployeeRegistry();
            EditEmployee();
        }
        else
        {
            Console.WriteLine("Employee's current info:");
            Employee emp = GetEmployeeFromRegistry(resp);
            Console.WriteLine(emp.ToString());

            //collect name
            Console.WriteLine($"The employee's current name is {emp.name}. What is the employee's new name?");
            string name = Console.ReadLine();
            emp.name = name;

            //collect salary
            Console.WriteLine($"The employee's current wage is {emp.wage.ToString()}. What will the employee's new wage be? [Format = ### USD (S/H)]");
            string strWage = Console.ReadLine();
            Wage wage = new Wage(strWage);
            emp.wage = wage;
        }
    }

    static void Save()
    {
        string binPath = Path.Combine(Directory.GetCurrentDirectory(), "..", ".."); //get relative path to bin
        string filepath = Path.Combine(binPath, "employee-registry.csv"); //get employee registry file path

        StreamWriter sw = new StreamWriter(filepath);

        foreach (var emp in employees)
        {
            string start = emp.startDate.ToString("yyyy-MM-dd");
            string term = "";
            if (emp.terminationDate != DateTime.MinValue)
            {
                term = emp.terminationDate.ToString("yyyy-MM-dd");
            }

            string line = $"{emp.emplid},{emp.name},{emp.job.jobcode},{emp.wage.ToString()},{start},{term},{emp.status}";
            sw.WriteLine(line);
        }

        sw.Close();
    }
}