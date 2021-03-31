using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using PositionDatabase.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace PositionDatabase.Client.Pages
{
    [Authorize]
    public partial class PeopleList : ComponentBase
    {
        [Inject]
        private HttpClient _http { get; set; }

        private IList<Person> Persons;
        private Person NewPerson = new Person();
        private string SuccessMessage = "", ErrorMessage = "";


        protected override async Task OnInitializedAsync()
        {
            try
            {
                Persons = await _http.GetFromJsonAsync<IList<Person>>("api/persons");
            }
            catch (AccessTokenNotAvailableException exception)
            {
                exception.Redirect();
            }
        }

        private async Task AddPerson()
        {
            if (!string.IsNullOrEmpty(NewPerson.FirstName)
                && !string.IsNullOrEmpty(NewPerson.LastName)
                && NewPerson.BirthDate.Year > 1)
            {
                SuccessMessage = "";
                ErrorMessage = "";
                //Saving the person
                using var response = await _http.PostAsJsonAsync("api/persons", NewPerson);

                if (!response.IsSuccessStatusCode)
                {
                    // set error message for display, log to console and return
                    ErrorMessage = response.ReasonPhrase;
                    return;
                }

                var savedPerson = await response.Content.ReadFromJsonAsync<Person>();

                Persons.Add(savedPerson);
                NewPerson = new Person();
                SuccessMessage = "User added successfully";

                _ = Task.Run(async () =>
                  {
                      await Task.Delay(3000);
                      SuccessMessage = "";
                      StateHasChanged();
                  });

            }
        }
    }
}
