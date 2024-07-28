// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Hosting.AzureProviderBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Framework.Hosting
{
  public abstract class AzureProviderBase : IAzureBlobProvider, IBlobProvider
  {
    public const int BlockSize = 1048576;
    public const int LargestBlockSize = 10485760;
    private const int c_timeoutFactor = 1;
    private TimeSpan DownloadToStreamTimeout = TimeSpan.FromMilliseconds(14000.0);
    private TimeSpan EnumerateBlobsTimeout = TimeSpan.FromMilliseconds(1000.0);
    private TimeSpan FetchAttributesTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan GetCloudBlobContainerTimeout = TimeSpan.FromMilliseconds(1000.0);
    private TimeSpan GetStreamTimeout = TimeSpan.FromMilliseconds(5000.0);
    private TimeSpan DeleteBlobTimeout = TimeSpan.FromMilliseconds(3000.0);
    private TimeSpan DeleteContainerTimeout = TimeSpan.FromMilliseconds(1000.0);
    private TimeSpan PutBlockTimeout = TimeSpan.FromMilliseconds(9000.0);
    private TimeSpan ReadBlobMetadataTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan ReadBlobPropertiesTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan RenameBlobTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan WriteBlobMetadataTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan WriteBlobTagsTimeout = TimeSpan.FromMilliseconds(16000.0);
    private TimeSpan BlobExistsTimeout = TimeSpan.FromMilliseconds(3000.0);
    private const int c_defaultNotificationThreshold = 5000;
    private readonly ConcurrentDictionary<Guid, string> m_tagNewAzureBlobProviderBaseBlobsPathSettings = new ConcurrentDictionary<Guid, string>();
    internal const string BlobStorageConnectionString = "BlobStorageConnectionString";
    public const string BlobStorageConnectionStringOverrideKey = "BlobStorageConnectionStringOverride";
    public const string BlobStorageUriOverrideKey = "BlobStorageUriOverride";
    public const string TableStorageUriOverrideKey = "TableStorageUriOverride";
    public const string SasTokenOverrideKey = "BlobStorageCredentialsOverride";
    public const string c_registryKeyDegreeOfParallelism = "/Service/AzureBlobProvider/Settings/ParallelismDegree";
    internal const string s_Area = "FileService";
    internal const string s_Layer = "BlobStorage";
    private bool m_initialized;
    private const string c_registrySettingsPath = "/Service/AzureBlobProvider/Settings/SlowWarningThreshold";
    protected IAzureProviderSettings m_settings;
    private const string OptimizeBlobServicePointFeatureName = "VisualStudio.FrameworkService.AzureStorage.OptimizeBlobServicePoint";

    public void ServiceStart(IVssRequestContext requestContext) => this.ServiceStart(requestContext, (IDictionary<string, string>) null);

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      registryEntries.ForEach<RegistryEntry>((Action<RegistryEntry>) (registryEntry => this.m_tagNewAzureBlobProviderBaseBlobsPathSettings.AddOrUpdate(Guid.Parse(registryEntry.Name), registryEntry.Value, (Func<Guid, string, string>) ((key, oldValue) => registryEntry.Value))));
    }

    public virtual void ServiceStart(
      IVssRequestContext requestContext,
      IDictionary<string, string> settings)
    {
      TeamFoundationTracingService.TraceRaw(15010, TraceLevel.Info, "FileService", "BlobStorage", "Blob storage service start");
      RegistryQuery filter = new RegistryQuery(FrameworkServerConstants.Migration_TagNewAzureBlobProviderBaseBlobsPath + "/**");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), in filter);
      foreach (RegistryEntry readEntry in service.ReadEntries(requestContext, (RegistryQuery) (FrameworkServerConstants.Migration_TagNewAzureBlobProviderBaseBlobsPath + "/*")))
      {
        RegistryEntry registryEntry = readEntry;
        this.m_tagNewAzureBlobProviderBaseBlobsPathSettings.AddOrUpdate(Guid.Parse(registryEntry.Name), registryEntry.Value, (Func<Guid, string, string>) ((key, oldValue) => registryEntry.Value));
      }
      string connectionString;
      if (settings != null && settings.TryGetValue("BlobStorageConnectionStringOverride", out connectionString))
      {
        if (!string.IsNullOrEmpty(connectionString))
        {
          this.Initialize(requestContext, connectionString);
          this.BlobServiceClient = new BlobServiceClient(connectionString);
        }
        else
          TeamFoundationTracingService.TraceRaw(15015, TraceLevel.Warning, "FileService", "BlobStorage", "Configuration data contained key {0}, but  a null or empty value was given", (object) "BlobStorageConnectionStringOverride");
      }
      string uriString1;
      string sasToken;
      if (!this.m_initialized && settings != null && settings.TryGetValue("BlobStorageUriOverride", out uriString1) && settings.TryGetValue("BlobStorageCredentialsOverride", out sasToken))
      {
        string uriString2;
        settings.TryGetValue("TableStorageUriOverride", out uriString2);
        if (!string.IsNullOrEmpty(uriString1) && !string.IsNullOrEmpty(sasToken))
        {
          Uri blobEndpointUri = new Uri(uriString1);
          this.Initialize(requestContext, sasToken, blobEndpointUri, string.IsNullOrEmpty(uriString2) ? (Uri) null : new Uri(uriString2));
          this.BlobServiceClient = new BlobServiceClient(blobEndpointUri, new AzureSasCredential(sasToken), (BlobClientOptions) null);
        }
        else
          TeamFoundationTracingService.TraceRaw(15016, TraceLevel.Warning, "FileService", "BlobStorage", "Configuration data contained keys {0} and {1}, but null or empty values were given", (object) "BlobStorageUriOverride", (object) "BlobStorageCredentialsOverride");
      }
      if (!this.m_initialized)
      {
        try
        {
          TeamFoundationTracingService.TraceRawAlwaysOn(15011, TraceLevel.Info, "FileService", "BlobStorage", "Attempting to get configuration data through Azure Runtime");
          string decryptedString = AzureRoleUtil.Configuration.GetDecryptedString("BlobStorageConnectionString");
          TeamFoundationTracingService.TraceRaw(15012, TraceLevel.Info, "FileService", "BlobStorage", "Found blobConnectionString.");
          if (!string.IsNullOrEmpty(decryptedString))
          {
            this.Initialize(requestContext, decryptedString);
            this.BlobServiceClient = new BlobServiceClient(decryptedString);
          }
          else
            TeamFoundationTracingService.TraceRaw(15013, TraceLevel.Info, "FileService", "BlobStorage", "Could not get configuration data through Azure Runtime");
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(15014, "FileService", "BlobStorage", ex);
        }
      }
      if (!this.m_initialized)
      {
        TeamFoundationTracingService.TraceRawAlwaysOn(15020, TraceLevel.Info, "FileService", "BlobStorage", "Attempting to get configuration data through TFS Registry");
        try
        {
          IVssRegistryService registryService1 = service;
          IVssRequestContext requestContext1 = requestContext;
          RegistryQuery registryQuery = (RegistryQuery) FrameworkServerConstants.FileServiceAccountName;
          ref RegistryQuery local1 = ref registryQuery;
          string empty1 = string.Empty;
          string accountName = registryService1.GetValue<string>(requestContext1, in local1, empty1);
          IVssRegistryService registryService2 = service;
          IVssRequestContext requestContext2 = requestContext;
          registryQuery = (RegistryQuery) FrameworkServerConstants.FileServiceAccountKey;
          ref RegistryQuery local2 = ref registryQuery;
          string empty2 = string.Empty;
          string keyValue = registryService2.GetValue<string>(requestContext2, in local2, empty2);
          if (!string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(keyValue))
          {
            Microsoft.Azure.Storage.CloudStorageAccount account = new Microsoft.Azure.Storage.CloudStorageAccount(new Microsoft.Azure.Storage.Auth.StorageCredentials(accountName, keyValue), true);
            this.Initialize(requestContext, account);
            this.BlobServiceClient = new BlobServiceClient(new Uri("https://" + accountName + ".blob.core.windows.net"), new StorageSharedKeyCredential(accountName, keyValue), (BlobClientOptions) null);
          }
          else
            TeamFoundationTracingService.TraceRaw(15021, TraceLevel.Info, "FileService", "BlobStorage", "Could not get configuration data through TFS Registry");
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(14410, "FileService", "BlobStorage", ex);
        }
      }
      if (!this.m_initialized)
        throw new InvalidOperationException("The Azure Blob Provider is not configured properly");
      this.Client.BufferManager = (IBufferManager) requestContext.To(TeamFoundationHostType.Deployment).GetService<BufferManagerService>().GetBufferManager();
    }

    public virtual void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public void Initialize(IVssRequestContext requestContext, string connectionString) => this.Initialize(requestContext, Microsoft.Azure.Storage.CloudStorageAccount.Parse(connectionString));

    protected virtual void Initialize(
      IVssRequestContext requestContext,
      Microsoft.Azure.Storage.CloudStorageAccount account)
    {
      this.Client = account.CreateCloudBlobClient();
      this.TableClient = account.ToTableStorageAccount().CreateCloudTableClient();
      this.ApplySettings(requestContext);
      this.Client.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(this.m_settings.DefaultBlobRequestClientTimeout);
      this.TableClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(this.m_settings.DefaultTableRequestClientTimeout);
      AzureProviderBase.ConfigureServicePoint(requestContext, account);
      this.m_initialized = true;
    }

    public virtual void Initialize(
      IVssRequestContext requestContext,
      string sasToken,
      Uri blobEndpointUri,
      Uri tableEndpointUri = null)
    {
      Microsoft.Azure.Storage.Auth.StorageCredentials storageCredentials = new Microsoft.Azure.Storage.Auth.StorageCredentials(sasToken);
      this.Client = new CloudBlobClient(blobEndpointUri, storageCredentials);
      this.TableClient = tableEndpointUri == (Uri) null ? (CloudTableClient) null : new CloudTableClient(tableEndpointUri, storageCredentials.ToTableCredentials());
      this.ApplySettings(requestContext);
      this.Client.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(this.m_settings.DefaultBlobRequestClientTimeout);
      if (this.TableClient != null)
        this.TableClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(this.m_settings.DefaultTableRequestClientTimeout);
      AzureProviderBase.ConfigureServicePoint(requestContext, new Microsoft.Azure.Storage.CloudStorageAccount(storageCredentials, blobEndpointUri, (Uri) null, tableEndpointUri, (Uri) null));
      this.m_initialized = true;
    }

    private void ApplySettings(IVssRequestContext requestContext) => this.m_settings = (IAzureProviderSettings) requestContext.GetService<AzureProviderSettingsService>().Settings;

    public RemoteStoreId RemoteStoreId => RemoteStoreId.AzureBlob;

    public string StorageAccountName => this.Client?.Credentials?.AccountName;

    public Uri Uri => this.BlobServiceClient?.Uri;

    protected abstract ICloudBlob GetCloudBlobReference(
      CloudBlobContainer container,
      string resourceId);

    public Page<TaggedBlobItem> FindBlobsByTags(
      IDictionary<string, string> tags,
      string containerName = null,
      string continuationToken = null)
    {
      return this.BlobServiceClient.FindBlobsByTags(this.BuildFindBlobsByTagsQuery(tags, containerName), new CancellationToken()).AsPages(continuationToken, new int?()).FirstOrDefault<Page<TaggedBlobItem>>();
    }

    private string BuildFindBlobsByTagsQuery(IDictionary<string, string> tags, string containerName = null)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (containerName != null)
        stringBuilder.AppendFormat("@container = '{0}'", (object) containerName);
      foreach (KeyValuePair<string, string> tag in (IEnumerable<KeyValuePair<string, string>>) tags)
      {
        if (stringBuilder.Length != 0)
          stringBuilder.Append(" AND ");
        stringBuilder.AppendFormat("\"{0}\" = '{1}'", (object) tag.Key, (object) tag.Value);
      }
      return stringBuilder.ToString();
    }

    public virtual bool DeleteBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.DeleteBlob(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public virtual bool DeleteBlob(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Delete, (long) this.m_settings.NotificationThreshold, nameof (DeleteBlob)))
      {
        Func<bool> run = (Func<bool>) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
            {
              bool flag = cloudBlobReference.DeleteIfExists(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.DeleteBlobClientTimeout));
              if (!flag)
              {
                if (!cloudBlobReference.Exists(this.GetBlobRequestOptions(clientTimeout, this.m_settings.BlobExistsClientTimeout)))
                  flag = true;
                else
                  TeamFoundationTracingService.TraceRawAlwaysOn(1013184, TraceLevel.Info, "FileService", "BlobStorage", "DeleteIfExists returned false, however the blob did exist. Blob: {0}", (object) cloudBlobReference.Uri);
              }
              tracer.SetContentLength(1L);
              return flag;
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.DeleteBlob." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.DeleteBlobTimeout));
        return new CommandService<bool>(requestContext, setter, run).Execute();
      }
    }

    public virtual List<Guid> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<Guid> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      return this.DeleteBlobs<Guid>(requestContext, containerId, resourceIds, (Func<Guid, string>) (id => id.ToString("n")), clientTimeout);
    }

    public virtual List<string> DeleteBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      List<string> resourceIds,
      TimeSpan? clientTimeout = null)
    {
      return this.DeleteBlobs<string>(requestContext, containerId, resourceIds, (Func<string, string>) (id => id), clientTimeout);
    }

    private List<T> DeleteBlobs<T>(
      IVssRequestContext requestContext,
      Guid containerId,
      List<T> resourceIds,
      Func<T, string> converter,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/AzureBlobProvider/Settings/ParallelismDegree", 10);
      int currentExecutingTasks = 0;
      CancellationToken cancellationToken = requestContext.CancellationToken;
      ConcurrentBag<T> deletes = new ConcurrentBag<T>();
      List<Task> tasks = new List<Task>(num);
      ConcurrentQueue<T> queue = new ConcurrentQueue<T>((IEnumerable<T>) resourceIds);
      requestContext.Trace(1013187, TraceLevel.Info, "FileService", "BlobStorage", string.Format("Starting blob cleanup. MaxParallelism is {0}", (object) num));
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Delete, (long) this.m_settings.NotificationThreshold, nameof (DeleteBlobs)))
      {
        using (SemaphoreSlim deleteSemaphore = new SemaphoreSlim(num))
        {
          Func<List<T>> run = (Func<List<T>>) (() =>
          {
            try
            {
              CloudBlobContainer container = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
              if (container == null)
                return deletes.ToList<T>();
              using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
              {
                BlobRequestOptions deleteOptions = this.GetBlobRequestOptions(clientTimeout, this.m_settings.DeleteBlobClientTimeout);
                BlobRequestOptions existsOptions = this.GetBlobRequestOptions(clientTimeout, this.m_settings.BlobExistsClientTimeout);
                while (!queue.IsEmpty)
                {
                  deleteSemaphore.Wait(cancellationToken);
                  Interlocked.Increment(ref currentExecutingTasks);
                  T resourceId;
                  if (queue.TryDequeue(out resourceId))
                    tasks.Add(Task.Run((Func<Task>) (async () =>
                    {
                      try
                      {
                        ICloudBlob blob = this.GetCloudBlobReference(container, converter(resourceId));
                        if (await blob.DeleteIfExistsAsync(DeleteSnapshotsOption.None, (AccessCondition) null, deleteOptions, (Microsoft.Azure.Storage.OperationContext) null, cancellationToken))
                          deletes.Add(resourceId);
                        else if (!await blob.ExistsAsync(existsOptions, (Microsoft.Azure.Storage.OperationContext) null, cancellationToken))
                          deletes.Add(resourceId);
                        else
                          TeamFoundationTracingService.TraceRawAlwaysOn(1013184, TraceLevel.Info, "FileService", "BlobStorage", "DeleteIfExists returned false, however the blob did exist. Blob: {0}", (object) blob.Uri);
                        blob = (ICloudBlob) null;
                      }
                      finally
                      {
                        Interlocked.Decrement(ref currentExecutingTasks);
                        deleteSemaphore.Release();
                      }
                    }), cancellationToken));
                  for (int index = tasks.Count - 1; index >= 0; --index)
                  {
                    if (tasks[index].IsCompleted)
                      tasks.RemoveAt(index);
                  }
                }
                if (tasks.Count > 0)
                  Task.WaitAll(tasks.ToArray(), cancellationToken);
                tracer.SetContentLength((long) deletes.Count);
                return deletes.ToList<T>();
              }
            }
            catch (Microsoft.Azure.Storage.StorageException ex)
            {
              this.FilterStorageExceptionForCircuitBreaker(ex);
              throw;
            }
          });
          TimeSpan timeSpan = TimeSpan.FromMilliseconds(this.DeleteBlobTimeout.TotalMilliseconds * (double) resourceIds.Count);
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.DeleteBlob." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(timeSpan));
          return new CommandService<List<T>>(requestContext, setter, run).Execute();
        }
      }
    }

    public virtual void DeleteContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      this.DeleteContainer(requestContext, containerId.ToString("n"), clientTimeout);
    }

    public virtual void DeleteContainer(
      IVssRequestContext requestContext,
      string containerId,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Delete, (long) this.m_settings.NotificationThreshold, nameof (DeleteContainer)))
      {
        Action run = (Action) (() =>
        {
          try
          {
            CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
            {
              try
              {
                containerInternal.Delete(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.DeleteContainerClientTimeout));
              }
              catch (Microsoft.Azure.Storage.StorageException ex) when (ex.RequestInformation != null)
              {
                switch (ex.RequestInformation.HttpStatusCode)
                {
                  case 404:
                    break;
                  case 409:
                    requestContext.TraceAlways(15044, TraceLevel.Error, "FileService", "BlobStorage", "{0} thrown during test: {1}\r\n    HttpStatusCode: {2}\r\n    ExtendedErrorInformation.Code: {3}\r\n    ServiceRequestID: {4}\r\n    ExtendedErrorInformation.Message: {5}\r\n    ExtendedErrorInformation.Details ({6}):{7}\r\n    StackTrace: {8}", (object) "StorageException", (object) ex.ToString(), (object) HttpStatusCode.Conflict, (object) ex.RequestInformation.ExtendedErrorInformation?.ErrorCode, (object) ex.RequestInformation.ServiceRequestID, (object) ex.RequestInformation.ExtendedErrorInformation?.ErrorMessage, (object) ex.RequestInformation.ExtendedErrorInformation?.AdditionalDetails.Count, (object) string.Join<KeyValuePair<string, string>>("; ", (IEnumerable<KeyValuePair<string, string>>) (ex.RequestInformation.ExtendedErrorInformation?.AdditionalDetails ?? (IDictionary<string, string>) new Dictionary<string, string>())), (object) ex.StackTrace);
                    throw;
                  default:
                    throw;
                }
              }
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.DeleteContainer." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.DeleteContainerTimeout));
        new CommandService(requestContext, setter, run).Execute();
        tracer.SetContentLength(0L);
      }
    }

    public CloudBlobContainer GetCloudBlobContainer(
      IVssRequestContext requestContext,
      Guid containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      return this.GetCloudBlobContainer(requestContext, containerId.ToString("n"), createIfNotExists, clientTimeout);
    }

    public CloudBlobContainer GetCloudBlobContainer(
      IVssRequestContext requestContext,
      string containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (GetCloudBlobContainer)))
      {
        CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, createIfNotExists, clientTimeout);
        tracer.SetContentLength(0L);
        return containerInternal;
      }
    }

    public CloudTable GetCloudTableReference(
      IVssRequestContext requestContext,
      string tableName,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      if (this.TableClient == null)
        throw new InvalidOperationException("GetCloudTableReference can only be used when a table endpoint is set");
      using (new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (GetCloudTableReference)))
      {
        Func<CloudTable> run = (Func<CloudTable>) (() =>
        {
          try
          {
            CloudTable tableReference = this.TableClient.GetTableReference(tableName);
            if (createIfNotExists)
            {
              TableRequestOptions tableRequestOptions = this.GetTableRequestOptions(clientTimeout, this.m_settings.GetCloudBlobContainerClientTimeout);
              tableReference.CreateIfNotExistsAsync(tableRequestOptions, (Microsoft.Azure.Cosmos.Table.OperationContext) null).SyncResult<bool>();
            }
            return tableReference;
          }
          catch (Microsoft.Azure.Cosmos.Table.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.GetCloudTableReference." + this.TableClient.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.GetCloudBlobContainerTimeout));
        return new CommandService<CloudTable>(requestContext, setter, run).Execute();
      }
    }

    internal virtual CloudBlobContainer GetCloudBlobContainerInternal(
      IVssRequestContext requestContext,
      Guid containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      return this.GetCloudBlobContainerInternal(requestContext, containerId.ToString("n"), createIfNotExists, clientTimeout);
    }

    internal virtual CloudBlobContainer GetCloudBlobContainerInternal(
      IVssRequestContext requestContext,
      string containerId,
      bool createIfNotExists,
      TimeSpan? clientTimeout = null)
    {
      Func<CloudBlobContainer> run = (Func<CloudBlobContainer>) (() =>
      {
        try
        {
          CloudBlobContainer containerReference = this.Client.GetContainerReference(containerId);
          if (createIfNotExists)
            this.CreateCloudBlobContainerIfNotExists(requestContext, containerReference, clientTimeout);
          return containerReference;
        }
        catch (Microsoft.Azure.Storage.StorageException ex)
        {
          this.FilterStorageExceptionForCircuitBreaker(ex);
          throw;
        }
      });
      return new CommandService<CloudBlobContainer>(requestContext, CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.GetCloudBlobContainer." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.GetCloudBlobContainerTimeout)), run).Execute();
    }

    public Stream GetStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (GetStream)))
      {
        Func<Stream> run = (Func<Stream>) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
            {
              Stream stream = cloudBlobReference.OpenRead(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.GetStreamClientTimeout));
              tracer.SetContentLength(stream.Length);
              return stream;
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.GetStream." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.GetStreamTimeout));
        return new CommandService<Stream>(requestContext, setter, run).Execute();
      }
    }

    internal abstract void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      CloudBlobContainer container,
      ICloudBlob blob,
      long length,
      int blockSizeBytes,
      TimeSpan executionCircuitBreakerTimeout,
      TimeSpan? clientTimeout = null);

    internal abstract void DownloadToStream(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream targetStream,
      CloudBlobContainer container,
      ICloudBlob blob,
      long length,
      int blockSizeBytes,
      TimeSpan executionCircuitBreakerTimeout,
      TimeSpan? clientTimeout = null);

    public void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      long length,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (DownloadToStream)))
      {
        CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
        ICloudBlob cloudBlobReference = this.GetCloudBlobReference(containerInternal, resourceId);
        this.DownloadToStream(requestContext, containerId, resourceId, targetStream, containerInternal, cloudBlobReference, length, 1048576, this.DownloadToStreamTimeout, clientTimeout);
        tracer.SetContentLength(length);
      }
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (DownloadToStream)))
      {
        CloudBlobContainer container;
        ICloudBlob blob;
        this.LoadBlobProperties(requestContext, containerId.ToString("n"), resourceId, clientTimeout, out container, out blob);
        this.DownloadToStream(requestContext, containerId, resourceId, targetStream, container, blob, blob.Properties.Length, 1048576, this.DownloadToStreamTimeout, clientTimeout);
        tracer.SetContentLength(blob.Properties.Length);
      }
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (DownloadToStream)))
      {
        CloudBlobContainer container;
        ICloudBlob blob;
        this.LoadBlobProperties(requestContext, containerId, resourceId, clientTimeout, out container, out blob);
        this.DownloadToStream(requestContext, containerId, resourceId, targetStream, container, blob, blob.Properties.Length, 1048576, this.DownloadToStreamTimeout, clientTimeout);
        tracer.SetContentLength(blob.Properties.Length);
      }
    }

    public void DownloadToStreamLargeBlocks(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream targetStream,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (DownloadToStreamLargeBlocks)))
      {
        CloudBlobContainer container;
        ICloudBlob blob;
        this.LoadBlobProperties(requestContext, containerId.ToString("n"), resourceId, clientTimeout, out container, out blob);
        long num = Math.Max(Math.Min(blob.Properties.Length, 10485760L) / 1048576L - 1L, 0L);
        TimeSpan executionCircuitBreakerTimeout = this.DownloadToStreamTimeout.Add(TimeSpan.FromSeconds((double) num));
        if (!clientTimeout.HasValue)
          clientTimeout = new TimeSpan?(TimeSpan.FromSeconds((double) ((long) this.m_settings.DownloadToStreamClientTimeout.TotalSeconds + 5L * num)));
        this.DownloadToStream(requestContext, containerId, resourceId, targetStream, container, blob, blob.Properties.Length, 10485760, executionCircuitBreakerTimeout, clientTimeout);
        tracer.SetContentLength(blob.Properties.Length);
      }
    }

    private void LoadBlobProperties(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout,
      out CloudBlobContainer container,
      out ICloudBlob blob)
    {
      CloudBlobContainer tempContainer = (CloudBlobContainer) null;
      ICloudBlob tempBlob = (ICloudBlob) null;
      Action run = (Action) (() =>
      {
        try
        {
          tempContainer = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
          tempBlob = this.GetCloudBlobReference(tempContainer, resourceId);
          tempBlob.FetchAttributes(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.ReadBlobPropertiesClientTimeout));
        }
        catch (Microsoft.Azure.Storage.StorageException ex)
        {
          this.FilterStorageExceptionForCircuitBreaker(ex);
          throw;
        }
      });
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.FetchAttributes." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.ReadBlobPropertiesTimeout));
      new CommandService(requestContext, setter, run).Execute();
      container = tempContainer;
      blob = tempBlob;
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      return this.EnumerateBlobsInternal(requestContext, containerId.ToString("n"), clientTimeout).Select<ICloudBlob, string>((Func<ICloudBlob, string>) (b => b.Name));
    }

    public IEnumerable<string> EnumerateBlobs(
      IVssRequestContext requestContext,
      string containerId,
      TimeSpan? clientTimeout = null)
    {
      return this.EnumerateBlobsInternal(requestContext, containerId, clientTimeout).Select<ICloudBlob, string>((Func<ICloudBlob, string>) (b => b.Name));
    }

    private IEnumerable<ICloudBlob> EnumerateBlobsInternal(
      IVssRequestContext requestContext,
      string containerId,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (EnumerateBlobsInternal)))
      {
        Func<IEnumerable<ICloudBlob>> run = (Func<IEnumerable<ICloudBlob>>) (() =>
        {
          try
          {
            CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
            {
              IEnumerable<ICloudBlob> cloudBlobs = containerInternal.ListBlobs(useFlatBlobListing: true, options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.EnumerateBlobsClientTimeout)).Select<IListBlobItem, ICloudBlob>((Func<IListBlobItem, ICloudBlob>) (b => (ICloudBlob) b));
              tracer.SetContentLength(0L);
              return cloudBlobs;
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.EnumerateBlobs." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.EnumerateBlobsTimeout));
        return new CommandService<IEnumerable<ICloudBlob>>(requestContext, setter, run).Execute();
      }
    }

    public virtual void PutStreamRaw(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout)
    {
      ICloudBlob cloudBlobReference = this.GetCloudBlobReference(container, resourceId);
      this.SetMetadata(cloudBlobReference, metadata);
      if (!clientTimeout.HasValue)
        cloudBlobReference.UploadFromStream(content);
      else
        cloudBlobReference.UploadFromStream(content, options: new BlobRequestOptions()
        {
          MaximumExecutionTime = clientTimeout
        });
    }

    public virtual void PutStreamRawUsingBlockBlobClient(
      CloudBlobContainer container,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      int? blockSize,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (!blockSize.HasValue)
        blockSize = new int?(1048576);
      BlockBlobClient blockBlobClient = SpecializedBlobExtensions.GetBlockBlobClient(this.BlobServiceClient.GetBlobContainerClient(container.Name), resourceId);
      Stream stream = content;
      BlobUploadOptions blobUploadOptions = new BlobUploadOptions();
      blobUploadOptions.Metadata = metadata;
      StorageTransferOptions storageTransferOptions = new StorageTransferOptions();
      ref StorageTransferOptions local1 = ref storageTransferOptions;
      int? nullable1 = blockSize;
      long? nullable2 = nullable1.HasValue ? new long?((long) nullable1.GetValueOrDefault()) : new long?();
      ((StorageTransferOptions) ref local1).InitialTransferSize = nullable2;
      ref StorageTransferOptions local2 = ref storageTransferOptions;
      nullable1 = blockSize;
      long? nullable3 = nullable1.HasValue ? new long?((long) (10 * nullable1.GetValueOrDefault())) : new long?();
      ((StorageTransferOptions) ref local2).MaximumTransferSize = nullable3;
      blobUploadOptions.TransferOptions = storageTransferOptions;
      CancellationToken cancellationToken1 = cancellationToken;
      blockBlobClient.Upload(stream, blobUploadOptions, cancellationToken1);
    }

    private bool ShouldTagBlobAfterUpload(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(FrameworkServerConstants.Migration_RollbackCopyTaggedTargetBlobsToSourceFeatureName) && this.m_tagNewAzureBlobProviderBaseBlobsPathSettings.TryGetValue(requestContext.ServiceHost.InstanceId, out string _);

    public virtual void PutStream(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.PutStream(requestContext, containerId.ToString("n"), resourceId, content, metadata, clientTimeout);
    }

    public virtual void PutStream(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      if (requestContext.ServiceHost.HostType.HasFlag((System.Enum) TeamFoundationHostType.Deployment))
        TeamFoundationTracingService.TraceRaw(15036, TraceLevel.Warning, "FileService", "BlobStorage", "Deployment context is writing to Container {0}. Deployment level servicehosts should not write perform writes using IAzureBlobProvider. Host Id: {1}", (object) containerId, (object) requestContext.ServiceHost.InstanceId);
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Upload, (long) this.m_settings.NotificationThreshold, nameof (PutStream)))
      {
        TeamFoundationTracingService.TraceRaw(15030, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage PutStream start", (object) containerId);
        try
        {
          long position = content.Position;
          if (content.Length <= 1048576L)
            this.PutStreamSingleChunk(requestContext, containerId, resourceId, content, position, metadata, clientTimeout);
          else
            this.PutStreamViaChunks(requestContext, containerId, resourceId, content, metadata, clientTimeout);
          tracer.SetContentLength(content.Length - position);
        }
        finally
        {
          TeamFoundationTracingService.TraceRaw(15031, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage PutStream end", (object) containerId);
        }
      }
    }

    private void WriteTagsAfterPut(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      try
      {
        TeamFoundationTracingService.TraceRaw(15032, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage ShouldTagBlobAfterUpload start", (object) containerId);
        if (!this.ShouldTagBlobAfterUpload(requestContext))
          return;
        TeamFoundationTracingService.TraceRaw(15033, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage ShouldTagBlobAfterUpload WriteBlobTags start", (object) containerId);
        IVssRequestContext requestContext1 = requestContext;
        string containerId1 = containerId;
        string resourceId1 = resourceId;
        Dictionary<string, string> tags = new Dictionary<string, string>();
        tags.Add("HostId", requestContext.ServiceHost.InstanceId.ToString("n"));
        tags.Add("HostType", requestContext.ServiceHost.HostType.ToString().Replace(",", ""));
        TimeSpan? clientTimeout1 = clientTimeout;
        this.WriteBlobTags(requestContext1, containerId1, resourceId1, (IDictionary<string, string>) tags, clientTimeout1);
        TeamFoundationTracingService.TraceRaw(15034, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage ShouldTagBlobAfterUpload WriteBlobTags end", (object) containerId);
      }
      finally
      {
        TeamFoundationTracingService.TraceRaw(15035, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage ShouldTagBlobAfterUpload end", (object) containerId);
      }
    }

    private void PutStreamSingleChunk(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream content,
      long position,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      ICloudBlob blob = (ICloudBlob) null;
      Action run = (Action) (() =>
      {
        try
        {
          CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
          blob = this.GetCloudBlobReference(containerInternal, resourceId);
          this.SetMetadata(blob, metadata);
          if (!content.CanSeek)
          {
            this.CreateCloudBlobContainerIfNotExists(requestContext, containerInternal, clientTimeout);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
              blob.UploadFromStream(content, options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
          }
          else
          {
            try
            {
              using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
                blob.UploadFromStream(content, options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
            }
            catch (Microsoft.Azure.Storage.StorageException ex)
            {
              if (!this.StorageExceptionIsContainerNotFound(ex))
              {
                throw;
              }
              else
              {
                content.Position = position;
                this.CreateCloudBlobContainerIfNotExists(requestContext, containerInternal, clientTimeout);
                using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
                  blob.UploadFromStream(content, options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
              }
            }
          }
          this.WriteTagsAfterPut(requestContext, containerId, resourceId, clientTimeout);
        }
        catch (Microsoft.Azure.Storage.StorageException ex)
        {
          this.FilterStorageExceptionForCircuitBreaker(ex);
          throw;
        }
      });
      new CommandService(requestContext, CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.PutStream." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.PutBlockTimeout)), run).Execute();
    }

    private void PutStreamViaChunks(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      Stream content,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      CloudBlobContainer container = (CloudBlobContainer) null;
      ICloudBlob blob = (ICloudBlob) null;
      byte[] contentBlock = new byte[1048576];
      int bytesread = 0;
      CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.PutStreamViaChunks." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.PutBlockTimeout));
      long offset = 0;
      long remainder = content.Length;
      while (remainder > 0L)
      {
        Action run = (Action) (() =>
        {
          try
          {
            if (container == null)
              container = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
            if (blob == null)
              blob = this.GetCloudBlobReference(container, resourceId);
            bytesread = content.Read(contentBlock, 0, (int) Math.Min(1048576L, remainder));
            remainder -= (long) bytesread;
            try
            {
              this.PutChunk(requestContext, container, blob, contentBlock, bytesread, content.Length, offset, remainder <= 0L, metadata, clientTimeout);
            }
            catch (Microsoft.Azure.Storage.StorageException ex)
            {
              if (!this.StorageExceptionIsContainerNotFound(ex))
              {
                throw;
              }
              else
              {
                this.CreateCloudBlobContainerIfNotExists(requestContext, container, clientTimeout);
                this.PutChunk(requestContext, container, blob, contentBlock, bytesread, content.Length, offset, remainder <= 0L, metadata, clientTimeout);
              }
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        new CommandService(requestContext, setter, run).Execute();
        offset += (long) bytesread;
      }
    }

    protected abstract void PutChunks(
      IVssRequestContext requestContext,
      CloudBlobContainer container,
      ICloudBlob blob,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null);

    public virtual void PutChunk(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Upload, (long) this.m_settings.NotificationThreshold, nameof (PutChunk)))
      {
        TeamFoundationTracingService.TraceRaw(15040, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage PutChunk start - Content Block length {1}, compressed length {2}", (object) containerId, (object) contentBlockLength, (object) compressedLength);
        try
        {
          Action run = (Action) (() =>
          {
            try
            {
              CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
              ICloudBlob cloudBlobReference = this.GetCloudBlobReference(containerInternal, resourceId);
              requestContext.Trace(15043, TraceLevel.Info, "FileService", "BlobStorage", "Blob refrence with name " + cloudBlobReference?.Name + " was created localy for resourceId " + resourceId);
              try
              {
                this.PutChunk(requestContext, containerInternal, cloudBlobReference, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, metadata, clientTimeout);
              }
              catch (Microsoft.Azure.Storage.StorageException ex)
              {
                if (!this.StorageExceptionIsContainerNotFound(ex))
                {
                  throw;
                }
                else
                {
                  this.CreateCloudBlobContainerIfNotExists(requestContext, containerInternal, clientTimeout);
                  this.PutChunk(requestContext, containerInternal, cloudBlobReference, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, metadata, clientTimeout);
                }
              }
            }
            catch (Microsoft.Azure.Storage.StorageException ex)
            {
              this.FilterStorageExceptionForCircuitBreaker(ex);
              throw;
            }
          });
          CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.PutChunk." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(TimeSpan.FromMilliseconds(Math.Max(this.PutBlockTimeout.TotalMilliseconds, (double) contentBlockLength * (this.PutBlockTimeout.TotalMilliseconds / 1048576.0)))));
          new CommandService(requestContext, setter, run).Execute();
          tracer.SetContentLength((long) contentBlockLength);
        }
        finally
        {
          TeamFoundationTracingService.TraceRaw(15041, TraceLevel.Info, "FileService", "BlobStorage", "Container {0} Blob Storage PutChunk end - Content Block length {1}, compressed length {2}", (object) containerId, (object) contentBlockLength, (object) compressedLength);
        }
      }
    }

    private void PutChunk(
      IVssRequestContext requestContext,
      CloudBlobContainer container,
      ICloudBlob blob,
      byte[] contentBlock,
      int contentBlockLength,
      long compressedLength,
      long offset,
      bool isLastChunk,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      this.SetMetadata(blob, metadata);
      if ((long) contentBlockLength == compressedLength)
      {
        using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
          blob.UploadFromStream((Stream) new MemoryStream(contentBlock, 0, contentBlockLength, false), options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.PutBlockClientTimeout));
      }
      else
        this.PutChunks(requestContext, container, blob, contentBlock, contentBlockLength, compressedLength, offset, isLastChunk, metadata, clientTimeout);
      this.WriteTagsAfterPut(requestContext, container.Name, blob.Name, clientTimeout);
    }

    protected abstract void StartCopy(
      ICloudBlob sourceBlob,
      ICloudBlob targetBlob,
      BlobRequestOptions requestOptions);

    public virtual void RenameBlob(
      IVssRequestContext requestContext,
      Guid containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      this.RenameBlob(requestContext, containerId.ToString("n"), sourceResourceId, targetResourceId, clientTimeout);
    }

    public virtual void RenameBlob(
      IVssRequestContext requestContext,
      string containerId,
      string sourceResourceId,
      string targetResourceId,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, "RenameBlob took {0} milliseconds.", AzureProviderBase.Tracer.Type.Rename, (long) this.m_settings.NotificationThreshold, nameof (RenameBlob)))
      {
        Action run = (Action) (() =>
        {
          try
          {
            CloudBlobContainer containerInternal = this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout);
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(containerInternal, sourceResourceId);
            ICloudBlob blockBlobReference = (ICloudBlob) containerInternal.GetBlockBlobReference(targetResourceId);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
            {
              this.StartCopy(cloudBlobReference, blockBlobReference, this.GetBlobRequestOptions(clientTimeout, this.m_settings.RenameBlobClientTimeout));
              cloudBlobReference.DeleteIfExists(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.DeleteBlobClientTimeout));
              tracer.SetContentLength(0L);
            }
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.RenameBlob." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.RenameBlobTimeout));
        new CommandService(requestContext, setter, run).Execute();
      }
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.BlobExists(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public bool BlobExists(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (BlobExists)))
      {
        Func<bool> run = (Func<bool>) (() =>
        {
          try
          {
            int num = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId).Exists(this.GetBlobRequestOptions(clientTimeout, this.m_settings.BlobExistsClientTimeout)) ? 1 : 0;
            tracer.SetContentLength(0L);
            return num != 0;
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.BlobExists." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.BlobExistsTimeout));
        return new CommandService<bool>(requestContext, setter, run).Execute();
      }
    }

    public bool ContainerExists(
      IVssRequestContext requestContext,
      Guid containerId,
      TimeSpan? clientTimeout = null)
    {
      try
      {
        return this.GetCloudBlobContainerInternal(requestContext, containerId.ToString("n"), false, clientTimeout).Exists(this.GetBlobRequestOptions(clientTimeout, this.m_settings.BlobExistsClientTimeout));
      }
      catch (Microsoft.Azure.Storage.StorageException ex)
      {
        this.LogContainerExistenceException(ex, containerId);
        throw;
      }
      catch (TaskCanceledException ex)
      {
        this.LogContainerExistenceException(ex, containerId);
        throw;
      }
    }

    public virtual void SetBlobHeaders(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      string cacheControl,
      string contentType,
      string contentDisposition,
      string contentEncoding,
      string contentLanguage,
      TimeSpan? clientTimeout = null)
    {
      this.SetBlobHeaders(requestContext, containerId.ToString("n"), resourceId, cacheControl, contentType, contentDisposition, contentEncoding, contentLanguage, clientTimeout);
    }

    public virtual void SetBlobHeaders(
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
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Upload, (long) this.m_settings.NotificationThreshold, nameof (SetBlobHeaders)))
      {
        Action run = (Action) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            if (cacheControl != null)
              cloudBlobReference.Properties.CacheControl = cacheControl;
            if (contentType != null)
              cloudBlobReference.Properties.ContentType = contentType;
            if (contentDisposition != null)
              cloudBlobReference.Properties.ContentDisposition = contentDisposition;
            if (contentEncoding != null)
              cloudBlobReference.Properties.ContentEncoding = contentEncoding;
            if (contentLanguage != null)
              cloudBlobReference.Properties.ContentLanguage = contentLanguage;
            cloudBlobReference.SetProperties(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.WriteBlobMetadataClientTimeout));
            tracer.SetContentLength(0L);
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.SetBlobHeaders." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.BlobExistsTimeout));
        new CommandService(requestContext, setter, run).Execute();
      }
    }

    public virtual void WriteBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      IDictionary<string, string> metadata,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Upload, (long) this.m_settings.NotificationThreshold, nameof (WriteBlobMetadata)))
      {
        Action run = (Action) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            this.SetMetadata(cloudBlobReference, metadata);
            using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
              cloudBlobReference.SetMetadata(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.WriteBlobMetadataClientTimeout));
            tracer.SetContentLength(0L);
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.WriteBlobMetadata." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.WriteBlobMetadataTimeout));
        new CommandService(requestContext, setter, run).Execute();
      }
    }

    private BlobClient GetBlobClient(string containerId, string resourceId)
    {
      string str = containerId;
      Guid result = Guid.Empty;
      if (Guid.TryParse(containerId, out result))
        str = result.ToString("n");
      return this.BlobServiceClient.GetBlobContainerClient(str).GetBlobClient(resourceId);
    }

    public virtual void WriteBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      IDictionary<string, string> tags,
      TimeSpan? clientTimeout = null)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Upload, (long) this.m_settings.NotificationThreshold, nameof (WriteBlobTags)))
      {
        TimeSpan writeBlobTagsTimeout = this.WriteBlobTagsTimeout;
        if (clientTimeout.HasValue)
          writeBlobTagsTimeout = clientTimeout.Value;
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderWrite.WriteBlobMetadata." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(writeBlobTagsTimeout));
        new CommandService(requestContext, setter, new Action(Run)).Execute();

        void Run()
        {
          try
          {
            ((BlobBaseClient) this.GetBlobClient(containerId, resourceId)).SetTags(tags, (BlobRequestConditions) null, new CancellationToken());
            tracer.SetContentLength(0L);
          }
          catch (RequestFailedException ex)
          {
            this.FilterRequestFailedExceptionForCircuitBreaker(ex);
            throw;
          }
        }
      }
    }

    public virtual IDictionary<string, string> ReadBlobTags(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId)
    {
      requestContext.CheckWriteAccess();
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (ReadBlobTags)))
      {
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.ReadBlobTags." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.FetchAttributesTimeout));
        return new CommandService<IDictionary<string, string>>(requestContext, setter, new Func<IDictionary<string, string>>(Run)).Execute();

        IDictionary<string, string> Run()
        {
          try
          {
            Response<GetBlobTagResult> tags = ((BlobBaseClient) this.GetBlobClient(containerId, resourceId)).GetTags((BlobRequestConditions) null, new CancellationToken());
            tracer.SetContentLength(0L);
            return ((NullableResponse<GetBlobTagResult>) tags).Value.Tags;
          }
          catch (RequestFailedException ex)
          {
            this.FilterRequestFailedExceptionForCircuitBreaker(ex);
            throw;
          }
        }
      }
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      return this.ReadBlobMetadata(requestContext, containerId.ToString("n"), resourceId, clientTimeout);
    }

    public IDictionary<string, string> ReadBlobMetadata(
      IVssRequestContext requestContext,
      string containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (ReadBlobMetadata)))
      {
        Func<IDictionary<string, string>> run = (Func<IDictionary<string, string>>) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            cloudBlobReference.FetchAttributes(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.FetchAttributesClientTimeout));
            IDictionary<string, string> metadata = cloudBlobReference.Metadata;
            tracer.SetContentLength(0L);
            return metadata;
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.ReadBlobMetadata." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.FetchAttributesTimeout));
        return new CommandService<IDictionary<string, string>>(requestContext, setter, run).Execute();
      }
    }

    public Microsoft.TeamFoundation.Framework.Server.BlobProperties ReadBlobProperties(
      IVssRequestContext requestContext,
      Guid containerId,
      string resourceId,
      TimeSpan? clientTimeout = null)
    {
      using (AzureProviderBase.Tracer tracer = new AzureProviderBase.Tracer(requestContext, this.Client.Credentials.AccountName, AzureProviderBase.Tracer.Type.Download, (long) this.m_settings.NotificationThreshold, nameof (ReadBlobProperties)))
      {
        Func<Microsoft.TeamFoundation.Framework.Server.BlobProperties> run = (Func<Microsoft.TeamFoundation.Framework.Server.BlobProperties>) (() =>
        {
          try
          {
            ICloudBlob cloudBlobReference = this.GetCloudBlobReference(this.GetCloudBlobContainerInternal(requestContext, containerId, false, clientTimeout), resourceId);
            cloudBlobReference.FetchAttributes(options: this.GetBlobRequestOptions(clientTimeout, this.m_settings.FetchAttributesClientTimeout));
            Microsoft.Azure.Storage.Blob.BlobProperties properties = cloudBlobReference.Properties;
            tracer.SetContentLength(0L);
            return new Microsoft.TeamFoundation.Framework.Server.BlobProperties(cloudBlobReference.Name, properties.LastModified, properties.Length);
          }
          catch (Microsoft.Azure.Storage.StorageException ex)
          {
            this.FilterStorageExceptionForCircuitBreaker(ex);
            throw;
          }
        });
        CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) ("AzureBlobProviderRead.ReadBlobProperties." + this.Client.Credentials.AccountName)).AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(this.FetchAttributesTimeout));
        return new CommandService<Microsoft.TeamFoundation.Framework.Server.BlobProperties>(requestContext, setter, run).Execute();
      }
    }

    protected BlobRequestOptions GetBlobRequestOptions(
      TimeSpan? argTimeout,
      TimeSpan settingsTimeout)
    {
      return new BlobRequestOptions()
      {
        MaximumExecutionTime = new TimeSpan?(argTimeout ?? settingsTimeout)
      };
    }

    protected TableRequestOptions GetTableRequestOptions(
      TimeSpan? argTimeout,
      TimeSpan settingsTimeout)
    {
      return new TableRequestOptions()
      {
        MaximumExecutionTime = new TimeSpan?(argTimeout ?? settingsTimeout)
      };
    }

    protected internal virtual bool CreateCloudBlobContainerIfNotExists(
      IVssRequestContext requestContext,
      CloudBlobContainer container,
      TimeSpan? clientTimeout)
    {
      BlobRequestOptions blobRequestOptions = this.GetBlobRequestOptions(clientTimeout, this.m_settings.GetCloudBlobContainerClientTimeout);
      blobRequestOptions.RetryPolicy = (Microsoft.Azure.Storage.RetryPolicies.IRetryPolicy) new CreateContainerExponentialRetryPolicy();
      using (requestContext.AcquireConnectionLock(ConnectionLockNameType.BlobStorage))
        return container.CreateIfNotExists(blobRequestOptions);
    }

    public void UpdateConnectionString(
      IVssRequestContext requestContext,
      string storageAccountConnectionString)
    {
      if (this.TableClient == null)
        throw new InvalidOperationException("UpdateConnectionString can only be used when a table endpoint is set");
      Microsoft.Azure.Cosmos.Table.CloudStorageAccount cloudStorageAccount = Microsoft.Azure.Cosmos.Table.CloudStorageAccount.Parse(storageAccountConnectionString);
      this.Client.Credentials.UpdateKey(cloudStorageAccount.Credentials.Key);
      this.TableClient.Credentials.UpdateKey(cloudStorageAccount.Credentials.Key);
    }

    public void DisableBufferManager() => this.Client.BufferManager = (IBufferManager) null;

    private bool StorageExceptionIsContainerNotFound(Microsoft.Azure.Storage.StorageException se) => se.RequestInformation != null && se.RequestInformation.ExtendedErrorInformation != null && StorageErrorCodeStrings.ContainerNotFound.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal) || se.RequestInformation != null && se.RequestInformation.HttpStatusCode == 404;

    protected bool FilterRequestFailedExceptionForCircuitBreaker(RequestFailedException rfe)
    {
      if (rfe != null)
      {
        switch (rfe.Status)
        {
          case 403:
          case 408:
          case 500:
          case 502:
          case 503:
          case 504:
            return true;
          default:
            if (rfe.Status == 0 && ((Exception) rfe).InnerException != null)
            {
              if (!(((Exception) rfe).InnerException is WebException innerException))
                innerException = ((Exception) rfe).InnerException?.InnerException as WebException;
              WebException webException = innerException;
              if (webException != null)
              {
                switch (webException.Status)
                {
                  case WebExceptionStatus.NameResolutionFailure:
                  case WebExceptionStatus.ConnectFailure:
                    return true;
                }
              }
              else
              {
                TeamFoundationTracingService.TraceExceptionRaw(15042, "FileService", "BlobStorage", (Exception) rfe);
                return true;
              }
            }
            if (((Exception) rfe).GetType().FullName == "System.Net.InternalException")
              return true;
            ((Exception) rfe).Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
            break;
        }
      }
      return false;
    }

    protected bool FilterStorageExceptionForCircuitBreaker(Microsoft.Azure.Storage.StorageException se)
    {
      if (se.RequestInformation != null)
      {
        switch (se.RequestInformation.HttpStatusCode)
        {
          case 403:
          case 408:
          case 500:
          case 502:
          case 503:
          case 504:
            return true;
          default:
            if (se.RequestInformation.HttpStatusCode == 0 && se.InnerException != null)
            {
              if (!(se.InnerException is WebException innerException))
                innerException = se.InnerException?.InnerException as WebException;
              WebException webException = innerException;
              if (webException != null)
              {
                switch (webException.Status)
                {
                  case WebExceptionStatus.NameResolutionFailure:
                  case WebExceptionStatus.ConnectFailure:
                    return true;
                }
              }
              else
              {
                TeamFoundationTracingService.TraceExceptionRaw(15042, "FileService", "BlobStorage", (Exception) se);
                return true;
              }
            }
            if (se.RequestInformation.ExtendedErrorInformation != null && (StorageErrorCodeStrings.InternalError.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal) || StorageErrorCodeStrings.ServerBusy.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal) || StorageErrorCodeStrings.OperationTimedOut.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal)))
              return true;
            break;
        }
      }
      if (se.GetType().FullName == "System.Net.InternalException")
        return true;
      se.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
      return false;
    }

    protected void LogContainerExistenceException(Microsoft.Azure.Storage.StorageException se, Guid containerId)
    {
      TeamFoundationTracingService.TraceRaw(15017, TraceLevel.Error, "FileService", "BlobStorage", "Container existence check exception. Storage exception. ContainerId {0}", (object) containerId);
      if (se.RequestInformation == null)
        return;
      if (se.RequestInformation.HttpStatusCode != 0)
      {
        TeamFoundationTracingService.TraceRaw(15019, TraceLevel.Error, "FileService", "BlobStorage", string.Format("Received code: {0}", (object) se.RequestInformation.HttpStatusCode));
      }
      else
      {
        if (se.RequestInformation.HttpStatusCode != 0 || se.InnerException == null)
          return;
        if (!(se.InnerException is WebException innerException))
          innerException = se.InnerException?.InnerException as WebException;
        WebException webException = innerException;
        if (webException != null)
          TeamFoundationTracingService.TraceRaw(15026, TraceLevel.Error, "FileService", "BlobStorage", string.Format("Web exception: {0}", (object) webException.Status));
        else
          TeamFoundationTracingService.TraceExceptionRaw(15024, "FileService", "BlobStorage", (Exception) se);
      }
    }

    protected void LogContainerExistenceException(TaskCanceledException tce, Guid containerId)
    {
      if (tce.InnerException is Microsoft.Azure.Storage.StorageException)
        this.LogContainerExistenceException(tce.InnerException as Microsoft.Azure.Storage.StorageException, containerId);
      else
        TeamFoundationTracingService.TraceRaw(150008, TraceLevel.Error, "FileService", "BlobStorage", "Container existence check exception. Task Canceled. ContainerId {0}", (object) containerId);
    }

    protected bool FilterStorageExceptionForCircuitBreaker(Microsoft.Azure.Cosmos.Table.StorageException se)
    {
      if (se.RequestInformation != null)
      {
        switch (se.RequestInformation.HttpStatusCode)
        {
          case 403:
          case 408:
          case 500:
          case 502:
          case 503:
          case 504:
            return true;
          default:
            if (se.RequestInformation.HttpStatusCode == 0 && se.InnerException != null)
            {
              if (!(se.InnerException is WebException innerException))
                innerException = se.InnerException?.InnerException as WebException;
              WebException webException = innerException;
              if (webException != null)
              {
                switch (webException.Status)
                {
                  case WebExceptionStatus.NameResolutionFailure:
                  case WebExceptionStatus.ConnectFailure:
                    return true;
                }
              }
              else
              {
                TeamFoundationTracingService.TraceExceptionRaw(15042, "FileService", "BlobStorage", (Exception) se);
                return true;
              }
            }
            if (se.RequestInformation.ExtendedErrorInformation != null && (StorageErrorCodeStrings.InternalError.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal) || StorageErrorCodeStrings.ServerBusy.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal) || StorageErrorCodeStrings.OperationTimedOut.Equals(se.RequestInformation.ExtendedErrorInformation.ErrorCode, StringComparison.Ordinal)))
              return true;
            break;
        }
      }
      if (se.GetType().FullName == "System.Net.InternalException")
        return true;
      se.Data.Add((object) "{421AC3F1-A306-4C9B-B3F6-5812F9121FC8}", (object) true);
      return false;
    }

    private void SetMetadata(ICloudBlob blob, IDictionary<string, string> metadata)
    {
      if (metadata == null || blob == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) metadata)
        blob.Metadata[keyValuePair.Key] = keyValuePair.Value;
    }

    private static void ConfigureServicePoint(
      IVssRequestContext requestContext,
      Microsoft.Azure.Storage.CloudStorageAccount account)
    {
      if (!requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.AzureStorage.OptimizeBlobServicePoint"))
        return;
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(account.BlobEndpoint);
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.Expect100Continue = false;
    }

    internal CloudBlobClient Client { get; set; }

    internal BlobServiceClient BlobServiceClient { get; set; }

    private CloudTableClient TableClient { get; set; }

    protected struct Tracer : IDisposable
    {
      private IVssRequestContext m_requestContext;
      private PerformanceTimer m_perfTimer;
      private bool m_isDisposed;
      private string m_methodName;
      private string m_accountName;
      private AzureProviderBase.Tracer.Type m_type;
      private long m_contentLength;
      private long m_notificationThreshold;

      public Tracer(
        IVssRequestContext requestContext,
        string accountName,
        AzureProviderBase.Tracer.Type type,
        long notificationThreshold,
        [CallerMemberName] string methodName = null)
      {
        this.m_methodName = methodName;
        this.m_accountName = accountName;
        this.m_type = type;
        this.m_requestContext = requestContext;
        VssPerformanceEventSource.Log.WindowsAzureStorageStart(requestContext.UniqueIdentifier, accountName, methodName);
        this.m_contentLength = -1L;
        this.m_isDisposed = false;
        this.m_perfTimer = PerformanceTimer.StartMeasure(requestContext, "BlobStorage");
        this.m_notificationThreshold = notificationThreshold;
        this.TraceStartBlobOperation();
      }

      private void TraceStartBlobOperation()
      {
        switch (this.m_type)
        {
          case AzureProviderBase.Tracer.Type.Upload:
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsExecuting").Increment();
            break;
          case AzureProviderBase.Tracer.Type.Download:
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsExecuting").Increment();
            break;
          case AzureProviderBase.Tracer.Type.Delete:
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesExecuting").Increment();
            break;
          case AzureProviderBase.Tracer.Type.Rename:
            VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesExecuting").Increment();
            break;
          default:
            throw new ArgumentException("The type provided to the Tracer is not supported");
        }
      }

      private void TraceEndBlobOperation(long contentLength)
      {
        switch (this.m_type)
        {
          case AzureProviderBase.Tracer.Type.Upload:
            VssPerformanceCounter performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsExecuting");
            performanceCounter1.Decrement();
            if (contentLength >= 0L)
            {
              performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadedBytesPerSec");
              performanceCounter1.IncrementBy(contentLength);
              performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsPerSec");
              performanceCounter1.Increment();
              break;
            }
            performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsFailed");
            performanceCounter1.Increment();
            performanceCounter1 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageUploadsFailedPerSec");
            performanceCounter1.Increment();
            break;
          case AzureProviderBase.Tracer.Type.Download:
            VssPerformanceCounter performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsExecuting");
            performanceCounter2.Decrement();
            if (contentLength >= 0L)
            {
              performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadedBytesPerSec");
              performanceCounter2.IncrementBy(contentLength);
              performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsPerSec");
              performanceCounter2.Increment();
              break;
            }
            performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsFailed");
            performanceCounter2.Increment();
            performanceCounter2 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDownloadsFailedPerSec");
            performanceCounter2.Increment();
            break;
          case AzureProviderBase.Tracer.Type.Delete:
            VssPerformanceCounter performanceCounter3 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesExecuting");
            performanceCounter3.Decrement();
            if (contentLength >= 0L)
            {
              performanceCounter3 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesPerSec");
              performanceCounter3.IncrementBy(contentLength);
              break;
            }
            performanceCounter3 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesFailed");
            performanceCounter3.Increment();
            performanceCounter3 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageDeletesFailedPerSec");
            performanceCounter3.Increment();
            break;
          case AzureProviderBase.Tracer.Type.Rename:
            VssPerformanceCounter performanceCounter4 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesExecuting");
            performanceCounter4.Decrement();
            if (contentLength >= 0L)
            {
              performanceCounter4 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesPerSec");
              performanceCounter4.Increment();
              break;
            }
            performanceCounter4 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesFailed");
            performanceCounter4.Increment();
            performanceCounter4 = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_AzureStorageRenamesFailedPerSec");
            performanceCounter4.Increment();
            break;
          default:
            throw new ArgumentException("The type provided to the Tracer is not supported");
        }
      }

      public void Dispose()
      {
        if (this.m_isDisposed)
          return;
        this.m_perfTimer.End();
        long duration = this.m_perfTimer.Duration / 10000L;
        VssPerformanceEventSource.Log.WindowsAzureStorageStop(this.m_requestContext.UniqueIdentifier, this.m_accountName, this.m_methodName, duration);
        this.TraceTimes(this.m_requestContext, this.m_methodName, duration);
        this.TraceEndBlobOperation(this.m_contentLength);
        this.m_perfTimer.Dispose();
        this.m_isDisposed = true;
      }

      private void TraceTimes(IVssRequestContext requestContext, string methodName, long duration)
      {
        if (duration > this.m_notificationThreshold)
          TeamFoundationTracingService.TraceRaw(1013181, TraceLevel.Warning, "FileService", "BlobStorage", "{0} took {1} ms", (object) methodName, (object) duration);
        else
          TeamFoundationTracingService.TraceRaw(1013182, TraceLevel.Info, "FileService", "BlobStorage", "{0} took {1} ms", (object) methodName, (object) duration);
      }

      public void SetContentLength(long length) => this.m_contentLength = length;

      public enum Type
      {
        Upload,
        Download,
        Delete,
        Rename,
      }
    }
  }
}
