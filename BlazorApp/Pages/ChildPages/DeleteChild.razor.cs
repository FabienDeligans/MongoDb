using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ChildPages
{
    public partial class DeleteChild
    {
        [Parameter]
        public string ChildId { get; set; }

        public Child Child { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();
            Child = childController.QueryCollection().First(v => v.Id == ChildId);
        }

        public void Yes()
        {
            var childController = new BaseController<Child>();
            var inscriptionController = new BaseController<Inscription>();

            inscriptionController.RemoveAll(v => v.ChildId == ChildId);
            childController.RemoveAll(v => v.Id == ChildId);

            BlazoredModal.Close();
        }

        private void No()
        {
            BlazoredModal.Cancel();
        }
    }
}
