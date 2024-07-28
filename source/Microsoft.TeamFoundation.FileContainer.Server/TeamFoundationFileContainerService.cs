// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationFileContainerService
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupStitcher;
using Microsoft.VisualStudio.Services.BlobStore.Server.ContentStitcher;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationFileContainerService : 
    ITeamFoundationFileContainerService,
    IVssFrameworkService
  {
    private IDisposableReadOnlyList<IContainerSecurityExtension> m_securityExtensions;
    private static readonly string s_layer = "Service";
    private static readonly string s_area = "FileContainer";
    private static readonly string s_buildArtifactScope = "buildartifacts";
    private const int c_slowRequestTracepoint = 1008341;
    private const int c_tracePointSpecificActivityStart = 1009000;
    private const int c_tracePointCreateContainer = 1009001;
    private const int c_tracePointDeleteContainer = 1009002;
    private const int c_tracePointDeleteContainers = 1009003;
    private const int c_tracePointFilterContainers = 1009004;
    private const int c_tracePointCreateItems = 1009005;
    private const int c_tracePointQueryItems = 1009006;
    private const int c_tracePointQuerySpecificItems = 1009007;
    private const int c_tracePointDeleteItems = 1009008;
    private const int c_tracePointUploadFile = 1009009;
    private const int c_tracePointCopyFolder = 1009010;
    private const int c_tracePointRenameFolder = 1009011;
    private const int c_tracePointCopyFiles = 1009012;
    private const int c_tracePointRenameFiles = 1009013;
    private const int c_tracePointCleanupBlobReference = 1009014;
    private const int c_tracePointBlobReferencesToDelete = 1009015;
    private const int c_tracePointDeleteBlobReferencesInParallel = 1009016;
    private const int c_tracePointStreamingFromExternalBlobStitcherFailed = 1009017;
    private const int DefaultSlowRequestThreshold = 5000;
    private const int DefaultBlobReferenceSqlRetentionPeriodInDays = 1;
    private const int DefaultBlobReferenceDeletionChunkSize = 1000;
    private const int DefaultBlobReferenceDeletionMaxBatchCount = 3;
    private const int DefaultBlobReferenceMaxParallelRootDeletions = 1;
    private int m_slowRequestThreshold;
    private int m_blobReferenceSqlRetentionPeriodInDays;
    private int m_blobReferenceDeletionChunkSize;
    private int m_blobReferenceDeletionMaxBatchCount;
    private int m_blobReferenceMaxParallelRootDeletions;
    private int m_blobStitchingBoundedCapacity;
    private int m_blobStitchingParallelism;
    private IDomainId m_defaultUploadDomain = WellKnownDomainIds.DefaultDomainId;
    private static readonly HttpClient chunkHttpClient = new HttpClient()
    {
      Timeout = TimeSpan.FromSeconds(5.0)
    };
    private static readonly Guid ContainerDeletedNotificationId = new Guid("3A52445B-BA39-49BB-B8F0-57187ECD4C8D");
    private static readonly Guid ContainersDeletedNotificationId = new Guid("218BEC82-FEFB-4297-AC31-453041BAD924");
    private XmlSerializer m_longArraySerializer = new XmlSerializer(typeof (long[]));
    private static byte[] m_zeroByteFileId;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService notificationService = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) || systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Application) || systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? systemRequestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      notificationService.RegisterNotification(systemRequestContext, "Default", TeamFoundationFileContainerService.ContainerDeletedNotificationId, new SqlNotificationCallback(this.OnContainerDeleted), true);
      notificationService.RegisterNotification(systemRequestContext, "Default", TeamFoundationFileContainerService.ContainersDeletedNotificationId, new SqlNotificationCallback(this.OnContainersDeleted), true);
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), (RegistryQuery) (FrameworkServerConstants.FileContainerServiceRegistryRootPath + "/**"));
      this.LoadRegistrySettings(systemRequestContext);
      this.m_securityExtensions = systemRequestContext.GetExtensions<IContainerSecurityExtension>();
      TeamFoundationFileContainerService.m_zeroByteFileId = Convert.FromBase64String("HlfPJ5KpANBsHN+zxFPzW8hvcniKqXJMlskp0cxrRWo=");
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", TeamFoundationFileContainerService.ContainerDeletedNotificationId, new SqlNotificationCallback(this.OnContainerDeleted), false);
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      if (this.m_securityExtensions == null)
        return;
      this.m_securityExtensions.Dispose();
      this.m_securityExtensions = (IDisposableReadOnlyList<IContainerSecurityExtension>) null;
    }

    public long CreateContainer(
      IVssRequestContext requestContext,
      Uri artifactUri,
      string securityToken,
      string name,
      string description,
      Guid scopeIdentifier,
      ContainerOptions options = ContainerOptions.None)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateContainer), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008101, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateContainer));
        ArgumentUtility.CheckForNull<Uri>(artifactUri, nameof (artifactUri));
        ArgumentUtility.CheckStringForNullOrEmpty(securityToken, nameof (securityToken));
        ArgumentUtility.CheckForNull<string>(name, nameof (name));
        bool flag = false;
        foreach (IContainerSecurityExtension securityExtension in (IEnumerable<IContainerSecurityExtension>) this.m_securityExtensions)
        {
          try
          {
            if (securityExtension.CanEvaluate(artifactUri))
            {
              flag = true;
              break;
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1008103, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
          }
        }
        if (!flag)
          throw new ArtifactUriNotSupportedException(artifactUri);
        long container = -1;
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          container = component.AddContainer(artifactUri, securityToken, name, description, options, scopeIdentifier, (string) null);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009001, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, string.Format("Container name: {0}, Container Id: {1}", (object) name, (object) container));
        requestContext.TraceLeave(1008102, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateContainer));
        return container;
      }
    }

    public void DeleteContainer(
      IVssRequestContext requestContext,
      long containerId,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainer), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008111, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainer));
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          component.DeleteContainer(containerId, new Guid?(scopeIdentifier));
        requestContext.GetService<FileContainerCacheService>().Remove(requestContext, containerId);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009002, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, string.Format("Deleting container {0}", (object) containerId));
        requestContext.TraceLeave(1008112, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainer));
      }
    }

    public void DeleteContainers(
      IVssRequestContext requestContext,
      IList<long> containerIds,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainers), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008121, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainers));
        try
        {
          for (int index = 0; index < containerIds.Count; ++index)
            ArgumentUtility.CheckForOutOfRange(containerIds[index], string.Format((IFormatProvider) CultureInfo.InvariantCulture, "containerId {0}", (object) containerIds[index]), 1L);
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            component.DeleteContainers(containerIds, scopeIdentifier);
        }
        finally
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
            requestContext.TraceAlways(1009003, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Deleting containers: " + string.Join<long>(",", (IEnumerable<long>) containerIds));
          requestContext.TraceLeave(1008122, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteContainers));
        }
      }
    }

    public Microsoft.VisualStudio.Services.FileContainer.FileContainer GetContainer(
      IVssRequestContext requestContext,
      long containerId,
      Guid scopeIdentifier,
      bool returnSize = true)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.GetContainer(requestContext, containerId, scopeIdentifier1, returnSize);
    }

    internal Microsoft.VisualStudio.Services.FileContainer.FileContainer GetContainer(
      IVssRequestContext requestContext,
      long containerId,
      bool returnSize = true)
    {
      return this.GetContainer(requestContext, containerId, new Guid?(), returnSize);
    }

    private Microsoft.VisualStudio.Services.FileContainer.FileContainer GetContainer(
      IVssRequestContext requestContext,
      long containerId,
      Guid? scopeIdentifier,
      bool returnSize = true,
      bool throwIfNotFound = false)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (GetContainer), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008161, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (GetContainer));
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        Microsoft.VisualStudio.Services.FileContainer.FileContainer container = (Microsoft.VisualStudio.Services.FileContainer.FileContainer) null;
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          container = component.GetContainer(containerId, scopeIdentifier, returnSize);
        if (container != null && !this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, container)))
          container = (Microsoft.VisualStudio.Services.FileContainer.FileContainer) null;
        if (throwIfNotFound && container == null)
          throw new ContainerNotFoundException(containerId);
        requestContext.TraceLeave(1008162, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (GetContainer));
        return container;
      }
    }

    internal List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      IList<Uri> artifactUris)
    {
      return this.QueryContainers(requestContext, artifactUris, new Guid?());
    }

    public List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      Guid scopeIdentifier)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.QueryContainers(requestContext, (IList<Uri>) null, scopeIdentifier1);
    }

    public List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      IList<Uri> artifactUris,
      Guid scopeIdentifier)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.QueryContainers(requestContext, artifactUris, scopeIdentifier1);
    }

    private List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> QueryContainers(
      IVssRequestContext requestContext,
      IList<Uri> artifactUris,
      Guid? scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryContainers), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008171, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryContainers));
        List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> containers = new List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
        if (artifactUris == null || artifactUris.Count > 0)
        {
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            containers = component.QueryContainers(artifactUris ?? (IList<Uri>) new List<Uri>(), scopeIdentifier);
        }
        this.CheckContainerPermissions(requestContext, containers);
        requestContext.TraceLeave(1008172, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryContainers));
        return containers;
      }
    }

    public List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> FilterContainers(
      IVssRequestContext requestContext,
      string artifactUriFilter,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (FilterContainers), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008181, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (FilterContainers));
        List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> containers = new List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>();
        if (!string.IsNullOrEmpty(artifactUriFilter))
        {
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            containers = component.FilterContainers(artifactUriFilter, new Guid?(scopeIdentifier));
        }
        this.CheckContainerPermissions(requestContext, containers);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009004, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Querying with filter " + artifactUriFilter);
        requestContext.TraceLeave(1008182, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (FilterContainers));
        return containers;
      }
    }

    private void CheckContainerPermissions(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.FileContainer.FileContainer> containers)
    {
      containers.RemoveAll((Predicate<Microsoft.VisualStudio.Services.FileContainer.FileContainer>) (item => !this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, item))));
    }

    internal List<FileContainerItem> CreateItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<FileContainerItem> items)
    {
      return this.CreateItems(requestContext, containerId, items, new Guid?());
    }

    public List<FileContainerItem> CreateItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<FileContainerItem> items,
      Guid scopeIdentifier)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.CreateItems(requestContext, containerId, items, scopeIdentifier1);
    }

    public List<FileContainerItem> CreateItemFromArtifact(
      IVssRequestContext requestContext,
      long containerId,
      FileContainerItem item,
      string artifactHash,
      Guid? scopeIdentifier = null)
    {
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        throw new ArgumentException(FrameworkResources.UploadBuildArtifactsToBlobNotEnabled());
      IVssRequestContext requestContext1 = requestContext;
      long containerId1 = containerId;
      List<FileContainerItem> items = new List<FileContainerItem>();
      items.Add(item);
      Guid? scopeIdentifier1 = scopeIdentifier;
      this.ValidateItemsAndWritePermissions(requestContext1, containerId1, (IList<FileContainerItem>) items, scopeIdentifier1);
      (IDomainId domainId, DedupIdentifier dedupId) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(artifactHash);
      item.ArtifactId = new long?(this.AddBlobReference(requestContext, domainId, dedupId, BlobCompressionType.None, scopeIdentifier).ArtifactId);
      return this.CreateItems(requestContext, containerId, (IList<FileContainerItem>) new FileContainerItem[1]
      {
        item
      }, scopeIdentifier);
    }

    private List<FileContainerItem> CreateItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<FileContainerItem> items,
      Guid? scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateItems), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008191, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateItems));
        Microsoft.VisualStudio.Services.FileContainer.FileContainer container = (Microsoft.VisualStudio.Services.FileContainer.FileContainer) null;
        try
        {
          this.ValidateItemsAndWritePermissions(requestContext, containerId, items, scopeIdentifier);
          container = this.GetContainer(requestContext, containerId, scopeIdentifier, false, true);
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          {
            using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "CreateItems - CreateItemsSql", Array.Empty<object>()))
              return component.CreateItems(containerId, items, scopeIdentifier);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1008193, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
          throw;
        }
        finally
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
            requestContext.TraceAlways(1009005, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, JsonConvert.SerializeObject((object) new CreateItemsTraceData(container, items)));
          requestContext.TraceLeave(1008192, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CreateItems));
        }
      }
    }

    private void ValidateItemsAndWritePermissions(
      IVssRequestContext requestContext,
      long containerId,
      IList<FileContainerItem> items,
      Guid? scopeIdentifier)
    {
      ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
      ArgumentUtility.CheckForNull<IList<FileContainerItem>>(items, nameof (items));
      foreach (FileContainerItem fileContainerItem in (IEnumerable<FileContainerItem>) items)
      {
        ArgumentUtility.CheckForNull<FileContainerItem>(fileContainerItem, "item");
        fileContainerItem.Validate(requestContext);
        if (fileContainerItem.ContentId != null && fileContainerItem.FileLength == 0L && !ArrayUtil.Equals(fileContainerItem.ContentId, TeamFoundationFileContainerService.m_zeroByteFileId))
          throw new ContainerItemContentException(FrameworkResources.FileContainerUploadContentIdDoesNotMatch());
      }
      this.CheckWritePermission(requestContext, scopeIdentifier, containerId);
    }

    internal List<FileContainerItem> QueryItems(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      bool isShallow = false,
      bool includeBlobMetadata = false)
    {
      return this.QueryItems(requestContext, containerId, path, new Guid?(), isShallow, includeBlobMetadata);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<FileContainerItem> QueryItems(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Guid scopeIdentifier,
      bool isShallow = false,
      bool includeBlobMetadata = false)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.QueryItems(requestContext, containerId, path, scopeIdentifier1, isShallow, includeBlobMetadata);
    }

    private List<FileContainerItem> QueryItems(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Guid? scopeIdentifier,
      bool isShallow,
      bool includeBlobMetadata = false)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryItems), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008201, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryItems));
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        List<FileContainerItem> source;
        if (!this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, containerId, scopeIdentifier)))
        {
          source = new List<FileContainerItem>();
        }
        else
        {
          path = FileContainerItem.EnsurePathFormat(path);
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            source = component.QueryItems(containerId, path, scopeIdentifier, isShallow);
        }
        if (includeBlobMetadata)
        {
          List<FileContainerItem> list = source.Where<FileContainerItem>((Func<FileContainerItem, bool>) (x => x.ArtifactId.HasValue)).ToList<FileContainerItem>();
          Dictionary<long, ContainerItemBlobReference> dictionary;
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            dictionary = component.GetBlobReferences(list.Select<FileContainerItem, long>((Func<FileContainerItem, long>) (x => x.ArtifactId.Value)).Distinct<long>(), scopeIdentifier).ToDictionary<ContainerItemBlobReference, long, ContainerItemBlobReference>((Func<ContainerItemBlobReference, long>) (k => k.ArtifactId), (Func<ContainerItemBlobReference, ContainerItemBlobReference>) (v => v));
          foreach (FileContainerItem fileContainerItem in list)
            fileContainerItem.BlobMetadata = dictionary.GetValueOrDefault<long, ContainerItemBlobReference>(fileContainerItem.ArtifactId.Value, (ContainerItemBlobReference) null);
        }
        requestContext.TraceLeave(1008202, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QueryItems));
        return source;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public List<FileContainerItem> QuerySpecificItems(
      IVssRequestContext requestContext,
      long containerId,
      IEnumerable<string> paths,
      Guid scopeIdentifier)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.QuerySpecificItems(requestContext, containerId, paths, scopeIdentifier1);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal List<FileContainerItem> QuerySpecificItems(
      IVssRequestContext requestContext,
      long containerId,
      IEnumerable<string> paths,
      Guid? scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QuerySpecificItems), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008211, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QuerySpecificItems));
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        List<FileContainerItem> fileContainerItemList;
        if (!this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, containerId, scopeIdentifier)))
        {
          fileContainerItemList = new List<FileContainerItem>();
        }
        else
        {
          paths = paths.Select<string, string>((Func<string, string>) (path => FileContainerItem.EnsurePathFormat(path)));
          using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
            fileContainerItemList = component.QuerySpecificItems(containerId, paths, scopeIdentifier);
        }
        requestContext.TraceLeave(1008212, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (QuerySpecificItems));
        return fileContainerItemList;
      }
    }

    internal void DeleteItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<string> paths)
    {
      this.DeleteItems(requestContext, containerId, paths, new Guid?());
    }

    public void DeleteItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<string> paths,
      Guid scopeIdentifier)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      this.DeleteItems(requestContext, containerId, paths, scopeIdentifier1);
    }

    private void DeleteItems(
      IVssRequestContext requestContext,
      long containerId,
      IList<string> paths,
      Guid? scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteItems), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008221, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteItems));
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        ArgumentUtility.CheckForNull<IList<string>>(paths, "items");
        this.CheckWritePermission(requestContext, scopeIdentifier, containerId);
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          component.DeleteItems(containerId, paths, scopeIdentifier);
        requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new FileContainerItemDeletedEvent(containerId, paths));
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009008, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, string.Format("Deleting {0} items from container id {1}.", (object) paths.Count<string>(), (object) containerId));
        requestContext.TraceLeave(1008222, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (DeleteItems));
      }
    }

    public Stream RetrieveFile(
      IVssRequestContext requestContext,
      long containerId,
      Guid? scopeIdentifier,
      FileContainerItem item,
      bool allowCompression,
      out CompressionType compressionType)
    {
      if (!this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, containerId, scopeIdentifier)))
      {
        compressionType = CompressionType.None;
        return (Stream) null;
      }
      if (item.ArtifactId.HasValue)
      {
        ContainerItemBlobReference blobReference = this.GetBlobReference(requestContext, item.ArtifactId.Value, new Guid?(item.ScopeIdentifier));
        compressionType = blobReference.CompressionType == BlobCompressionType.GZip ? CompressionType.GZip : CompressionType.None;
        bool requestCompression = allowCompression && blobReference.CompressionType == BlobCompressionType.GZip && requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.GZipBlobStreamsForDownload");
        (IDomainId domainId, DedupIdentifier dedupId) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(blobReference.ArtifactHash);
        return requestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => this.GetStreamFromBlobStoreAsync(requestContext, requestCompression, domainId, dedupId)));
      }
      if (item.FileId != 0)
        return requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) item.FileId, allowCompression, out byte[] _, out long _, out compressionType);
      compressionType = CompressionType.None;
      return (Stream) null;
    }

    internal static (IDomainId domainId, DedupIdentifier dedupId) GetDomainIdAndDedupIdFromArtifactHash(
      string artifactHash)
    {
      string[] strArray = artifactHash.Split(',');
      if (strArray.Length == 1)
        return (WellKnownDomainIds.DefaultDomainId, DedupIdentifier.Deserialize(strArray[0]));
      return strArray.Length == 2 ? (DomainIdFactory.Create(strArray[0]), DedupIdentifier.Deserialize(strArray[1])) : throw new ArgumentException("Invalid artifact hash: " + artifactHash, nameof (artifactHash));
    }

    internal async Task<Stream> GetStreamFromBlobStoreAsync(
      IVssRequestContext requestContext,
      bool requestCompression,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      try
      {
        if (requestContext.IsFeatureEnabled("FileContainer.RedirectToContentStitcher"))
          return await TeamFoundationFileContainerService.GetTransitiveContentFromContentStitcherAsync(requestContext, domainId, dedupId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1009017, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
      }
      return requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.NewBlobStitcherLibrary") ? await new DedupContentStreamFactory((IDedupContentProvider) new DedupContentDownloader(requestContext, TeamFoundationFileContainerService.chunkHttpClient), this.m_blobStitchingBoundedCapacity, this.m_blobStitchingParallelism).GetStreamAsync(domainId, dedupId, requestContext.CancellationToken) : await BlobStoreUtils.GetStreamAsync(requestContext, domainId, dedupId);
    }

    internal static async Task<Uri> GetContentStitcherRedirectAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      string redirectFileName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      try
      {
        HttpResponseMessage signedUriAsync = await vssRequestContext.GetService<IContentStitcherService>().GetSignedUriAsync(domainId, dedupId, vssRequestContext, redirectFileName);
        if (signedUriAsync != null && signedUriAsync.IsSuccessStatusCode)
          return new Uri(signedUriAsync.Content.ReadAsStringAsync().Result.Replace("\"", ""));
        throw new InvalidOperationException(string.Format("Get file stream error: {0}\n Error message: {1}", (object) signedUriAsync.StatusCode, (object) signedUriAsync.ReasonPhrase));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1009090, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
        return (Uri) null;
      }
    }

    private static async Task<Stream> GetTransitiveContentFromContentStitcherAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      HttpResponseMessage stitchedFileAsync = await vssRequestContext.GetService<IContentStitcherService>().GetStitchedFileAsync(domainId, dedupId, vssRequestContext);
      if (stitchedFileAsync != null)
      {
        if (stitchedFileAsync.IsSuccessStatusCode && stitchedFileAsync.Content != null)
          return await stitchedFileAsync.Content.ReadAsStreamAsync();
        if (stitchedFileAsync.StatusCode == HttpStatusCode.NotFound)
          throw new DedupNotFoundException(string.Format("Dedup id {0} not found in domain {1}.", (object) dedupId, (object) domainId));
      }
      throw new InvalidOperationException(string.Format("Get file stream error: {0}\n Error message: {1}", (object) stitchedFileAsync.StatusCode, (object) stitchedFileAsync.ReasonPhrase));
    }

    internal FileContainerItem UploadFile(
      IVssRequestContext requestContext,
      long containerId,
      FileContainerItem item,
      Stream fileStream,
      long offsetFrom,
      long compressedLength,
      CompressionType compressionType,
      byte[] clientContentId = null)
    {
      return this.UploadFile(requestContext, containerId, item, fileStream, offsetFrom, compressedLength, compressionType, new Guid?(), clientContentId);
    }

    public FileContainerItem UploadFile(
      IVssRequestContext requestContext,
      long containerId,
      FileContainerItem item,
      Stream fileStream,
      long offsetFrom,
      long compressedLength,
      CompressionType compressionType,
      Guid scopeIdentifier,
      byte[] clientContentId = null)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      return this.UploadFile(requestContext, containerId, item, fileStream, offsetFrom, compressedLength, compressionType, scopeIdentifier1, clientContentId);
    }

    private FileContainerItem UploadFile(
      IVssRequestContext requestContext,
      long containerId,
      FileContainerItem item,
      Stream fileStream,
      long offsetFrom,
      long compressedLength,
      CompressionType compressionType,
      Guid? scopeIdentifier,
      byte[] clientContentId = null)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (UploadFile), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008231, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (UploadFile));
        bool exceptionThrown = false;
        try
        {
          VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalUploadChunks").Increment();
          ArgumentUtility.CheckForOutOfRange(offsetFrom, nameof (offsetFrom), 0L);
          ArgumentUtility.CheckForOutOfRange(compressedLength, nameof (compressedLength), 0L);
          ArgumentUtility.CheckForNull<Stream>(fileStream, nameof (fileStream));
          ArgumentUtility.CheckForNull<FileContainerItem>(item, nameof (item));
          long num = -1;
          if (fileStream.CanSeek)
            num = fileStream.Length;
          this.CheckWritePermission(requestContext, scopeIdentifier, containerId);
          FileContainerItem fileContainerItem;
          using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "UploadFile-QuerySpecificItems", Array.Empty<object>()))
          {
            IVssRequestContext requestContext1 = requestContext;
            long containerId1 = containerId;
            List<string> paths = new List<string>();
            paths.Add(item.Path);
            Guid? scopeIdentifier1 = scopeIdentifier;
            fileContainerItem = this.QuerySpecificItems(requestContext1, containerId1, (IEnumerable<string>) paths, scopeIdentifier1).FirstOrDefault<FileContainerItem>();
          }
          if (fileContainerItem == null)
            throw new ContainerItemContentException(FrameworkResources.FileContainerUploadItemDoesNotExist());
          if (fileContainerItem.Status == ContainerItemStatus.Created && !requestContext.IsServicingContext)
            throw new ContainerItemContentException(FrameworkResources.FileContainerContentAlreadyComplete());
          byte[] contentId = (byte[]) null;
          bool flag1 = false;
          int fileId = fileContainerItem.FileId;
          long? artifactId1 = fileContainerItem.ArtifactId;
          bool flag2 = offsetFrom == 0L && num == compressedLength;
          ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
          if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableFileContainerUploadToBlobStore"))
          {
            using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "UploadFile-BlobStore", Array.Empty<object>()))
              flag1 = this.UploadStreamToBlobStore(requestContext, ref artifactId1, fileStream, compressedLength, item.FileLength, offsetFrom, compressionType, fileContainerItem.ScopeIdentifier);
          }
          else
          {
            using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "UploadFile-FileService", Array.Empty<object>()))
              flag1 = service.UploadFile(requestContext, ref fileId, fileStream, fileContainerItem.FileHash, compressedLength, item.FileLength, offsetFrom, compressionType, OwnerId.FileContainer, fileContainerItem.ScopeIdentifier, (string) null);
          }
          VssPerformanceCounter performanceCounter;
          if (num > 0L)
          {
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalBytesUploaded");
            performanceCounter.IncrementBy(num);
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_BytesUploadedSec");
            performanceCounter.IncrementBy(num);
          }
          if (flag1)
          {
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalUploads");
            performanceCounter.Increment();
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_UploadSec");
            performanceCounter.Increment();
            this.UpdateItemStatus(requestContext, containerId, fileContainerItem.Path, fileId, ContainerItemStatus.Created, scopeIdentifier, item.FileLength, contentId, artifactId1);
            fileContainerItem.Status = ContainerItemStatus.Created;
            requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new FileContainerItemUploadedEvent(fileContainerItem));
          }
          else
          {
            if (fileId == fileContainerItem.FileId)
            {
              long? nullable = artifactId1;
              long? artifactId2 = fileContainerItem.ArtifactId;
              if (nullable.GetValueOrDefault() == artifactId2.GetValueOrDefault() & nullable.HasValue == artifactId2.HasValue)
                goto label_31;
            }
            this.UpdateItemStatus(requestContext, containerId, fileContainerItem.Path, fileId, ContainerItemStatus.PendingUpload, scopeIdentifier, contentId: contentId, artifactId: artifactId1);
            fileContainerItem.Status = ContainerItemStatus.PendingUpload;
          }
