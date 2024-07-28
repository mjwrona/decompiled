// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.VSTSExtensionItem
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class VSTSExtensionItem : BaseExtensionItem
  {
    [JsonProperty("ic")]
    public int InstallCount { get; set; }

    [JsonProperty("dc")]
    public int DownloadCount { get; set; }

    [JsonIgnore]
    public VSSItemType ItemType { get; set; }

    [JsonProperty("rc")]
    public int RatingCount { get; set; }

    [JsonIgnore]
    public string DefaultIcon { get; set; }

    [JsonIgnore]
    public ExtensionDeploymentTechnology DeploymentTechnology { get; set; }

    public VSTSExtensionItem(PublishedExtension publishedExtension)
    {
      this.Author = publishedExtension.Publisher?.DisplayName;
      this.PublisherDomain = publishedExtension.Publisher?.Domain;
      PublisherFacts publisher1 = publishedExtension.Publisher;
      this.IsPublisherDomainVerified = publisher1 != null && publisher1.IsDomainVerified;
      this.Summary = !string.IsNullOrEmpty(publishedExtension.LongDescription) ? publishedExtension.LongDescription : publishedExtension.ShortDescription;
      this.Thumbnail = VSTSExtensionItem.GetAssetUrl(publishedExtension, "Microsoft.VisualStudio.Services.Icons.Small");
      this.FallbackThumbnail = VSTSExtensionItem.GetAssetUrl(publishedExtension, "Microsoft.VisualStudio.Services.Icons.Small", true);
      if (string.IsNullOrEmpty(this.Thumbnail))
      {
        this.Thumbnail = VSTSExtensionItem.GetAssetUrl(publishedExtension, "Microsoft.VisualStudio.Services.Icons.Default");
        this.FallbackThumbnail = VSTSExtensionItem.GetAssetUrl(publishedExtension, "Microsoft.VisualStudio.Services.Icons.Default", true);
      }
      this.Link = VSTSExtensionItem.GetItemDetailsURLFromNames(publishedExtension.Publisher?.PublisherName, publishedExtension.ExtensionName);
      this.Title = publishedExtension.DisplayName;
      this.CostCategory = (int) VSTSExtensionItem.GetCostCategory(publishedExtension);
      this.InstallCount = (int) GalleryServerUtil.GetInstallCount(publishedExtension.Statistics);
      this.DownloadCount = VSTSExtensionItem.GetDownloadCount(publishedExtension);
      this.RatingCount = VSTSExtensionItem.GetRatingCount(publishedExtension);
      this.Rating = VSTSExtensionItem.GetAverageRating(publishedExtension);
      this.ItemType = VSTSExtensionItem.GetItemType(publishedExtension);
      PublisherFacts publisher2 = publishedExtension.Publisher;
      this.IsPublisherCertified = publisher2 != null && (publisher2.Flags & PublisherFlags.Certified) > PublisherFlags.None;
      this.DefaultIcon = VSTSExtensionItem.GetAssetUrl(publishedExtension, "Microsoft.VisualStudio.Services.Icons.Default");
      this.DeploymentTechnology = publishedExtension.DeploymentType;
    }

    public static string GetItemDetailsURL(string itemName) => "items?itemName=" + HttpUtility.UrlEncode(itemName);

    public static string GetAssetUrl(
      PublishedExtension extension,
      string assetType,
      bool usefallback = false)
    {
      return VSTSExtensionItem.GetExtensionAsset(extension, assetType, usefallback)?.Source ?? string.Empty;
    }

    private static ExtensionFile GetExtensionAsset(
      PublishedExtension extension,
      string assetType,
      bool usefallback = false)
    {
      ExtensionFile asset = (ExtensionFile) null;
      if (extension != null && extension.Versions != null && extension.Versions.Count > 0 && extension.Versions[0].Files != null)
      {
        extension.Versions[0].Files.Any<ExtensionFile>((Func<ExtensionFile, bool>) (file =>
        {
          if (file == null || !string.Equals(file.AssetType, assetType, StringComparison.OrdinalIgnoreCase))
            return false;
          asset = file.ShallowCopy();
          return true;
        }));
        if (asset != null & usefallback && !extension.Versions[0].FallbackAssetUri.IsNullOrEmpty<char>() && !extension.Versions[0].AssetUri.IsNullOrEmpty<char>())
          asset.Source = asset.Source.Replace(extension.Versions[0].AssetUri, extension.Versions[0].FallbackAssetUri);
      }
      return asset;
    }

    private static string GetItemDetailsURLFromNames(string publisherName, string extensionName) => VSTSExtensionItem.GetItemDetailsURL(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) publisherName, (object) extensionName));

    private static Pricing GetCostCategory(PublishedExtension extension)
    {
      Pricing costCategory = Pricing.Free;
      if (extension == null)
        return costCategory;
      bool flag1 = VSTSExtensionItem.GetItemType(extension) == VSSItemType.VSSExtension;
      bool flag2 = extension.IsByolExtension();
      bool flag3 = (extension.Flags & PublishedExtensionFlags.Preview) != 0;
      bool flag4 = extension.IsPaid();
      if ((extension.IsTrial() || extension.IsTrialEnabledForByolExtension() ? 1 : (!(flag4 & flag1) || (extension.Flags & PublishedExtensionFlags.Preview) != PublishedExtensionFlags.None ? 0 : (!flag2 ? 1 : 0))) != 0)
        costCategory = Pricing.Trial;
      else if (flag4)
        costCategory = !(flag1 & flag3) ? Pricing.Paid : Pricing.Trial;
      return costCategory;
    }

    private static int GetDownloadCount(PublishedExtension extension)
    {
      int downloadCount = 0;
      List<ExtensionStatistic> statistics = extension.Statistics;
      // ISSUE: explicit non-virtual call
      if ((statistics != null ? (__nonvirtual (statistics.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        foreach (ExtensionStatistic statistic in extension.Statistics)
        {
          if (string.Equals(statistic.StatisticName, "updateCount", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "install", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "onpremDownloads", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "migratedInstallCount", StringComparison.OrdinalIgnoreCase))
            downloadCount += (int) statistic.Value;
        }
      }
      return downloadCount;
    }

    private static int GetRatingCount(PublishedExtension extension)
    {
      int ratingCount = 0;
      List<ExtensionStatistic> statistics = extension.Statistics;
      // ISSUE: explicit non-virtual call
      if ((statistics != null ? (__nonvirtual (statistics.Count) > 0 ? 1 : 0) : 0) != 0)
        extension.Statistics.Any<ExtensionStatistic>((Func<ExtensionStatistic, bool>) (value =>
        {
          if (!string.Equals(value.StatisticName, "ratingcount", StringComparison.OrdinalIgnoreCase))
            return false;
          ratingCount = (int) value.Value;
          return true;
        }));
      return ratingCount;
    }

    private static double GetAverageRating(PublishedExtension extension)
    {
      double averageRating = 0.0;
      List<ExtensionStatistic> statistics = extension.Statistics;
      // ISSUE: explicit non-virtual call
      if ((statistics != null ? (__nonvirtual (statistics.Count) > 0 ? 1 : 0) : 0) != 0)
        extension.Statistics.Any<ExtensionStatistic>((Func<ExtensionStatistic, bool>) (value =>
        {
          if (!string.Equals(value.StatisticName, "averagerating", StringComparison.OrdinalIgnoreCase))
            return false;
          averageRating = value.Value;
          return true;
        }));
      return averageRating;
    }

    private static VSSItemType GetItemType(PublishedExtension extension)
    {
      VSSItemType itemType = VSSItemType.VSSExtension;
      if (extension.InstallationTargets != null)
      {
        foreach (InstallationTarget installationTarget in extension.InstallationTargets)
        {
          string target = installationTarget.Target;
          if (string.Equals(target, "Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) || string.Equals(target, "Microsoft.VisualStudio.Services.Cloud", StringComparison.OrdinalIgnoreCase) || string.Equals(target, "Microsoft.TeamFoundation.Server", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VSSExtension;
            break;
          }
          if (string.Equals(target, "Microsoft.VisualStudio.Services.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(target, "Microsoft.TeamFoundation.Server.Integration", StringComparison.OrdinalIgnoreCase) || string.Equals(target, "Microsoft.VisualStudio.Services.Cloud.Integration", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VSSIntegration;
            break;
          }
          if (string.Equals(target, "Microsoft.VisualStudio.Offer", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VSSOffer;
            break;
          }
          if (string.Equals(target, "Microsoft.VisualStudio.Code", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VSCodeExtension;
            break;
          }
          if (string.Equals(target, "Microsoft.VisualStudio.Ide", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VSIdeExtension;
            break;
          }
          if (string.Equals(target, "Microsoft.VisualStudio.Services.Resource.Cloud", StringComparison.OrdinalIgnoreCase))
          {
            itemType = VSSItemType.VssHostedResource;
            break;
          }
        }
      }
      return itemType;
    }
  }
}
