using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.Modal;
using MongoDbCore.Controller;
using MongoDbCore.Models;
using MongoDbCore.Pagination;

namespace BlazorApp.Pages.FamilyPages
{
    public partial class FamiliesList
    {
        public List<Family> Families { get; set; }

        public PaginatedList<Family> PaginatedFamily { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            var familyController = new BaseController<Family>();
            Families = familyController.QueryCollection().ToList();

            NumberOfPage = (int)Math.Ceiling(Families.Count / (double)PageSize); ;
            Index = 1;
            PaginatedFamily = PaginatedList<Family>.Create(Families, Index, PageSize);
        }

        public async Task Detail(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailFamily.Id), id);

            Modal.Show<DetailFamily>("Detail of  this family", parameters);
        }

        public async Task Edit(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailFamily.Id), id);

            var modalForm = Modal.Show<EditFamily>("Edit this family", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Create()
        {
            var modalForm = Modal.Show<EditFamily>("Insert new family");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Delete(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailFamily.Id), id);

            var modalForm = Modal.Show<DeleteFamily>("Remove this family", parameters);
            var result = await modalForm.Result;
            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public void Pagination(int nb)
        {
            Index = nb;
            PaginatedFamily = PaginatedList<Family>.Create(Families, Index, PageSize);
            StateHasChanged();
        }
    }
}
