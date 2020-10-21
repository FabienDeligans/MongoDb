using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.FamilyPages
{
    public partial class EditFamily
    {
        [Parameter]
        public string Id { get; set; }
        public Family Family { get; set; }
        public bool Edit { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            var familyController = new BaseController<Family>();
            base.OnInitialized();
            if (Id != null)
            {
                Family = familyController.QueryCollection().First(v => v.Id == Id);
                Edit = true;
            }
            else
            {
                Family = new Family();
            }
        }

        public void Save()
        {
            var familyController = new BaseController<Family>();
            if (Family.FamilyName == null) return;
            if (Edit)
            {
                familyController.ReplaceOne(Family);
            }
            else
            {
                familyController.Insert(Family);
            }

            BlazoredModal.Close();
        }

        private void Cancel()
        {
            BlazoredModal.Cancel();
        }

    }
}
