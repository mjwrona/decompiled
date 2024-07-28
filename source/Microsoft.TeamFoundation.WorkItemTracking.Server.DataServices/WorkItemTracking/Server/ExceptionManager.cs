// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExceptionManager
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal static class ExceptionManager
  {
    internal static Type[] s_nonReportableExceptions = new Type[14]
    {
      typeof (ArgumentException),
      typeof (ArgumentNullException),
      typeof (WebException),
      typeof (IdentityNotMappedException),
      typeof (UnauthorizedAccessException),
      typeof (SecurityException),
      typeof (ConfigurationException),
      typeof (InvalidOperationException),
      typeof (TeamFoundationServerException),
      typeof (ThreadAbortException),
      typeof (SoapException),
      typeof (AccessViolationException),
      typeof (XmlException),
      typeof (CircuitBreakerShortCircuitException)
    };
    internal static Type[] s_loggableExceptions = new Type[12]
    {
      typeof (WebException),
      typeof (ConfigurationException),
      typeof (InvalidOperationException),
      typeof (GroupSecuritySubsystemServiceException),
      typeof (AuthorizationSubsystemServiceException),
      typeof (CommonStructureSubsystemServiceException),
      typeof (AccessViolationException),
      typeof (LegacyConfigurationException),
      typeof (DatabaseConfigurationException),
      typeof (LegacySqlErrorFullTextException),
      typeof (XmlException),
      typeof (ProjectException)
    };

    internal static void ThrowProperSoapException(IVssRequestContext requestContext, Exception e)
    {
      if (ExceptionManager.IsReportable(e))
        ExceptionManager.ReportException(requestContext, e);
      else if (ExceptionManager.IsLoggable(e))
        TeamFoundationEventLog.Default.LogException(requestContext, e.Message, e, TeamFoundationEventId.WitBaseEventId, EventLogEntryType.Error);
      XmlNode xmlNode = (XmlNode) null;
      int num = 0;
      XmlQualifiedName code;
      SoapFaultSubCode subCode;
      switch (e)
      {
        case SoapException _:
          throw e;
        case ArgumentNullException _:
          num = -1;
          code = Soap12FaultCodes.SenderFaultCode;
          subCode = WorkItemTrackingFaultCodes.ArgumentNullException;
          break;
        case ArgumentException _:
          num = -1;
          code = Soap12FaultCodes.SenderFaultCode;
          subCode = WorkItemTrackingFaultCodes.ArgumentException;
          break;
        case IdentityNotMappedException _:
          code = Soap12FaultCodes.SenderFaultCode;
          subCode = WorkItemTrackingFaultCodes.UserNotFound;
          break;
        case SecurityException _:
          code = Soap12FaultCodes.SenderFaultCode;
          subCode = WorkItemTrackingFaultCodes.Security;
          break;
        case ThreadAbortException _:
          code = Soap12FaultCodes.ReceiverFaultCode;
          subCode = WorkItemTrackingFaultCodes.Service;
          break;
        case TeamFoundationServiceException serviceException:
          num = serviceException.ErrorCode;
          switch (serviceException)
          {
            case LegacyValidationException validationException:
              code = Soap12FaultCodes.SenderFaultCode;
              subCode = WorkItemTrackingFaultCodes.Application;
              xmlNode = validationException.Details;
              break;
            case WorkItemTrackingGetMetadataTooManyConcurrentUsersException _:
            case WorkItemTrackingGetMetadataServerBusyException _:
              num = -2;
              code = Soap12FaultCodes.SenderFaultCode;
              subCode = WorkItemTrackingFaultCodes.Service;
              break;
            case LegacySecurityException _:
              code = Soap12FaultCodes.SenderFaultCode;
              subCode = WorkItemTrackingFaultCodes.Security;
              break;
            case LegacyServiceException _:
            case DateTimeShiftDetectedException _:
              code = Soap12FaultCodes.ReceiverFaultCode;
              subCode = WorkItemTrackingFaultCodes.Service;
              break;
            default:
              code = Soap12FaultCodes.ReceiverFaultCode;
              subCode = WorkItemTrackingFaultCodes.Unknown;
              break;
          }
          break;
        default:
          code = Soap12FaultCodes.ReceiverFaultCode;
          subCode = WorkItemTrackingFaultCodes.Unknown;
          break;
      }
      XmlDocument xmlDocument = new XmlDocument();
      XmlNode node1 = xmlDocument.CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
      XmlNode node2 = xmlDocument.CreateNode(XmlNodeType.Element, "details", "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultdetail/03");
      XmlAttribute attribute1 = xmlDocument.CreateAttribute("id");
      attribute1.Value = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      node2.Attributes.Append(attribute1);
      XmlAttribute attribute2 = xmlDocument.CreateAttribute("ExceptionMessage");
      attribute2.Value = e.Message;
      node1.Attributes.Append(attribute2);
      XmlAttribute attribute3 = xmlDocument.CreateAttribute("BaseExceptionName");
      string str = e.GetType().FullName;
      if (str.StartsWith("Microsoft.TeamFoundation.WorkItemTracking"))
        str = str.Replace("Legacy", "");
      attribute3.Value = str;
      node1.Attributes.Append(attribute3);
      if (xmlNode != null)
        node2.InnerXml = xmlNode.InnerXml;
      node1.AppendChild(node2);
      throw new SoapException(e.Message, code, HttpContext.Current.Request.Url.AbsolutePath, string.Empty, node1, subCode, (Exception) null);
    }

    internal static bool IsReportable(Exception ex)
    {
      Type type = ex.GetType();
      foreach (Type reportableException in ExceptionManager.s_nonReportableExceptions)
      {
        if (reportableException.IsAssignableFrom(type))
          return false;
      }
      return !(ex is VssException) || ((VssException) ex).ReportException;
    }

    internal static bool IsLoggable(Exception e)
    {
      Type type = e.GetType();
      foreach (Type loggableException in ExceptionManager.s_loggableExceptions)
      {
        if (loggableException.IsAssignableFrom(type))
          return true;
      }
      return e is TeamFoundationServiceException serviceException && serviceException.LogException;
    }

    internal static void HandleStaleViewsException(
      DataAccessLayerImpl dal,
      IVssRequestContext requestContext,
      XmlElement package,
      IVssIdentity user,
      Exception e)
    {
      if (!(e is LegacySqlErrorViewBindingErrorException))
        return;
      dal.HandleStaleViewsException(package, user, e);
    }

    internal static void ReportException(IVssRequestContext requestContext, Exception exception)
    {
      if (requestContext == null)
        return;
      requestContext.TraceException(900603, "WebServices", nameof (ExceptionManager), exception);
      if (requestContext.ServiceHost == null || !requestContext.ServiceHost.SendWatsonReports)
        return;
      requestContext.ServiceHost.ReportException("wit", "Database", exception, (string[]) null);
    }
  }
}
