// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.FeedSourcesValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public class FeedSourcesValidator
  {
    public const int DefaultMaxUpstreamSourceCount = 20;
    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
    private IVssRequestContext requestContext;
    private ProjectReference projectReferenceForThisFeed;

    public static int GetMaxUpstreamSourceCount(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      RegistryQuery registryQuery;
      if (feed != null)
      {
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext1 = requestContext;
        registryQuery = FeedRegistryConstants.PerFeedMaxUpstreamSourceCount(feed);
        ref RegistryQuery local = ref registryQuery;
        int upstreamSourceCount = registryService.GetValue<int>(requestContext1, in local);
        if (upstreamSourceCount > 0)
          return upstreamSourceCount;
      }
      IVssRegistryService registryService1 = service;
      IVssRequestContext requestContext2 = requestContext;
      registryQuery = (RegistryQuery) "/Configuration/Feed/MaxUpstreamSourceCount";
      ref RegistryQuery local1 = ref registryQuery;
      return registryService1.GetValue<int>(requestContext2, in local1, false, 20);
    }

    public FeedSourcesValidator(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      ProjectReference project)
    {
      this.feed = feed;
      this.requestContext = requestContext;
      this.projectReferenceForThisFeed = project;
    }

    public async Task ValidateSourcesAsync(IList<UpstreamSource> upstreamSources)
    {
      HashSet<UpstreamIdentifier> NameAndLocationSet;
      if (upstreamSources == null)
        NameAndLocationSet = (HashSet<UpstreamIdentifier>) null;
      else if (!upstreamSources.Any<UpstreamSource>())
      {
        NameAndLocationSet = (HashSet<UpstreamIdentifier>) null;
      }
      else
      {
        int upstreamSourceCount = FeedSourcesValidator.GetMaxUpstreamSourceCount(this.requestContext, this.feed);
        foreach (IGrouping<string, UpstreamSource> source in upstreamSources.GroupBy<UpstreamSource, string>((Func<UpstreamSource, string>) (x => x.Protocol)))
        {
          if (source.Count<UpstreamSource>() > upstreamSourceCount)
            throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_TooManyUpstreamSourcesForProtocol((object) source.Key, (object) upstreamSourceCount));
        }
        Guid? nullable1;
        if (this.feed != null)
        {
          Guid? id = this.feed.Project?.Id;
          nullable1 = this.projectReferenceForThisFeed?.Id;
          if ((id.HasValue == nullable1.HasValue ? (id.HasValue ? (id.GetValueOrDefault() != nullable1.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
            throw new ArgumentException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_PassedProjectDoesNotMatchFeedProject());
        }
        NameAndLocationSet = new HashSet<UpstreamIdentifier>(UpstreamSourceHelper.UpstreamIdentifierComparer);
        Dictionary<string, HashSet<string>> PerProtocolNamesSet = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> PerProtocolLocationSet = new Dictionary<string, HashSet<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        UpstreamSourceValidator.SupportedUpstreamProtocols.ForEach<string>((Action<string>) (p => PerProtocolNamesSet[p] = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
        UpstreamSourceValidator.SupportedUpstreamProtocols.ForEach<string>((Action<string>) (p => PerProtocolLocationSet[p] = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
        ITeamFoundationFeatureAvailabilityService service = this.requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
        bool isUPackSameCollectionUpstreamsEnabled = service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UPackSameCollectionUpstreams");
        bool isUPackCrossCollectionUpstreamsEnabled = service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UPackCrossCollectionUpstreams");
        bool flag1 = service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UpstreamsAllowedForPublicFeeds.MSFT") && this.requestContext.IsMicrosoftTenant();
        bool isUpstreamsForPublicFeedsAllowed = service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UpstreamsAllowedForPublicFeeds") | flag1;
        if (isUPackSameCollectionUpstreamsEnabled | isUPackCrossCollectionUpstreamsEnabled)
        {
          PerProtocolNamesSet["UPack"] = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          PerProtocolLocationSet["UPack"] = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        }
        foreach (UpstreamSource upstreamSource1 in (IEnumerable<UpstreamSource>) upstreamSources)
        {
          UpstreamSource upstreamSource = upstreamSource1;
          UpstreamSourceValidator validator = UpstreamSourceValidator.Create(this.requestContext, upstreamSource);
          Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed1 = this.feed;
          UpstreamSource upstreamSource2;
          if (feed1 == null)
          {
            upstreamSource2 = (UpstreamSource) null;
          }
          else
          {
            IList<UpstreamSource> upstreamSources1 = feed1.UpstreamSources;
            upstreamSource2 = upstreamSources1 != null ? upstreamSources1.FirstOrDefault<UpstreamSource>((Func<UpstreamSource, bool>) (u => u.Id == upstreamSource.Id)) : (UpstreamSource) null;
          }
          UpstreamSource upstreamSource3 = upstreamSource2;
          Guid guid;
          if (upstreamSource3 != null && upstreamSource3.Location != upstreamSource.Location)
          {
            nullable1 = upstreamSource.InternalUpstreamFeedId;
            if (nullable1.HasValue)
            {
              nullable1 = upstreamSource.InternalUpstreamViewId;
              if (nullable1.HasValue)
              {
                nullable1 = upstreamSource.InternalUpstreamCollectionId;
                string collection = (nullable1 ?? this.requestContext.ServiceHost.InstanceId).ToString();
                nullable1 = upstreamSource.InternalUpstreamProjectId;
                ref Guid? local = ref nullable1;
                string project;
                if (!local.HasValue)
                {
                  project = (string) null;
                }
                else
                {
                  guid = local.GetValueOrDefault();
                  project = guid.ToString();
                }
                nullable1 = upstreamSource.InternalUpstreamFeedId;
                string feed2 = nullable1.ToString();
                nullable1 = upstreamSource.InternalUpstreamViewId;
                string view = nullable1.ToString();
                string internalUpstreamLocator = UpstreamSourceHelper.CreateInternalUpstreamLocator(collection, project, feed2, view);
                if (internalUpstreamLocator == upstreamSource3.Location)
                  upstreamSource.Location = internalUpstreamLocator;
              }
            }
          }
          // ISSUE: explicit non-virtual call
          bool flag2 = upstreamSource3 != null && !__nonvirtual (upstreamSource3.Equals(upstreamSource));
          if (((this.feed == null || this.feed.UpstreamSources == null ? 1 : (upstreamSource3 == null ? 1 : 0)) | (flag2 ? 1 : 0)) != 0)
            await validator.ValidateAsync();
          PerProtocolNamesSet[upstreamSource.Protocol].Add(validator.ValidateUpstreamNameAgainstOtherSources(PerProtocolNamesSet));
          PerProtocolLocationSet[upstreamSource.Protocol].Add(validator.ValidateUpstreamLocationAgainstOtherSources(PerProtocolLocationSet));
          NameAndLocationSet.Add(validator.CheckUpstreamNameLocationMatchOtherSource(NameAndLocationSet));
          if (isUpstreamsForPublicFeedsAllowed && this.projectReferenceForThisFeed?.Visibility == "Public")
          {
            bool flag3 = upstreamSource.UpstreamSourceType == UpstreamSourceType.Internal;
            if (flag3)
              flag3 = !await InternalSourceValidator.ElevatedIsPublicProjectScopedFeedAsync(this.requestContext, upstreamSource);
            if (flag3)
              throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_PublicFeedsCannotHavePrivateFeedAsUpstream());
            if (upstreamSource.UpstreamSourceType == UpstreamSourceType.Public && (object) WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource.Location) == null)
              throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_PublicFeedsCannotHaveCustomPublicUpstreams());
          }
          if (upstreamSource.UpstreamSourceType == UpstreamSourceType.Internal)
          {
            nullable1 = upstreamSource.InternalUpstreamFeedId;
            Guid? nullable2 = this.feed?.Id;
            if ((nullable1.HasValue == nullable2.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
              throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamToSameFeedIsDisallowed());
            Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed3 = this.feed;
            if (!((FeedCapabilities) (feed3 != null ? (int) feed3.Capabilities : 1)).HasFlag((Enum) FeedCapabilities.UpstreamV2))
              throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstream((object) upstreamSource.Location));
            if (!isUPackCrossCollectionUpstreamsEnabled & isUPackSameCollectionUpstreamsEnabled && upstreamSource.Protocol == "UPack")
            {
              nullable2 = upstreamSource.InternalUpstreamCollectionId;
              guid = this.requestContext.ServiceHost.InstanceId;
              if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != guid ? 1 : 0) : 0) : 1) != 0)
                throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_CrossCollectionUPackUpstreamInvalid((object) upstreamSource.Location));
            }
          }
          validator = (UpstreamSourceValidator) null;
        }
        NameAndLocationSet = (HashSet<UpstreamIdentifier>) null;
      }
    }
  }
}
