using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ChildPages
{
    public partial class DetailChild
    {
        [Parameter]
        public string ChildId { get; set; }

        public Child Child { get; set; }
        public List<Inscription> Inscriptions { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();
            Child = childController.QueryCollection().First(v => v.Id == ChildId);

            var inscriptionController = new BaseController<Inscription>();
            Inscriptions = inscriptionController.QueryCollection().Where(v => v.ChildId == ChildId).ToList();
        }
    }
}
