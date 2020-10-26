using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Pages.ChildPages;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;
using MongoDbCore.Pagination;

namespace BlazorApp.Pages.Component
{
    public partial class ChildrenComponent
    {

        [Parameter]
        public string FamilyId { get; set; }

        public List<Child> Children { get; set; }

        public PaginatedList<Child> PaginatedChild { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();
            Children = FamilyId != null ? childController.QueryCollection().Where(v => v.FamilyId == FamilyId).ToList() : childController.QueryCollection().ToList();
            
            NumberOfPage = (int)Math.Ceiling(Children.Count / (double)PageSize); 
            Index = 1;
            PaginatedChild = PaginatedList<Child>.Create(Children, Index, PageSize);
        }

        public void Detail(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailChild.ChildId), id);

            Modal.Show<DetailChild>("Detail of  this Child", parameters);
        }

        public async Task Edit(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailChild.ChildId), id);

            var modalForm = Modal.Show<EditChild>("Edit this Child", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Create()
        {
            var modalForm = Modal.Show<EditChild>("Insert new Child");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Delete(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailChild.ChildId), id);

            var modalForm = Modal.Show<DeleteChild>("Remove this Child", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public void InscriptionOfChild(string id)
        {
            Navigation.NavigateTo($"/inscriptionPage/{id}");
        }

        public void Pagination(int nb)
        {
            Index = nb;
            PaginatedChild = PaginatedList<Child>.Create(Children, Index, PageSize);
            StateHasChanged();
        }
    }
}
