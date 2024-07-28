// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ServiceBus.ExternalServiceBusPublisherService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.ServiceBus
{
  public class ExternalServiceBusPublisherService : 
    VssBaseService,
    IExternalMessageBusPublisherService,
    IVssFrameworkService
  {
    private readonly int MaxPublishSize = 262144;
    private static readonly byte[] HaskKeyBytes = Encoding.UTF8.GetBytes("ExternalPublisherInternalCacheHashKey");
    private const string s_Area = "ExternalServiceBus";
    private const string s_Layer = "ExternalServiceBusPublisherService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool Publish(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName,
      object serializableObject)
    {
      if (!(serializableObject is MessageBusMessage messageBusMessage))
        messageBusMessage = new MessageBusMessage(serializableObject);
      MessageBusMessage message = messageBusMessage;
      return this.Publish(requestContext, credentials, messageBusName, message);
    }

    public Task<bool> PublishAsync(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName,
      object serializableObject)
    {
      requestContext.AssertAsyncExecutionEnabled();
      if (!(serializableObject is MessageBusMessage messageBusMessage))
        messageBusMessage = new MessageBusMessage(serializableObject);
      MessageBusMessage message = messageBusMessage;
      return this.PublishAsync(requestContext, credentials, messageBusName, message);
    }

    internal bool Publish(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName,
      MessageBusMessage message)
    {
      return requestContext.RunSynchronously<bool>((Func<Task<bool>>) (() => this.PublishAsync(requestContext, credentials, messageBusName, message)));
    }

    internal async Task<bool> PublishAsync(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName,
      MessageBusMessage message)
    {
      requestContext.TraceEnter(1005150, "ExternalServiceBus", nameof (ExternalServiceBusPublisherService), nameof (PublishAsync));
      bool flag;
      try
      {
        bool? success = new bool?();
        Stopwatch watch = Stopwatch.StartNew();
        try
        {
          string messageBusIdentifier = "External." + ExternalServiceBusPublisherService.GetConnectionKey(credentials, messageBusName) + "." + messageBusName;
          IServiceBusPublishConnection connection;
          if (this.TryGetOrCreateConnection(requestContext, credentials, messageBusName, out connection))
          {
            await connection.PublishAsync(requestContext, messageBusIdentifier, new MessageBusMessage[1]
            {
              message
            });
            success = new bool?(true);
          }
          else
            requestContext.Trace(1005153, TraceLevel.Error, "ExternalServiceBus", nameof (ExternalServiceBusPublisherService), "Failed to publish. Unable to GetOrCreate connection from cache for key " + messageBusIdentifier);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(1005153, "ExternalServiceBus", nameof (ExternalServiceBusPublisherService), ex);
          success = new bool?(false);
          throw;
        }
        finally
        {
          watch.Stop();
          if (success.HasValue)
          {
            VssPerformanceCounter performanceCounter;
            if (success.Value)
            {
              performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesTotal", messageBusName);
              performanceCounter.IncrementBy(1L);
              performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusPublishedMessagesPerSec", messageBusName);
              performanceCounter.IncrementBy(1L);
            }
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTime", messageBusName);
            performanceCounter.IncrementMilliseconds(watch.ElapsedMilliseconds);
            performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_ServiceBusAveragePublishTimeBase", messageBusName);
            performanceCounter.IncrementBy(1L);
          }
        }
        flag = success.HasValue && success.Value;
      }
      finally
      {
        requestContext.TraceLeave(1005170, "ExternalServiceBus", nameof (ExternalServiceBusPublisherService), nameof (PublishAsync));
      }
      return flag;
    }

    internal bool TryGetOrCreateConnection(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName,
      out IServiceBusPublishConnection connection)
    {
      ExternalServiceBusCacheService service = requestContext.GetService<ExternalServiceBusCacheService>();
      IServiceBusPublishConnection publishConnection1 = (IServiceBusPublishConnection) null;
      string connectionKey = ExternalServiceBusPublisherService.GetConnectionKey(credentials, messageBusName);
      if (!service.TryGetValue(requestContext, connectionKey, out publishConnection1))
      {
        IServiceBusPublishConnection publishConnection2 = (IServiceBusPublishConnection) this.CreateServiceBusPublishConnection(requestContext, credentials, messageBusName);
        if (service.TryAdd(requestContext, connectionKey, publishConnection2))
        {
          publishConnection1 = publishConnection2;
        }
        else
        {
          publishConnection2.Dispose();
          if (!service.TryGetValue(requestContext, connectionKey, out publishConnection1))
          {
            connection = (IServiceBusPublishConnection) null;
            return false;
          }
        }
      }
      connection = publishConnection1;
      return true;
    }

    private ServiceBusPublishConnection CreateServiceBusPublishConnection(
      IVssRequestContext requestContext,
      ExternalMessageBusCredentials credentials,
      string messageBusName)
    {
      switch (credentials.CredentialType)
      {
        case ExternalMessageBusCredentialTypes.ConnectionString:
          ServiceBusConnectionStringCredentials stringCredentials = (ServiceBusConnectionStringCredentials) credentials;
          return new ServiceBusPublishConnection(requestContext, stringCredentials.ConnectionString, messageBusName, this.MaxPublishSize);
        case ExternalMessageBusCredentialTypes.SharedAccessSignatureToken:
          SharedAccessSignatureTokenCredentials tokenCredentials = (SharedAccessSignatureTokenCredentials) credentials;
          return new ServiceBusPublishConnection(requestContext, tokenCredentials.Namespace, tokenCredentials.SharedAccessSignatureToken, messageBusName, this.MaxPublishSize);
        default:
          throw new NotSupportedException();
      }
    }

    private static string GetConnectionKey(
      ExternalMessageBusCredentials credentials,
      string messageBusName)
    {
      switch (credentials.CredentialType)
      {
        case ExternalMessageBusCredentialTypes.ConnectionString:
          return ExternalServiceBusPublisherService.Hash256(((ServiceBusConnectionStringCredentials) credentials).ConnectionString + "_" + messageBusName);
        case ExternalMessageBusCredentialTypes.SharedAccessSignatureToken:
          SharedAccessSignatureTokenCredentials tokenCredentials = (SharedAccessSignatureTokenCredentials) credentials;
          return ExternalServiceBusPublisherService.Hash256(tokenCredentials.Namespace + "_" + tokenCredentials.SharedAccessSignatureToken + "_" + messageBusName);
        default:
          throw new NotSupportedException();
      }
    }

    private static string Hash256(string text)
    {
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(text, ExternalServiceBusPublisherService.HaskKeyBytes))
        return hmacshA256Hash.HashBase32Encoded.TrimEnd('=').ToLower();
    }
  }
}
