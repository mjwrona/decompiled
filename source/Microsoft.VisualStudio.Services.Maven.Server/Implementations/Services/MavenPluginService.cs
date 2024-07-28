// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.Services.MavenPluginService
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Constants;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Microsoft.VisualStudio.Services.Maven.Server.Utilities;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.Services
{
  public class MavenPluginService : IMavenPluginService, IVssFrameworkService
  {
    private const int MaxRetries = 10;

    protected string ProtocolReadOnlyFeatureFlagName => "Packaging.Maven.ReadOnly";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<MavenPluginList> GetPluginListAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<string>(groupId, nameof (groupId));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, MavenTracerPoints.PluginService.TraceData, 12090710, nameof (GetPluginListAsync)))
      {
        MavenPluginServiceItem pluginServiceItem = await MavenPluginService.FetchPluginServiceItemAsync(requestContext, feed, groupId);
        if (pluginServiceItem?.Plugins == null || !pluginServiceItem.Plugins.Any<MavenPluginItem>())
          return (MavenPluginList) null;
        return new MavenPluginList()
        {
          Plugins = pluginServiceItem.Plugins
        };
      }
    }

    public async Task AppendPluginDataAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      MavenPomMetadata pomMetadata)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<MavenPomMetadata>(pomMetadata, nameof (pomMetadata));
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, MavenTracerPoints.PluginService.TraceData, 12090720, nameof (AppendPluginDataAsync)))
      {
        string groupId = pomMetadata.GroupId;
        MavenPluginServiceItem pluginServiceItem = await MavenPluginService.FetchPluginServiceItemAsync(requestContext, feed, groupId);
        MavenPluginItem mavenPluginItem = MavenPluginService.BuildPluginFromPomMetadata(pomMetadata);
        if (pluginServiceItem == null)
          await MavenPluginService.CreatePluginServiceItemAsync(requestContext, feed, groupId, (IEnumerable<MavenPluginItem>) new List<MavenPluginItem>()
          {
            mavenPluginItem
          });
        else
          await MavenPluginService.UpdatePluginServiceItemAsync(requestContext, feed, groupId, pluginServiceItem, (IEnumerable<MavenPluginItem>) new List<MavenPluginItem>()
          {
            mavenPluginItem
          }, false);
        groupId = (string) null;
      }
    }

    private static Task<MavenPluginServiceItem> FetchPluginServiceItemAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId)
    {
      MavenSecurityHelper.CheckForReadFeedPermission(requestContext, feed);
      MavenItemStore service = requestContext.GetService<MavenItemStore>();
      Locator feedContainerName = PackagingUtils.ComputeFeedContainerName(feed);
      Locator pathLocator = MavenPluginService.GetPathLocator(groupId);
      IVssRequestContext requestContext1 = requestContext;
      Locator containerName = feedContainerName;
      Locator path = pathLocator;
      return service.GetItemAsync<MavenPluginServiceItem>(requestContext1, containerName, path, LatencyPreference.PreferHighThroughput);
    }

    private static async Task CreatePluginServiceItemAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      IEnumerable<MavenPluginItem> plugins)
    {
      MavenSecurityHelper.CheckForReadAndAddPackagePermissions(requestContext, feed);
      MavenItemStore itemStore = requestContext.GetService<MavenItemStore>();
      Locator containerId = PackagingUtils.ComputeFeedContainerName(feed);
      Locator path = MavenPluginService.GetPathLocator(groupId);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await new RetryHelper(requestContext, 10, RetryUtils.GetDefaultMaxRetryDelay(requestContext), MavenPluginService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (MavenPluginService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException))).Invoke((Func<Task>) (async () =>
      {
        if (!await itemStore.CompareSwapItemAsync(requestContext, containerId, path, (StoredItem) new MavenPluginServiceItem(plugins)))
          throw new ChangeConflictException();
      }));
    }

    private static async Task UpdatePluginServiceItemAsync(
      IVssRequestContext requestContext,
      FeedCore feed,
      string groupId,
      MavenPluginServiceItem item,
      IEnumerable<MavenPluginItem> plugins,
      bool overwrite)
    {
      MavenSecurityHelper.CheckForReadAndUpdatePackagePermissions(requestContext, feed);
      MavenItemStore itemStore = requestContext.GetService<MavenItemStore>();
      Locator containerId = PackagingUtils.ComputeFeedContainerName(feed);
      Locator path = MavenPluginService.GetPathLocator(groupId);
      List<MavenPluginItem> list = plugins.ToList<MavenPluginItem>();
      if (overwrite)
      {
        item.Plugins = list;
      }
      else
      {
        MavenPluginServiceItem pluginServiceItem = item;
        List<MavenPluginItem> plugins1 = item.Plugins;
        List<MavenPluginItem> mavenPluginItemList = (plugins1 != null ? plugins1.Union<MavenPluginItem>(plugins).ToList<MavenPluginItem>() : (List<MavenPluginItem>) null) ?? list;
        pluginServiceItem.Plugins = mavenPluginItemList;
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      await new RetryHelper(requestContext, 10, RetryUtils.GetDefaultMaxRetryDelay(requestContext), MavenPluginService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (MavenPluginService.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException))).Invoke((Func<Task>) (async () =>
      {
        if (!await itemStore.CompareSwapItemAsync(requestContext, containerId, path, (StoredItem) item))
          throw new TargetModifiedAfterReadException(nameof (MavenPluginService));
      }));
    }

    private static Locator GetPathLocator(string groupId) => new Locator(new string[3]
    {
      "maven",
      "plugins",
      groupId.Replace('.', '/').ToLowerInvariant()
    });

    private static string GetPrefixFromArtifactId(string artifactId)
    {
      string str1 = "maven";
      string str2 = "plugin";
      return artifactId.StartsWith(str1 + "-", StringComparison.OrdinalIgnoreCase) && artifactId.EndsWith("-" + str2, StringComparison.OrdinalIgnoreCase) ? artifactId.TrimStart(str1 + "-").TrimEnd("-" + str2) : artifactId.TrimEnd("-" + str1 + "-" + str2);
    }

    public static MavenPluginItem BuildPluginFromPomMetadata(MavenPomMetadata pomMetadata)
    {
      string artifactId = pomMetadata.ArtifactId;
      string str1 = string.IsNullOrWhiteSpace(pomMetadata.Name) ? pomMetadata.ArtifactId : pomMetadata.Name;
      string str2 = pomMetadata?.Build?.Plugins?.Find((Predicate<Plugin>) (x => string.Equals(x.ArtifactId, "maven-plugin-plugin", StringComparison.OrdinalIgnoreCase)))?.Configuration?.GoalPrefix;
      if (string.IsNullOrWhiteSpace(str2))
        str2 = MavenPluginService.GetPrefixFromArtifactId(artifactId);
      return new MavenPluginItem()
      {
        Prefix = str2,
        ArtifactId = artifactId,
        Name = string.IsNullOrWhiteSpace(str1) ? artifactId : str1
      };
    }
  }
}
