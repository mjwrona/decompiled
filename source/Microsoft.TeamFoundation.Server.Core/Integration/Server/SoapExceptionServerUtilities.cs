// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SoapExceptionServerUtilities
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Security;
using System.Web;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class SoapExceptionServerUtilities
  {
    private const string SortableDateTimeWithMilliseconds = "yyy-MM-ddTHH:mm:ss:fff";

    public static Exception LogAndFilter(Exception e) => SoapExceptionServerUtilities.LogAndFilter(e, string.Empty);

    public static Exception LogAndFilter(Exception e, string userName) => (Exception) SoapExceptionServerUtilities.CreateSoapException(ServerExceptionUtilities.LogAndFilter(e, userName));

    private static SoapException CreateSoapException(Exception e)
    {
      if (e is SoapException)
        return (SoapException) e;
      XmlNode detailNode = SoapExceptionServerUtilities.GetDetailNode();
      string val1 = DateTime.Now.ToString("yyy-MM-ddTHH:mm:ss:fff", (IFormatProvider) CultureInfo.InvariantCulture);
      SoapExceptionServerUtilities.SetDetailAttribute(detailNode, ServerExceptionUtilities.ServerTimeStamp, val1);
      SoapFaultSubCode subCode = (SoapFaultSubCode) null;
      string str = (string) null;
      string val2;
      XmlQualifiedName code;
      if (e is SqlException)
      {
        int number = ((SqlException) e).Number;
        if (number >= 50000)
          str = e.Message;
        SoapExceptionServerUtilities.SetDetailAttribute(detailNode, ServerExceptionUtilities.SqlNumber, string.Empty + number.ToString());
        val2 = typeof (SqlException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else if (e is SecurityException)
      {
        val2 = typeof (SecurityException).FullName;
        code = SoapException.ClientFaultCode;
      }
      else if (e is UnauthorizedAccessException)
      {
        val2 = typeof (SecurityException).FullName;
        str = e.Message;
        code = SoapException.ClientFaultCode;
      }
      else if (e is ArgumentNullException)
      {
        str = e.Message;
        val2 = typeof (ArgumentNullException).FullName;
        code = SoapException.ClientFaultCode;
      }
      else if (e is ArgumentException)
      {
        str = e.Message;
        val2 = typeof (ArgumentException).FullName;
        code = SoapException.ClientFaultCode;
      }
      else if (e is AuthorizationSubsystemServiceException)
      {
        str = e.Message;
        val2 = "Microsoft.TeamFoundation.Server.AuthorizationSubsystemException";
        code = SoapException.ServerFaultCode;
      }
      else if (e is GroupSecuritySubsystemServiceException)
      {
        str = e.Message;
        val2 = "Microsoft.TeamFoundation.Server.GroupSecuritySubsystemException";
        code = SoapException.ServerFaultCode;
      }
      else if (e is AnalysisServiceConnectionException)
      {
        str = e.Message;
        val2 = typeof (AnalysisServiceConnectionException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else if (e is CommonStructureSubsystemServiceException)
      {
        str = e.Message;
        val2 = "Microsoft.TeamFoundation.Server.CommonStructureSubsystemException";
        code = SoapException.ServerFaultCode;
      }
      else if (e is ProjectException)
      {
        str = e.Message;
        val2 = typeof (ProjectException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else if (e is SyncSubsystemServiceException)
      {
        str = e.Message;
        val2 = typeof (SyncSubsystemServiceException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else if (e is TeamFoundationServiceUnavailableException)
      {
        str = e.Message;
        val2 = typeof (TeamFoundationServiceUnavailableException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else if (e is TeamFoundationServerException)
      {
        str = e.Message;
        val2 = typeof (TeamFoundationServerException).FullName;
        code = SoapException.ServerFaultCode;
      }
      else
      {
        val2 = typeof (Exception).FullName;
        code = SoapException.ServerFaultCode;
      }
      string message = e.Message;
      SoapExceptionServerUtilities.SetDetailAttribute(detailNode, ServerExceptionUtilities.ExceptionMessage, message);
      SoapExceptionServerUtilities.SetDetailAttribute(detailNode, ServerExceptionUtilities.BaseExceptionName, val2);
      return new SoapException(message, code, HttpContext.Current.Request.Url.AbsolutePath, string.Empty, detailNode, subCode, e);
    }

    private static XmlNode GetDetailNode() => new XmlDocument().CreateNode(XmlNodeType.Element, SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);

    private static void SetDetailAttribute(XmlNode detailNode, string name, string val)
    {
      if (val == null)
        return;
      XmlAttribute attribute = detailNode.OwnerDocument.CreateAttribute(name, "");
      attribute.Value = val;
      detailNode.Attributes.Append(attribute);
    }
  }
}
