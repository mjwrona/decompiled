// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Extensions.OnPremPublishedExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Extensions
{
  internal class OnPremPublishedExtensionService : 
    IOnPremPublishedExtensionService,
    IVssFrameworkService
  {
    private const string s_layer = "OnPremPublishedExtensionService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckOnPremisesDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<Guid> UpdateOnPremExtensionsFromHosted(IVssRequestContext requestContext)
    {
      List<Guid> guidList = new List<Guid>();
      IPublishedExtensionService service = requestContext.GetService<IPublishedExtensionService>();
      ExtensionQuery extensionQuery1 = new ExtensionQuery()
      {
        AssetTypes = new List<string>()
        {
          "Microsoft.VisualStudio.Services.VSIXPackage"
        },
        Flags = ExtensionQueryFlags.ExcludeNonValidated,
        Filters = new List<QueryFilter>()
        {
          new QueryFilter()
          {
            Criteria = new List<FilterCriteria>()
            {
              new FilterCriteria() { FilterType = 1, Value = "__market" },
              new FilterCriteria() { FilterType = 1, Value = "$market" }
            }
          }
        }
      };
      IVssRequestContext requestContext1 = requestContext;
      ExtensionQuery extensionQuery2 = extensionQuery1;
      ExtensionQueryResult extensionQueryResult = service.QueryExtensions(requestContext1, extensionQuery2, (string) null);
      if (extensionQueryResult.Results != null && extensionQueryResult.Results.Count > 0 && extensionQueryResult.Results[0].Extensions != null && extensionQueryResult.Results[0].Extensions.Count > 0)
      {
        foreach (PublishedExtension extension in extensionQueryResult.Results[0].Extensions)
        {
          Guid guid = this.UpdateOnPremExtensionFromHosted(requestContext, extension.Publisher.PublisherName, extension.ExtensionName);
          guidList.Add(guid);
        }
      }
      return guidList;
    }

    public PublishExtensionOnPremResult PublishExtensionFromHosted(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      bool onlyUpdateForNewVersion = false,
      bool blockPreviewToPaidUpgrade = false)
    {
      PublishedExtension publishedExtension = (PublishedExtension) null;
      try
      {
        requestContext.TraceEnter(12061052, "Gallery", nameof (OnPremPublishedExtensionService), "Run");
        IPublishedExtensionService service1 = requestContext.GetService<IPublishedExtensionService>();
        string version1 = (string) null;
        bool flag = true;
        try
        {
          publishedExtension = service1.QueryExtension(requestContext, publisherName, extensionName, (string) null, ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeLatestVersionOnly, (string) null);
          if (publishedExtension.ShouldNotDownload())
          {
            requestContext.Trace(12061052, TraceLevel.Info, "gallery", nameof (OnPremPublishedExtensionService), "Should not download extension");
            return PublishExtensionOnPremResult.ShouldNotDownloadExtension;
          }
          if (onlyUpdateForNewVersion && publishedExtension.Versions != null && publishedExtension.Versions.Count > 0)
            version1 = publishedExtension.Versions[0].Version;
          flag = !publishedExtension.IsMarketExtension();
        }
        catch (ExtensionDoesNotExistException ex)
        {
          publishedExtension = (PublishedExtension) null;
        }
        ExtensionIdentifier extensionIdentifier = new ExtensionIdentifier(publisherName, extensionName);
        PublishedExtension marketExtension = this.GetMarketExtension(requestContext, extensionIdentifier.ToString());
        if (marketExtension == null)
          return PublishExtensionOnPremResult.ExtensionDoesNotExistInHosted;
        IPublisherService service2 = requestContext.GetService<IPublisherService>();
        Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher1;
        try
        {
          publisher1 = service2.QueryPublisher(requestContext, publisherName, PublisherQueryFlags.None);
          if (publisher1 != null)
          {
            if (publisher1.Flags == marketExtension.Publisher.Flags)
            {
              if (string.Equals(publisher1.DisplayName, marketExtension.Publisher.DisplayName, StringComparison.InvariantCulture))
                goto label_16;
            }
            requestContext.Trace(12061052, TraceLevel.Info, "gallery", nameof (OnPremPublishedExtensionService), string.Format("Updating publisher. Local publisher display name '{0}' and flags '{1}'. Marketplace publisher display name '{2}' and flags '{3}'", (object) publisher1.DisplayName, (object) publisher1.Flags.ToString(), (object) marketExtension.Publisher.DisplayName, (object) marketExtension.Publisher.Flags.ToString()));
            service2.UpdatePublisher(requestContext, publisherName, marketExtension.Publisher.DisplayName, marketExtension.Publisher.Flags, publisher1.ShortDescription, publisher1.LongDescription);
          }
        }
        catch (PublisherDoesNotExistException ex)
        {
          publisher1 = (Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher) null;
        }
label_16:
        if (blockPreviewToPaidUpgrade && publishedExtension != null && publishedExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Preview) && marketExtension != null && marketExtension.Flags.HasFlag((Enum) PublishedExtensionFlags.Paid))
          return PublishExtensionOnPremResult.CannotUpdateFromPreviewToPaid;
        ExtensionVersion version2 = marketExtension.Versions.Count == 1 ? marketExtension.Versions[0] : (ExtensionVersion) null;
        string uriString = string.Empty;
        if (version2 != null)
        {
          if (version1 != null)
          {
            Version version3 = new Version(version2.Version);
            Version version4 = new Version(version1);
            if (version4.CompareTo(version3) >= 0)
            {
              service1.AddExtensionIndexedTerm(requestContext, publisherName, extensionName, TagType.BasicTag, "__market", false);
              if (version4.CompareTo(version3) == 0)
                service1.QueueValidation(requestContext, publishedExtension, version1);
              return PublishExtensionOnPremResult.LocalVersionSameOrGreaterThanHosted;
            }
          }
          foreach (ExtensionFile file in version2.Files)
          {
            if (file.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage", StringComparison.OrdinalIgnoreCase))
            {
              uriString = file.Source;
              break;
            }
          }
        }
        Uri uri = !string.IsNullOrEmpty(uriString) ? new Uri(uriString) : throw new ExtensionVersionNotFoundException(extensionIdentifier.ToString());
        if (flag)
          uri = uri.AppendQuery("onpremDownload", bool.TrueString);
        using (Stream stream = this.GetHostedGalleryHttpClient(uri).GetAsset(uri.ToString()).SyncResult<Stream>())
        {
          PublishExtensionOnPremResult extensionOnPremResult = PublishExtensionOnPremResult.SuccessCreated;
          using (MemoryStream memoryStream = new MemoryStream())
          {
            stream.CopyTo((Stream) memoryStream);
            if (publishedExtension != null)
            {
              service1.UpdateExtension(requestContext, (Stream) memoryStream, extensionName, publisherName);
              extensionOnPremResult = PublishExtensionOnPremResult.SuccessUpdated;
            }
            else
            {
              service1.CreateExtension(requestContext, (Stream) memoryStream, publisherName);
              if (publisher1 == null)
              {
                Microsoft.VisualStudio.Services.Gallery.WebApi.Publisher publisher2 = service2.QueryPublisher(requestContext, publisherName, PublisherQueryFlags.None);
                service2.UpdatePublisher(requestContext, publisherName, marketExtension.Publisher.DisplayName, marketExtension.Publisher.Flags, publisher2.ShortDescription, publisher2.LongDescription);
              }
            }
          }
          service1.AddExtensionIndexedTerm(requestContext, publisherName, extensionName, TagType.BasicTag, "__market", false);
          return extensionOnPremResult;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12061052, "Gallery", nameof (OnPremPublishedExtensionService), ex);
        string action = "PublishTimeError";
        requestContext.GetService<IGalleryTelemetryHelperService>().PublishAppInsightsPerExtensionTelemetryHelper(requestContext, publishedExtension, action);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12061052, "Gallery", nameof (OnPremPublishedExtensionService), "Run");
      }
    }

    private Guid UpdateOnPremExtensionFromHosted(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      requestContext.GetService<IVssRegistryService>();
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      PublishExtensionJobData objectToSerialize = new PublishExtensionJobData()
      {
        PublisherName = publisherName,
        ExtensionName = extensionName,
        OnlyUpdateForNewVersion = true,
        BlockPreviewToPaidUpgrade = true
      };
      XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) objectToSerialize);
      string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Publish extension : {0}.{1}", (object) objectToSerialize.PublisherName, (object) objectToSerialize.ExtensionName);
      IVssRequestContext requestContext1 = requestContext;
      string jobName = str;
      XmlNode jobData = xml;
      return service.QueueOneTimeJob(requestContext1, jobName, "Microsoft.VisualStudio.Services.Gallery.Extensions.JobAgentPlugins.PublishExtensionJob", jobData, true);
    }

    private PublishedExtension GetMarketExtension(
      IVssRequestContext requestContext,
      string itemName)
    {
      string uriString = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/MarketplaceRootURL", "https://marketplace.visualstudio.com/");
      ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeLatestVersionOnly | ExtensionQueryFlags.UseFallbackAssetUri;
      QueryFilter queryFilter = new QueryFilter()
      {
        Criteria = new List<FilterCriteria>()
        {
          new FilterCriteria() { FilterType = 7, Value = itemName }
        },
        Direction = PagingDirection.Forward,
        PageSize = 10,
        PageNumber = 1,
        SortBy = 0,
        SortOrder = 1,
        PagingToken = (string) null
      };
      ExtensionQuery query = new ExtensionQuery()
      {
        Filters = new List<QueryFilter>() { queryFilter },
        Flags = extensionQueryFlags,
        AssetTypes = (List<string>) null
      };
      ExtensionQueryResult extensionQueryResult = this.GetHostedGalleryHttpClient(new Uri(uriString)).GetQueryResult("/_apis/public/gallery/extensionquery", query).SyncResult<ExtensionQueryResult>();
      PublishedExtension marketExtension = (PublishedExtension) null;
      if (extensionQueryResult.Results != null && extensionQueryResult.Results.Count > 0 && extensionQueryResult.Results[0].Extensions != null && extensionQueryResult.Results[0].Extensions.Count > 0)
        marketExtension = extensionQueryResult.Results[0].Extensions[0];
      return marketExtension;
    }

    internal virtual IHostedGalleryHttpClient GetHostedGalleryHttpClient(Uri baseUri) => (IHostedGalleryHttpClient) new HostedGalleryHttpClient(baseUri);
  }
}
