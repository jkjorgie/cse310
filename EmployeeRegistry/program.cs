using System;
using System.IO;
using MyApp.Employee;
using MyApp.Job;
using MyApp.Wage;

class Program
{
    static List<Employee> employees;
    static List<Job> jobs;
    static void Main(string[] args)
    {
        employees = new List<Employee>();
        jobs = new List<Job>();

        WelcomeUser();

        int resp = -1;
        while (resp != 0)
        {
            resp = PromptForAction();
            HandleAction(resp);
        }

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
        Console.WriteLine("  [5] - Edit Employee]");
        Console.WriteLine("  [6] - View Job Registry");
        Console.WriteLine("  [7] - Create New Job");
        Console.WriteLine("  [0] - Quit");

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
                Console.WriteLine("Zero");
                break;
            case 1: // view employee registry
                Console.WriteLine("One");
                break;
            case 2: // hire new employee
                Console.WriteLine("Two");
                break;
            case 3: // terminate employee
                Console.WriteLine("Three");
                break;
            case 4: // view employee info
                Console.WriteLine("Four");
                break;
            case 5: // edit employee
                Console.WriteLine("Five");
                break;
            case 6: // view job registry
                Console.WriteLine("Six");
                break;
            case 7: // create new job
                Console.WriteLine("Seven");
                break;
            default: // other
                Console.WriteLine("Invalid action indicated...\n");
                break;
        }
    }

    static void ReadJobDataFromFile()
    {
        jobs = new List<Job>();

        string binPath = Path.Combine(Directory.GetCurrentDirectory(), "bin"); //get relative path with /bin/ appended
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

        string binPath = Path.Combine(Directory.GetCurrentDirectory(), "bin"); //get relative path with /bin/ appended
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
                DateTime termDate = DateTime.Parse(empParts[5]);
                string status = empParts[6];

                Wage wage = new Wage(strWage);
                Job job = GetJobFromRegistry(jobcode);

                Employee emp = new Employee(emplid, name, job, wage, startDate, termDate, status);
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
}