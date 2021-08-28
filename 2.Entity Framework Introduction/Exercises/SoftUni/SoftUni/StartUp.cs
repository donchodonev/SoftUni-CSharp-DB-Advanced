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

        public static void Main(string[] args)
        {
            var dbContext = new SoftUniContext();

            /*

            //2.Database First exercise

            //3.Employees Full Information

            var result = GetEmployeesFullInformation(softUniDbContext);

            Console.WriteLine(result);

            //4.Employees with Salary Over 50 000 in

            var result = GetEmployeesWithSalaryOver50000(dbContext);

            Console.WriteLine(result);

            //5.Employees from Research and Development

            var result = GetEmployeesFromResearchAndDevelopment(dbContext);

            Console.WriteLine(result);

            //6.Adding a New Address and Updating Employee

            var result = AddNewAddressToEmployee(dbContext);

            Console.WriteLine(result);

            //7.Employees and Projects

            var result = GetEmployeesInPeriod(dbContext);

            Console.WriteLine(result);

            //8.Address by Town

            var result = GetAddressesByTown(dbContext);

            Console.WriteLine(result);


            //9.Employee 147

            var result = GetEmployee147(dbContext);

            Console.WriteLine(result);


            //10.Departments with More Than 5 Employees

            var result = GetDepartmentsWithMoreThan5Employees(dbContext);

            Console.WriteLine(result);

             */

            //11.Find Latest 10 Projects

            var result = GetLatestProjects(dbContext);

            Console.WriteLine(result);
        }
    }
}