using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SoftUni.Data;
using SoftUni.Models;

namespace SoftUni
{
    public class StartUp
    {
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();

            var employees = context
                .Employees
                .Select(x => new
                {
                    x.EmployeeId,
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary
                }
                )
                .OrderBy(x => x.EmployeeId)
                .ToList();

            foreach (var employee in employees)
            {
                sb.AppendLine(
                    $"{employee.FirstName} {employee.LastName} {employee.MiddleName} {employee.JobTitle} {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }


        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context
                .Employees.Select(x => new
                {
                    x.FirstName,
                    x.Salary
                })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName)
                .ToList();


            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} - {employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Department,
                    x.Salary
                })
                .Where(x => x.Department.Name == "Research and Development")
                .OrderBy(x => x.Salary)
                .ThenByDescending(x => x.FirstName);

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine(
                    $"{employee.FirstName} {employee.LastName} from {employee.Department.Name} - ${employee.Salary:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            context.Employees.First(x => x.LastName == "Nakov").Address = address;

            context.SaveChanges();

            var employees = context
                .Employees
                .Select(x => new
                {
                    x.Address.AddressId,
                    x.Address.AddressText
                })
                .OrderByDescending(x => x.AddressId)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context
                .EmployeesProjects
                .Where(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003)
                .Select(x => new
                {
                    x.Employee.EmployeeId,
                    x.Employee.FirstName,
                    x.Employee.LastName,
                    x.Employee.Manager
                })
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                var empProjects = context
                    .EmployeesProjects
                    .Select(x => new
                    {
                        x.EmployeeId,
                        x.Project
                    })
                    .Where(x => x.EmployeeId == employee.EmployeeId)
                    .ToList();

                sb.AppendLine(
                    $"{employee.FirstName} {employee.LastName} - Manager: {employee.Manager.FirstName} {employee.Manager.LastName}");


                var employeeProjects = context
                    .EmployeesProjects
                    .Where(x => x.EmployeeId == employee.EmployeeId);

                foreach (var employeeProject in employeeProjects)
                {
                    string projectStartDate = employeeProject.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt");
                    string projectEndDate;

                    if (employeeProject.Project.EndDate == null)
                    {
                        projectEndDate = "not finished";
                    }
                    else
                    {
                        projectEndDate = employeeProject.Project.EndDate?.ToString("M/d/yyyy h:mm:ss tt");
                    }

                    sb.AppendLine(
                        $"--{employeeProject.Project.Name} - {employeeProject.Project.StartDate:M/d/yyyy h:mm:ss tt} - {projectEndDate}");
                }
            }


            return sb.ToString().TrimEnd();
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context
                .Addresses
                .Select(x => new
                {
                    x.AddressText,
                    x.Town.Name,
                    x.Employees.Count
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var address in addresses)
            {
                sb.AppendLine($"{address.AddressText}, {address.Name} - {address.Count} employees");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147 = context
                .Employees
                .Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    EmployeeProjects = x.EmployeesProjects
                        .Select(p => new
                        {
                            p.Project.Name
                        })
                })
                .First();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{employee147.FirstName} {employee147.LastName} - {employee147.JobTitle}");

            foreach (var employeeProject in employee147
                .EmployeeProjects
                .OrderBy(x => x.Name))
            {
                sb.AppendLine($"{employeeProject.Name}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context
                .Departments
                .Where(x => x.Employees.Count > 5)
                .OrderBy(x => x.Employees.Count())
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    DepName = x.Name,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    EmployeesCollection = x.Employees
                        .OrderBy(x => x.FirstName)
                        .ThenByDescending(x => x.LastName)
                        .ToList()
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var department in departments)
            {
                sb.AppendLine($"{department.DepName} - {department.ManagerFirstName} {department.ManagerLastName}");


                foreach (var employee in department.EmployeesCollection)
                {
                    sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var latestProjects = context
                .Projects.Select(x => new
                {
                    x.StartDate,
                    x.Description,
                    x.Name
                })
                .OrderByDescending(x => x.StartDate)
                .Take(10)
                .OrderBy(x => x.Name)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var latestProject in latestProjects)
            {
                sb.AppendLine($"{latestProject.Name}");
                sb.AppendLine($"{latestProject.Description}");
                sb.AppendLine($"{latestProject.StartDate.ToString("M/d/yyyy h:mm:ss tt")}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            string[] departmentsGettingSalaryIncreased = new string[]
            {
                "Engineering",
                "Tool Design",
                "Marketing",
                "Information Services"
            };

            var employeesWithIncreasedSalary = context
                .Employees
                .Where(x => departmentsGettingSalaryIncreased.Contains(x.Department.Name))
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();


            foreach (var employee in employeesWithIncreasedSalary)
            {
                employee.Salary *= 1.12m;
            }

            context.SaveChanges();

            foreach (var employee in employeesWithIncreasedSalary)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context
                .Employees
                .Where(x => EF.Functions.Like(x.FirstName,"Sa%"))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                })
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var employee in employees)
            {
                sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle} - (${employee.Salary:F2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static void Main(string[] args)
        {
            var dbContext = new SoftUniContext();
            string result = string.Empty;

            //2.Database First exercise - n/a

            //3.Employees Full Information

            //result = GetEmployeesFullInformation(dbContext);

            //4.Employees with Salary Over 50 000 in

            //result = GetEmployeesWithSalaryOver50000(dbContext);

            //5.Employees from Research and Development

            //result = GetEmployeesFromResearchAndDevelopment(dbContext);

            //6.Adding a New Address and Updating Employee

            //result = AddNewAddressToEmployee(dbContext);

            //7.Employees and Projects

            //result = GetEmployeesInPeriod(dbContext);

            //8.Address by Town

            //result = GetAddressesByTown(dbContext);

            //9.Employee 147

            //result = GetEmployee147(dbContext);

            //10.Departments with More Than 5 Employees

            //result = GetDepartmentsWithMoreThan5Employees(dbContext);

            //11.Find Latest 10 Projects

            //result = GetLatestProjects(dbContext);

            //12.Increase Salaries

            //result = IncreaseSalaries(dbContext);

            //13.Find Employees by First Name Starting with "Sa"

            result = GetEmployeesByFirstNameStartingWithSa(dbContext);

            Console.WriteLine(result);
        }
    }
}