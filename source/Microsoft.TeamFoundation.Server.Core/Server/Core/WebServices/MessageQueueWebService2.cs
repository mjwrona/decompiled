// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.MessageQueueWebService2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  [TeamFoundationErrorHandler]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
  [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any, ConcurrencyMode = ConcurrencyMode.Multiple, IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.PerCall, UseSynchronizationContext = false)]
  [ServiceContract(Name = "MessageQueueService", Namespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2")]
  public sealed class MessageQueueWebService2
  {
    private static readonly string s_layer = "Service";
    private static readonly string s_area = "MessageQueue";
    private IVssRequestContext m_requestContext;
    private TeamFoundationMessageQueueService m_queueService;

    public TeamFoundationMessageQueueService QueueService
    {
      get
      {
        if (this.m_queueService == null)
          this.m_queueService = this.RequestContext.GetService<TeamFoundationMessageQueueService>();
        return this.m_queueService;
      }
    }

    public IVssRequestContext RequestContext
    {
      get
      {
        if (this.m_requestContext == null)
          this.m_requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) "IVssRequestContext"];
        return this.m_requestContext;
      }
    }

    [OperationContract(Action = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Acknowledge", AsyncPattern = true, ReplyAction = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/AcknowledgeResponse")]
    public IAsyncResult BeginAcknowledge(
      AcknowledgementRange[] ranges,
      AsyncCallback callback,
      object state)
    {
      this.RequestContext.ServiceName = MessageQueueWebService2.s_area;
      MethodInformation methodInformation = new MethodInformation("Acknowledge", Microsoft.TeamFoundation.Framework.Server.MethodType.ReadWrite, EstimatedMethodCost.VeryLow);
      methodInformation.AddParameter("To", (object) OperationContext.Current.RequestContext.RequestMessage.Headers.To);
      methodInformation.AddArrayParameter<AcknowledgementRange>(nameof (ranges), (IList<AcknowledgementRange>) ranges);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        return this.QueueService.BeginAcknowledge(this.RequestContext, OperationContext.Current.RequestContext.RequestMessage.Headers.To.Host, this.RequestContext.UniqueIdentifier, (IList<AcknowledgementRange>) ranges, OperationContext.Current.RequestContext.RequestMessage.Headers, OperationContext.Current.Channel.OperationTimeout, callback, state);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, MessageQueueWebService2.s_area, MessageQueueWebService2.s_layer, ex);
        this.RequestContext.LeaveMethod();
        throw;
      }
    }

    public void EndAcknowledge(IAsyncResult result)
    {
      try
      {
        this.QueueService.EndAcknowledge(result);
      }
      catch (TimeoutException ex)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, MessageQueueWebService2.s_area, MessageQueueWebService2.s_layer, "EndAcknowledge::The operation has timed out");
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
    }

    [OperationContract(Action = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Dequeue", AsyncPattern = true, ReplyAction = "*")]
    public IAsyncResult BeginDequeue(Message message, AsyncCallback callback, object state)
    {
      this.RequestContext.ServiceName = MessageQueueWebService2.s_area;
      MethodInformation methodInformation = new MethodInformation("Dequeue", Microsoft.TeamFoundation.Framework.Server.MethodType.LightWeight, EstimatedMethodCost.VeryLow, false, true);
      methodInformation.AddParameter("To", (object) message.Headers.To);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        AcknowledgementRange[] ranges = (AcknowledgementRange[]) null;
        long lastMessageId = LastMessageHeader.ReadHeader(message.Headers);
        AcknowledgementHeaderV2 acknowledgementHeaderV2 = AcknowledgementHeaderV2.ReadHeader(message.Headers);
        if (acknowledgementHeaderV2 != null)
          ranges = acknowledgementHeaderV2.Ranges.ToArray();
        return this.QueueService.BeginDequeue(this.RequestContext, message.Headers.To.Host, this.RequestContext.UniqueIdentifier, lastMessageId, (IList<AcknowledgementRange>) ranges, message.Headers, this.QueueService.IdleTimeout, TfsMessageQueueVersion.V2, callback, state);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, MessageQueueWebService2.s_area, MessageQueueWebService2.s_layer, ex);
        this.RequestContext.LeaveMethod();
        throw;
      }
    }

    public Message EndDequeue(IAsyncResult result)
    {
      Message message = (Message) null;
      try
      {
        message = this.QueueService.EndDequeue(result);
      }
      catch (TimeoutException ex)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, MessageQueueWebService2.s_area, MessageQueueWebService2.s_layer, "EndDequeue::The operation has timed out");
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
      if (message == null)
        message = MessageQueueWebService2.CreateEmptyResponse(OperationContext.Current.IncomingMessageVersion);
      return message;
    }

    private static Message CreateEmptyResponse(MessageVersion messageVersion)
    {
      Message message = Message.CreateMessage(messageVersion, "");
      message.Properties.Add(HttpResponseMessageProperty.Name, (object) new HttpResponseMessageProperty()
      {
        StatusCode = HttpStatusCode.Accepted,
        SuppressEntityBody = true
      });
      return message;
    }
  }
}
