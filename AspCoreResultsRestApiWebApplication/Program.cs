using AspCoreRestApiWebApplication;

var builder = WebApplication.CreateBuilder(args);

List<Employee> employees = new List<Employee>()
{
    new(){ Id = Guid.NewGuid().ToString(), Name = "Bobby", Age = 25 },
    new(){ Id = Guid.NewGuid().ToString(), Name = "Sammy", Age = 31 },
    new(){ Id = Guid.NewGuid().ToString(), Name = "Jimmy", Age = 19 },
};

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api", () => employees);

app.MapGet("/api/{id}", (string id) =>
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee is null)
        return Results.NotFound(new { Message = "Employee not found" });

    return Results.Json(employee);
});

app.MapPost("/api", (Employee employee) =>
{
    employee.Id = Guid.NewGuid().ToString();
    employees.Add(employee);
    return Results.Json(employee);
});

app.MapPut("/api", (Employee employeeClient) =>
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == employeeClient.Id);

    if (employee is null)
        return Results.NotFound(new { Message = "Employee not found" });

    employee.Name = employeeClient.Name;
    employee.Age = employeeClient.Age;

    return Results.Json(employee);
});

app.MapDelete("/api/{id}", (string id) =>
{
    Employee? employee = employees.FirstOrDefault(e => e.Id == id);

    if (employee is null)
        return Results.NotFound(new { Message = "Employee not found" });

    employees.Remove(employee);
    return Results.Json(employee);
});

app.Run();
