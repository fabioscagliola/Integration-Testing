using com.fabioscagliola.IntegrationTesting.WebApi;
using com.fabioscagliola.IntegrationTesting.WebApi.Controllers;
using FluentAssertions;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

#nullable disable

namespace com.fabioscagliola.IntegrationTesting.WebApiTest;

public class PersonControllerTest : BaseTest
{
    const string FNAME = "Fabio";
    const string LNAME = "Scagliola";

    [Test]
    public async Task GivenFNameOrLNameAreNullOrEmpty_WhenCreatingPerson_ThenReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        PersonCreateData personCreateData = new() { FName = null, LName = "" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Create", JsonContent.Create(personCreateData));
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string fNameOrLNameAreNullOrEmpty = await httpResponseMessage.Content.ReadAsStringAsync();
        fNameOrLNameAreNullOrEmpty.Should().NotBeNull();
        fNameOrLNameAreNullOrEmpty.Should().Be(PersonController.FNAMEORLNAMEARENULLOREMPTY);
    }

    [Test]
    public async Task GivenFNameAndLNameAreNotNullOrEmpty_WhenCreatingPerson_ThenSucceeds()
    {
        Person person = await CreatePerson(FNAME, LNAME);
        person.Should().NotBeNull();
        person.Id.Should().NotBe(0);
        person.FName.Should().Be(FNAME);
        person.LName.Should().Be(LNAME);
    }

    [Test]
    public async Task GivenNonExistingId_WhenReadingPerson_ThenReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/0");
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        notfound.Should().NotBeNull();
        notfound.Should().Be(PersonController.NOTFOUND);
    }

    [Test]
    public async Task GivenExistingId_WhenReadingPerson_ThenSucceeds()
    {
        Person expected = await CreatePerson(FNAME, LNAME);
        expected.Should().NotBeNull();

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/{expected.Id}");
        httpResponseMessage.EnsureSuccessStatusCode();
        Person actual = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task WhenReadingPersonList_ThenSucceeds()
    {
        Person expected = await CreatePerson(FNAME, LNAME);
        expected.Should().NotBeNull();

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/ReadList");
        httpResponseMessage.EnsureSuccessStatusCode();
        List<Person> people = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(List<Person>)) as List<Person>;
        people.Should().NotBeNull();
        people.Should().NotBeEmpty();
        Person actual = people.SingleOrDefault(x => x.Id == expected.Id);
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task GivenNonExistingPerson_WhenUpdatingPerson_ThenReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        notfound.Should().NotBeNull();
        notfound.Should().Be(PersonController.NOTFOUND);
    }

    [Test]
    public async Task GivenFNameOrLNameAreNullOrEmpty_WhenUpdatingPerson_ThenReturnsBadRequest()
    {
        Person temp = await CreatePerson(FNAME, LNAME);
        temp.Should().NotBeNull();

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new() { Id = temp.Id, FName = null, LName = "" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string fNameOrLNameAreNullOrEmpty = await httpResponseMessage.Content.ReadAsStringAsync();
        fNameOrLNameAreNullOrEmpty.Should().NotBeNull();
        fNameOrLNameAreNullOrEmpty.Should().Be(PersonController.FNAMEORLNAMEARENULLOREMPTY);
    }

    [Test]
    public async Task GivenFNameAndLNameAreNotNullOrEmpty_WhenUpdatingPerson_ThenSucceeds()
    {
        Person temp = await CreatePerson(FNAME, LNAME);
        temp.Should().NotBeNull();

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new() { Id = temp.Id, FName = "Laura", LName = "Bernasconi" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        httpResponseMessage.EnsureSuccessStatusCode();
        Person actual = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task GivenNonExistingId_WhenDeletingPerson_ThenReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync($"{Settings.Instance.WebApiUrl}/Person/Delete/0");
        httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        notfound.Should().NotBeNull();
        notfound.Should().Be(PersonController.NOTFOUND);
    }

    [Test]
    public async Task GivenExistingId_WhenDeletingPerson_ThenSucceeds()
    {
        Person expected = await CreatePerson(FNAME, LNAME);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();

        {
            expected.Should().NotBeNull();
            HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync($"{Settings.Instance.WebApiUrl}/Person/Delete/{expected.Id}");
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/{expected.Id}");
            httpResponseMessage.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
            notfound.Should().NotBeNull();
            notfound.Should().Be(PersonController.NOTFOUND);
        }
    }

    async Task<Person> CreatePerson(string fName, string lName)
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        PersonCreateData personCreateData = new() { FName = fName, LName = lName };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Create", JsonContent.Create(personCreateData));
        httpResponseMessage.EnsureSuccessStatusCode();
        return await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
    }

    static void MakeAssertions(Person expected, Person actual)
    {
        expected.Should().NotBeNull();
        actual.Should().NotBeNull();
        actual.Id.Should().Be(expected.Id);
        actual.FName.Should().Be(expected.FName);
        actual.LName.Should().Be(expected.LName);
    }
}
