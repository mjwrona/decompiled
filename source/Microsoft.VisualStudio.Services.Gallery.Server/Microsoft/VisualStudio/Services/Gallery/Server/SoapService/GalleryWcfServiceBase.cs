// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.GalleryWcfServiceBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService
{
  public abstract class GalleryWcfServiceBase
  {
    private MethodInformation m_methodInformation;
    private const string c_area = "Market";
    private const string c_layer = "WebService";
    private IVssRequestContext m_requestContext;

    public IVssRequestContext RequestContext
    {
      get
      {
        if (this.m_requestContext == null)
          this.m_requestContext = (IVssRequestContext) HttpContext.Current.Items[(object) "IVssRequestContext"];
        return this.m_requestContext;
      }
    }

    protected virtual void EnterMethod(MethodInformation methodInformation)
    {
      if (this.RequestContext == null)
        return;
      this.m_methodInformation = methodInformation;
      this.m_requestContext.EnterMethod(methodInformation);
      this.m_requestContext.RequestTimer.SetPreControllerTime();
    }

    protected virtual void LeaveMethod()
    {
      if (this.m_requestContext == null)
        return;
      this.m_requestContext.RequestTimer.SetControllerTime();
      this.m_requestContext.LeaveMethod();
    }

    protected virtual Exception HandleException(Exception exception)
    {
      bool flag = true;
      Guid activityId = Guid.Empty;
      TeamFoundationTrace.TraceException(exception);
      if (this.m_requestContext != null)
      {
        this.m_requestContext.Status = exception;
        activityId = this.m_requestContext.ActivityId;
        this.m_requestContext.TraceException(12062020, TraceLevel.Error, "Market", "WebService", exception);
      }
      else
        TeamFoundationTracingService.TraceExceptionRaw(12062020, TraceLevel.Error, "Market", "WebService", exception);
      switch (exception)
      {
        case SoapException _:
          return exception;
        case ArgumentException _:
        case HttpException _:
        case NotSupportedException _:
        case SecurityException _:
        case UnauthorizedAccessException _:
          flag = false;
          break;
      }
      XmlNode xml;
      using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        using (XmlTextWriter xmlTextWriter = new XmlTextWriter((TextWriter) w))
        {
          xmlTextWriter.WriteStartDocument();
          xmlTextWriter.WriteStartElement(SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
          xmlTextWriter.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
          xmlTextWriter.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
          xmlTextWriter.WriteAttributeString("ExceptionMessage", SecretUtility.ScrubSecrets(exception.Message));
          xmlTextWriter.WriteAttributeString("BaseExceptionName", exception.GetType().FullName);
          xmlTextWriter.WriteEndElement();
          xmlTextWriter.WriteEndDocument();
          xml = TeamFoundationSerializationUtility.SerializeToXml(w.ToString());
        }
      }
      SoapException soapException = new SoapException(UserFriendlyError.GetMessageFromException(exception, activityId: activityId), Soap12FaultCodes.SenderFaultCode, string.Empty, string.Empty, xml, new SoapFaultSubCode(new XmlQualifiedName(exception.GetType().Name)), (Exception) null);
      if (!flag)
        return (Exception) soapException;
      TeamFoundationEventLog.Default.LogException(this.m_requestContext, FrameworkResources.UnhandledExceptionError(), exception);
      if (this.m_requestContext == null)
        return (Exception) soapException;
      this.m_requestContext.TraceException(12062020, "Market", "WebService", exception);
      return (Exception) soapException;
    }
  }
}
