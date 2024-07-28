// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.TeamFoundationWebService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public abstract class TeamFoundationWebService : WebService
  {
    private IVssRequestContext m_requestContext;
    private MethodInformation m_methodInformation;
    private const string c_area = "TeamFoundationWebService";
    private const string c_layer = "WebService";

    protected TeamFoundationWebService()
    {
      this.m_requestContext = (HttpContextFactory.Current.ApplicationInstance as VisualStudioServicesApplication).VssRequestContext;
      this.m_requestContext.ServiceName = this.GetType().Name;
      if (this.m_requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        this.RequestContext.CheckOnPremisesDeployment(true);
      if ((this.m_requestContext.WebRequestContextInternal().RequestRestrictions.AllowedHandlers & AllowedHandler.TfsWebService) == AllowedHandler.None)
        throw this.HandleException((Exception) new InvalidAccessException(FrameworkResources.InvalidAccessException()));
    }

    protected IVssRequestContext RequestContext => this.m_requestContext;

    protected void CheckOnPremises()
    {
      if (!this.m_requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidAccessException(FrameworkResources.InvalidAccessException());
    }

    protected virtual void EnterMethod(MethodInformation methodInformation)
    {
      if (this.m_requestContext == null)
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
        this.m_requestContext.TraceException(7511, TraceLevel.Warning, nameof (TeamFoundationWebService), "WebService", exception);
      }
      else
        TeamFoundationTracingService.TraceExceptionRaw(7511, TraceLevel.Warning, nameof (TeamFoundationWebService), "WebService", exception);
      if (exception is AccessCheckException ex1)
        exception = (Exception) new LegacyAccessCheckException(ex1);
      if (exception is SoapException)
        return exception;
      SoapException soapException;
      if (exception is TeamFoundationServiceException)
      {
        TeamFoundationServiceException ex2 = exception as TeamFoundationServiceException;
        flag = ex2.LogException;
        soapException = ex2.ToSoapException();
      }
      else if (exception is SqlException)
      {
        soapException = ((SqlException) exception).ToSoapException();
      }
      else
      {
        if (exception is ArgumentException || exception is HttpException || exception is NotSupportedException || exception is System.Security.SecurityException || exception is UnauthorizedAccessException)
          flag = false;
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
        soapException = new SoapException(UserFriendlyError.GetMessageFromException(exception, activityId: activityId), Soap12FaultCodes.SenderFaultCode, string.Empty, string.Empty, xml, new SoapFaultSubCode(new XmlQualifiedName(exception.GetType().Name)), (Exception) null);
      }
      if (flag)
      {
        TeamFoundationEventLog.Default.LogException(this.m_requestContext, FrameworkResources.UnhandledExceptionError(), exception);
        if (this.m_requestContext != null)
          this.m_requestContext.TraceException(7514, nameof (TeamFoundationWebService), "WebService", exception);
      }
      return (Exception) soapException;
    }

    protected void AddWebServiceResource(IDisposable resource) => this.m_requestContext.AddDisposableResource(resource);

    public static string CreateCustomKeyForAspNetWebServiceMetadataCache(
      Type protocol,
      Type service,
      string originalString)
    {
      return !typeof (SoapServerProtocol).IsAssignableFrom(protocol) || originalString.EndsWith("CachePressure", StringComparison.OrdinalIgnoreCase) ? originalString : protocol.FullName + service.FullName + service.TypeHandle.Value.ToString();
    }
  }
}
