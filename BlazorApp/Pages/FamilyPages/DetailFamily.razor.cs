using System.Linq;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.FamilyPages
{
    public partial class DetailFamily
    {
        [Parameter]
        public string Id { get; set; }

        public Family Family { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var familyController = new BaseController<Family>();
            Family = familyController.QueryCollection().First(v => v.Id == Id);
        }
    }
}
