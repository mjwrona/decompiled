// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryServerUtil
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Configuration;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class GalleryServerUtil
  {
    private static readonly HttpClient client = new HttpClient();
    private static readonly string ServiceLayer = nameof (GalleryServerUtil);
    private static readonly IReadOnlyCollection<string> c_contentDispositionAttachmentAssetSuffixes = (IReadOnlyCollection<string>) new List<string>()
    {
      ".Payload",
      ".vsix",
      ".exe",
      ".msi",
      ".VSIXPackage"
    };
    private const string s_area = "Gallery";
    private const string s_layer = "GalleryServerUtil";
    public static Guid CategorySQLNotifications = new Guid("2E901E60-7938-41B9-BC26-FB6A85948B51");
    public static HashSet<string> ExtensionInternalTags = new HashSet<string>()
    {
      "$BuiltIn",
      "$Featured",
      "$FromMS",
      "$TopFree",
      "$TopPaid",
      "$TopRated"
    };
    public static readonly Dictionary<string, string> c_oldCategoryToVerticalCategoryMapping = new Dictionary<string, string>((IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "Build and release",
        "Azure Pipelines"
      },
      {
        "Pipelines",
        "Azure Pipelines"
      },
      {
        "Code",
        "Azure Repos"
      },
      {
        "Repos",
        "Azure Repos"
      },
      {
        "Collaborate",
        "Azure Boards"
      },
      {
        "Plan and track",
        "Azure Boards"
      },
      {
        "Integrate",
        "Azure Boards"
      },
      {
        "Boards",
        "Azure Boards"
      },
      {
        "Test",
        "Azure Test Plans"
      },
      {
        "Test Plans",
        "Azure Test Plans"
      },
      {
        "Artifacts",
        "Azure Artifacts"
      }
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    public static readonly Dictionary<string, string> c_verticalCategoryToOldCategoryMapping = new Dictionary<string, string>((IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "Azure Pipelines",
        "Build and release"
      },
      {
        "Pipelines",
        "Build and release"
      },
      {
        "Azure Repos",
        "Code"
      },
      {
        "Repos",
        "Code"
      },
      {
        "Azure Boards",
        "Plan and track"
      },
      {
        "Boards",
        "Plan and track"
      },
      {
        "Azure Test Plans",
        "Test"
      },
      {
        "Test Plans",
        "Test"
      }
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    internal static string TryUseAccountTokenFromHttpHeader(
      IVssRequestContext requestContext,
      HttpRequestHeaders headers,
      string callingMethod,
      string accountToken)
    {
      string str1 = accountToken;
      string str2 = (string) null;
      bool flag = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseAccountTokenFromHeader");
      if (flag)
      {
        str2 = GalleryServerUtil.GetHeaderValue(headers, "X-Market-AccountToken");
        if (!string.IsNullOrEmpty(str2))
          accountToken = str2;
      }
      requestContext.Trace(12062060, TraceLevel.Info, "Gallery", callingMethod, string.Format("{0}.TryUseAccountTokenFromHttpHeader: isUseAccountTokenFromHeaderFFEnabled:{1} IsNullOrEmpty(accountToken):{2} ", (object) callingMethod, (object) flag, (object) string.IsNullOrEmpty(str1)) + string.Format("IsNullOrEmpty(accountTokenFromHeader):{0}", (object) string.IsNullOrEmpty(str2)));
      if (!string.IsNullOrEmpty(str1))
        requestContext.TraceAlways(12062060, TraceLevel.Error, "Gallery", callingMethod, callingMethod + ".TryUseAccountTokenFromHttpHeader: Account token found in URI as well as in http header." + string.Format("UserAgent:{0}, IsNullOrEmpty(accountTokenFromHeader):{1}", (object) requestContext.UserAgent, (object) string.IsNullOrEmpty(str2)));
      return accountToken;
    }

    public static ArtifactSpec GetExtensionVersionArtifactSpec(
      PublishedExtension extension,
      string version,
      string targetPlatform = null)
    {
      string str = version;
      if (!string.IsNullOrWhiteSpace(targetPlatform))
        str = string.Format("{0}@{1}", (object) version, (object) targetPlatform);
      return new ArtifactSpec(GalleryServiceConstants.ExtensionVersionArtifactKind, GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName) + "-" + str, 0);
    }

    public static ArtifactSpec GetExtensionNameArtifactSpec(PublishedExtension extension) => new ArtifactSpec(GalleryServiceConstants.ExtensionNameArtifactKind, GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), 0);

    public static ArtifactSpec GetExtensionRedirectionArtifactSpec(string itemName) => new ArtifactSpec(GalleryServiceConstants.ExtensionRedirectionArtifactKind, itemName, 0);

    public static void GetPublisherNameExtensionNameFromProductId(
      string productId,
      out string publisherName,
      out string extensionName)
    {
      string[] strArray = productId.Split('.');
      publisherName = strArray[0];
      extensionName = strArray[1];
    }

    public static void NotifyGalleryDataChanged(
      IVssRequestContext requestContext,
      Guid eventId,
      string eventData)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, eventId, eventData);
    }

    public static string EllipticalString(string originalString, int maxLength) => string.IsNullOrWhiteSpace(originalString) || originalString.Length <= maxLength ? originalString : originalString.Substring(0, maxLength) + "...";

    public static double ExtractStatisticsValue(
      List<ExtensionStatistic> statistics,
      string statisticName)
    {
      double statisticsValue = 0.0;
      if (!statistics.IsNullOrEmpty<ExtensionStatistic>())
      {
        foreach (ExtensionStatistic statistic in statistics)
        {
          if (string.Equals(statistic.StatisticName, statisticName, StringComparison.OrdinalIgnoreCase))
            statisticsValue = statistic.Value;
        }
      }
      return statisticsValue;
    }

    public static double GetDownloadCount(List<ExtensionStatistic> statistics)
    {
      double downloadCount = 0.0;
      if (!statistics.IsNullOrEmpty<ExtensionStatistic>())
      {
        foreach (ExtensionStatistic statistic in statistics)
        {
          if (string.Equals(statistic.StatisticName, "install", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "updateCount", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "onpremDownloads", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "migratedInstallCount", StringComparison.OrdinalIgnoreCase))
            downloadCount += statistic.Value;
        }
      }
      return downloadCount;
    }

    public static double GetInstallCount(List<ExtensionStatistic> statistics)
    {
      double installCount = 0.0;
      if (!statistics.IsNullOrEmpty<ExtensionStatistic>())
      {
        foreach (ExtensionStatistic statistic in statistics)
        {
          if (string.Equals(statistic.StatisticName, "install", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "onpremDownloads", StringComparison.OrdinalIgnoreCase) || string.Equals(statistic.StatisticName, "migratedInstallCount", StringComparison.OrdinalIgnoreCase))
            installCount += statistic.Value;
        }
      }
      return installCount;
    }

    public static ExtensionVersion GetLatestValidatedExtensionVersion(
      List<ExtensionVersion> extensionVersions)
    {
      foreach (ExtensionVersion extensionVersion in extensionVersions)
      {
        if (extensionVersion.Flags.HasFlag((System.Enum) ExtensionVersionFlags.Validated))
          return extensionVersion;
      }
      return (ExtensionVersion) null;
    }

    public static List<ExtensionVersion> GetLatestValidatedExtensionVersionForEachTargetPlatform(
      List<ExtensionVersion> extensionVersions)
    {
      if (extensionVersions.IsNullOrEmpty<ExtensionVersion>())
        return extensionVersions;
      List<ExtensionVersion> eachTargetPlatform = new List<ExtensionVersion>();
      ISet<string> stringSet = (ISet<string>) new HashSet<string>();
      foreach (ExtensionVersion extensionVersion in extensionVersions)
      {
        if (extensionVersion.Flags.HasFlag((System.Enum) ExtensionVersionFlags.Validated) && stringSet.Add(extensionVersion.TargetPlatform))
          eachTargetPlatform.Add(extensionVersion);
      }
      return eachTargetPlatform;
    }

    public static bool IsExtensionHasSupportForWebTargetPlatform(
      List<ExtensionVersion> extensionVersions)
    {
      if (!extensionVersions.IsNullOrEmpty<ExtensionVersion>())
      {
        foreach (ExtensionVersion extensionVersion in extensionVersions)
        {
          if (extensionVersion.Flags.HasFlag((System.Enum) ExtensionVersionFlags.Validated) && string.Equals("web", extensionVersion.TargetPlatform))
            return true;
        }
      }
      return false;
    }

    public static List<string> GetSupportedTargetPlatformsForExtension(PublishedExtension extension) => extension == null ? Enumerable.Empty<string>().ToList<string>() : GalleryServerUtil.GetTargetPlatformsFromExtensionVersions(GalleryServerUtil.GetLatestValidatedExtensionVersionForEachTargetPlatform(extension.Versions));

    public static List<string> GetTargetPlatformsFromExtensionVersions(
      List<ExtensionVersion> extensionVersions)
    {
      ISet<string> source = (ISet<string>) new HashSet<string>();
      if (!extensionVersions.IsNullOrEmpty<ExtensionVersion>())
      {
        foreach (ExtensionVersion extensionVersion in extensionVersions)
        {
          if (!string.IsNullOrWhiteSpace(extensionVersion.TargetPlatform))
            source.Add(extensionVersion.TargetPlatform);
          else
            source.Add("universal");
        }
      }
      return source.ToList<string>();
    }

    public static void DeleteExtensionStatistics(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      requestContext.GetService<IExtensionStatisticService>().UpdateStatistics(requestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
      {
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "averagerating"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "ratingcount"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "install"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "trendingdaily"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "trendingmonthly"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "trendingweekly"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "onpremDownloads"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "updateCount"),
        GalleryServerUtil.ExtensionStatisticDeleteRowCreate(publisherName, extensionName, "migratedInstallCount")
      });
    }

    private static ExtensionStatisticUpdate ExtensionStatisticDeleteRowCreate(
      string publisherName,
      string extensionName,
      string StatisticName)
    {
      return new ExtensionStatisticUpdate()
      {
        Statistic = new ExtensionStatistic()
        {
          StatisticName = StatisticName
        },
        Operation = ExtensionStatisticOperation.Delete,
        PublisherName = publisherName,
        ExtensionName = extensionName
      };
    }

    public static void UpdateExtensionRatingStatistics(
      IVssRequestContext requestContext,
      KeyValuePair<string, string> extensionPublisherNameToExtensionName,
      float averageRating,
      long ratingCount)
    {
      requestContext.GetService<IExtensionStatisticService>().UpdateStatistics(requestContext, (IEnumerable<ExtensionStatisticUpdate>) new List<ExtensionStatisticUpdate>()
      {
        new ExtensionStatisticUpdate()
        {
          Statistic = new ExtensionStatistic()
          {
            StatisticName = "averagerating",
            Value = (double) averageRating
          },
          Operation = ExtensionStatisticOperation.Set,
          PublisherName = extensionPublisherNameToExtensionName.Key,
          ExtensionName = extensionPublisherNameToExtensionName.Value
        },
        new ExtensionStatisticUpdate()
        {
          Statistic = new ExtensionStatistic()
          {
            StatisticName = "ratingcount",
            Value = (double) ratingCount
          },
          Operation = ExtensionStatisticOperation.Set,
          PublisherName = extensionPublisherNameToExtensionName.Key,
          ExtensionName = extensionPublisherNameToExtensionName.Value
        }
      });
    }

    public static string GetExtensionState(PublishedExtensionFlags flags)
    {
      string extensionState = "Private";
      if (flags.HasFlag((System.Enum) PublishedExtensionFlags.Public))
        extensionState = "Public";
      if (flags.HasFlag((System.Enum) PublishedExtensionFlags.Unpublished))
        extensionState = "Unpublished";
      return extensionState;
    }

    public static void QueueExtensionDataCleanUpJob(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      List<ExtensionIdentifierForDeleteJob> objectToSerialize = new List<ExtensionIdentifierForDeleteJob>();
      foreach (PublishedExtension extension in extensions)
        objectToSerialize.Add(new ExtensionIdentifierForDeleteJob()
        {
          ExtensionId = extension.ExtensionId,
          ProductId = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName)
        });
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      service.QueueOneTimeJob(requestContext, "Delete extension data like stats, reviews and qna.", "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.DeleteExtensionDataJob", xml, JobPriorityLevel.Normal);
    }

    public static string GetGalleryUrl(IVssRequestContext tfsRequestContext, string relativeUrl = "") => new Uri(new Uri(tfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(tfsRequestContext, new Guid("00000029-0000-8888-8000-000000000000"), AccessMappingConstants.ClientAccessMappingMoniker)), relativeUrl).AbsoluteUri;

    public static string GetGalleryDetailsPageUrl(
      IVssRequestContext tfsRequestContext,
      string publisherName,
      string ExtensionName)
    {
      string relativeUri = "/items?itemName=" + HttpUtility.UrlEncode(GalleryUtil.CreateFullyQualifiedName(publisherName, ExtensionName));
      return new Uri(new Uri(tfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(tfsRequestContext, new Guid("00000029-0000-8888-8000-000000000000"), AccessMappingConstants.ClientAccessMappingMoniker)), relativeUri).AbsoluteUri;
    }

    public static string GetManagePageUrl(
      IVssRequestContext tfsRequestContext,
      string publisherName)
    {
      string relativeUri = "/manage/publishers/" + HttpUtility.UrlEncode(publisherName);
      return new Uri(new Uri(tfsRequestContext.GetService<ILocationService>().GetLocationServiceUrl(tfsRequestContext, new Guid("00000029-0000-8888-8000-000000000000"), AccessMappingConstants.ClientAccessMappingMoniker)), relativeUri).AbsoluteUri;
    }

    public static string GetValidationLogsUrl(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      string validationLogsUrl = string.Empty;
      if (requestContext != null && extension != null && extension.Publisher != null && !extension.Versions.IsNullOrEmpty<ExtensionVersion>())
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        Guid verificationLogLocationId = GalleryResourceIds.VerificationLogLocationId;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["publisherName"] = (object) extension.Publisher.PublisherName;
        dictionary["extensionName"] = (object) extension.ExtensionName;
        dictionary["version"] = (object) extension.Versions[0].Version;
        IVssRequestContext requestContext1 = requestContext;
        Guid identifier = verificationLogLocationId;
        Dictionary<string, object> routeValues = dictionary;
        validationLogsUrl = service.GetResourceUri(requestContext1, "gallery", identifier, (object) routeValues).AbsoluteUri;
        if (!string.IsNullOrEmpty(extension.Versions[0].TargetPlatform))
          validationLogsUrl = validationLogsUrl + "?targetPlatform=" + extension.Versions[0].TargetPlatform;
      }
      else
      {
        ArgumentException argumentException = new ArgumentException("Invalid arguments for GetValidationLogsUrl");
        requestContext.TraceException(12061094, "Gallery", "ValidatePackageSize", (Exception) argumentException);
      }
      return validationLogsUrl;
    }

    public static string GetContentValidationLogsUrl(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      string validationLogsUrl = string.Empty;
      if (requestContext != null && extension != null && extension.Publisher != null && !extension.Versions.IsNullOrEmpty<ExtensionVersion>())
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        Guid verificationLogLocationId = GalleryResourceIds.ContentVerificationLogLocationId;
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        dictionary["publisherName"] = (object) extension.Publisher.PublisherName;
        dictionary["extensionName"] = (object) extension.ExtensionName;
        IVssRequestContext requestContext1 = requestContext;
        Guid identifier = verificationLogLocationId;
        Dictionary<string, object> routeValues = dictionary;
        validationLogsUrl = service.GetResourceUri(requestContext1, "gallery", identifier, (object) routeValues).AbsoluteUri;
      }
      else
      {
        ArgumentException argumentException = new ArgumentException("Invalid arguments for GetContentValidationLogsUrl");
        requestContext.TraceException(12061094, "Gallery", "ValidatePackageSize", (Exception) argumentException);
      }
      return validationLogsUrl;
    }

    public static Stream GetExtensionPackageStream(ExtensionPackage extensionPackage) => (Stream) new MemoryStream(Convert.FromBase64String(extensionPackage.ExtensionManifest));

    public static string GetFileContent(IVssRequestContext requestContext, int fileId)
    {
      using (Stream stream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) fileId, false, out byte[] _, out long _, out CompressionType _))
      {
        using (StreamReader streamReader = new StreamReader(stream))
          return streamReader.ReadToEnd();
      }
    }

    public static string GetVsixId(this PublishedExtension extension)
    {
      if (extension.Metadata.IsNullOrEmpty<ExtensionMetadata>())
        return (string) null;
      string vsixId = (string) null;
      foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
      {
        if (string.Equals(extensionMetadata.Key, "VsixId", StringComparison.OrdinalIgnoreCase))
          vsixId = extensionMetadata.Value;
      }
      return vsixId;
    }

    public static Stream GetExtensionPackageStream(HttpRequestMessage Request)
    {
      string property = Request.Properties["Gallery_PackageFileName"] as string;
      try
      {
        return (Stream) new FileStream(property, FileMode.Open, FileAccess.Read, FileShare.Read, 32768, FileOptions.RandomAccess | FileOptions.DeleteOnClose);
      }
      catch (Exception ex) when (
      {
        // ISSUE: unable to correctly present filter
        int num;
        switch (ex)
        {
          case ArgumentNullException _:
          case ArgumentException _:
            num = 1;
            break;
          default:
            num = ex is FileNotFoundException ? 1 : 0;
            break;
        }
        if ((uint) num > 0U)
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        throw new HttpException(400, GalleryResources.PackageNotFound());
      }
    }

    public static bool IsValidVersion(string versionStr) => Version.TryParse(versionStr, out Version _) || int.TryParse(versionStr, out int _);

    public static void ParseInstallationTargetVersion(IEnumerable<InstallationTarget> targets)
    {
      if (targets == null)
        return;
      GalleryServerUtil.RefineInstallationTargets(targets);
      foreach (InstallationTarget target in targets)
        GalleryServerUtil.InternalParseInstallationTargetVersion(target);
    }

    public static long GetSuggestedMaxPackageSize(
      IVssRequestContext requestContext,
      IEnumerable<InstallationTarget> installationTargets,
      string publisherName,
      string extensionName = null)
    {
      long suggestedMaxPackageSize = 104857600;
      if (installationTargets.IsNullOrEmpty<InstallationTarget>())
        return suggestedMaxPackageSize;
      string installationTargets1 = GalleryUtil.GetProductTypeForInstallationTargets(installationTargets);
      if (string.Equals(installationTargets1, "vscode", StringComparison.OrdinalIgnoreCase))
        suggestedMaxPackageSize = 209715200L;
      else if (string.Equals(installationTargets1, "vs", StringComparison.OrdinalIgnoreCase))
        suggestedMaxPackageSize = 482344960L;
      if (!string.IsNullOrEmpty(publisherName))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        long num1 = 0;
        if (!string.IsNullOrEmpty(extensionName))
        {
          string query = "/Configuration/Service/Gallery/LargeExtensionUpload/" + publisherName + "/" + extensionName + "/MaxPackageSizeMB";
          num1 = service.GetValue<long>(requestContext, (RegistryQuery) query, 0L);
        }
        if (num1 > 0L)
        {
          suggestedMaxPackageSize = num1 * 1048576L;
        }
        else
        {
          string query = "/Configuration/Service/Gallery/LargeExtensionUpload/" + publisherName + "/MaxPackageSizeMB";
          long num2 = service.GetValue<long>(requestContext, (RegistryQuery) query, 0L);
          if (num2 > 0L)
            suggestedMaxPackageSize = num2 * 1048576L;
        }
      }
      return suggestedMaxPackageSize;
    }

    public static long GetMaxPackageSizeInBytes(
      IVssRequestContext requestContext,
      long suggestedMaxPackageSizeInBytes)
    {
      long packageSizeInBytes = suggestedMaxPackageSizeInBytes;
      long num = requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/LargeExtensionUpload/MaxPackageSizeMB", 0L);
      if (num > 0L)
        packageSizeInBytes = num * 1048576L;
      return packageSizeInBytes;
    }

    public static long GetMaxPublisherLogoSizeInBytes(
      IVssRequestContext requestContext,
      long suggestedMaxPackageSizeInBytes)
    {
      return GalleryServerUtil.GetMaxPackageSizeInBytes(requestContext, suggestedMaxPackageSizeInBytes);
    }

    public static TimeSpan GetFileUploadTimeout(
      IVssRequestContext requestContext,
      int suggestedTimeoutMinutes)
    {
      int num1 = suggestedTimeoutMinutes;
      int num2 = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/LargeExtensionUpload/PackageTimeoutMins", 0);
      if (num2 > 0)
        num1 = num2;
      return TimeSpan.FromMinutes((double) num1);
    }

    public static string ConvertTimeSpanToReadableString(TimeSpan value) => value.TotalHours >= 1.0 ? (value.TotalDays >= 1.0 ? (value.Hours <= 0 ? value.Days.ToString() + " day(s)" : value.Days.ToString() + " day(s), " + value.Hours.ToString() + " hour(s)") : value.Hours.ToString() + " hour(s), " + value.Minutes.ToString() + " minute(s)") : value.Minutes.ToString() + " minute(s)";

    public static void ValidatePackageSize(
      IVssRequestContext requestContext,
      long packageSize,
      long suggestedMaxPackageSizeInBytes)
    {
      long packageSizeInBytes = GalleryServerUtil.GetMaxPackageSizeInBytes(requestContext, suggestedMaxPackageSizeInBytes);
      if (packageSize > packageSizeInBytes)
      {
        ExtensionSizeExceededException exceededException = new ExtensionSizeExceededException(packageSize, packageSizeInBytes);
        requestContext.TraceException(12062011, "Gallery", nameof (ValidatePackageSize), (Exception) exceededException);
        throw exceededException;
      }
    }

    public static string TruncateString(string value, int limitingLength) => string.IsNullOrWhiteSpace(value) || value.Length < limitingLength ? value : value.Substring(0, limitingLength);

    public static bool IsRequestFromChinaRegion(IVssRequestContext requestContext)
    {
      string requestCountryCode = requestContext.GetService<IGeoLocationService>().GetRequestCountryCode(requestContext);
      return !string.IsNullOrWhiteSpace(requestCountryCode) && requestCountryCode.Equals("CN", StringComparison.OrdinalIgnoreCase);
    }

    internal static void InternalParseInstallationTargetVersion(InstallationTarget target)
    {
      if (target == null)
        throw new ArgumentNullException(nameof (target));
      if (!string.IsNullOrEmpty(target.TargetVersion) && target.TargetVersion.Length > 128)
        throw new InvalidVersionException(GalleryResources.InvalidVersionRange((object) target.TargetVersion));
      if (string.IsNullOrEmpty(target.TargetVersion) || string.IsNullOrWhiteSpace(target.TargetVersion))
      {
        target.MinInclusive = true;
        target.MaxInclusive = true;
        target.MinVersion = new Version(0, 0, 0, 0);
        target.MaxVersion = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
        target.TargetVersion = "";
      }
      else if (GalleryServerUtil.IsSingleVersion(target.TargetVersion))
      {
        target.MinInclusive = true;
        target.MaxInclusive = true;
        string versionString = target.TargetVersion.StartsWith("[", StringComparison.CurrentCultureIgnoreCase) ? target.TargetVersion.Substring(1, target.TargetVersion.Length - 2) : target.TargetVersion;
        target.MinVersion = GalleryServerUtil.ConvertStringToVersion(versionString, false);
        target.MaxVersion = GalleryServerUtil.ConvertStringToVersion(versionString, true);
      }
      else
      {
        GalleryServerUtil.TokenizerVersionRange(target);
        if (target.MinVersion > target.MaxVersion)
          throw new InvalidVersionException(GalleryResources.InvalidMinGreaterThanMaxVersion((object) target.TargetVersion));
      }
    }

    private static bool IsSingleVersion(string version)
    {
      Regex regex = new Regex("^(\\d+)(\\.\\d+){0,3}$");
      return regex.Match(version).Success || version.Length > 2 && version.StartsWith("[", StringComparison.CurrentCultureIgnoreCase) && version.EndsWith("]", StringComparison.CurrentCultureIgnoreCase) && regex.Match(version.Substring(1, version.Length - 2)).Success;
    }

    private static void RefineInstallationTargets(IEnumerable<InstallationTarget> targets)
    {
      IList<InstallationTarget> targets1 = targets as IList<InstallationTarget>;
      GalleryServerUtil.SplitVstsInstallationTarget(targets1, "Microsoft.VisualStudio.Services");
      GalleryServerUtil.SplitVstsInstallationTarget(targets1, "Microsoft.VisualStudio.Services.Integration");
    }

    private static void SplitVstsInstallationTarget(
      IList<InstallationTarget> targets,
      string extensionTarget)
    {
      string str1 = extensionTarget.Equals("Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) ? "Microsoft.TeamFoundation.Server" : "Microsoft.TeamFoundation.Server.Integration";
      string str2 = extensionTarget.Equals("Microsoft.VisualStudio.Services", StringComparison.OrdinalIgnoreCase) ? "Microsoft.VisualStudio.Services.Cloud" : "Microsoft.VisualStudio.Services.Cloud.Integration";
      List<InstallationTarget> list = targets.Where<InstallationTarget>((Func<InstallationTarget, bool>) (x => string.Compare(x.Target, extensionTarget, StringComparison.OrdinalIgnoreCase) == 0)).ToList<InstallationTarget>();
      if (list == null)
        return;
      foreach (InstallationTarget installationTarget1 in list)
      {
        InstallationTarget installationTarget2 = new InstallationTarget();
        installationTarget2.Target = str1;
        installationTarget2.TargetVersion = installationTarget1.TargetVersion;
        InstallationTarget installationTarget3 = new InstallationTarget();
        installationTarget3.Target = str2;
        installationTarget3.TargetVersion = string.Empty;
        targets.Add(installationTarget2);
        targets.Add(installationTarget3);
      }
      foreach (InstallationTarget installationTarget in list)
        targets.Remove(installationTarget);
    }

    private static bool TokenizerVersionRange(InstallationTarget target)
    {
      string versionRange = target.TargetVersion.Trim();
      bool minInclusive;
      bool maxInclusive;
      GalleryServerUtil.GetMinAndMaxInclusive(versionRange, out minInclusive, out maxInclusive);
      string[] strArray = versionRange.Substring(1, versionRange.Length - 2).Trim().Split(',');
      target.MinVersion = strArray.Length == 2 ? GalleryServerUtil.ConvertStringToVersion(strArray[0].Trim(), !minInclusive) : throw new InvalidVersionException(GalleryResources.InvalidVersionRange((object) target.TargetVersion));
      target.MaxVersion = !string.IsNullOrEmpty(strArray[1]) ? GalleryServerUtil.ConvertStringToVersion(strArray[1].Trim(), maxInclusive) : new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
      target.MinInclusive = minInclusive;
      target.MaxInclusive = maxInclusive;
      return true;
    }

    internal static Version ConvertStringToVersion(
      string versionString,
      bool isSetDefaultToMaxValue)
    {
      Regex regex = new Regex("^(\\d+)(\\.\\d+){0,3}$");
      versionString = !versionString.IsNullOrEmpty<char>() ? versionString.Trim() : throw new InvalidVersionException(GalleryResources.InvalidTargetVersion((object) "null"));
      string input = versionString;
      if (!regex.Match(input).Success || !GalleryServerUtil.IsValidVersion(versionString))
        throw new InvalidVersionException(GalleryResources.InvalidTargetVersion((object) versionString));
      string[] strArray = versionString.Trim().Split('.');
      int num = 0;
      if (isSetDefaultToMaxValue)
        num = int.MaxValue;
      if (strArray.Length < 1 || strArray.Length > 4)
        throw new InvalidVersionException(GalleryResources.InvalidTargetVersion((object) versionString));
      int[] numArray = new int[4]
      {
        int.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture),
        strArray.Length == 1 ? num : int.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture),
        strArray.Length < 1 || strArray.Length > 2 ? int.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture) : num,
        strArray.Length < 1 || strArray.Length > 3 ? int.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture) : num
      };
      return new Version(numArray[0], numArray[1], numArray[2], numArray[3]);
    }

    private static void GetMinAndMaxInclusive(
      string versionRange,
      out bool minInclusive,
      out bool maxInclusive)
    {
      char ch1 = versionRange[0];
      char ch2 = versionRange[versionRange.Length - 1];
      if (ch1 == '[')
      {
        minInclusive = true;
      }
      else
      {
        if (ch1 != '(')
          throw new InvalidVersionException(GalleryResources.InvalidMinInclusiveCharacter((object) versionRange));
        minInclusive = false;
      }
      if (ch2 == ']')
      {
        maxInclusive = true;
      }
      else
      {
        if (ch2 != ')')
          throw new InvalidVersionException(GalleryResources.InvalidMaxInclusiveCharacter((object) versionRange));
        maxInclusive = false;
      }
    }

    public static DateTime GetAfterDateForLastNDays(int? lastNDays)
    {
      DateTime dateForLastNdays = DateTime.MinValue;
      if (lastNDays.HasValue)
        dateForLastNdays = DateTime.UtcNow.AddDays((double) (-1 * Math.Abs(lastNDays.Value))).Date;
      return dateForLastNdays;
    }

    public static bool IsVSCodeUserAgent(IVssRequestContext tfsRequestContext)
    {
      string userAgent = tfsRequestContext.UserAgent;
      return !string.IsNullOrEmpty(userAgent) && userAgent.StartsWith("VSCode", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsVSIdeUserAgent(IVssRequestContext tfsRequestContext)
    {
      string userAgent = tfsRequestContext.UserAgent;
      return !string.IsNullOrEmpty(userAgent) && userAgent.StartsWith("VSIDE", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsVsCodeExtensionRequest(ExtensionQuery extensionQuery)
    {
      if (!extensionQuery.Filters.Any<QueryFilter>())
        return false;
      IEnumerable<FilterCriteria> filterCriterias = extensionQuery.Filters[0].Criteria.Where<FilterCriteria>((Func<FilterCriteria, bool>) (x => x.FilterType == 8));
      if (filterCriterias.IsNullOrEmpty<FilterCriteria>())
        return false;
      List<InstallationTarget> other = new List<InstallationTarget>()
      {
        new InstallationTarget()
        {
          Target = filterCriterias.FirstOrDefault<FilterCriteria>().Value
        }
      };
      return GalleryInstallationTargets.VsCodeInstallationTargets.Overlaps((IEnumerable<InstallationTarget>) other);
    }

    public static Guid GetUserVsid(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (requestContext.UserContext != (IdentityDescriptor) null)
        identity = requestContext.GetUserIdentity();
      Guid userVsid = Guid.Empty;
      if (identity != null)
        userVsid = identity.Id;
      return userVsid;
    }

    public static bool IsVsExtensionPackage(PackageDetails containerPackageDetails)
    {
      int num1 = GalleryUtil.IsVsVsixTypeZipPackage(containerPackageDetails) ? 1 : 0;
      bool flag = false;
      if (containerPackageDetails?.Manifest.Installation != null)
        flag = GalleryUtil.IsVSInstallationTargets((IEnumerable<InstallationTarget>) containerPackageDetails.Manifest.Installation);
      int num2 = flag ? 1 : 0;
      return (num1 | num2) != 0;
    }

    public static PackageDetails ParseVSIXPackage(
      IVssRequestContext requestContext,
      Stream extensionPackageStream)
    {
      try
      {
        return VSIXPackage.Parse(extensionPackageStream);
      }
      catch (InvalidPackageFormatException ex)
      {
        requestContext.Trace(12061000, TraceLevel.Error, "Gallery", nameof (GalleryServerUtil), requestContext.ActivityId.ToString());
        throw;
      }
      catch (InvalidOperationException ex)
      {
        throw new InvalidPackageFormatException(GalleryResources.InvalidContentPackageManifest((object) Regex.Match(ex.Message, "\\(([^)]*)\\)").Groups[1].Value, (object) ex.InnerException.Message.Replace("Instance validation error:", "")));
      }
      catch (FileFormatException ex)
      {
        throw new InvalidPackageFormatException(GalleryResources.InvalidPackageData());
      }
    }

    public static bool IsMsPublisher(string publisherName) => string.Equals(publisherName, "ms", StringComparison.OrdinalIgnoreCase);

    public static string GetPricingCategory(PublishedExtensionFlags extensionFlags)
    {
      string pricingCategory = GalleryResources.PricingFree();
      if (extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.Trial))
        pricingCategory = GalleryResources.PricingTrial();
      else if (extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.Paid))
        pricingCategory = GalleryResources.PricingPaid();
      return pricingCategory;
    }

    public static string GetPayloadFileName(PublishedExtension extension)
    {
      if (extension.Versions.IsNullOrEmpty<ExtensionVersion>())
        return (string) null;
      string payloadFileName = (string) null;
      ExtensionVersion version = extension.Versions[0];
      if (!version.Properties.IsNullOrEmpty<KeyValuePair<string, string>>())
      {
        foreach (KeyValuePair<string, string> property in version.Properties)
        {
          if (string.Equals("Microsoft.VisualStudio.Services.Payload.FileName", property.Key))
            return property.Value;
        }
      }
      int num = 0;
      foreach (ExtensionFile file in version.Files)
      {
        if (string.Equals(file.AssetType, "Microsoft.VisualStudio.Ide.Payload"))
        {
          num = file.FileId;
          break;
        }
      }
      foreach (ExtensionFile file in extension.Versions[0].Files)
      {
        if (!string.Equals(file.AssetType, "Microsoft.VisualStudio.Ide.Payload") && num == file.FileId)
        {
          payloadFileName = file.AssetType;
          break;
        }
      }
      return payloadFileName;
    }

    public static List<Badge> GetExtensionBadges(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      Guid validationId,
      string targetPlatform = null)
    {
      List<Badge> extensionBadges = new List<Badge>();
      IPublisherAssetService service = requestContext.GetService<IPublisherAssetService>();
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo("Microsoft.VisualStudio.Services.VsixManifest", (string) null)
      };
      if (!extension.IsVsExtension())
      {
        try
        {
          ExtensionAsset extensionAsset = service.QueryAsset(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, version, validationId, (IEnumerable<AssetInfo>) assetTypes, (string) null, (string) null, true, targetPlatform);
          if (extensionAsset != null && extensionAsset.AssetFile != null)
          {
            ExtensionFile assetFile = extensionAsset.AssetFile;
            using (Stream stream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _))
              extensionBadges = GalleryServerUtil._getBadgesAndMetadata(extension, stream);
          }
          else
            extensionBadges = GalleryServerUtil._getBadgesAndMetadataOld(requestContext, service, extension, version);
        }
        catch (ExtensionAssetNotFoundException ex)
        {
          extensionBadges = GalleryServerUtil._getBadgesAndMetadataOld(requestContext, service, extension, version);
        }
      }
      return extensionBadges;
    }

    public static IReadOnlyCollection<string> GetAllowedVSCodeTargetPlatforms(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<string>) GalleryServerUtil.GetAllowedVSCodeTargetPlatformPairs(requestContext).Keys.ToList<string>();
    }

    public static IReadOnlyCollection<string> GetLegacyVSCodeTargetPlatforms(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<string>) GalleryServerUtil.GetLegacyVSCodeTargetPlatformPairs(requestContext).Keys.ToList<string>();
    }

    public static IReadOnlyCollection<string> GetAllVSCodeTargetPlatforms(
      IVssRequestContext requestContext)
    {
      return (IReadOnlyCollection<string>) GalleryServerUtil.GetAllVSCodeTargetPlatformPairs(requestContext).Keys.ToList<string>();
    }

    public static IReadOnlyDictionary<string, string> GetAllowedVSCodeTargetPlatformPairs(
      IVssRequestContext requestContext)
    {
      return GalleryServerUtil.GetSeparatedPlatforms(GalleryServerUtil.GetValueFromRegistryForTypeString(requestContext, "/Configuration/Service/Gallery/PlatformSpecificExtensions/SupportedVSCodeTargetPlatforms"));
    }

    public static IReadOnlyDictionary<string, string> GetLegacyVSCodeTargetPlatformPairs(
      IVssRequestContext requestContext)
    {
      return GalleryServerUtil.GetSeparatedPlatforms(GalleryServerUtil.GetValueFromRegistryForTypeString(requestContext, "/Configuration/Service/Gallery/PlatformSpecificExtensions/LegacySupportedVSCodeTargetPlatforms"));
    }

    public static IReadOnlyDictionary<string, string> GetAllVSCodeTargetPlatformPairs(
      IVssRequestContext requestContext)
    {
      string registryForTypeString = GalleryServerUtil.GetValueFromRegistryForTypeString(requestContext, "/Configuration/Service/Gallery/PlatformSpecificExtensions/LegacySupportedVSCodeTargetPlatforms");
      return GalleryServerUtil.GetSeparatedPlatforms(string.Join(";", new string[2]
      {
        GalleryServerUtil.GetValueFromRegistryForTypeString(requestContext, "/Configuration/Service/Gallery/PlatformSpecificExtensions/SupportedVSCodeTargetPlatforms"),
        registryForTypeString
      }));
    }

    private static IReadOnlyDictionary<string, string> GetSeparatedPlatforms(
      string semiColonSeparatedAllowedVSCodeTargetPlatforms)
    {
      IDictionary<string, string> dictionary = (IDictionary<string, string>) new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(semiColonSeparatedAllowedVSCodeTargetPlatforms))
      {
        foreach (string delimiterSeparatedTerm in (IEnumerable<string>) GalleryUtil.SplitDelimiterSeparatedTerms(semiColonSeparatedAllowedVSCodeTargetPlatforms, ';'))
        {
          if (!string.IsNullOrEmpty(delimiterSeparatedTerm))
          {
            List<string> list = GalleryUtil.SplitDelimiterSeparatedTerms(delimiterSeparatedTerm, ',').ToList<string>();
            if (list.Count == 2)
              dictionary.TryAdd<string, string>(list[0], list[1]);
          }
        }
      }
      return (IReadOnlyDictionary<string, string>) dictionary;
    }

    public static int GetManageVSCodeReportsRecordsCountPerPage(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/PlatformSpecificExtensions/RecordsCountPerPageOnManageTabInReports", 8);

    public static bool IsVersionHasSupportForTargetPlatform(
      PublishedExtension extension,
      string versionRequested,
      string targetPlatform)
    {
      bool requestedVersionHasSupportForRequestedTargetPlatform = false;
      extension.Versions.ForEach((Action<ExtensionVersion>) (extensionVersion =>
      {
        if (!string.Equals(versionRequested, extensionVersion.Version) || !string.Equals(extensionVersion.TargetPlatform, targetPlatform))
          return;
        requestedVersionHasSupportForRequestedTargetPlatform = true;
      }));
      return requestedVersionHasSupportForRequestedTargetPlatform;
    }

    public static void ValidateIfExtensionVersionEverSupportedTargetPlatform(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string version,
      string targetPlatform)
    {
      if (!GalleryServerUtil.GetAllVSCodeTargetPlatforms(requestContext).Contains<string>(targetPlatform))
        throw new NotSupportedException(GalleryResources.ErrorUnSupportedVSCodeTargetPlatformSupplied((object) targetPlatform));
      if (!GalleryServerUtil.IsVersionHasSupportForTargetPlatform(extension, version, targetPlatform))
        throw new ExtensionVersionHasNoSupportForRequestedTargetPlatformsException(GalleryResources.ErrorExtensionVersionHasNoSupportForRequestedTargetPlatform((object) version, (object) GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName), (object) targetPlatform));
    }

    public static string GetValueFromRegistryForTypeString(
      IVssRequestContext requestContext,
      string registryPath)
    {
      return requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) registryPath, string.Empty);
    }

    private static List<Badge> _getBadgesAndMetadataOld(
      IVssRequestContext requestContext,
      IPublisherAssetService publisherAssetService,
      PublishedExtension extension,
      string version)
    {
      List<Badge> extensionBadges = new List<Badge>();
      string message = string.Format("Using old method to fetch vsix for {0}.{1}.{2}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) version);
      requestContext.Trace(12061113, TraceLevel.Info, "Gallery", "GetExtensionBadges", message);
      AssetInfo[] assetTypes = new AssetInfo[1]
      {
        new AssetInfo("Microsoft.VisualStudio.Services.VSIXPackage", (string) null)
      };
      ExtensionFile assetFile = publisherAssetService.QueryAsset(requestContext, extension.Publisher.PublisherName, extension.ExtensionName, version, Guid.Empty, (IEnumerable<AssetInfo>) assetTypes, (string) null, (string) null, true).AssetFile;
      if (assetFile != null)
      {
        using (Stream packageStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) assetFile.FileId, false, out byte[] _, out long _, out CompressionType _))
        {
          VSIXPackage.Parse(packageStream);
          VSIXPackage.ExtractParts(packageStream, (Action<Uri, string, Stream>) ((partUri, contentType, partStream) =>
          {
            if (!partUri.ToString().EndsWith(".vsixmanifest", StringComparison.OrdinalIgnoreCase))
              return;
            extensionBadges = GalleryServerUtil._getBadgesAndMetadata(extension, partStream);
          }));
        }
      }
      return extensionBadges;
    }

    private static List<Badge> _getBadgesAndMetadata(PublishedExtension extension, Stream stream)
    {
      List<Badge> badgesAndMetadata = new List<Badge>();
      if (stream != null)
      {
        XmlDocument document = XmlUtility.GetDocument(stream);
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
        nsmgr.AddNamespace("x", document.DocumentElement.NamespaceURI);
        XmlNodeList xmlNodeList = document.SelectNodes("/x:PackageManifest/x:Metadata/x:Badges/x:Badge", nsmgr);
        if (xmlNodeList != null && xmlNodeList.Count != 0)
        {
          foreach (XmlElement xmlElement in document.SelectNodes("/x:PackageManifest/x:Metadata/x:Badges/x:Badge", nsmgr).OfType<XmlElement>())
          {
            if (!badgesAndMetadata.Contains(new Badge(xmlElement.GetAttribute("Link"), xmlElement.GetAttribute("ImgUri"), xmlElement.GetAttribute("Description"))))
            {
              badgesAndMetadata.Add(new Badge(xmlElement.GetAttribute("Link"), xmlElement.GetAttribute("ImgUri"), xmlElement.GetAttribute("Description")));
              if (badgesAndMetadata.Count == 5)
                break;
            }
          }
        }
        if (!extension.Flags.HasFlag((System.Enum) PublishedExtensionFlags.Public))
        {
          if (extension.InstallationTargets == null)
            extension.InstallationTargets = document.SelectNodes("/x:PackageManifest/x:Installation/x:InstallationTarget", nsmgr).OfType<XmlElement>().Select<XmlElement, InstallationTarget>((Func<XmlElement, InstallationTarget>) (elem => new InstallationTarget()
            {
              Target = elem.GetAttribute("Id")
            })).ToList<InstallationTarget>();
          if (extension.Tags == null && document.SelectSingleNode("/x:PackageManifest/x:Metadata/x:Tags", nsmgr) is XmlElement xmlElement1 && !string.IsNullOrWhiteSpace(xmlElement1.InnerText))
            extension.Tags = ((IEnumerable<string>) xmlElement1.InnerText.Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (t => t != null ? t.Trim() : t)).Where<string>((Func<string, bool>) (t => !string.IsNullOrWhiteSpace(t))).ToList<string>();
          if (extension.Categories == null && document.SelectSingleNode("/x:PackageManifest/x:Metadata/x:Categories", nsmgr) is XmlElement xmlElement2 && !string.IsNullOrWhiteSpace(xmlElement2.InnerText))
            extension.Categories = ((IEnumerable<string>) xmlElement2.InnerText.Split(new char[1]
            {
              ','
            }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (c => c != null ? c.Trim() : c)).Where<string>((Func<string, bool>) (c => !string.IsNullOrWhiteSpace(c))).ToList<string>();
        }
      }
      return badgesAndMetadata;
    }

    public static string GetContentDispositionHeaderValueForAsset(
      IVssRequestContext requestContext,
      string assetName)
    {
      if (assetName.EndsWith("/", StringComparison.OrdinalIgnoreCase))
        assetName = assetName.Substring(0, assetName.Length - 1);
      string fileName = Path.GetFileName(assetName);
      Path.GetExtension(fileName);
      string headerValueForAsset = (string) null;
      foreach (string attachmentAssetSuffix in (IEnumerable<string>) GalleryServerUtil.c_contentDispositionAttachmentAssetSuffixes)
      {
        if (fileName.EndsWith(attachmentAssetSuffix, StringComparison.OrdinalIgnoreCase))
        {
          headerValueForAsset = "attachment";
          break;
        }
      }
      return headerValueForAsset;
    }

    public static string GetSubjectNameFromThumbprint(string thumbPrint)
    {
      string nameFromThumbprint = string.Empty;
      CertHandler certHandler = new CertHandler();
      X509Certificate2 certificateByThumbprint = certHandler.FindCertificateByThumbprint(thumbPrint);
      if (certificateByThumbprint != null)
        nameFromThumbprint = certHandler.GetSubjectCommonName(certificateByThumbprint);
      return nameFromThumbprint;
    }

    public static string CalculateBase64SHA256Hash(Stream stream)
    {
      using (MemoryStream destination = new MemoryStream())
      {
        stream.Seek(0L, SeekOrigin.Begin);
        stream.CopyTo((Stream) destination);
        using (SHA256Cng shA256Cng = new SHA256Cng())
        {
          byte[] array = destination.ToArray();
          return Convert.ToBase64String(shA256Cng.ComputeHash(array));
        }
      }
    }

    public static string CalculateSHA256Hash(string input)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(input);
      using (SHA256Cng shA256Cng = new SHA256Cng())
        return BitConverter.ToString(shA256Cng.ComputeHash(bytes)).Replace("-", "");
    }

    public static bool IsSharedWithUser(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid userId)
    {
      bool flag = false;
      if (requestContext != null && extension != null)
        flag = !requestContext.GetService<IPublishedExtensionService>().GetAccountsSharedWithUser(requestContext, extension, userId).IsNullOrEmpty<ExtensionShare>();
      return flag;
    }

    public static IEnumerable<string> MapOldCategoriesToVerticalAlignedCategories(
      IVssRequestContext requestContext,
      PublishedExtensionFlags extensionFlags,
      InstallationTarget[] installationTargetArray,
      IEnumerable<string> categories,
      bool bMandateNewCategoriesForVsts)
    {
      if (!GalleryUtil.IsVSTSExtensionResourceOrIntegration((IEnumerable<InstallationTarget>) installationTargetArray))
        return categories;
      if (categories.IsNullOrEmpty<string>())
      {
        if (!extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.BuiltIn) && !extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.System))
          throw new InvalidCategoryException(GalleryResources.OneOrMoreCategoryExpected());
        return categories;
      }
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return GalleryServerUtil.GetMappedCategories(categories);
      if (categories.Contains<string>("Other", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidCategoryException(GalleryResources.DeprecatedAzureDevopsCategory((object) "Other"));
      if (!bMandateNewCategoriesForVsts || extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.BuiltIn) || extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.System) || extensionFlags.HasFlag((System.Enum) PublishedExtensionFlags.Trusted))
        return GalleryServerUtil.GetMappedCategories(categories);
      HashSet<string> stringSet = new HashSet<string>((IEnumerable<string>) new List<string>()
      {
        "Azure Pipelines",
        "Azure Repos",
        "Azure Boards",
        "Azure Test Plans",
        "Azure Artifacts"
      }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string enumerable = string.Empty;
      for (int index = 0; index < categories.Count<string>(); ++index)
      {
        if (!stringSet.Contains(categories.ElementAt<string>(index)))
          enumerable = enumerable + categories.ElementAt<string>(index) + ", ";
      }
      if (!enumerable.IsNullOrEmpty<char>())
        throw new InvalidCategoryException(GalleryResources.DeprecatedAzureDevopsCategory((object) enumerable.Substring(0, enumerable.Length - 2)));
      return categories;
    }

    private static IEnumerable<string> GetMappedCategories(IEnumerable<string> categories)
    {
      HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string str = "";
      foreach (string category in categories)
      {
        if (!GalleryServerUtil.c_oldCategoryToVerticalCategoryMapping.TryGetValue(category, out str))
          str = category;
        source.Add(str);
      }
      return (IEnumerable<string>) source.ToList<string>();
    }

    public static void UpdateVsixManifest(
      IVssRequestContext requestContext,
      ref Stream packageStream,
      string publisherName,
      bool addMarketTag)
    {
      VSIXPackage.ExtractParts(packageStream, (Action<Uri, string, Stream>) ((partUri, contentType, partStream) =>
      {
        if (!partUri.ToString().EndsWith(".vsixmanifest", StringComparison.OrdinalIgnoreCase))
          return;
        XmlDocument document = XmlUtility.GetDocument(partStream);
        GalleryServerUtil.UpdateVsixManifestXML(requestContext, document, publisherName, addMarketTag);
        partStream.Seek(0L, SeekOrigin.Begin);
        document.Save(partStream);
        partStream.SetLength(partStream.Position);
        partStream.Close();
      }), FileAccess.ReadWrite);
      packageStream.Flush();
      packageStream.Seek(0L, SeekOrigin.Begin);
    }

    internal static void UpdateVsixManifestXML(
      IVssRequestContext requestContext,
      XmlDocument vsixManifest,
      string publisherDisplayName,
      bool addMarketTag)
    {
      XmlNamespaceManager nsmgr = new XmlNamespaceManager(vsixManifest.NameTable);
      nsmgr.AddNamespace("x", vsixManifest.DocumentElement.NamespaceURI);
      XmlElement xmlElement = vsixManifest.SelectSingleNode("/x:PackageManifest/x:Metadata", nsmgr) as XmlElement;
      XmlNode newChild1 = vsixManifest.SelectSingleNode("/x:PackageManifest/x:Metadata/x:PublisherDetails", nsmgr);
      if (newChild1 == null)
      {
        newChild1 = (XmlNode) vsixManifest.CreateElement("PublisherDetails", vsixManifest.DocumentElement.NamespaceURI);
        xmlElement.AppendChild(newChild1);
      }
      XmlNode newChild2 = vsixManifest.SelectSingleNode("/x:PackageManifest/x:Metadata/x:PublisherDetails/x:DisplayName", nsmgr);
      if (newChild2 == null)
      {
        newChild2 = (XmlNode) vsixManifest.CreateElement("DisplayName", vsixManifest.DocumentElement.NamespaceURI);
        newChild1.AppendChild(newChild2);
      }
      newChild2.InnerText = publisherDisplayName;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment & addMarketTag)
      {
        XmlNode newChild3 = vsixManifest.SelectSingleNode("/x:PackageManifest/x:Metadata/x:Tags", nsmgr);
        if (newChild3 == null)
        {
          newChild3 = (XmlNode) vsixManifest.CreateElement("Tags", vsixManifest.DocumentElement.NamespaceURI);
          xmlElement.AppendChild(newChild3);
        }
        if (newChild3.InnerText != "")
          newChild3.InnerText += ",";
        newChild3.InnerText += "__market";
      }
      XmlNode xmlNode = vsixManifest.SelectSingleNode("/x:PackageManifest/x:Metadata/x:Categories", nsmgr);
      if (xmlNode == null)
        return;
      List<string> list = ((IEnumerable<string>) xmlNode.InnerText.Split(',')).Select<string, string>((Func<string, string>) (p => p.Trim())).ToList<string>();
      HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool flag = false;
      string str = "";
      for (int index = 0; index < list.Count; ++index)
      {
        if (GalleryServerUtil.c_verticalCategoryToOldCategoryMapping.TryGetValue(list[index], out str))
        {
          list[index] = str;
          flag = true;
        }
        source.Add(list[index]);
      }
      if (!flag)
        return;
      xmlNode.InnerText = string.Join(",", (IEnumerable<string>) source.ToList<string>());
    }

    internal static string GetHeaderValue(HttpRequestHeaders requestHeaders, string headerName)
    {
      string headerValue = (string) null;
      IEnumerable<string> values;
      if (requestHeaders != null && requestHeaders.TryGetValues(headerName, out values))
        headerValue = values.FirstOrDefault<string>();
      return headerValue;
    }

    internal static GalleryProductTypesEnum GetProductTypeEnumForInstallationTargets(
      IEnumerable<InstallationTarget> installationTargets,
      bool useAzureDevOps = false)
    {
      if (!installationTargets.IsNullOrEmpty<InstallationTarget>())
      {
        if (GalleryInstallationTargets.VstsInstallationTargets.Overlaps(installationTargets))
          return !useAzureDevOps ? GalleryProductTypesEnum.Vsts : GalleryProductTypesEnum.AzureDevOps;
        if (GalleryInstallationTargets.VsCodeInstallationTargets.Overlaps(installationTargets))
          return GalleryProductTypesEnum.VsCode;
        if (GalleryInstallationTargets.VsInstallationTargets.Overlaps(installationTargets))
          return GalleryProductTypesEnum.Vs;
        if (GalleryInstallationTargets.VsForMacInstallationTargets.Overlaps(installationTargets))
          return GalleryProductTypesEnum.VsForMac;
      }
      else
        TeamFoundationTracingService.TraceRaw(12062069, TraceLevel.Error, "Gallery", nameof (GalleryServerUtil), "Extension's installationTargets can't be empty.");
      return GalleryProductTypesEnum.None;
    }

    public static List<PublishedExtension> SetEffectiveDisplayedStarRating(
      IVssRequestContext requestContext,
      List<PublishedExtension> extensions)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/RatingAndReview/RegistryPathForMinimumReviewsToShowStarRatings", 0);
      foreach (PublishedExtension extension in extensions)
      {
        if (extension != null && extension.Statistics != null && extension.Statistics.Exists((Predicate<ExtensionStatistic>) (x => x.StatisticName == "ratingcount")) && extension.Statistics.Find((Predicate<ExtensionStatistic>) (x => x.StatisticName == "ratingcount")).Value <= (double) num && extension.Statistics.Exists((Predicate<ExtensionStatistic>) (x => x.StatisticName == "averagerating")))
          extension.Statistics.Find((Predicate<ExtensionStatistic>) (x => x.StatisticName == "averagerating")).Value = 0.0;
      }
      return extensions;
    }

    public static bool IsValidUrl(string urlString)
    {
      GalleryServerUtil.CheckUrlLength(urlString);
      Uri result;
      return (!Uri.TryCreate(urlString, UriKind.Absolute, out result) ? 0 : (result.Scheme == Uri.UriSchemeHttp ? 1 : (result.Scheme == Uri.UriSchemeHttps ? 1 : 0))) != 0 && GalleryServerUtil.IsValidTld(result);
    }

    private static bool IsValidTld(Uri uri)
    {
      int num = 32;
      string[] strArray = uri.Host.Split('.');
      return strArray.Length >= 1 && strArray[strArray.Length - 1].Length <= num;
    }

    private static void CheckUrlLength(string url)
    {
      int num = 512;
      if (url.Length > num)
        throw new ArgumentException(GalleryResources.URLSizeTooBig());
    }

    public static bool IsValidDomain(IVssRequestContext requestContext, string domain)
    {
      int num = 0;
      if (!GalleryServerUtil.IsValidUrl(domain))
        return false;
      Uri uri = new Uri(domain);
      string[] source = uri.Host.Split('.');
      if (source[0] == "www")
        num = 1;
      if (source.Length - num > 2 && (((IEnumerable<string>) source).Last<string>().Length != 2 || source.Length - num != 3))
        throw new ArgumentException(GalleryResources.UrlContainsSubdomain());
      if (uri.Scheme != Uri.UriSchemeHttps)
        throw new ArgumentException(GalleryResources.UrlSchemeIsNotHttps());
      if (uri.PathAndQuery != "/")
        throw new ArgumentException(GalleryResources.UrlHasPathParameters());
      try
      {
        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, domain);
        ProductInfoHeaderValue productInfoHeaderValue = new ProductInfoHeaderValue("VSMarketplace", "1.0");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 1.0));
        request.Headers.UserAgent.Add(productInfoHeaderValue);
        using (HttpResponseMessage result = GalleryServerUtil.client.SendAsync(request).GetAwaiter().GetResult())
        {
          if (result.StatusCode != HttpStatusCode.OK)
            throw new ArgumentException(GalleryResources.DomainCouldNotBeReached());
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(12062088, TraceLevel.Error, "gallery", GalleryServerUtil.ServiceLayer, ex.Message);
        throw new ArgumentException(GalleryResources.DomainCouldNotBeReached());
      }
      return true;
    }

    public static bool IsVsixConsolidationEnabledForVsExtension(List<ExtensionMetadata> metadata) => !metadata.IsNullOrEmpty<ExtensionMetadata>() && metadata.Any<ExtensionMetadata>((Func<ExtensionMetadata, bool>) (m => "HasConsolidatedVsix".Equals(m.Key) && bool.TrueString.Equals(m.Value, StringComparison.OrdinalIgnoreCase)));

    public static int GetApplicableLatestVersionRecordIndex(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ISet<string> SKUs,
      Version vsIdeVersion,
      string productArchitecture)
    {
      int versionRecordIndex = 0;
      if (extension.Versions.Count > 1)
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        ISet<string> latestTargetPlatformSet = (ISet<string>) new HashSet<string>();
        extension.Versions.ForEach((Action<ExtensionVersion>) (version =>
        {
          if (version.TargetPlatform == null)
            return;
          latestTargetPlatformSet.Add(version.TargetPlatform);
        }));
        List<InstallationTarget> all = extension.InstallationTargets.FindAll((Predicate<InstallationTarget>) (it => latestTargetPlatformSet.Contains(it.TargetPlatform)));
        all.ForEach((Action<InstallationTarget>) (it => GalleryServerUtil.InternalParseInstallationTargetVersion(it)));
        all.Sort((Comparison<InstallationTarget>) ((it1, it2) => Version.Parse(it2.ExtensionVersion).CompareTo(Version.Parse(it1.ExtensionVersion))));
        string str = (string) null;
        foreach (InstallationTarget installationTarget in all)
        {
          if ((productArchitecture == null || productArchitecture.Equals(installationTarget.ProductArchitecture)) && (SKUs.IsNullOrEmpty<string>() || SKUs.Contains(installationTarget.Target)) && installationTarget.IsApplicableForVersion(vsIdeVersion))
          {
            str = installationTarget.TargetPlatform;
            break;
          }
        }
        if (str != null)
        {
          for (int index = 0; index < extension.Versions.Count; ++index)
          {
            if (extension.Versions[index].TargetPlatform.Equals(str))
            {
              versionRecordIndex = index;
              break;
            }
          }
        }
        stopwatch.Stop();
        string format = string.Format("GetApplicableVersionIndexOfVsExtensionWithConsolidatedVsixs: TimeTakenMs:{0}", (object) stopwatch.ElapsedMilliseconds);
        requestContext.TraceAlways(12062093, TraceLevel.Info, "gallery", "GetApplicableVersionIndexOfVsExtensionWithConsolidatedVsixs", format);
      }
      return versionRecordIndex;
    }

    public static bool IsVsixConsolidationEnabledForVsExtension(
      IList<KeyValuePair<string, string>> metadata)
    {
      return !metadata.IsNullOrEmpty<KeyValuePair<string, string>>() && metadata.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (m => "HasConsolidatedVsix".Equals(m.Key) && bool.TrueString.Equals(m.Value, StringComparison.OrdinalIgnoreCase)));
    }

    public static int GetDeletePreventMinAcquisitionCount(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/RemoveExtensionMinAcquisitionCount", 100);

    public static int GetDeletePreventMinDaysCount(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/RemoveExtensionMinDayCount", 30);
  }
}
