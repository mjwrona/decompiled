// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.StorageQueueEnqueueActionTask
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public sealed class StorageQueueEnqueueActionTask : ActionTask
  {
    private const string c_storageConnectionStringFormat = "DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}";
    private bool m_useHttps;
    private int m_messageTtl;
    private string m_queueName;
    private CloudQueue m_queue;
    private string m_accountKey;
    private string m_accountName;
    private string m_messageContent;
    private int m_visibilityTimeout;
    private string m_overriddenStorageConnectionString;

    public StorageQueueEnqueueActionTask(
      string messageContent,
      string accountName,
      string accountKey,
      string queueName,
      bool useHttps,
      int messageTtl,
      int visibilityTimeout,
      string overrideStorageConnectionString = null)
    {
      this.m_messageContent = messageContent;
      this.m_accountName = accountName;
      this.m_accountKey = accountKey;
      this.m_queueName = queueName;
      this.m_useHttps = useHttps;
      this.m_messageTtl = messageTtl;
      this.m_visibilityTimeout = visibilityTimeout;
      this.m_overriddenStorageConnectionString = overrideStorageConnectionString;
    }

    public override async Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      StorageQueueEnqueueActionTask enqueueActionTask = this;
      enqueueActionTask.InitializeQueue();
      Stopwatch sw = new Stopwatch();
      enqueueActionTask.UpdateNotificationForRequest(requestContext, enqueueActionTask.m_messageContent);
      try
      {
        CloudQueueMessage message = new CloudQueueMessage(enqueueActionTask.m_messageContent);
        sw.Start();
        try
        {
          await enqueueActionTask.m_queue.AddMessageAsync(message, new TimeSpan?(TimeSpan.FromSeconds((double) enqueueActionTask.m_messageTtl)), new TimeSpan?(TimeSpan.FromSeconds((double) enqueueActionTask.m_visibilityTimeout)), (QueueRequestOptions) null, (OperationContext) null);
        }
        finally
        {
          sw.Stop();
        }
        enqueueActionTask.UpdateNotificationForResponse(requestContext, AzureConsumerResources.Response_OK, new double?(sw.Elapsed.TotalSeconds));
        return new ActionTaskResult(ActionTaskResultLevel.Success);
      }
      catch (StorageException ex)
      {
        RequestResult requestInformation = ex.RequestInformation;
        if (requestInformation != null)
        {
          string errorMessage = string.Format("{0} ({1})", (object) requestInformation.HttpStatusMessage, (object) requestInformation.HttpStatusCode);
          enqueueActionTask.UpdateNotificationForResponse(requestContext, "", new double?(sw.Elapsed.TotalSeconds), errorMessage, ex.ToString());
          return new ActionTaskResult(ActionTaskResultLevel.TransientError, (Exception) ex, errorMessage);
        }
        throw;
      }
      catch (Exception ex)
      {
        enqueueActionTask.UpdateNotificationForResponse(requestContext, AzureConsumerResources.Response_Error, new double?(sw.Elapsed.TotalSeconds), ex.Message, ex.ToString());
        return new ActionTaskResult(ActionTaskResultLevel.TransientError, ex, ex.Message);
      }
    }

    private void InitializeQueue()
    {
      this.m_queue = CloudStorageAccount.Parse(this.m_overriddenStorageConnectionString ?? string.Format("DefaultEndpointsProtocol={0};AccountName={1};AccountKey={2}", this.m_useHttps ? (object) Uri.UriSchemeHttps : (object) Uri.UriSchemeHttp, (object) this.m_accountName, (object) this.m_accountKey)).CreateCloudQueueClient().GetQueueReference(this.m_queueName.ToLower());
      this.m_queue.CreateIfNotExists();
    }
  }
}
