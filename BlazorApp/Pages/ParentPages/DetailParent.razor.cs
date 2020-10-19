using System.Linq;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ParentPages
{
    public partial class DetailParent
    {

        [Parameter]
        public string ParentId { get; set; }

        public Parent Parent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var parentController = new BaseController<Parent>();

            Parent = parentController.QueryCollection().First(v => v.Id == ParentId); 
        }
    }
}
