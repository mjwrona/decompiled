// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure.ServiceBusActionTaskBase
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.ServiceBus.Messaging;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.Common;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Azure
{
  public abstract class ServiceBusActionTaskBase : ActionTask
  {
    public string ServiceBusConnectionString { get; private set; }

    public string MessageContent { get; private set; }

    protected ServiceBusActionTaskBase(string serviceBusConnectionString, string messageContent)
    {
      this.ServiceBusConnectionString = serviceBusConnectionString;
      this.MessageContent = messageContent;
    }

    protected virtual void InitializeAction(TimeSpan timeout)
    {
    }

    protected virtual void CleanupAction()
    {
    }

    protected virtual bool TryCreateEndpointWhenNotFound() => false;

    protected abstract Task<string> PerformActionAsync(TimeSpan timeout);

    public override async Task<ActionTaskResult> RunAsync(
      IVssRequestContext requestContext,
      TimeSpan timeout)
    {
      ServiceBusActionTaskBase busActionTaskBase = this;
      bool doRetry = false;
      bool errorWhileCreatingEndpoint = false;
      Exception exception = (Exception) null;
      string errorMessage = (string) null;
      Stopwatch stopwatch = new Stopwatch();
      ActionTaskResultLevel resultLevel = ActionTaskResultLevel.EnduringFailure;
      try
      {
        string actionResponse = (string) null;
        busActionTaskBase.UpdateNotificationForRequest(requestContext, busActionTaskBase.MessageContent);
        stopwatch.Start();
        busActionTaskBase.InitializeAction(timeout);
        try
        {
          actionResponse = await busActionTaskBase.PerformActionAsync(timeout);
        }
        catch (MessagingEntityNotFoundException ex)
        {
          bool endpointWhenNotFound;
          try
          {
            endpointWhenNotFound = busActionTaskBase.TryCreateEndpointWhenNotFound();
          }
          catch
          {
            errorWhileCreatingEndpoint = true;
            throw;
          }
          if (endpointWhenNotFound)
            doRetry = true;
          else
            throw;
        }
        finally
        {
          stopwatch.Stop();
        }
        if (doRetry)
        {
          stopwatch.Start();
          try
          {
            actionResponse = await busActionTaskBase.PerformActionAsync(timeout);
          }
          finally
          {
            stopwatch.Stop();
          }
        }
        string response = string.IsNullOrEmpty(actionResponse) ? AzureConsumerResources.Response_OK : actionResponse;
        busActionTaskBase.UpdateNotificationForResponse(requestContext, response, new double?(stopwatch.Elapsed.TotalSeconds));
        return new ActionTaskResult(ActionTaskResultLevel.Success);
      }
      catch (TimeoutException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.TransientError;
        errorMessage = AzureConsumerResources.ServiceBus_Response_Error_Timeout;
      }
      catch (ServerBusyException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.TransientError;
        errorMessage = ex.Message;
      }
      catch (UnauthorizedAccessException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.EnduringFailure;
        errorMessage = errorWhileCreatingEndpoint ? AzureConsumerResources.ServiceBus_Response_Error_Management_Unauthorized : AzureConsumerResources.ServiceBus_Response_Error_Unauthorized;
      }
      catch (MessagingEntityNotFoundException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.EnduringFailure;
        errorMessage = AzureConsumerResources.ServiceBus_Response_Error_EndpointNotFound;
      }
      catch (MessageSizeExceededException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.EventPayloadError;
        errorMessage = AzureConsumerResources.ServiceBusQueueMessageTooLong;
      }
      catch (Microsoft.ServiceBus.Messaging.QuotaExceededException ex)
      {
        exception = (Exception) ex;
        resultLevel = ActionTaskResultLevel.EnduringFailure;
        errorMessage = ex.Message;
      }
      catch (Exception ex)
      {
        exception = ex;
        resultLevel = ActionTaskResultLevel.EnduringFailure;
      }
      finally
      {
        busActionTaskBase.CleanupAction();
      }
      busActionTaskBase.UpdateNotificationForResponse(requestContext, errorMessage ?? AzureConsumerResources.Response_Error, new double?(stopwatch.Elapsed.TotalSeconds), exception.Message, exception.ToString());
      return new ActionTaskResult(resultLevel, exception, exception.Message);
    }

    internal Func<string, ServiceBusNamespaceManagerWrapper> CreateNamespaceManagerFunc { set; private get; }

    protected ServiceBusNamespaceManagerWrapper CreateNamespaceManagerFromConnectionString(
      string connectionString)
    {
      return this.CreateNamespaceManagerFunc != null ? this.CreateNamespaceManagerFunc(connectionString) : ServiceBusNamespaceManagerWrapper.CreateFromConnectionString(connectionString);
    }

    internal Func<Uri, MessagingFactorySettings, MessagingFactoryWrapper> CreateMessagingFactoryFunc { set; private get; }

    protected MessagingFactoryWrapper CreateMessagingFactory(
      Uri address,
      MessagingFactorySettings factorySettings)
    {
      return this.CreateNamespaceManagerFunc != null ? this.CreateMessagingFactoryFunc(address, factorySettings) : MessagingFactoryWrapper.Create(address, factorySettings);
    }
  }
}
