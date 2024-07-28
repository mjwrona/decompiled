// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationErrorHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal sealed class TeamFoundationErrorHandler : Attribute, IErrorHandler, IServiceBehavior
  {
    bool IErrorHandler.HandleError(Exception error) => true;

    void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
    {
      TeamFoundationTracingService.TraceExceptionRaw(1813389984, nameof (TeamFoundationErrorHandler), nameof (TeamFoundationErrorHandler), error);
      bool flag = true;
      SoapException soapException;
      switch (error)
      {
        case SoapException _:
          soapException = error as SoapException;
          break;
        case TeamFoundationServiceException _:
          flag = ((VssException) error).LogException;
          soapException = ((TeamFoundationServiceException) error).ToSoapException();
          break;
        case SqlException _:
          soapException = ((SqlException) error).ToSoapException();
          break;
        default:
          soapException = new SoapException(UserFriendlyError.GetMessageFromException(error), Soap12FaultCodes.SenderFaultCode, string.Empty, string.Empty, (XmlNode) null, new SoapFaultSubCode(new XmlQualifiedName(error.GetType().Name)), (Exception) null);
          if (error is ArgumentException || error is HttpException || error is NotSupportedException || error is SecurityException || error is UnauthorizedAccessException)
          {
            flag = false;
            break;
          }
          break;
      }
      if (flag)
        TeamFoundationEventLog.Default.LogException(FrameworkResources.UnhandledExceptionError(), error);
      if (soapException == null)
        return;
      FaultCode code = !(soapException.Code == Soap12FaultCodes.SenderFaultCode) ? (soapException.SubCode == null ? FaultCode.CreateReceiverFaultCode((FaultCode) null) : FaultCode.CreateReceiverFaultCode(new FaultCode(soapException.SubCode.Code.Name, soapException.SubCode.Code.Namespace))) : (soapException.SubCode == null ? FaultCode.CreateSenderFaultCode((FaultCode) null) : FaultCode.CreateSenderFaultCode(new FaultCode(soapException.SubCode.Code.Name, soapException.SubCode.Code.Namespace)));
      fault = Message.CreateMessage(version, MessageFault.CreateFault(code, new FaultReason(soapException.Message)), (string) null);
    }

    void IServiceBehavior.AddBindingParameters(
      ServiceDescription serviceDescription,
      ServiceHostBase serviceHostBase,
      Collection<ServiceEndpoint> endpoints,
      BindingParameterCollection bindingParameters)
    {
    }

    void IServiceBehavior.ApplyDispatchBehavior(
      ServiceDescription serviceDescription,
      ServiceHostBase serviceHostBase)
    {
      foreach (ChannelDispatcher channelDispatcher in (SynchronizedCollection<ChannelDispatcherBase>) serviceHostBase.ChannelDispatchers)
        channelDispatcher.ErrorHandlers.Add((IErrorHandler) this);
    }

    void IServiceBehavior.Validate(
      ServiceDescription serviceDescription,
      ServiceHostBase serviceHostBase)
    {
    }
  }
}
