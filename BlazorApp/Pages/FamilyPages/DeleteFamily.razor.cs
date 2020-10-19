using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.FamilyPages
{
    public partial class DeleteFamily
    {
        [Parameter]
        public string Id { get; set; }

        public Family Family { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var familyController = new BaseController<Family>();
            Family = familyController.QueryCollection().First(v => v.Id == Id);
        }

        public void Yes()
        {
            var familyController = new BaseController<Family>();
            var childController = new BaseController<Child>();
            var inscriptionController = new BaseController<Inscription>();
            var parentController = new BaseController<Parent>();

            var childrenIds = childController.QueryCollection().Where(v => v.FamilyId == Id).Select(v => v.Id);

            foreach (var id in childrenIds)
            {
                inscriptionController.RemoveAll(v => v.ChildId == id);
            }

            childController.RemoveAll(v => v.FamilyId == Family.Id);
            parentController.RemoveAll(v => v.FamilyId == Family.Id);
            familyController.Remove(Family);

            BlazoredModal.Close();
        }

        private void No()
        {
            BlazoredModal.Cancel();
        }
    }
}
