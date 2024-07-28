// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CategoriesController
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService;
using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowStream = true)]
  [VersionedApiControllerCustomName(Area = "gallery", ResourceName = "categories")]
  public class CategoriesController : GalleryController
  {
    [HttpGet]
    [ClientLocationId("E0A5A71E-3AC3-43A0-AE7D-0BB5C3046A2A")]
    public List<string> GetCategories(string languages = null)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      if (string.IsNullOrEmpty(languages))
        languages = "en-us";
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string[] languages1 = languages.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      CategoriesResult categoriesResult = service.QueryAvailableCategories(tfsRequestContext, (IEnumerable<string>) languages1);
      List<string> categories = new List<string>();
      foreach (ExtensionCategory category in categoriesResult.Categories)
      {
        foreach (CategoryLanguageTitle languageTitle in category.LanguageTitles)
          categories.Add(languageTitle.Title);
      }
      return categories;
    }

    [HttpGet]
    [ClientLocationId("75D3C04D-84D2-4973-ACD2-22627587DABC")]
    public CategoriesResult GetCategoryDetails(
      string categoryName,
      string languages = null,
      string product = null)
    {
      IPublishedExtensionService service = this.TfsRequestContext.GetService<IPublishedExtensionService>();
      if (string.IsNullOrEmpty(languages))
        languages = "en-us";
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string[] languages1 = languages.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries);
      string categoryName1 = categoryName;
      string product1 = product;
      return service.QueryAvailableCategories(tfsRequestContext, (IEnumerable<string>) languages1, categoryName1, product1);
    }

    [HttpGet]
    [ClientLocationId("31FBA831-35B2-46F6-A641-D05DE5A877D8")]
    public ProductCategoriesResult GetRootCategories(
      string product,
      int lcid = 1033,
      string source = null,
      string productVersion = null,
      string skus = null,
      string subSkus = null)
    {
      if (!"vs".Equals(product, StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException();
      ArgumentUtility.CheckStringForNullOrEmpty(productVersion, nameof (productVersion), "Gallery");
      ArgumentUtility.CheckStringForNullOrEmpty(source, nameof (source), "Gallery");
      IVisualStudioApiService service = this.TfsRequestContext.GetService<IVisualStudioApiService>();
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary.Add("LCID", lcid.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      dictionary.Add("VSVersion", productVersion);
      dictionary.Add("OSVersion", (string) null);
      dictionary.Add("SearchSource", source);
      dictionary.Add("Skus", skus);
      dictionary.Add("SubSkus", subSkus);
      VisualStudioIdeVersion studioIdeVersion = ServiceHelper.GetVisualStudioIdeVersion(productVersion, VisualStudioIdeVersion.Dev17);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      int vsVersion = (int) studioIdeVersion;
      IDictionary<string, string> requestParameters = dictionary;
      ProductCategoriesResult categoriesResult = this.ConvertToProductCategoriesResult(service.GetRootCategories(tfsRequestContext, (VisualStudioIdeVersion) vsVersion, requestParameters));
      foreach (ProductCategory category in categoriesResult.Categories)
        category.HasChildren = true;
      return categoriesResult;
    }

    [HttpGet]
    [ClientLocationId("1102BB42-82B0-4955-8D8A-435D6B4CEDD3")]
    public ProductCategory GetCategoryTree(
      string product,
      string categoryId,
      int lcid = 1033,
      string source = null,
      string productVersion = null,
      string skus = null,
      string subSkus = null,
      string productArchitecture = "x86")
    {
      if (!"vs".Equals(product, StringComparison.OrdinalIgnoreCase))
        throw new NotSupportedException(GalleryResources.UnsupportedProductText());
      ArgumentUtility.CheckStringForNullOrEmpty(productVersion, nameof (productVersion), "Gallery");
      ArgumentUtility.CheckStringForNullOrEmpty(source, nameof (source), "Gallery");
      ArgumentUtility.CheckStringForNullOrEmpty(skus, nameof (skus), "Gallery");
      ArgumentUtility.CheckStringForNullOrEmpty(categoryId, "catgoryId", "Gallery");
      Guid result;
      if (!Guid.TryParse(categoryId, out result))
        throw new ArgumentException(GalleryResources.InvalidCategoryIdFormatText());
      IVisualStudioApiService service = this.TfsRequestContext.GetService<IVisualStudioApiService>();
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      dictionary.Add("LCID", lcid.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      dictionary.Add("VSVersion", productVersion);
      dictionary.Add("OSVersion", (string) null);
      dictionary.Add("TemplateType", (string) null);
      dictionary.Add("SearchSource", source);
      dictionary.Add("Skus", skus);
      if (!string.IsNullOrEmpty(subSkus))
        dictionary.Add("SubSkus", subSkus);
      if (this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableProductArchitectureSupportForVS"))
        dictionary.Add("ProductArchitecture", productArchitecture);
      VisualStudioIdeVersion studioIdeVersion = ServiceHelper.GetVisualStudioIdeVersion(productVersion, VisualStudioIdeVersion.Dev17);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      int vsVersion = (int) studioIdeVersion;
      Guid categoryId1 = result;
      IDictionary<string, string> requestParameters = dictionary;
      return this.ConvertToProductCategory(service.GetCategoryTree(tfsRequestContext, (VisualStudioIdeVersion) vsVersion, categoryId1, 1, requestParameters));
    }

    private ProductCategory ConvertToProductCategory(IdeCategory category)
    {
      if (category == null)
        return (ProductCategory) null;
      ProductCategory productCategory = this.GetProductCategory(category);
      if (category.Children != null)
      {
        productCategory.Children = new List<ProductCategory>();
        foreach (IdeCategory child in (IEnumerable<IdeCategory>) category.Children)
        {
          productCategory.Children.Add(this.GetProductCategory(child));
          productCategory.HasChildren = true;
        }
      }
      return productCategory;
    }

    private ProductCategory GetProductCategory(IdeCategory ideCategory) => new ProductCategory()
    {
      Title = ideCategory.Title,
      HasChildren = ideCategory.HasMore,
      Id = ideCategory.Id
    };

    private ProductCategoriesResult ConvertToProductCategoriesResult(
      ICollection<IdeCategory> categories)
    {
      ProductCategoriesResult categoriesResult = new ProductCategoriesResult();
      categoriesResult.Categories = new List<ProductCategory>();
      foreach (IdeCategory category in (IEnumerable<IdeCategory>) categories)
        categoriesResult.Categories.Add(this.GetProductCategory(category));
      return categoriesResult;
    }
  }
}
