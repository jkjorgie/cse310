# Overview

This application is a simple employee management system designed for an HR user to take basic actions. It facilitates the following:

- Viewing all employees
- Viewing all jobs
- Hiring new employees
- Terminating existing, active employees
- Editing employee data for existing employees
- Viewing data for a specific employee

Additionally, it saves all changes by writing data out to csv files.

My purpose was to extend my C# knowledge and comfort level, building a simple, yet robust, HR employee management system that can be easily extended for additional functionality.

# Development Environment

I built this app in VS Code with C# Dev Kit delivered by Microsoft, as well as RainbowCSV.

I used C# and its delivered libraries for the application.

# Useful Websites

- [Use Visual C# to read from and write to a text file](https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/read-write-text-file)
- [Converting a String to DateTime](https://stackoverflow.com/questions/919244/converting-a-string-to-datetime)
- [Instantiating a Null DateTime](https://stackoverflow.com/questions/221732/datetime-null-uninitialized-value)
- [Get Relative File Path in C#](https://stackoverflow.com/questions/40994534/get-relative-path-of-a-file-c-sharp)

# Future Work

- Add/Change jobs
- Validate wages to ensure they fall within the min/max for the specified jobcode
- Handle incorrect user entry more cohesively everywhere in the app
