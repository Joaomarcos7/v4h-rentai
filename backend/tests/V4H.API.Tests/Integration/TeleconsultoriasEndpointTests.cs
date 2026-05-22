using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using V4H.Domain.Enums;
using Xunit;

namespace V4H.API.Tests.Integration;

public class TeleconsultoriasEndpointTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TeleconsultoriasEndpointTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateClient() => _factory.CreateClient();

    private async Task<string> RegisterAndLoginAsync(HttpClient client, string email, UserRole role)
    {
        var registerResp = await client.PostAsJsonAsync("/api/auth/register", new
        {
            name = "Test User",
            email,
            password = "Pass123!",
            role = (int)role
        });

        registerResp.EnsureSuccessStatusCode();

        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email,
            password = "Pass123!"
        });

        loginResp.EnsureSuccessStatusCode();
        var json = await loginResp.Content.ReadFromJsonAsync<JsonElement>();
        return json.GetProperty("token").GetString()!;
    }

    [Fact]
    public async Task CreateTeleconsultoria_AsSolicitante_Returns201()
    {
        var client = CreateClient();
        var token = await RegisterAndLoginAsync(client, $"sol_{Guid.NewGuid()}@test.com", UserRole.Solicitante);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await client.PostAsJsonAsync("/api/teleconsultorias", new
        {
            patientName = "Paciente Teste",
            birthDate = "1990-01-01",
            specialty = 1,
            diagnosticHypothesis = "Hipótese",
            clinicalHistory = "Histórico clínico detalhado"
        });

        Assert.Equal(HttpStatusCode.Created, resp.StatusCode);
    }

    [Fact]
    public async Task CreateTeleconsultoria_AsEspecialista_Returns403()
    {
        var client = CreateClient();
        var token = await RegisterAndLoginAsync(client, $"esp_{Guid.NewGuid()}@test.com", UserRole.Especialista);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await client.PostAsJsonAsync("/api/teleconsultorias", new
        {
            patientName = "P",
            birthDate = "1990-01-01",
            specialty = 1,
            diagnosticHypothesis = "H",
            clinicalHistory = "C"
        });

        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task ListTeleconsultorias_Authenticated_Returns200()
    {
        var client = CreateClient();
        var token = await RegisterAndLoginAsync(client, $"list_{Guid.NewGuid()}@test.com", UserRole.Especialista);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await client.GetAsync("/api/teleconsultorias");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task GetTeleconsultoria_NotFound_Returns404()
    {
        var client = CreateClient();
        var token = await RegisterAndLoginAsync(client, $"notfound_{Guid.NewGuid()}@test.com", UserRole.Especialista);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var resp = await client.GetAsync($"/api/teleconsultorias/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task ListTeleconsultorias_Unauthenticated_Returns401()
    {
        var client = CreateClient();
        var resp = await client.GetAsync("/api/teleconsultorias");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }
}
