var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages(); // Add Razor Pages service

// Add CORS policy here
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:44345") // Allow requests from your frontend
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Serve static files from wwwroot
app.UseRouting(); // Add routing support

app.UseCors(); // Use CORS middleware after routing

app.UseAuthorization(); // Authorization middleware (optional)

// Map Razor Pages (for serving index.cshtml)
app.MapRazorPages();

// Car data
var cars = new List<Car>
{
    new Car { Id = 1, Make = "Toyota", Model = "Corolla", Year = 2020 },
    new Car { Id = 2, Make = "Honda", Model = "Civic", Year = 2019 }
};

// Root endpoint
app.MapGet("/", () => "Backend is running.");

// Get all cars
app.MapGet("/cars", () => cars)
   .WithName("GetAllCars");

// Get car by ID
app.MapGet("/cars/{id}", (int id) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    return car is not null ? Results.Ok(car) : Results.NotFound();
})
.WithName("GetCarById");

// Create a new car
app.MapPost("/cars", (Car newCar) =>
{
    newCar.Id = cars.Max(c => c.Id) + 1;
    cars.Add(newCar);
    return Results.Created($"/cars/{newCar.Id}", newCar);
})
.WithName("CreateCar");

// Update an existing car
app.MapPut("/cars/{id}", (int id, Car updatedCar) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    if (car is null) return Results.NotFound();

    car.Make = updatedCar.Make;
    car.Model = updatedCar.Model;
    car.Year = updatedCar.Year;
    return Results.NoContent();
})
.WithName("UpdateCar");

// Delete a car
app.MapDelete("/cars/{id}", (int id) =>
{
    var car = cars.FirstOrDefault(c => c.Id == id);
    if (car is null) return Results.NotFound();

    cars.Remove(car);
    return Results.NoContent();
})
.WithName("DeleteCar");

app.Run();

// Car record definition
record Car
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}
