// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.MessageQueueWebService
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
  [ServiceContract(Name = "MessageQueueService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Framework")]
  public sealed class MessageQueueWebService
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

    [OperationContract(Action = "http://docs.oasis-open.org/ws-rx/wsmc/200702/MakeConnection", AsyncPattern = true, ReplyAction = "*")]
    public IAsyncResult BeginMakeConnection(Message message, AsyncCallback callback, object state)
    {
      MethodInformation methodInformation = new MethodInformation("MakeConnection", Microsoft.TeamFoundation.Framework.Server.MethodType.LightWeight, EstimatedMethodCost.VeryLow, false, true);
      methodInformation.AddParameter("To", (object) message.Headers.To);
      try
      {
        this.RequestContext.EnterMethod(methodInformation);
        long messageId;
        if (!AcknowledgementHeaderV1.TryReadHeader(message.Headers, out messageId))
          messageId = long.MinValue;
        AcknowledgementRange[] ranges = (AcknowledgementRange[]) null;
        if (messageId != long.MinValue)
          ranges = new AcknowledgementRange[1]
          {
            new AcknowledgementRange()
            {
              Lower = messageId,
              Upper = messageId
            }
          };
        return this.QueueService.BeginDequeue(this.RequestContext, message.Headers.To.Host, this.RequestContext.UniqueIdentifier, messageId, (IList<AcknowledgementRange>) ranges, message.Headers, this.QueueService.IdleTimeout, TfsMessageQueueVersion.V1, callback, state);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, MessageQueueWebService.s_area, MessageQueueWebService.s_layer, ex);
        this.RequestContext.LeaveMethod();
        throw;
      }
    }

    public Message EndMakeConnection(IAsyncResult result)
    {
      Message message = (Message) null;
      try
      {
        message = this.QueueService.EndDequeue(result);
      }
      catch (TimeoutException ex)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Info, MessageQueueWebService.s_area, MessageQueueWebService.s_layer, "EndMakeConnection::The operation has timed out");
      }
      finally
      {
        this.RequestContext.LeaveMethod();
      }
      if (message == null)
        message = MessageQueueWebService.CreateEmptyResponse(OperationContext.Current.IncomingMessageVersion);
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
