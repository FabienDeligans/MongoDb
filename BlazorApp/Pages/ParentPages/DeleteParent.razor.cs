using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ParentPages
{
    public partial class DeleteParent
    {
        [Parameter]
        public string ParentId { get; set; }

        public Parent Parent { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var parentController = new BaseController<Parent>();
            Parent = parentController.QueryCollection().First(v => v.Id == ParentId);
        }

        public void Yes()
        {
            var parentController = new BaseController<Parent>();
            parentController.RemoveAll(v => v.Id == ParentId);

            BlazoredModal.Close();
        }

        private void No()
        {
            BlazoredModal.Cancel();
        }
    }
}