using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Devices.Application.Devices;

namespace Devices.Tests.Integration;

public class DevicesApiTests :
    IClassFixture<DevicesApiFactory>,
    IAsyncLifetime
{
    private readonly DevicesApiFactory _factory;
    private readonly HttpClient _client;

    public DevicesApiTests(DevicesApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        await _factory.ResetDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateAndGetDevice_ShouldSucceed()
    {
        // Create
        var createResponse = await _client.PostAsJsonAsync(
            "/api/devices",
            new
            {
                name = "iPhone 15",
                brand = "Apple",
                state = "available"
            });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdBody =
            await createResponse.Content.ReadFromJsonAsync<CreateResponse>();

        Assert.NotNull(createdBody);

        // Get
        var getResponse =
            await _client.GetAsync($"/api/devices/{createdBody!.Id}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetDevices_ByBrand_ShouldReturnMatchingDevices()
    {
        await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "Available");

        await CreateDeviceAsync(
            "Galaxy",
            "Samsung",
            "Available");

        var response =
            await _client.GetAsync("/api/devices?brand=Apple");

        response.EnsureSuccessStatusCode();

        var devices =
            await response.Content.ReadFromJsonAsync<List<DeviceResponse>>(JsonOptions);

        Assert.NotNull(devices);

        Assert.Single(devices);
        Assert.Equal("Apple", devices[0].Brand);
    }

    [Fact]
    public async Task GetDevices_ByState_ShouldReturnMatchingDevices()
    {
        await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "Available");

        await CreateDeviceAsync(
            "Galaxy",
            "Samsung",
            "Inactive");

        var response =
            await _client.GetAsync("/api/devices?state=Available");

        response.EnsureSuccessStatusCode();

        var devices =
            await response.Content.ReadFromJsonAsync<List<DeviceResponse>>(JsonOptions);

        Assert.NotNull(devices);

        Assert.Single(devices);
        Assert.Equal("Apple", devices[0].Brand);
    }

    [Fact]
    public async Task Put_ShouldUpdateDevice()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "Available");

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/devices/{id}",
            new
            {
                name = "iPhone 16",
                brand = "Apple",
                state = "InUse"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            updateResponse.StatusCode);

        var getResponse =
            await _client.GetAsync($"/api/devices/{id}");

        getResponse.EnsureSuccessStatusCode();

        var device =
            await getResponse.Content.ReadFromJsonAsync<DeviceResponse>(JsonOptions);

        Assert.NotNull(device);

        Assert.Equal("iPhone 16", device.Name);
        Assert.Equal("Apple", device.Brand);
        Assert.Equal("InUse", device.State.ToString());
    }

    [Fact]
    public async Task Put_ShouldReturnNotFound_WhenDeviceDoesNotExist()
    {
        var response = await _client.PutAsJsonAsync(
            $"/api/devices/{Guid.NewGuid()}",
            new
            {
                name = "iPhone",
                brand = "Apple",
                state = "Available"
            });

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Patch_ShouldUpdateOnlyProvidedFields()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "Available");

        var patchResponse = await _client.PatchAsJsonAsync(
            $"/api/devices/{id}",
            new
            {
                state = "Inactive"
            });

        Assert.Equal(
            HttpStatusCode.NoContent,
            patchResponse.StatusCode);

        var getResponse =
            await _client.GetAsync($"/api/devices/{id}");

        getResponse.EnsureSuccessStatusCode();

        var device =
            await getResponse.Content.ReadFromJsonAsync<DeviceResponse>(JsonOptions);

        Assert.NotNull(device);

        Assert.Equal("iPhone", device.Name);
        Assert.Equal("Apple", device.Brand);
        Assert.Equal("Inactive", device.State.ToString());
    }

    [Fact]
    public async Task Patch_ShouldReturnConflict_WhenChangingBrandOfInUseDevice()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "InUse");

        var response = await _client.PatchAsJsonAsync(
            $"/api/devices/{id}",
            new
            {
                brand = "Samsung"
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Patch_ShouldReturnConflict_WhenChangingNameOfInUseDevice()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "InUse");

        var response = await _client.PatchAsJsonAsync(
            $"/api/devices/{id}",
            new
            {
                name = "New Name"
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Patch_ShouldReturnNotFound_WhenDeviceDoesNotExist()
    {
        var response = await _client.PatchAsJsonAsync(
            $"/api/devices/{Guid.NewGuid()}",
            new
            {
                state = "Inactive"
            });

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldSoftDeleteDevice()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "Available");

        var deleteResponse =
            await _client.DeleteAsync($"/api/devices/{id}");

        Assert.Equal(
            HttpStatusCode.NoContent,
            deleteResponse.StatusCode);

        var getResponse =
            await _client.GetAsync($"/api/devices/{id}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenDeviceDoesNotExist()
    {
        var response = await _client.DeleteAsync(
            $"/api/devices/{Guid.NewGuid()}");

        Assert.Equal(
            HttpStatusCode.NotFound,
            response.StatusCode);
    }

    [Fact]
    public async Task Put_ShouldReturnConflict_WhenChangingBrandOfInUseDevice()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "InUse");

        var response = await _client.PutAsJsonAsync(
            $"/api/devices/{id}",
            new
            {
                name = "iPhone",
                brand = "Samsung",
                state = "InUse"
            });

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnConflict_WhenDeviceIsInUse()
    {
        var id = await CreateDeviceAsync(
            "iPhone",
            "Apple",
            "InUse");

        var response =
            await _client.DeleteAsync($"/api/devices/{id}");

        Assert.Equal(
            HttpStatusCode.Conflict,
            response.StatusCode);
    }

    private sealed record CreateResponse(Guid Id);

    private async Task<Guid> CreateDeviceAsync(
        string name,
        string brand,
        string state)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/devices",
            new
            {
                name,
                brand,
                state
            });

        response.EnsureSuccessStatusCode();

        var result =
            await response.Content.ReadFromJsonAsync<CreateResponse>();

        return result!.Id;
    }

    private static readonly JsonSerializerOptions JsonOptions =
        new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}