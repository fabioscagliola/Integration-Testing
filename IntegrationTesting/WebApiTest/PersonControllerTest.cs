using com.fabioscagliola.IntegrationTesting.WebApi;
using com.fabioscagliola.IntegrationTesting.WebApi.Controllers;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace com.fabioscagliola.IntegrationTesting.WebApiTest;

public class PersonControllerTest : BaseTest
{
    const string FNAME = "Fabio";
    const string LNAME = "Scagliola";

    [Test]
    public async Task Person_Create_WhenFNameOrLNameAreNullOrEmpty_ReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        PersonCreateData personCreateData = new() { FName = null, LName = "" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Create", JsonContent.Create(personCreateData));
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        string fNameOrLNameAreNullOrEmpty = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.That(fNameOrLNameAreNullOrEmpty, Is.Not.Null);
        Assert.That(fNameOrLNameAreNullOrEmpty, Is.EqualTo(PersonController.FNAMEORLNAMEARENULLOREMPTY));
    }

    [Test]
    public async Task Person_Create_Succeeds()
    {
        Person? person = await CreatePerson(FNAME, LNAME);
        Assert.That(person, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(person.Id, Is.Not.Zero);
            Assert.That(person.FName, Is.EqualTo(FNAME));
            Assert.That(person.LName, Is.EqualTo(LNAME));
        });
    }

    [Test]
    public async Task Person_Read_ReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/0");
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.That(notfound, Is.Not.Null);
        Assert.That(notfound, Is.EqualTo(PersonController.NOTFOUND));
    }

    [Test]
    public async Task Person_Read_Succeeds()
    {
        Person? expected = await CreatePerson(FNAME, LNAME);
        Assert.That(expected, Is.Not.Null);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/{expected.Id}");
        httpResponseMessage.EnsureSuccessStatusCode();
        Person? actual = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task Person_ReadList_Succeeds()
    {
        Person? expected = await CreatePerson(FNAME, LNAME);
        Assert.That(expected, Is.Not.Null);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/ReadList");
        httpResponseMessage.EnsureSuccessStatusCode();
        List<Person>? people = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(List<Person>)) as List<Person>;
        Assert.That(people, Is.Not.Null);
        Assert.That(people, Is.Not.Empty);
        Person? actual = people.SingleOrDefault(x => x.Id == expected.Id);
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task Person_Update_WhenNotFound_ReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new();
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.That(notfound, Is.Not.Null);
        Assert.That(notfound, Is.EqualTo(PersonController.NOTFOUND));
    }

    [Test]
    public async Task Person_Update_WhenFNameOrLNameAreNullOrEmpty_ReturnsBadRequest()
    {
        Person? temp = await CreatePerson(FNAME, LNAME);
        Assert.That(temp, Is.Not.Null);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new() { Id = temp.Id, FName = null, LName = "" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        string fNameOrLNameAreNullOrEmpty = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.That(fNameOrLNameAreNullOrEmpty, Is.Not.Null);
        Assert.That(fNameOrLNameAreNullOrEmpty, Is.EqualTo(PersonController.FNAMEORLNAMEARENULLOREMPTY));
    }

    [Test]
    public async Task Person_Update_Succeeds()
    {
        Person? temp = await CreatePerson(FNAME, LNAME);
        Assert.That(temp, Is.Not.Null);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        Person expected = new() { Id = temp.Id, FName = "Laura", LName = "Bernasconi" };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Update", JsonContent.Create(expected));
        httpResponseMessage.EnsureSuccessStatusCode();
        Person? actual = await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
        MakeAssertions(expected, actual);
    }

    [Test]
    public async Task Person_Delete_ReturnsBadRequest()
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync($"{Settings.Instance.WebApiUrl}/Person/Delete/0");
        Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
        Assert.That(notfound, Is.Not.Null);
        Assert.That(notfound, Is.EqualTo(PersonController.NOTFOUND));
    }

    [Test]
    public async Task Person_Delete_Succeeds()
    {
        Person? expected = await CreatePerson(FNAME, LNAME);

        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();

        {
            Assert.That(expected, Is.Not.Null);
            HttpResponseMessage httpResponseMessage = await httpClient.DeleteAsync($"{Settings.Instance.WebApiUrl}/Person/Delete/{expected.Id}");
            httpResponseMessage.EnsureSuccessStatusCode();
        }

        {
            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync($"{Settings.Instance.WebApiUrl}/Person/Read/{expected.Id}");
            Assert.That(httpResponseMessage.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            string notfound = await httpResponseMessage.Content.ReadAsStringAsync();
            Assert.That(notfound, Is.Not.Null);
            Assert.That(notfound, Is.EqualTo(PersonController.NOTFOUND));
        }
    }

    async Task<Person?> CreatePerson(string fName, string lName)
    {
        HttpClient httpClient = WebApiTestWebApplicationFactory.CreateClient();
        PersonCreateData personCreateData = new() { FName = fName, LName = lName };
        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"{Settings.Instance.WebApiUrl}/Person/Create", JsonContent.Create(personCreateData));
        httpResponseMessage.EnsureSuccessStatusCode();
        return await httpResponseMessage.Content.ReadFromJsonAsync(typeof(Person)) as Person;
    }

    static void MakeAssertions(Person? expected, Person? actual)
    {
        Assert.That(expected, Is.Not.Null);
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.FName, Is.EqualTo(expected.FName));
        Assert.That(actual.LName, Is.EqualTo(expected.LName));
    }
}
