// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Helpers.URLHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Helpers
{
  public class URLHelper
  {
    private static readonly string c_galleryUrlKey = "galleryUrl";

    public static string GetHomePageUrl(Dictionary<string, object> generalInfo, string product)
    {
      object obj = (object) null;
      return generalInfo.TryGetValue(URLHelper.c_galleryUrlKey, out obj) ? (string) obj + product : string.Empty;
    }

    public static string GetAzureDevopsCategoryUrl(
      Dictionary<string, object> generalInfo,
      string categoryName)
    {
      string str = "search" + "?sortBy=Installs" + "&category=" + HttpUtility.UrlEncode(categoryName) + "&target=AzureDevOps";
      return URLHelper.GetGalleryHomeUrl(generalInfo) + str;
    }

    public static string GetVsCodeCategoryUrl(
      Dictionary<string, object> generalInfo,
      string categoryName)
    {
      string str = "search" + "?sortBy=Installs" + "&category=" + HttpUtility.UrlEncode(categoryName) + "&target=VSCode";
      return URLHelper.GetGalleryHomeUrl(generalInfo) + str;
    }

    public static string GetVSCategoryUrl(
      Dictionary<string, object> generalInfo,
      string categoryName)
    {
      string category;
      string subCategory;
      URLHelper.GetCategoryParts(categoryName, out category, out subCategory);
      string str = "search" + "?sortBy=Installs" + "&category=" + HttpUtility.UrlEncode(category) + "&target=VS";
      if (subCategory != "")
        str = str + "&subCategory=" + HttpUtility.UrlEncode(subCategory);
      return URLHelper.GetGalleryHomeUrl(generalInfo) + str;
    }

    public static void GetCategoryParts(
      string categoryName,
      out string category,
      out string subCategory)
    {
      string[] strArray = categoryName.Split('/');
      subCategory = "";
      category = "";
      if (strArray.Length > 1)
      {
        category = strArray[0];
        subCategory = strArray[1];
      }
      else
        category = strArray[0];
    }

    public static string GetReportsLink(
      string galleryUrl,
      string publisherName,
      string extensionName)
    {
      return galleryUrl + "manage/publishers/" + HttpUtility.UrlEncode(publisherName.ToLower(CultureInfo.InvariantCulture)) + "/extensions/" + HttpUtility.UrlEncode(extensionName.ToLower(CultureInfo.InvariantCulture)) + "/hub";
    }

    public static string GetGalleryItemEditLink(
      string galleryUrl,
      string publisherName,
      string extensionName)
    {
      string str = HttpUtility.UrlEncode(publisherName.ToLower(CultureInfo.InvariantCulture));
      return galleryUrl + "manage/publishers/" + str + "?src=" + str + "." + HttpUtility.UrlEncode(extensionName.ToLower(CultureInfo.InvariantCulture));
    }

    public static string GetAzureDevOpsAcqLink(string galleryUrl, PublishedExtension extension) => galleryUrl + "acquisition?itemName=" + extension.GetFullyQualifiedName();

    public static string GetVSPackageUrl(
      string galleryUrl,
      string publisherName,
      string extensionName,
      string version)
    {
      return galleryUrl + URLHelper.GetVSPackageDownloadURLSuffix(publisherName, extensionName, version);
    }

    public static string GetVSPackageDownloadURLSuffix(
      string publisherName,
      string extensionName,
      string version)
    {
      return "_apis/public/gallery/publishers/" + publisherName + "/vsextensions/" + extensionName + "/" + version + "/vspackage";
    }

    private static string GetGalleryHomeUrl(Dictionary<string, object> generalInfo)
    {
      object obj = (object) null;
      return generalInfo.TryGetValue(URLHelper.c_galleryUrlKey, out obj) ? (string) obj : string.Empty;
    }
  }
}
