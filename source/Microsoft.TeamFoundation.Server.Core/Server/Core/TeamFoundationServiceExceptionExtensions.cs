// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationServiceExceptionExtensions
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TeamFoundationServiceExceptionExtensions
  {
    public static SoapException ToSoapException(this TeamFoundationServiceException ex)
    {
      XmlNode detail = (XmlNode) null;
      ExceptionPropertyCollection properties = new ExceptionPropertyCollection();
      ex.GetExceptionProperties(properties);
      if (ex.InnerException != null || properties.Count > 0)
      {
        using (StringWriter w = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
        {
          using (XmlTextWriter writer = new XmlTextWriter((TextWriter) w))
          {
            writer.WriteStartDocument();
            writer.WriteStartElement(SoapException.DetailElementName.Name, SoapException.DetailElementName.Namespace);
            writer.WriteAttributeString("xmlns", "xsd", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema");
            writer.WriteAttributeString("xmlns", "xsi", "http://www.w3.org/2000/xmlns/", "http://www.w3.org/2001/XMLSchema-instance");
            if (ex.InnerException != null)
            {
              string str = ex.InnerException is SqlException ? FrameworkResources.Error_InternalTFSFailure() : SecretUtility.ScrubSecrets(ex.InnerException.Message);
              writer.WriteAttributeString("InnerExceptionType", ex.InnerException.GetType().Name);
              writer.WriteAttributeString("InnerExceptionMessage", str);
            }
            if (properties.Count > 0)
              properties.ToXml((XmlWriter) writer, "ExceptionProperties");
            writer.WriteEndElement();
            writer.WriteEndDocument();
            detail = TeamFoundationSerializationUtility.SerializeToXml(w.ToString());
          }
        }
      }
      return new SoapException(ex.Message, ex.FaultCode, string.Empty, string.Empty, detail, new SoapFaultSubCode(new XmlQualifiedName(ex.SerializedExceptionName)), (Exception) null);
    }

    public static SoapException ToSoapException(this SqlException sqlException) => new SoapException(FrameworkResources.SqlException((object) sqlException.Number), Soap12FaultCodes.ReceiverFaultCode, (Exception) null);
  }
}
