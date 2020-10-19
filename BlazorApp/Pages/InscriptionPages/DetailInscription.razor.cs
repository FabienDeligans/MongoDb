using System.Linq;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.InscriptionPages
{
    public partial class DetailInscription
    {
        [Parameter]
        public string InscriptionId { get; set; }

        public Inscription Inscription { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            Inscription = inscriptionController.QueryCollection().First(v => v.Id == InscriptionId); 
        }
    }
}
