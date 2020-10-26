using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.InscriptionPages
{
    public partial class DeleteInscription
    {
        [Parameter]
        public string InscriptionId { get; set; }

        public Inscription Inscription { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            Inscription = inscriptionController.QueryCollection().First(v => v.Id == InscriptionId); 
        }

        public void Yes()
        {
            var inscriptionController = new BaseController<Inscription>();

            inscriptionController.DeleteLinkedList(Inscription);
            inscriptionController.RemoveAll(v => v.Id == InscriptionId);

            BlazoredModal.Close();
        }

        private void No()
        {
            BlazoredModal.Cancel();
        }
    }
}