label_31:
          if (flag2 && !flag1)
            requestContext.Trace(1008230, TraceLevel.Error, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Failed to fully upload container item '{0}'.", (object) fileContainerItem.Path);
          fileContainerItem.FileId = fileId;
          fileContainerItem.ContentId = contentId;
          fileContainerItem.ArtifactId = artifactId1;
          return fileContainerItem;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1008233, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
          exceptionThrown = true;
          throw;
        }
        finally
        {
          if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          {
            Microsoft.VisualStudio.Services.FileContainer.FileContainer container = this.GetContainer(requestContext, containerId, scopeIdentifier, false);
            requestContext.TraceAlways(1009009, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, JsonConvert.SerializeObject((object) new FileContainerItemTraceData(item, container, exceptionThrown, offsetFrom)));
          }
          requestContext.TraceLeave(1008232, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (UploadFile));
        }
      }
    }

    private bool UploadStreamToBlobStore(
      IVssRequestContext requestContext,
      ref long? artifactId,
      Stream fileStream,
      long compressedLength,
      long fileLength,
      long offsetFrom,
      CompressionType compressionType,
      Guid scopeIdentifier)
    {
      long num = !fileStream.CanSeek ? compressedLength : fileStream.Length;
      bool isComplete = offsetFrom + num >= compressedLength;
      bool isFirstPart = offsetFrom == 0L;
      if (isFirstPart & isComplete)
      {
        artifactId = new long?(this.UploadFullStreamToBlobStore(requestContext, fileStream, compressionType, scopeIdentifier));
        return true;
      }
      long offsetTo = offsetFrom + (num - 1L);
      artifactId = new long?(this.UploadMultiPartStreamToBlobStore(requestContext, artifactId, fileStream, compressionType, compressedLength, fileLength, offsetFrom, offsetTo, scopeIdentifier, isFirstPart, isComplete));
      return isComplete;
    }

    private long UploadFullStreamToBlobStore(
      IVssRequestContext requestContext,
      Stream fileStream,
      CompressionType compressionType,
      Guid scopeIdentifier)
    {
      DedupBlobMultipartHttpClient dedupBlobMultipartHttpClient = requestContext.Elevate().GetClient<DedupBlobMultipartHttpClient>();
      string str = requestContext.RunSynchronously<string>((Func<Task<string>>) (() => dedupBlobMultipartHttpClient.PostFileStreamAsync(this.m_defaultUploadDomain, fileStream, requestContext.CancellationToken)));
      DedupIdentifier dedupId = !string.IsNullOrEmpty(str) ? DedupIdentifier.Deserialize(str.Substring(str.LastIndexOf("/") + 1)) : throw new Exception("Failed to get artifact URI from blobstore.");
      return this.AddBlobReference(requestContext, this.m_defaultUploadDomain, dedupId, compressionType == CompressionType.GZip ? BlobCompressionType.GZip : BlobCompressionType.None, new Guid?(scopeIdentifier)).ArtifactId;
    }

    private long UploadMultiPartStreamToBlobStore(
      IVssRequestContext requestContext,
      long? artifactId,
      Stream fileStream,
      CompressionType compressionType,
      long compressedLength,
      long uncompressedLength,
      long offsetFrom,
      long offsetTo,
      Guid scopeIdentifier,
      bool isFirstPart,
      bool isComplete)
    {
      DedupBlobMultipartHttpClient dedupBlobMultipartHttpClient = requestContext.Elevate().GetClient<DedupBlobMultipartHttpClient>();
      Guid? sessionId = new Guid?();
      BlobCompressionType compressionType1 = compressionType == CompressionType.GZip ? BlobCompressionType.GZip : BlobCompressionType.None;
      ContainerItemBlobReference itemBlobReference;
      if (isFirstPart)
      {
        sessionId = new Guid?(requestContext.RunSynchronously<Guid>((Func<Task<Guid>>) (() => dedupBlobMultipartHttpClient.CreateSessionAsync(this.m_defaultUploadDomain, requestContext.CancellationToken))));
        itemBlobReference = this.AddPendingBlobReference(requestContext, sessionId.Value, compressionType1, new Guid?(scopeIdentifier));
      }
      else
      {
        itemBlobReference = this.GetBlobReference(requestContext, artifactId.Value, new Guid?(scopeIdentifier));
        sessionId = itemBlobReference?.SessionId;
      }
      if (!sessionId.HasValue)
        throw new Exception("No SessionId found for existing multi-part upload.");
      requestContext.RunSynchronously((Func<Task>) (() => dedupBlobMultipartHttpClient.UploadPartsAsync(this.m_defaultUploadDomain, sessionId.Value, fileStream, offsetFrom, offsetTo, compressedLength, requestContext.CancellationToken)));
      if (isComplete)
      {
        string artifactHash = requestContext.RunSynchronously<string>((Func<Task<string>>) (() => dedupBlobMultipartHttpClient.CompleteSessionAsync(this.m_defaultUploadDomain, sessionId.Value, requestContext.CancellationToken)));
        this.CompletePendingBlobReference(requestContext, itemBlobReference.ArtifactId, artifactHash, new Guid?(scopeIdentifier));
      }
      return itemBlobReference.ArtifactId;
    }

    private void UpdateItemStatus(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      int fileId,
      ContainerItemStatus status,
      Guid? scopeIdentifier,
      long fileLength = -1,
      byte[] contentId = null,
      long? artifactId = null)
    {
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        component.UpdateItemStatus(containerId, path, fileId, status, scopeIdentifier, fileLength, contentId, artifactId);
    }

    public List<FileContainerItem> CopyFolder(
      IVssRequestContext requestContext,
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFolder), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008241, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFolder));
        List<FileContainerItem> fileContainerItemList = this.CopyFolder(requestContext, containerId, folderSourcePath, folderTargetPath, false, scopeIdentifier);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009010, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Copying from " + folderSourcePath + " to " + folderTargetPath + ".");
        requestContext.TraceLeave(1008242, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFolder));
        return fileContainerItemList;
      }
    }

    public List<FileContainerItem> RenameFolder(
      IVssRequestContext requestContext,
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (RenameFolder), Array.Empty<object>()))
      {
        requestContext.TraceEnter(251, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (RenameFolder));
        List<FileContainerItem> fileContainerItemList = this.CopyFolder(requestContext, containerId, folderSourcePath, folderTargetPath, true, scopeIdentifier);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009011, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Renamed folder from " + folderSourcePath + " to " + folderTargetPath + ".");
        requestContext.TraceLeave(1008252, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (RenameFolder));
        return fileContainerItemList;
      }
    }

    public List<FileContainerItem> CopyFiles(
      IVssRequestContext requestContext,
      long containerId,
      IList<Tuple<string, string>> sourcesAndTargets,
      Guid scopeIdentifier,
      bool ignoreMissingSources = false,
      bool overwriteTargets = false)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFiles), Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008261, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFiles));
        List<FileContainerItem> fileContainerItemList = this.CopyFiles(requestContext, containerId, sourcesAndTargets, false, ignoreMissingSources, overwriteTargets, scopeIdentifier);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009012, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, string.Format("Copying {0} files for container id {1}.", (object) sourcesAndTargets.Count<Tuple<string, string>>(), (object) containerId));
        requestContext.TraceLeave(1008262, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFiles));
        return fileContainerItemList;
      }
    }

    public List<FileContainerItem> RenameFiles(
      IVssRequestContext requestContext,
      long containerId,
      IList<Tuple<string, string>> sourcesAndTargets,
      Guid scopeIdentifier,
      bool ignoreMissingSources = false,
      bool overwriteTargets = false)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "RenamesFiles", Array.Empty<object>()))
      {
        requestContext.TraceEnter(1008271, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "RenamesFiles");
        List<FileContainerItem> fileContainerItemList = this.CopyFiles(requestContext, containerId, sourcesAndTargets, true, ignoreMissingSources, overwriteTargets, scopeIdentifier);
        if (requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.EnableLoggingFileContainerUsage"))
          requestContext.TraceAlways(1009013, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, string.Format("Renaming {0} files for container id {1}.", (object) sourcesAndTargets.Count<Tuple<string, string>>(), (object) containerId));
        requestContext.TraceLeave(1008272, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "RenamesFiles");
        return fileContainerItemList;
      }
    }

    private List<FileContainerItem> CopyFolder(
      IVssRequestContext requestContext,
      long containerId,
      string folderSourcePath,
      string folderTargetPath,
      bool deleteSourceFolder,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFolder), Array.Empty<object>()))
      {
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        folderSourcePath = FileContainerItem.EnsurePathFormat(folderSourcePath);
        folderTargetPath = FileContainerItem.EnsurePathFormat(folderTargetPath);
        ArgumentUtility.CheckStringForNullOrEmpty(folderSourcePath, nameof (folderSourcePath));
        ArgumentUtility.CheckStringForNullOrEmpty(folderTargetPath, nameof (folderTargetPath));
        this.CheckWritePermission(requestContext, new Guid?(scopeIdentifier), containerId);
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          return component.CopyFolder(containerId, folderSourcePath, folderTargetPath, deleteSourceFolder, scopeIdentifier);
      }
    }

    private List<FileContainerItem> CopyFiles(
      IVssRequestContext requestContext,
      long containerId,
      IList<Tuple<string, string>> sourcesAndTargets,
      bool deleteSources,
      bool ignoreMissingSources,
      bool overwriteTargets,
      Guid scopeIdentifier)
    {
      using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (CopyFiles), Array.Empty<object>()))
      {
        ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
        ArgumentUtility.CheckForNull<IList<Tuple<string, string>>>(sourcesAndTargets, nameof (sourcesAndTargets));
        List<KeyValuePair<string, string>> sourcesAndTargets1 = new List<KeyValuePair<string, string>>();
        foreach (Tuple<string, string> sourcesAndTarget in (IEnumerable<Tuple<string, string>>) sourcesAndTargets)
        {
          string str = FileContainerItem.EnsurePathFormat(sourcesAndTarget.Item1);
          string stringVar = FileContainerItem.EnsurePathFormat(sourcesAndTarget.Item2);
          ArgumentUtility.CheckStringForNullOrEmpty(str, "source");
          ArgumentUtility.CheckStringForNullOrEmpty(stringVar, "target");
          sourcesAndTargets1.Add(new KeyValuePair<string, string>(str, stringVar));
        }
        this.CheckWritePermission(requestContext, new Guid?(scopeIdentifier), containerId);
        using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
          return component.CopyFiles(containerId, sourcesAndTargets1, deleteSources, ignoreMissingSources, overwriteTargets, scopeIdentifier);
      }
    }

    public ContainerItemBlobReference GetBlobReference(
      IVssRequestContext requestContext,
      long artifactId,
      Guid? scopeIdentifier)
    {
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        return component.GetBlobReference(artifactId, scopeIdentifier);
    }

    public string CreateDomainHashForUpload(string artifactHash)
    {
      (IDomainId domainId, DedupIdentifier dedupId) fromArtifactHash = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(artifactHash);
      return TeamFoundationFileContainerService.CreateDomainHash(fromArtifactHash.domainId, fromArtifactHash.dedupId);
    }

    public static string CreateDomainHash(IDomainId domainId, DedupIdentifier dedupId) => domainId != WellKnownDomainIds.DefaultDomainId ? domainId.Serialize() + "," + dedupId.ValueString : dedupId.ValueString;

    private ContainerItemBlobReference AddBlobReference(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DedupIdentifier dedupId,
      BlobCompressionType compressionType,
      Guid? scopeIdentifier)
    {
      ContainerItemBlobReference blobRef;
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        blobRef = component.AddBlobReference(TeamFoundationFileContainerService.CreateDomainHash(domainId, dedupId), compressionType, scopeIdentifier);
      IVssRequestContext elevatedContext = requestContext.Elevate();
      IDedupStore dedupStoreService = elevatedContext.GetService<IDedupStore>();
      IdBlobReference dedupRef = this.GetDedupReference(scopeIdentifier, blobRef);
      requestContext.RunSynchronously((Func<Task>) (() => dedupStoreService.PutRootAsync(elevatedContext, domainId, dedupId, dedupRef)));
      return blobRef;
    }

    private ContainerItemBlobReference AddPendingBlobReference(
      IVssRequestContext requestContext,
      Guid sessionId,
      BlobCompressionType compressionType,
      Guid? scopeIdentifier)
    {
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        return component.AddPendingBlobReference(sessionId, compressionType, scopeIdentifier);
    }

    private ContainerItemBlobReference CompletePendingBlobReference(
      IVssRequestContext requestContext,
      long artifactId,
      string artifactHash,
      Guid? scopeIdentifier)
    {
      (IDomainId domainId, DedupIdentifier dedupId) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(artifactHash);
      ContainerItemBlobReference blobRef;
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        blobRef = component.CompletePendingBlobReference(artifactId, TeamFoundationFileContainerService.CreateDomainHash(domainId, dedupId), scopeIdentifier);
      IVssRequestContext elevatedContext = requestContext.Elevate();
      IDedupStore dedupStoreService = elevatedContext.GetService<IDedupStore>();
      IdBlobReference dedupRef = this.GetDedupReference(scopeIdentifier, blobRef);
      requestContext.RunSynchronously((Func<Task>) (() => dedupStoreService.PutRootAsync(elevatedContext, domainId, dedupId, dedupRef)));
      return blobRef;
    }

    public (int numberOfFilesDeleted, int numberOfFailures, bool batchLimitReached) CleanupBlobReferences(
      IVssRequestContext requestContext)
    {
      int maxNumberOfFiles = this.m_blobReferenceDeletionChunkSize * this.m_blobReferenceDeletionMaxBatchCount;
      DatabaseConnectionType databaseConnectionType = requestContext.IsFeatureEnabled("VisualStudio.FileContainerService.UseReadReplicaForBlobReferenceCleanup") ? DatabaseConnectionType.IntentReadOnly : DatabaseConnectionType.Default;
      IList<ContainerItemBlobReference> unusedBlobReferences;
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>(connectionType: new DatabaseConnectionType?(databaseConnectionType)))
        unusedBlobReferences = (IList<ContainerItemBlobReference>) component.GetUnusedBlobReferences(maxNumberOfFiles, this.m_blobReferenceSqlRetentionPeriodInDays);
      requestContext.Trace(1009015, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Count of blob references to cleanup: {0}.", (object) unusedBlobReferences.Count);
      (IList<long> longList, int failuresCount) = this.DeleteBlobReferences(requestContext, unusedBlobReferences);
      int num = 0;
      if (longList.Count > 0)
      {
        foreach (IList<long> artifactIds in longList.Batch<long>(this.m_blobReferenceDeletionChunkSize))
        {
          if (!requestContext.IsCanceled)
          {
            using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
              component.HardDeleteBlobReferences((IEnumerable<long>) artifactIds);
            num += artifactIds.Count;
          }
          else
            break;
        }
      }
      bool flag = unusedBlobReferences.Count >= maxNumberOfFiles;
      return (num, failuresCount, flag);
    }

    private (IList<long> removedArtifactIds, int failuresCount) DeleteBlobReferences(
      IVssRequestContext requestContext,
      IList<ContainerItemBlobReference> blobReferencesToCleanup)
    {
      if (this.m_blobReferenceMaxParallelRootDeletions > 1)
      {
        requestContext.Trace(1009016, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Deleting {0} blob references with the degree of parallelism: {1}.", (object) blobReferencesToCleanup.Count, (object) this.m_blobReferenceMaxParallelRootDeletions);
        return this.DeleteBlobReferencesInParallel(requestContext, blobReferencesToCleanup);
      }
      requestContext.Trace(1009016, TraceLevel.Info, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, "Deleting {0} blob references without parallelism.", (object) blobReferencesToCleanup.Count);
      return this.DeleteBlobReferencesSynchronously(requestContext, blobReferencesToCleanup);
    }

    private (IList<long> removedArtifactIds, int failuresCount) DeleteBlobReferencesSynchronously(
      IVssRequestContext requestContext,
      IList<ContainerItemBlobReference> blobReferencesToCleanup)
    {
      IVssRequestContext elevatedContext = requestContext.Elevate();
      List<long> longList = new List<long>(blobReferencesToCleanup.Count);
      int num = 0;
      foreach (ContainerItemBlobReference blobReference in (IEnumerable<ContainerItemBlobReference>) blobReferencesToCleanup)
      {
        if (!requestContext.IsCanceled)
        {
          if (this.DeleteBlobReference(elevatedContext, blobReference))
            longList.Add(blobReference.ArtifactId);
          else
            ++num;
        }
        else
          break;
      }
      return ((IList<long>) longList, num);
    }

    private async Task DeleteBlobReferencesAsync(
      IVssRequestContext requestContext,
      ConcurrentQueue<ContainerItemBlobReference> blobReferencesToCleanup,
      TeamFoundationFileContainerService.BlobReferencesParallelDeletionResult deletionResult)
    {
      IVssRequestContext elevatedContext = requestContext.Elevate();
      ContainerItemBlobReference blobReference;
      while (blobReferencesToCleanup.TryDequeue(out blobReference))
      {
        if (requestContext.IsCanceled)
        {
          elevatedContext = (IVssRequestContext) null;
          return;
        }
        try
        {
          await this.DeleteBlobReferenceAsync(elevatedContext, blobReference);
          deletionResult.AddRemovedArtifactId(blobReference.ArtifactId);
        }
        catch (Exception ex)
        {
          deletionResult.IncrementFailuresCount();
          requestContext.TraceException(1009014, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
        }
      }
      elevatedContext = (IVssRequestContext) null;
    }

    private (IList<long> removedArtifactIds, int failuresCount) DeleteBlobReferencesInParallel(
      IVssRequestContext requestContext,
      IList<ContainerItemBlobReference> blobReferencesToCleanup)
    {
      ConcurrentQueue<ContainerItemBlobReference> blobReferencesToCleanupQueue = new ConcurrentQueue<ContainerItemBlobReference>((IEnumerable<ContainerItemBlobReference>) blobReferencesToCleanup);
      TeamFoundationFileContainerService.BlobReferencesParallelDeletionResult deletionResult = new TeamFoundationFileContainerService.BlobReferencesParallelDeletionResult();
      List<Task> taskList = new List<Task>(this.m_blobReferenceMaxParallelRootDeletions);
      for (int index = 0; index < this.m_blobReferenceMaxParallelRootDeletions && !blobReferencesToCleanupQueue.IsEmpty; ++index)
        taskList.Add(requestContext.Fork((Func<IVssRequestContext, Task>) (forkedContext => this.DeleteBlobReferencesAsync(forkedContext, blobReferencesToCleanupQueue, deletionResult)), nameof (DeleteBlobReferencesInParallel)));
      if (taskList.Count > 0)
        requestContext.Join((IEnumerable<Task>) taskList);
      return (deletionResult.GetRemovedArtifactIds(), deletionResult.GetFailuresCount());
    }

    private Task DeleteBlobReferenceAsync(
      IVssRequestContext elevatedContext,
      ContainerItemBlobReference blobReference)
    {
      IDedupStore service = elevatedContext.GetService<IDedupStore>();
      IdBlobReference dedupReference = this.GetDedupReference(new Guid?(blobReference.ScopeIdentifier), blobReference);
      (IDomainId domainId1, DedupIdentifier dedupId1) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(blobReference.ArtifactHash);
      IVssRequestContext requestContext = elevatedContext;
      IDomainId domainId2 = domainId1;
      DedupIdentifier dedupId2 = dedupId1;
      IdBlobReference rootRef = dedupReference;
      return service.DeleteRootAsync(requestContext, domainId2, dedupId2, rootRef);
    }

    private bool DeleteBlobReference(
      IVssRequestContext elevatedContext,
      ContainerItemBlobReference blobReference)
    {
      try
      {
        elevatedContext.RunSynchronously((Func<Task>) (() => this.DeleteBlobReferenceAsync(elevatedContext, blobReference)));
        return true;
      }
      catch (Exception ex)
      {
        elevatedContext.TraceException(1009014, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
        return false;
      }
    }

    internal void WriteContents(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Stream outputStream,
      bool preserveServerTimestamp = false,
      bool saveAbsolutePath = true)
    {
      this.WriteContents(requestContext, containerId, path, outputStream, new Guid?(), preserveServerTimestamp, saveAbsolutePath);
    }

    public void WriteContents(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Stream outputStream,
      Guid scopeIdentifier,
      bool preserveServerTimestamp = false,
      bool saveAbsolutePath = true)
    {
      Guid? scopeIdentifier1 = new Guid?(scopeIdentifier);
      this.WriteContents(requestContext, containerId, path, outputStream, scopeIdentifier1, preserveServerTimestamp, saveAbsolutePath);
    }

    private void WriteContents(
      IVssRequestContext requestContext,
      long containerId,
      string path,
      Stream outputStream,
      Guid? scopeIdentifier,
      bool preserveServerTimestamp,
      bool saveAbsolutePath = true)
    {
      try
      {
        using (new TraceWatch(requestContext, 1008341, TraceLevel.Warning, TimeSpan.FromMilliseconds((double) this.m_slowRequestThreshold), TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (WriteContents), Array.Empty<object>()))
        {
          requestContext.TraceEnter(1008301, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (WriteContents));
          ArgumentUtility.CheckForOutOfRange(containerId, nameof (containerId), 1L);
          ArgumentUtility.CheckForNull<Stream>(outputStream, nameof (outputStream));
          string prefix = string.Empty;
          List<FileContainerItem> fileContainerItemList = new List<FileContainerItem>();
          if (this.HasReadPermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, containerId, scopeIdentifier)))
          {
            path = FileContainerItem.EnsurePathFormat(path);
            using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
              fileContainerItemList = component.QueryItems(containerId, path, scopeIdentifier);
            prefix = FileContainerPathHelper.GetParentFolderPath(path);
          }
          TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
          using (SmartPushStreamContentStream streamContentStream = new SmartPushStreamContentStream(outputStream))
          {
            using (ZipArchive zipArchive = new ZipArchive((Stream) streamContentStream, ZipArchiveMode.Create))
            {
              foreach (FileContainerItem fileContainerItem in fileContainerItemList)
              {
                if (fileContainerItem.Status != ContainerItemStatus.PendingUpload)
                {
                  if (fileContainerItem.ItemType == ContainerItemType.File)
                  {
                    ZipArchiveEntry zipEntry = !saveAbsolutePath ? zipArchive.CreateEntry(FileContainerPathHelper.RemovePathPrefix(prefix, fileContainerItem.Path)) : zipArchive.CreateEntry(fileContainerItem.Path);
                    if (preserveServerTimestamp)
                      this.PreserveServerTimestamp(requestContext, fileContainerItem.DateCreated, zipEntry);
                    if (fileContainerItem.FileLength != 0L)
                    {
                      if (fileContainerItem.ArtifactId.HasValue)
                      {
                        using (Stream destination = zipEntry.Open())
                        {
                          (IDomainId domainId, DedupIdentifier dedupId) = TeamFoundationFileContainerService.GetDomainIdAndDedupIdFromArtifactHash(this.GetBlobReference(requestContext, fileContainerItem.ArtifactId.Value, scopeIdentifier).ArtifactHash);
                          requestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => this.GetStreamFromBlobStoreAsync(requestContext, false, domainId, dedupId))).CopyTo(destination);
                        }
                      }
                      else
                      {
                        using (Stream destination = zipEntry.Open())
                        {
                          using (Stream stream = service.RetrieveFile(requestContext, (long) fileContainerItem.FileId, false, out byte[] _, out long _, out CompressionType _))
                            stream.CopyTo(destination);
                        }
                      }
                    }
                  }
                  else
                  {
                    string path1 = fileContainerItem.Path;
                    if (!path1.EndsWith("/", StringComparison.Ordinal) && !path1.EndsWith("\\", StringComparison.Ordinal))
                      path1 += "/";
                    ZipArchiveEntry zipEntry = !saveAbsolutePath ? zipArchive.CreateEntry(FileContainerPathHelper.RemovePathPrefix(prefix, path1)) : zipArchive.CreateEntry(path1);
                    if (preserveServerTimestamp)
                      this.PreserveServerTimestamp(requestContext, fileContainerItem.DateCreated, zipEntry);
                  }
                }
              }
            }
          }
          requestContext.TraceLeave(1008302, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, nameof (WriteContents));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1008303, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
        throw;
      }
    }

    private bool HasReadPermission(
      IVssRequestContext requestContext,
      SecurityEvaluationInformation securityInformation)
    {
      bool flag = requestContext.IsSystemContext;
      if (!flag)
      {
        foreach (IContainerSecurityExtension securityExtension in (IEnumerable<IContainerSecurityExtension>) this.m_securityExtensions)
        {
          try
          {
            if (securityExtension.CanEvaluate(securityInformation.ArtifactUri))
            {
              if (securityExtension.HasReadPermission(requestContext, securityInformation.ArtifactUri, securityInformation.SecurityToken))
              {
                flag = true;
                break;
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1008311, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
          }
        }
      }
      return flag;
    }

    private void CheckWritePermission(
      IVssRequestContext requestContext,
      Guid? scopeIdentifier,
      long containerId)
    {
      if (!this.HasWritePermission(requestContext, this.GetSecurityEvaluationInformation(requestContext, containerId, scopeIdentifier)))
        throw new ContainerWriteAccessDeniedException(FrameworkResources.ContainerItemWriteAccessDenied((object) containerId));
    }

    private bool HasWritePermission(
      IVssRequestContext requestContext,
      SecurityEvaluationInformation securityInformation)
    {
      bool flag = requestContext.IsSystemContext;
      if (!flag)
      {
        foreach (IContainerSecurityExtension securityExtension in (IEnumerable<IContainerSecurityExtension>) this.m_securityExtensions)
        {
          try
          {
            if (securityExtension.CanEvaluate(securityInformation.ArtifactUri))
            {
              if (securityExtension.HasWritePermission(requestContext, securityInformation.ArtifactUri, securityInformation.SecurityToken))
              {
                flag = true;
                break;
              }
            }
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1008321, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
          }
        }
      }
      return flag;
    }

    private SecurityEvaluationInformation GetSecurityEvaluationInformation(
      IVssRequestContext requestContext,
      long containerId,
      Guid? scopeIdentifier)
    {
      SecurityEvaluationInformation info;
      if (requestContext.GetService<IFileContainerCacheService>().TryGetValue(requestContext, containerId, out info))
        return info;
      Microsoft.VisualStudio.Services.FileContainer.FileContainer container;
      using (FileContainerComponent component = requestContext.CreateComponent<FileContainerComponent>())
        container = component.GetContainer(containerId, scopeIdentifier);
      return container != null ? this.GetSecurityEvaluationInformation(requestContext, container) : throw new ContainerNotFoundException(containerId);
    }

    private SecurityEvaluationInformation GetSecurityEvaluationInformation(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.FileContainer.FileContainer container)
    {
      SecurityEvaluationInformation info = new SecurityEvaluationInformation(container.ArtifactUri, container.SecurityToken);
      requestContext.GetService<IFileContainerCacheService>().Set(requestContext, container.Id, info);
      return info;
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadRegistrySettings(requestContext);
    }

    private void LoadRegistrySettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) (FrameworkServerConstants.FileContainerServiceRegistryRootPath + "/**"));
      this.m_slowRequestThreshold = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceSlowRequestThreshold, 5000);
      this.m_blobReferenceDeletionChunkSize = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobReferenceDeletionChunkSize, 1000);
      this.m_blobReferenceSqlRetentionPeriodInDays = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobReferenceSqlRetentionPeriodInDays, 1);
      this.m_blobReferenceDeletionMaxBatchCount = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobReferenceMaxBatchCount, 3);
      this.m_blobReferenceMaxParallelRootDeletions = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobReferenceMaxParallelRootDeletions, 1);
      this.m_blobStitchingBoundedCapacity = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobStitchingBoundedCapacity, 20);
      this.m_blobStitchingParallelism = registryEntryCollection.GetValueFromPath<int>(FrameworkServerConstants.FileContainerServiceBlobStitchingParallelism, 5);
    }

    private void OnContainerDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      long result;
      if (!long.TryParse(eventData, out result))
        return;
      requestContext.GetService<FileContainerCacheService>().Remove(requestContext, result);
    }

    private void OnContainersDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      try
      {
        XmlReaderSettings settings = new XmlReaderSettings()
        {
          DtdProcessing = DtdProcessing.Prohibit,
          XmlResolver = (XmlResolver) null
        };
        long[] numArray;
        using (StringReader input = new StringReader(eventData))
        {
          using (XmlReader xmlReader = XmlReader.Create((TextReader) input, settings))
            numArray = (long[]) this.m_longArraySerializer.Deserialize(xmlReader);
        }
        FileContainerCacheService service = requestContext.GetService<FileContainerCacheService>();
        foreach (long containerId in numArray)
          service.Remove(requestContext, containerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1008122, TeamFoundationFileContainerService.s_area, TeamFoundationFileContainerService.s_layer, ex);
      }
    }

    private IdBlobReference GetDedupReference(
      Guid? scopeIdentifier,
      ContainerItemBlobReference blobRef)
    {
      return new IdBlobReference(string.Format("{0}/{1}", (object) scopeIdentifier, (object) blobRef.ArtifactId), TeamFoundationFileContainerService.s_buildArtifactScope);
    }

    internal void PreserveServerTimestamp(
      IVssRequestContext requestContext,
      DateTime dateTime,
      ZipArchiveEntry zipEntry)
    {
      TimeZoneInfo local = TimeZoneInfo.Local;
      zipEntry.LastWriteTime = (DateTimeOffset) TimeZoneInfo.ConvertTimeFromUtc(dateTime, local);
    }

    private class BlobReferencesParallelDeletionResult
    {
      private ConcurrentBag<long> m_removedArtifactIds = new ConcurrentBag<long>();
      private int m_failuresCount;

      public void AddRemovedArtifactId(long artifactId) => this.m_removedArtifactIds.Add(artifactId);

      public void IncrementFailuresCount() => Interlocked.Increment(ref this.m_failuresCount);

      public IList<long> GetRemovedArtifactIds() => (IList<long>) this.m_removedArtifactIds.ToList<long>();

      public int GetFailuresCount() => this.m_failuresCount;
    }
  }
}
