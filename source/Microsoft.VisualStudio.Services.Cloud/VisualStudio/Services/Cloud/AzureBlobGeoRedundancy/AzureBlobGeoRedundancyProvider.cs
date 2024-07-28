// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyProvider : AzureProvider
  {
    private readonly Func<IVssRequestContext, AzureProvider> m_secondaryFactory;
    private BlobGeoRedundancyEndpoint m_endpoint;
    internal AzureProvider m_secondary;
    private string m_secondaryDrawerName;
    private string m_secondaryLookupKey;
    public const string OptimizeQueueServicePointFeatureName = "VisualStudio.FrameworkService.AzureStorage.OptimizeQueueServicePoint";
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyProvider";

    public AzureBlobGeoRedundancyProvider()
      : this((Func<IVssRequestContext, AzureProvider>) (rc => new AzureProvider()))
    {
    }

    internal AzureBlobGeoRedundancyProvider(
      Func<IVssRequestContext, AzureProvider> secondaryFactory)
    {
      ArgumentUtility.CheckForNull<Func<IVssRequestContext, AzureProvider>>(secondaryFactory, nameof (secondaryFactory));
      this.m_secondaryFactory = secondaryFactory;
    }

    public override void ServiceStart(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings)
    {
      base.ServiceStart(requestContext, settings);
      string drawerName;
      string lookupKey;
      if (settings.TryGetValue("DrawerName", out drawerName) && settings.TryGetValue("LookupKey", out lookupKey))
      {
        IEnumerable<GeoRedundantStorageAccountSettings> redundantStorageAccounts = requestContext.GetService<IAzureBlobGeoRedundancyManagementService>().GetGeoRedundantStorageAccounts(requestContext);
        GeoRedundantStorageAccountSettings storageAccountSettings = redundantStorageAccounts.Where<GeoRedundantStorageAccountSettings>((Func<GeoRedundantStorageAccountSettings, bool>) (a => string.Equals(a.DrawerName, drawerName, StringComparison.Ordinal) && string.Equals(a.PrimaryLookupKey, lookupKey, StringComparison.Ordinal))).FirstOrDefault<GeoRedundantStorageAccountSettings>();
        if (storageAccountSettings != null)
        {
          this.m_secondaryDrawerName = storageAccountSettings.DrawerName;
          this.m_secondaryLookupKey = storageAccountSettings.SecondaryLookupKey;
        }
        else
        {
          storageAccountSettings = redundantStorageAccounts.Where<GeoRedundantStorageAccountSettings>((Func<GeoRedundantStorageAccountSettings, bool>) (a => string.Equals(a.DrawerName, drawerName, StringComparison.Ordinal) && string.Equals(a.SecondaryLookupKey, lookupKey, StringComparison.Ordinal))).FirstOrDefault<GeoRedundantStorageAccountSettings>();
          if (storageAccountSettings != null)
          {
            this.m_secondaryDrawerName = storageAccountSettings.DrawerName;
            this.m_secondaryLookupKey = storageAccountSettings.PrimaryLookupKey;
          }
        }
        if (storageAccountSettings == null)
        {
          requestContext.TraceAlways(15303002, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "No configured geo-redundant storage account found for the given lookup key. DrawerName: {0}, LookupKey: {1}.", (object) drawerName, (object) lookupKey);
        }
        else
        {
          this.OnStrongBoxChanged(requestContext, (IEnumerable<StrongBoxItemName>) null);
          if (requestContext.IsTracing(15303010, TraceLevel.Verbose, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider)))
          {
            StackTrace stackTrace = new StackTrace();
            requestContext.Trace(15303010, TraceLevel.Verbose, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), string.Format("{0} being called in {1} :\n {2}", (object) "RegisterNotification", (object) nameof (AzureBlobGeoRedundancyProvider), (object) stackTrace));
          }
          requestContext.GetService<ITeamFoundationStrongBoxService>().RegisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged), this.m_secondaryDrawerName, (IEnumerable<string>) new string[1]
          {
            this.m_secondaryLookupKey
          });
        }
      }
      else
        requestContext.TraceAlways(15303005, TraceLevel.Warning, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "Missing {0} and {1} settings. No secondary blob provider will be created.", (object) "DrawerName", (object) "LookupKey");
    }

    public override void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationStrongBoxService>().UnregisterNotification(requestContext, new StrongBoxItemChangedCallback(this.OnStrongBoxChanged));
      base.ServiceEnd(requestContext);
      this.m_secondary?.ServiceEnd(requestContext);
    }

    internal void OnStrongBoxChanged(
      IVssRequestContext requestContext,
      IEnumerable<StrongBoxItemName> itemNames)
    {
      AzureProvider azureProvider = (AzureProvider) null;
      AzureProvider secondary = this.m_secondary;
      try
      {
        azureProvider = this.m_secondaryFactory(requestContext);
        azureProvider.ServiceStart(requestContext, (IDictionary<string, string>) new Dictionary<string, string>()
        {
          ["BlobStorageConnectionStringOverride"] = AzureBlobGeoRedundancyUtils.GetStorageAccountConnectionString(requestContext, this.m_secondaryDrawerName, this.m_secondaryLookupKey)
        });
        if (string.Equals(this.StorageAccountName, azureProvider.StorageAccountName, StringComparison.OrdinalIgnoreCase))
        {
          requestContext.TraceAlways(15303004, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "The primary and secondary storage accounts are the same. Dual write will not be possible. DrawerName: {0}, SecondaryLookupKey: {1}, StorageAccount: {2}", (object) this.m_secondaryDrawerName, (object) this.m_secondaryLookupKey, (object) azureProvider.StorageAccountName);
          azureProvider.ServiceEnd(requestContext);
          this.m_secondary = (AzureProvider) null;
        }
        else
        {
          requestContext.TraceAlways(15303003, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "Configured secondary blob provider for synchronous writes. DrawerName: {0}, PrimaryStorageAccount: {1}, SecondaryStorageAccount: {2}.", (object) this.m_secondaryDrawerName, (object) this.StorageAccountName, (object) azureProvider.StorageAccountName);
          this.m_secondary = azureProvider;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(15303001, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "An error occurred while creating the secondary blob provider. Dual write will not be possible. Exception: {0}", (object) ex);
        azureProvider?.ServiceEnd(requestContext);
        this.m_secondary = (AzureProvider) null;
      }
      secondary?.ServiceEnd(requestContext);
    }

    protected override void Initialize(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      base.Initialize(requestContext, account);
      this.m_endpoint = requestContext.GetService<IAzureBlobGeoRedundancyService>().SetupEndpoint(requestContext, account);
      AzureBlobGeoRedundancyProvider.ConfigureQueueServiceEndpoint(requestContext, account);
      requestContext.TraceAlways(15303000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "Initialized blob provider with geo-redundancy enabled.");
    }

    public override void Initialize(
      IVssRequestContext requestContext,
      string sasToken,
      Uri blobEndpointUri,
      Uri tableEndpointUri = null)
    {
      base.Initialize(requestContext, sasToken, blobEndpointUri, tableEndpointUri);
      requestContext.Trace(15303000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyProvider), "Initialized blob provider with blob storage Uri and SAS token. No blob geo-redundancy will be used.");
    }

    public override bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.DeleteBlob(requestContext, containerId, resourceId, clientTimeout);
      else
        this.RecordDeleteBlob(requestContext, containerId, resourceId);
      return base.DeleteBlob(requestContext, containerId, resourceId, clientTimeout);
    }

    public override List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
      else
        this.RecordDeleteBlobs(requestContext, containerId, (IEnumerable<Guid>) resourceIds);
      return base.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
    }

    public override List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
      else
        this.RecordDeleteBlobs(requestContext, containerId, (IEnumerable<string>) resourceIds);
      return base.DeleteBlobs(requestContext, containerId, resourceIds, clientTimeout);
    }

    protected internal override bool CreateCloudBlobContainerIfNotExists(
      IVssRequestContext requestContext,
      CloudBlobContainer container,
      TimeSpan? clientTimeout)
    {
      bool containerIfNotExists = base.CreateCloudBlobContainerIfNotExists(requestContext, container, clientTimeout);
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.CreateCloudBlobContainerIfNotExists(requestContext, container, clientTimeout);
      else if (containerIfNotExists)
        this.RecordCreateContainer(requestContext, container.Name);
      return containerIfNotExists;
    }

    public override void DeleteContainer(
      IVssRequestContext requestContext,
      string containerId,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.DeleteContainer(requestContext, containerId, clientTimeout);
      else
        this.RecordDeleteContainer(requestContext, containerId);
      base.DeleteContainer(requestContext, containerId, clientTimeout);
    }

    public override void PutStreamRaw(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      BlobGeoRedundancyEndpoint endpoint = this.m_endpoint;
      if (endpoint != null)
      {
        if (this.m_secondary != null && endpoint.Settings.SynchronousWritesEnabled)
          throw new InvalidOperationException("Can not call PutStreamRaw when synchronous writes are enabled");
        AzureBlobGeoRedundancyService.CreateBlobRaw(endpoint, container.Name, resourceId);
      }
      base.PutStreamRaw(container, resourceId, content, metadata, clientTimeout);
    }

    public override void PutStreamRawUsingBlockBlobClient(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      int? blockSize = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BlobGeoRedundancyEndpoint endpoint = this.m_endpoint;
      if (endpoint != null)
      {
        if (this.m_secondary != null && endpoint.Settings.SynchronousWritesEnabled)
          throw new InvalidOperationException("Can not call PutStreamRawUsingBlockBlobClient when synchronous writes are enabled");
        AzureBlobGeoRedundancyService.CreateBlobRaw(endpoint, container.Name, resourceId);
      }
      base.PutStreamRawUsingBlockBlobClient(container, resourceId, content, metadata, blockSize, cancellationToken);
    }

    public override void PutStream(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId,
      Stream stream,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
      {
        this.DualWriteStream(requestContext, secondary, containerName, resourceId, stream, metadata, clientTimeout);
      }
      else
      {
        this.RecordCreateBlob(requestContext, containerName, resourceId);
        base.PutStream(requestContext, containerName, resourceId, stream, metadata, clientTimeout);
      }
    }

    public override void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> blobMetadata = null,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      this.AreSynchronousWritesEnabled(requestContext, out secondary);
      this.PutChunkInternal(requestContext, secondary, containerId, resourceId, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, blobMetadata, clientTimeout);
    }

    public override void RenameBlob(
      IVssRequestContext requestContext,
      string containerName,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
      {
        secondary.RenameBlob(requestContext, containerName, sourceResourceId, targetResourceId, clientTimeout);
      }
      else
      {
        this.RecordCreateBlob(requestContext, containerName, targetResourceId);
        this.RecordDeleteBlob(requestContext, containerName, sourceResourceId);
      }
      base.RenameBlob(requestContext, containerName, sourceResourceId, targetResourceId, clientTimeout);
    }

    public override void SetBlobHeaders(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
      base.SetBlobHeaders(requestContext, containerId, resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public override void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      AzureProvider secondary;
      if (this.AreSynchronousWritesEnabled(requestContext, out secondary))
        secondary.WriteBlobMetadata(requestContext, containerId, resourceId, metadata, clientTimeout);
      base.WriteBlobMetadata(requestContext, containerId, resourceId, metadata, clientTimeout);
    }

    internal void PutChunkInternal(
      IVssRequestContext requestContext,
      AzureProvider secondary,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> blobMetadata = null,
      TimeSpan? clientTimeout = null)
    {
      if (secondary != null)
        secondary.PutChunk(requestContext, containerId, resourceId, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, blobMetadata, clientTimeout);
      else if (isLastChunk)
        this.RecordCreateBlob(requestContext, containerId, resourceId);
      base.PutChunk(requestContext, containerId, resourceId, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, blobMetadata, clientTimeout);
    }

    internal void DualWriteStream(
      IVssRequestContext requestContext,
      AzureProvider secondary,
      string containerName,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      if (content.CanSeek)
      {
        secondary.PutStream(requestContext, containerName, resourceId, content, metadata, clientTimeout);
        content.Seek(0L, SeekOrigin.Begin);
        base.PutStream(requestContext, containerName, resourceId, content, metadata, clientTimeout);
      }
      else
      {
        Guid containerId = new Guid(containerName);
        byte[] numArray = new byte[1048576];
        long offset = 0;
        long length = content.Length;
        while (length > 0L)
        {
          int contentBlockLength = content.Read(numArray, 0, (int) Math.Min(1048576L, length));
          length -= (long) contentBlockLength;
          this.PutChunkInternal(requestContext, secondary, containerId, resourceId, numArray, contentBlockLength, content.Length, offset, length <= 0L, metadata, clientTimeout);
          offset += (long) contentBlockLength;
        }
      }
    }

    internal virtual bool AreSynchronousWritesEnabled(
      IVssRequestContext requestContext,
      out AzureProvider secondary)
    {
      secondary = this.m_secondary;
      if (secondary == null)
      {
        secondary = (AzureProvider) null;
        return false;
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IAzureBlobGeoRedundancyService service = vssRequestContext.GetService<IAzureBlobGeoRedundancyService>();
      if (service.IsEnabled(vssRequestContext) && service.AreSynchronousWritesEnabled(vssRequestContext))
        return true;
      secondary = (AzureProvider) null;
      return false;
    }

    private void InvokeIfEnabled(
      IVssRequestContext requestContext,
      Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint> action)
    {
      BlobGeoRedundancyEndpoint endpoint = this.m_endpoint;
      if (endpoint == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IAzureBlobGeoRedundancyService service = vssRequestContext.GetService<IAzureBlobGeoRedundancyService>();
      if (!service.IsEnabled(vssRequestContext))
        return;
      action(vssRequestContext, service, endpoint);
    }

    private void RecordCreateBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId)
    {
      this.RecordCreateBlob(requestContext, containerId.ToString("N"), resourceId);
    }

    private void RecordCreateBlob(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId)
    {
      this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.CreateBlob(deploymentContext, endpoint, containerName, resourceId)));
    }

    private void RecordDeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId)
    {
      this.RecordDeleteBlob(requestContext, containerId.ToString("N"), resourceId);
    }

    private void RecordDeleteBlob(
      IVssRequestContext requestContext,
      string containerName,
      string resourceId)
    {
      this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.DeleteBlob(deploymentContext, endpoint, containerName, resourceId)));
    }

    private void RecordDeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      IEnumerable<string> resourceIds)
    {
      this.RecordDeleteBlobs(requestContext, containerId.ToString("N"), resourceIds);
    }

    private void RecordDeleteBlobs(
      IVssRequestContext requestContext,
      string containerName,
      IEnumerable<string> resourceIds)
    {
      this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.DeleteBlobs(deploymentContext, endpoint, containerName, resourceIds)));
    }

    private void RecordDeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      IEnumerable<Guid> resourceIds)
    {
      this.RecordDeleteBlobs(requestContext, containerId.ToString("N"), resourceIds);
    }

    private void RecordDeleteBlobs(
      IVssRequestContext requestContext,
      string containerName,
      IEnumerable<Guid> resourceIds)
    {
      this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.DeleteBlobs(deploymentContext, endpoint, containerName, resourceIds.Select<Guid, string>((Func<Guid, string>) (id => id.ToString("N"))))));
    }

    private void RecordCreateContainer(IVssRequestContext requestContext, string containerName) => this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.CreateContainer(deploymentContext, endpoint, containerName)));

    private void RecordDeleteContainer(IVssRequestContext requestContext, string containerName) => this.InvokeIfEnabled(requestContext, (Action<IVssRequestContext, IAzureBlobGeoRedundancyService, BlobGeoRedundancyEndpoint>) ((deploymentContext, geoRedundancyService, endpoint) => geoRedundancyService.DeleteContainer(deploymentContext, endpoint, containerName)));

    private static void ConfigureQueueServiceEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount account)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.AzureStorage.OptimizeQueueServicePoint"))
        return;
      AzureBlobGeoRedundancyUtils.OptimizeQueueServiceEndpoint(requestContext, account);
    }
  }
}
