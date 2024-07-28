// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.Storage.Queue.Protocol;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Hosting;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyService : IAzureBlobGeoRedundancyService, IVssFrameworkService
  {
    private AzureBlobGeoRedundancyServiceSettings m_settings;
    private static int s_counter;
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), AzureBlobGeoRedundancyServiceSettings.RegistryFilter);
      Interlocked.CompareExchange<AzureBlobGeoRedundancyServiceSettings>(ref this.m_settings, new AzureBlobGeoRedundancyServiceSettings(systemRequestContext), (AzureBlobGeoRedundancyServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public AzureBlobGeoRedundancyServiceSettings Settings => this.m_settings;

    public bool IsEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy");

    public int GetNumberOfQueues(IVssRequestContext requestContext) => this.m_settings.NumberOfQueues;

    public bool AreSynchronousWritesEnabled(IVssRequestContext requestContext) => this.m_settings.SynchronousWritesEnabled;

    public BlobGeoRedundancyEndpoint SetupEndpoint(
      IVssRequestContext requestContext,
      CloudStorageAccount storageAccount)
    {
      CloudQueueClient cloudQueueClient = storageAccount.CreateCloudQueueClient();
      AzureProviderSettingsService service = requestContext.GetService<AzureProviderSettingsService>();
      cloudQueueClient.DefaultRequestOptions.MaximumExecutionTime = new TimeSpan?(service.Settings.DefaultQueueRequestClientTimeout);
      return new BlobGeoRedundancyEndpoint()
      {
        StorageAccount = storageAccount,
        QueueClient = cloudQueueClient,
        Settings = this.m_settings
      };
    }

    public virtual void CreateBlob(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      string blobName)
    {
      BlobActionMessage blobActionMessage = new BlobActionMessage()
      {
        Action = BlobAction.CreateBlob,
        ContainerName = containerName,
        BlobName = blobName
      };
      AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
      AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, this.m_settings, new TraceMethod(((VssRequestContextExtensions) requestContext).Trace));
    }

    public static void CreateBlobRaw(
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      string blobName)
    {
      BlobActionMessage blobActionMessage = new BlobActionMessage()
      {
        Action = BlobAction.CreateBlob,
        ContainerName = containerName,
        BlobName = blobName
      };
      AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, endpoint.Settings, AzureBlobGeoRedundancyService.\u003C\u003EO.\u003C0\u003E__TraceRaw ?? (AzureBlobGeoRedundancyService.\u003C\u003EO.\u003C0\u003E__TraceRaw = new TraceMethod(TeamFoundationTracingService.TraceRaw)));
    }

    public virtual void DeleteBlob(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName,
      string blobName)
    {
      BlobActionMessage blobActionMessage = new BlobActionMessage()
      {
        Action = BlobAction.DeleteBlob,
        ContainerName = containerName,
        BlobName = blobName
      };
      AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
      AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, this.m_settings, new TraceMethod(((VssRequestContextExtensions) requestContext).Trace));
    }

    public virtual void CreateContainer(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName)
    {
      BlobActionMessage blobActionMessage = new BlobActionMessage()
      {
        Action = BlobAction.CreateContainer,
        ContainerName = containerName
      };
      AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
      AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, this.m_settings, new TraceMethod(((VssRequestContextExtensions) requestContext).Trace));
    }

    public virtual void DeleteContainer(
      IVssRequestContext requestContext,
      BlobGeoRedundancyEndpoint endpoint,
      string containerName)
    {
      BlobActionMessage blobActionMessage = new BlobActionMessage()
      {
        Action = BlobAction.DeleteContainer,
        ContainerName = containerName
      };
      AzureBlobGeoRedundancyUtils.CheckValidBlobActionMessageValues(blobActionMessage);
      AzureBlobGeoRedundancyService.EnqueueMessage(endpoint.QueueClient, blobActionMessage, this.m_settings, new TraceMethod(((VssRequestContextExtensions) requestContext).Trace));
    }

    protected internal static void EnqueueMessage(
      CloudQueueClient queueClient,
      BlobActionMessage message,
      AzureBlobGeoRedundancyServiceSettings settings,
      TraceMethod traceMethod)
    {
      string serializedMessage = message.ToJson();
      Guid messageId = Guid.NewGuid();
      new RetryManager(settings.NumberOfRetries, settings.RetryDelay, (Action<Exception>) (e => traceMethod(15300001, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyService), "EnqueueMessage Error. MessageId: {0}, Exception: {1}", new object[2]
      {
        (object) messageId,
        (object) e
      }))).Invoke((Action) (() =>
      {
        int index = Interlocked.Increment(ref AzureBlobGeoRedundancyService.s_counter) % settings.NumberOfQueues;
        if (index < 0)
          index += settings.NumberOfQueues;
        string queueName = AzureBlobGeoRedundancyUtils.GetQueueName(index);
        traceMethod(15300000, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyService), "EnqueueMessage. MessageId: {0}, Queue: {1}, Message: {2}", new object[3]
        {
          (object) messageId,
          (object) queueName,
          (object) serializedMessage
        });
        AzureBlobGeoRedundancyService.ExecuteActionOnQueue(queueClient.GetQueueReference(queueName), (Action<CloudQueue>) (q => q.AddMessage(new CloudQueueMessage(serializedMessage))));
      }));
    }

    protected static void ExecuteActionOnQueue(CloudQueue queue, Action<CloudQueue> action)
    {
      try
      {
        action(queue);
      }
      catch (StorageException ex) when (AzureBlobGeoRedundancyService.IsQueueNotFound(ex))
      {
        queue.CreateIfNotExists();
        action(queue);
      }
    }

    protected static bool IsQueueNotFound(StorageException e)
    {
      RequestResult requestInformation = e.RequestInformation;
      return (requestInformation != null ? (requestInformation.HttpStatusCode == 404 ? 1 : 0) : 0) != 0 && e.RequestInformation?.ExtendedErrorInformation?.ErrorCode == QueueErrorCodeStrings.QueueNotFound;
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection chanedEntries)
    {
      requestContext.TraceAlways(15300002, TraceLevel.Info, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyService), "Registry settings changed!");
      Volatile.Write<AzureBlobGeoRedundancyServiceSettings>(ref this.m_settings, new AzureBlobGeoRedundancyServiceSettings(requestContext));
    }
  }
}
