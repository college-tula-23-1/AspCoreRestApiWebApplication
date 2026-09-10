using AspCoreRestApiWebApplication;
using System.Text.RegularExpressions;

List<Employee> employees = new List<Employee>()
{
    new(){ Id = Guid.NewGuid().ToString(), Name = "Bobby", Age = 25 },
    new(){ Id = Guid.NewGuid().ToString(), Name = "Sammy", Age = 31 },
    new(){ Id = Guid.NewGuid().ToString(), Name = "Jimmy", Age = 19 },
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.Run(async context =>
{
    var request = context.Request;
    var response = context.Response;
    var path = request.Path;
    var method = request.Method;

    string guidPattern = @"^/api/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";

    // GET ALL
    if(path == "/api" && method == "GET")
    {
        await response.WriteAsJsonAsync(employees);
    }
    // GET ONE
    else if(Regex.IsMatch(path, guidPattern) && method == "GET")
    {
        string? id = path.Value?.Split("/")[2];

        Employee? employee = employees.FirstOrDefault(e => e.Id == id);

        if(employee is not null)
            await response.WriteAsJsonAsync(employee);
        else
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            await response.WriteAsJsonAsync(new { message = "Employee not found" });
        }
    }
    // POST / CREATE NEW
    else if(path == "/api" && method == "POST")
    {
        try
        {
            Employee? employee = await request.ReadFromJsonAsync<Employee>();
            if(employee is not null)
            {
                employee.Id = Guid.NewGuid().ToString();
                employees.Add(employee);
                await response.WriteAsJsonAsync(employee);
            }
            else
            {
                throw new Exception();
            }
        }
        catch(Exception)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            await response.WriteAsJsonAsync(new { message = "Incorrect data" });
        }
    }
    // PUT / UPDATE
    else if(path == "/api" && method == "PUT")
    {
        try
        {
            Employee? employeeClient = await request.ReadFromJsonAsync<Employee>();

            if (employeeClient is not null)
            {
                Employee? employee = employees.FirstOrDefault(e => e.Id == employeeClient.Id);

                if(employee is not null)
                {
                    employee.Name = employeeClient.Name;
                    employee.Age = employeeClient.Age;
                    await response.WriteAsJsonAsync(employee);
                }
                else
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    await response.WriteAsJsonAsync(new { message = "Employee not found" });
                }
            }
            else
                throw new Exception();
        }
        catch(Exception)
        {
            response.StatusCode = StatusCodes.Status400BadRequest;
            await response.WriteAsJsonAsync(new { message = "Incorrect data )))" });
        }
    }
    // DELETE
    else if(Regex.IsMatch(path, guidPattern) && method == "DELETE")
    {
        string? id = path.Value?.Split("/")[2];

        Employee? employee = employees.FirstOrDefault(e => e.Id == id);
        if(employee is not null)
        {
            employees.Remove(employee);
            await response.WriteAsJsonAsync(employee);
        }
        else
        {
            response.StatusCode = StatusCodes.Status404NotFound;
            await response.WriteAsJsonAsync(new { message = "Employee not found" });
        }
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("index.html");
    }
});


app.Run();
