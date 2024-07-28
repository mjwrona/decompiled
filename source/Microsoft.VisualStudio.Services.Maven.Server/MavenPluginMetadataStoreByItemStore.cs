// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPluginMetadataStoreByItemStore
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Maven.Server.Models.Xml;
using Microsoft.VisualStudio.Services.Maven.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataStores.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPluginMetadataStoreByItemStore : 
    IWritableMavenPluginMetadataStore,
    IMavenPluginMetadataStore
  {
    private readonly IFeedPerms permsFacade;
    private readonly IContentItemstore itemstore;
    private readonly ITracerService tracerService;
    private readonly IRegistryService registryService;
    private const int MaxRetries = 10;

    public MavenPluginMetadataStoreByItemStore(
      IContentItemstore itemstore,
      IFeedPerms permsFacade,
      ITracerService tracerService,
      IRegistryService registryService)
    {
      this.itemstore = itemstore;
      this.permsFacade = permsFacade;
      this.tracerService = tracerService;
      this.registryService = registryService;
    }

    public async Task<MavenPluginList> GetPluginListAsync(FeedCore feed, string groupId)
    {
      MavenPluginMetadataStoreByItemStore sendInTheThisObject = this;
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      ArgumentUtility.CheckForNull<string>(groupId, nameof (groupId));
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetPluginListAsync)))
      {
        MavenPluginServiceItem pluginServiceItem = await sendInTheThisObject.FetchPluginServiceItemAsync(feed, groupId);
        if (pluginServiceItem?.Plugins == null || !pluginServiceItem.Plugins.Any<MavenPluginItem>())
          return (MavenPluginList) null;
        return new MavenPluginList()
        {
          Plugins = pluginServiceItem.Plugins
        };
      }
    }

    public async Task AppendPluginDataAsync(
      FeedCore feed,
      MavenPomMetadata pomMetadata,
      MavenPackageIdentity identity,
      IngestionDirection ingestionDirection)
    {
      MavenPluginMetadataStoreByItemStore sendInTheThisObject = this;
      ArgumentUtility.CheckForNull<FeedCore>(feed, nameof (feed));
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (AppendPluginDataAsync)))
      {
        string groupId = identity.Name.GroupId;
        MavenPluginItem item = MavenPluginMetadataStoreByItemStore.BuildPluginFromPomMetadata(pomMetadata, identity);
        sendInTheThisObject.permsFacade.Validate(feed, PackageIngestionUtils.GetRequiredAddPackagePermission(ingestionDirection));
        Locator containerId = PackagingUtils.ComputeFeedContainerName(feed);
        Locator path = MavenPluginMetadataStoreByItemStore.GetPathLocator(groupId);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        await new RetryHelper(sendInTheThisObject.tracerService, 10, sendInTheThisObject.registryService.GetValue<TimeSpan>((RegistryQuery) "/Configuration/Packaging/RetryHelper/DefaultMaxRetryDelay", TimeSpan.FromSeconds(1.0)), MavenPluginMetadataStoreByItemStore.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException ?? (MavenPluginMetadataStoreByItemStore.\u003C\u003EO.\u003C0\u003E__IsRetryableStorageException = new Func<Exception, bool>(RetryUtils.IsRetryableStorageException))).Invoke((Func<Task>) (async () =>
        {
          MavenPluginServiceItem pluginServiceItem = await this.FetchPluginServiceItemAsync(feed, groupId);
          if (pluginServiceItem == null)
            await this.CreatePluginServiceItemAsync(containerId, path, (IEnumerable<MavenPluginItem>) new List<MavenPluginItem>()
            {
              item
            });
          else
            await this.UpdatePluginServiceItemAsync(containerId, path, pluginServiceItem, (IEnumerable<MavenPluginItem>) new List<MavenPluginItem>()
            {
              item
            }, false);
        }));
      }
    }

    private async Task CreatePluginServiceItemAsync(
      Locator containerId,
      Locator path,
      IEnumerable<MavenPluginItem> plugins)
    {
      if (!await this.itemstore.CompareSwapItemAsync(containerId, path, (StoredItem) new MavenPluginServiceItem(plugins)))
        throw new ChangeConflictException();
    }

    private async Task UpdatePluginServiceItemAsync(
      Locator containerId,
      Locator path,
      MavenPluginServiceItem item,
      IEnumerable<MavenPluginItem> plugins,
      bool overwrite)
    {
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
      if (!await this.itemstore.CompareSwapItemAsync(containerId, path, (StoredItem) item))
        throw new TargetModifiedAfterReadException(nameof (MavenPluginMetadataStoreByItemStore));
    }

    private async Task<MavenPluginServiceItem> FetchPluginServiceItemAsync(
      FeedCore feed,
      string groupId)
    {
      this.permsFacade.Validate(feed, FeedPermissionConstants.ReadPackages);
      return await this.itemstore.GetItemAsync<MavenPluginServiceItem>(PackagingUtils.ComputeFeedContainerName(feed), MavenPluginMetadataStoreByItemStore.GetPathLocator(groupId));
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

    private static MavenPluginItem BuildPluginFromPomMetadata(
      MavenPomMetadata pomMetadata,
      MavenPackageIdentity packageIdentity)
    {
      string artifactId = packageIdentity.Name.ArtifactId;
      string str1 = string.IsNullOrWhiteSpace(pomMetadata.Name) ? pomMetadata.ArtifactId : pomMetadata.Name;
      string str2 = pomMetadata?.Build?.Plugins?.Find((Predicate<Plugin>) (x => string.Equals(x.ArtifactId, "maven-plugin-plugin", StringComparison.OrdinalIgnoreCase)))?.Configuration?.GoalPrefix;
      if (string.IsNullOrWhiteSpace(str2))
        str2 = MavenPluginMetadataStoreByItemStore.GetPrefixFromArtifactId(artifactId);
      return new MavenPluginItem()
      {
        Prefix = str2,
        ArtifactId = artifactId,
        Name = string.IsNullOrWhiteSpace(str1) ? artifactId : str1
      };
    }
  }
}
