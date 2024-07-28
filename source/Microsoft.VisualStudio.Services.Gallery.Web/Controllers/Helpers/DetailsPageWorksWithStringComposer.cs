// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers.DetailsPageWorksWithStringComposer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Controllers.Helpers
{
  public class DetailsPageWorksWithStringComposer
  {
    private const string VstsProductFamily = "ms.VstsProducts.vsts";
    private const string VsProductFamily = "ms.VsProducts.vs";
    private const string VsCodeProductFamily = "ms.VscodeProducts.vscode";
    private const string TfsProductId = "ms.VstsProducts.tfs-product";
    private const string vsProductWorksWithPrefix = "Visual Studio ";

    public IEnumerable<string> GetWorksWithString(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      HashSet<string> collection = new HashSet<string>();
      IProductService service = requestContext.GetService<IProductService>();
      IDictionary<InstallationTarget, ProductFamily> dictionary1 = service.QueryProductFamilies(requestContext, extension);
      HashSet<string> stringSet = new HashSet<string>();
      if (extension == null)
        return (IEnumerable<string>) collection;
      try
      {
        foreach (KeyValuePair<InstallationTarget, ProductFamily> keyValuePair in (IEnumerable<KeyValuePair<InstallationTarget, ProductFamily>>) dictionary1)
        {
          if (keyValuePair.Value != null && !stringSet.Contains(keyValuePair.Value.Id))
            stringSet.Add(keyValuePair.Value.Id);
        }
        if (!stringSet.Contains("ms.VstsProducts.vsts") && !stringSet.Contains("ms.VsProducts.vs"))
        {
          if (!stringSet.Contains("ms.VscodeProducts.vscode"))
            goto label_39;
        }
        IDictionary<InstallationTarget, IList<Product>> dictionary2 = service.QueryProducts(requestContext, extension);
        IList<Product> listOfProducts = (IList<Product>) new List<Product>();
        foreach (KeyValuePair<InstallationTarget, IList<Product>> keyValuePair in (IEnumerable<KeyValuePair<InstallationTarget, IList<Product>>>) dictionary2)
        {
          if (keyValuePair.Value != null)
          {
            foreach (Product product in (IEnumerable<Product>) keyValuePair.Value)
            {
              if (!this.isProductExists(listOfProducts, product.Id))
                listOfProducts.Add(product);
            }
          }
        }
        if (stringSet.Contains("ms.VstsProducts.vsts"))
        {
          List<string> vstsWorksWithString = this.GetVSTSWorksWithString(requestContext, extension, service, listOfProducts);
          if (vstsWorksWithString != null)
          {
            if (vstsWorksWithString.Count > 0)
              collection.AddRange<string, HashSet<string>>((IEnumerable<string>) vstsWorksWithString);
          }
        }
        else if (stringSet.Contains("ms.VsProducts.vs"))
        {
          string vsWorksWithString = this.GetVsWorksWithString(this.GetReleasesList(service.QueryReleases(requestContext, (IList<InstallationTarget>) extension.InstallationTargets)));
          if (!string.IsNullOrEmpty(vsWorksWithString))
            collection.Add(vsWorksWithString);
        }
        else if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnablePlatformSpecificExtensionsForVSCode"))
        {
          if (stringSet.Contains("ms.VscodeProducts.vscode"))
          {
            string codeWorksWithString = this.GetVsCodeWorksWithString(requestContext, extension);
            if (!string.IsNullOrEmpty(codeWorksWithString))
              collection.Add(codeWorksWithString);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(12061089, TraceLevel.Error, "gallery", nameof (GetWorksWithString), "Works With string evaluation failed for extension name : {0}.{1} with error {3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) ex.Message);
      }
label_39:
      return (IEnumerable<string>) collection;
    }

    private string GetVsWorksWithString(IList<ProductRelease> listOfReleases)
    {
      string vsWorksWithString = "";
      SortedList<string, string> sortedList = new SortedList<string, string>();
      if (listOfReleases.Count > 0)
      {
        vsWorksWithString += "Visual Studio ";
        foreach (ProductRelease listOfRelease in (IEnumerable<ProductRelease>) listOfReleases)
        {
          string name = listOfRelease.Name;
          if (!sortedList.ContainsKey(name))
            sortedList.Add(name, listOfRelease.Name);
        }
        int num = 0;
        foreach (string str in (IEnumerable<string>) sortedList.Values)
        {
          vsWorksWithString = num != sortedList.Values.Count - 1 ? vsWorksWithString + str + ", " : vsWorksWithString + str;
          ++num;
        }
      }
      return vsWorksWithString;
    }

    private List<string> GetVSTSWorksWithString(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      IProductService productService,
      IList<Product> listOfProducts)
    {
      List<string> vstsWorksWithString = new List<string>();
      foreach (Product listOfProduct in (IEnumerable<Product>) listOfProducts)
      {
        if (listOfProduct.ProductReleasesId == null || listOfProduct.ProductReleasesId.Count == 0)
          vstsWorksWithString.Insert(0, listOfProduct.Name);
        else if (string.Equals(listOfProduct.Id, "ms.VstsProducts.tfs-product") && this.isTfsInstallationTargetVersionZeroToIntMax(extension))
        {
          vstsWorksWithString.Add(listOfProduct.Name);
        }
        else
        {
          IList<ProductRelease> sortedReleaseList1 = this.GetSortedReleaseList(productService.QueryReleases(requestContext, extension, listOfProduct.Id));
          IList<ProductRelease> productReleaseList = (IList<ProductRelease>) new List<ProductRelease>();
          if (sortedReleaseList1 != null)
          {
            bool flag = false;
            if (sortedReleaseList1.Count == listOfProduct.ProductReleasesId.Count)
              flag = true;
            foreach (ProductRelease productRelease in (IEnumerable<ProductRelease>) sortedReleaseList1)
            {
              IList<ProductRelease> sortedReleaseList2 = this.GetSortedReleaseList(productService.QueryReleases(requestContext, extension, productRelease.Id));
              if (sortedReleaseList2.Count != 0)
              {
                if (productRelease.ProductSubReleasesId.Count == sortedReleaseList2.Count)
                {
                  productReleaseList.Add(productRelease);
                }
                else
                {
                  flag = false;
                  productReleaseList.AddRange<ProductRelease, IList<ProductRelease>>((IEnumerable<ProductRelease>) sortedReleaseList2);
                }
              }
              else
                flag = false;
            }
            if (flag)
              vstsWorksWithString.Add(listOfProduct.Name);
            else if (productReleaseList.Count > 0)
            {
              List<string> stringSortedByRelease = this.GetDisplayStringSortedByRelease(productReleaseList);
              if (stringSortedByRelease.Count > 0)
                vstsWorksWithString.AddRange((IEnumerable<string>) stringSortedByRelease);
            }
          }
        }
      }
      return vstsWorksWithString;
    }

    private string GetVsCodeWorksWithString(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      IReadOnlyDictionary<string, string> targetPlatformPairs = GalleryServerUtil.GetAllVSCodeTargetPlatformPairs(requestContext);
      HashSet<string> values = new HashSet<string>();
      string str = "";
      string version1 = extension.Versions[0].Version;
      foreach (ExtensionVersion version2 in extension.Versions)
      {
        if (string.Equals(version2.Version, version1))
        {
          if (version2.TargetPlatform != null)
          {
            if (targetPlatformPairs.ContainsKey(version2.TargetPlatform))
              values.Add(targetPlatformPairs[version2.TargetPlatform]);
          }
          else
            values.Add("Universal");
        }
        else
          break;
      }
      if (!extension.Tags.IsNullOrEmpty<string>() && extension.Tags.Contains("__web_extension"))
        values.Add(targetPlatformPairs["web"]);
      return str + string.Join(", ", (IEnumerable<string>) values);
    }

    private bool isTfsInstallationTargetVersionZeroToIntMax(PublishedExtension extension)
    {
      Version version1 = new Version(0, 0, 0, 0);
      Version version2 = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
      if (extension.InstallationTargets != null)
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          if ((string.Equals(installationTarget.Target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase) || string.Equals(installationTarget.Target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase)) && installationTarget.MinVersion == version1 && installationTarget.MaxVersion == version2)
            return true;
        }
      }
      return false;
    }

    private bool isProductExists(IList<Product> listOfProducts, string Id)
    {
      foreach (object listOfProduct in (IEnumerable<Product>) listOfProducts)
      {
        if (listOfProduct.Equals((object) Id))
          return true;
      }
      return false;
    }

    private bool isReleaseExists(IList<ProductRelease> listOfReleases, string Id)
    {
      foreach (object listOfRelease in (IEnumerable<ProductRelease>) listOfReleases)
      {
        if (listOfRelease.Equals((object) Id))
          return true;
      }
      return false;
    }

    private IList<ProductRelease> GetReleasesList(
      IDictionary<InstallationTarget, IList<ProductRelease>> releasesDictionary)
    {
      IList<ProductRelease> listOfReleases = (IList<ProductRelease>) new List<ProductRelease>();
      foreach (KeyValuePair<InstallationTarget, IList<ProductRelease>> releases in (IEnumerable<KeyValuePair<InstallationTarget, IList<ProductRelease>>>) releasesDictionary)
      {
        if (releases.Value != null)
        {
          foreach (ProductRelease productRelease in (IEnumerable<ProductRelease>) releases.Value)
          {
            if (!this.isReleaseExists(listOfReleases, productRelease.Id))
              listOfReleases.Add(productRelease);
          }
        }
      }
      return listOfReleases;
    }

    private List<string> GetDisplayStringSortedByRelease(IList<ProductRelease> listOfReleases)
    {
      List<string> stringSortedByRelease = new List<string>();
      SortedList<Version, string> sortedList = new SortedList<Version, string>();
      foreach (ProductRelease listOfRelease in (IEnumerable<ProductRelease>) listOfReleases)
      {
        if (!sortedList.ContainsKey(listOfRelease.MinVersion))
          sortedList.Add(listOfRelease.MinVersion, listOfRelease.Name);
      }
      foreach (string str in (IEnumerable<string>) sortedList.Values)
        stringSortedByRelease.Add(str);
      return stringSortedByRelease;
    }

    private IList<ProductRelease> GetSortedReleaseList(
      IDictionary<InstallationTarget, IList<ProductRelease>> releasesDictionary)
    {
      SortedList<Version, ProductRelease> sortedList = new SortedList<Version, ProductRelease>();
      if (releasesDictionary != null)
      {
        foreach (ProductRelease releases in (IEnumerable<ProductRelease>) this.GetReleasesList(releasesDictionary))
        {
          if (!sortedList.ContainsKey(releases.MinVersion))
            sortedList.Add(releases.MinVersion, releases);
        }
      }
      return sortedList.Values;
    }
  }
}
