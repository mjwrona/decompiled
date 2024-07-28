// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Helpers.ItemDetailsPageHelpers.BreadcrumbHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.VisualStudio.Services.Gallery.Web.Models;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Helpers.ItemDetailsPageHelpers
{
  internal class BreadcrumbHelper
  {
    public static List<BreadcrumbItem> GetBreadCrumbMembers(
      PublishedExtension extension,
      Dictionary<string, object> generalInfo)
    {
      List<BreadcrumbItem> breadCrumbMembers = new List<BreadcrumbItem>();
      VSTSExtensionItem vstsExtensionItem = new VSTSExtensionItem(extension);
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      VSSItemType itemType = new VSTSExtensionItem(extension).ItemType;
      string str1;
      string homePageUrl;
      Func<Dictionary<string, object>, string, string> func;
      switch (itemType)
      {
        case VSSItemType.VSSOffer:
          str1 = GalleryCommonResources.Subs_Header;
          homePageUrl = URLHelper.GetHomePageUrl(generalInfo, "subscriptions");
          func = (Func<Dictionary<string, object>, string, string>) null;
          break;
        case VSSItemType.VSCodeExtension:
          str1 = GalleryCommonResources.VSCode_Header;
          homePageUrl = URLHelper.GetHomePageUrl(generalInfo, "vscode");
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          func = BreadcrumbHelper.\u003C\u003EO.\u003C0\u003E__GetVsCodeCategoryUrl ?? (BreadcrumbHelper.\u003C\u003EO.\u003C0\u003E__GetVsCodeCategoryUrl = new Func<Dictionary<string, object>, string, string>(URLHelper.GetVsCodeCategoryUrl));
          break;
        case VSSItemType.VSIdeExtension:
          str1 = GalleryCommonResources.VS_Header;
          homePageUrl = URLHelper.GetHomePageUrl(generalInfo, "vs");
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          func = BreadcrumbHelper.\u003C\u003EO.\u003C1\u003E__GetVSCategoryUrl ?? (BreadcrumbHelper.\u003C\u003EO.\u003C1\u003E__GetVSCategoryUrl = new Func<Dictionary<string, object>, string, string>(URLHelper.GetVSCategoryUrl));
          break;
        default:
          str1 = GalleryCommonResources.AzureDevOps_Header;
          homePageUrl = URLHelper.GetHomePageUrl(generalInfo, "azuredevops");
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          func = BreadcrumbHelper.\u003C\u003EO.\u003C2\u003E__GetAzureDevopsCategoryUrl ?? (BreadcrumbHelper.\u003C\u003EO.\u003C2\u003E__GetAzureDevopsCategoryUrl = new Func<Dictionary<string, object>, string, string>(URLHelper.GetAzureDevopsCategoryUrl));
          break;
      }
      breadCrumbMembers.Add(new BreadcrumbItem()
      {
        Name = str1,
        Url = homePageUrl
      });
      if (itemType != VSSItemType.VSSOffer)
      {
        string empty3 = string.Empty;
        string str2 = extension.Categories == null || extension.Categories.Count <= 0 ? GalleryResources.OtherItemCategory : extension.Categories[0];
        breadCrumbMembers.Add(new BreadcrumbItem()
        {
          Name = str2,
          Url = func(generalInfo, str2)
        });
      }
      breadCrumbMembers.Add(new BreadcrumbItem()
      {
        Name = extension.DisplayName,
        Url = ""
      });
      return breadCrumbMembers;
    }

    public static BreadcrumbProps GetBreadcrumbProps(
      PublishedExtension extension,
      List<BreadcrumbItem> items)
    {
      return new BreadcrumbProps()
      {
        m1Text = items[0].Name,
        m1Url = items[0].Url,
        m2Text = items[1].Name,
        m2Url = items[1].Url,
        m3Text = items.Count > 2 ? items[2].Name : (string) null,
        vscodeIntroText = GalleryResources.VSCodeIntroText,
        vscodeEditorDownloadLink = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://go.microsoft.com/fwlink?linkid=846418&pub={0}&ext={1}&utm_source=vsmp&utm_campaign=mpdetails", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName),
        vscodeDownloadDescription = GalleryResources.VSCodeDownloadDescription,
        vscodeEditorDownloadText = GalleryResources.VSCodeDownloadText
      };
    }
  }
}
