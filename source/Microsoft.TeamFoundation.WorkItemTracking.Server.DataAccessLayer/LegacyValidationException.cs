// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyValidationException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Resources;
using System.Runtime.Serialization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  [Serializable]
  public class LegacyValidationException : LegacyServerException
  {
    private XmlNode m_details;

    public LegacyValidationException()
    {
    }

    public LegacyValidationException(string message)
      : base(message, 600171)
    {
    }

    public LegacyValidationException(string message, Exception innerException)
      : base(message, 600171, innerException)
    {
    }

    protected LegacyValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public LegacyValidationException(string message, int id)
      : base(message, id)
    {
    }

    public LegacyValidationException(string message, int id, Exception innerException)
      : base(message, id, innerException)
    {
    }

    public LegacyValidationException(
      string message,
      int id,
      Exception innerException,
      XmlNode details)
      : base(message, id, innerException)
    {
      this.m_details = details;
    }

    public LegacyValidationException(
      IVssRequestContext requestContext,
      ResourceManager resMgr,
      int id,
      string subtype,
      string messageResourceName,
      object[] messageParameters)
      : base(LegacyValidationException.FormatMessage(resMgr, messageResourceName, messageParameters), id)
    {
      XmlDocument doc;
      LegacyValidationException.PrepareDetailsElement(id, out doc, out this.m_details);
      XmlElement element1 = doc.CreateElement("MessageResource");
      element1.SetAttribute("ResourceName", messageResourceName);
      if (messageParameters != null)
      {
        foreach (object messageParameter in messageParameters)
        {
          XmlElement element2 = doc.CreateElement("Param");
          element2.InnerText = messageParameter == null ? "" : messageParameter.ToString();
          element1.AppendChild((XmlNode) element2);
        }
      }
      this.m_details.AppendChild((XmlNode) element1);
      if (subtype == null)
        return;
      XmlElement element3 = doc.CreateElement("ExceptionSubType");
      element3.SetAttribute("SubType", subtype);
      this.m_details.AppendChild((XmlNode) element3);
    }

    protected override void SetEventId() => this.EventId = TeamFoundationEventId.ValidationException;

    public XmlNode Details
    {
      get => this.m_details;
      set => this.m_details = value;
    }

    private static string FormatMessage(
      ResourceManager resMgr,
      string messageResourceName,
      params object[] messageParameters)
    {
      string format = resMgr.GetString(messageResourceName, CultureInfo.CurrentUICulture);
      if (messageParameters != null)
        format = string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, messageParameters);
      return format;
    }

    private static void PrepareDetailsElement(
      int errorId,
      out XmlDocument doc,
      out XmlNode details)
    {
      doc = new XmlDocument();
      details = doc.CreateNode(XmlNodeType.Element, nameof (details), "http://schemas.microsoft.com/TeamFoundation/2005/06/WorkItemTracking/faultdetail/03");
      TFCommonUtil.AddXmlAttribute(details, "id", errorId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }
  }
}
