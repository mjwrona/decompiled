// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeserializationErrorHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DeserializationErrorHandler
  {
    private string m_xml;
    private Type m_targetType;
    private static readonly string s_area = "Serialization";
    private static readonly string s_layer = nameof (DeserializationErrorHandler);

    public DeserializationErrorHandler(string xml, Type targetType)
    {
      this.m_xml = xml;
      this.m_targetType = targetType;
    }

    public StringBuilder Errors { get; private set; }

    public void OnUnknownXmlElement(object sender, XmlElementEventArgs e)
    {
      if (e.ObjectBeingDeserialized is HostProperties)
      {
        if (string.Equals(e.Element.Name, "ConnectionString", StringComparison.Ordinal))
        {
          TeamFoundationTracingService.TraceRaw(9500, TraceLevel.Info, DeserializationErrorHandler.s_area, DeserializationErrorHandler.s_layer, "Unknown xml element is a ConnectionString. Ignoring value.");
          return;
        }
        if (string.Equals(e.Element.Name, "ConnectionInfo", StringComparison.Ordinal))
        {
          TeamFoundationTracingService.TraceRaw(9500, TraceLevel.Info, DeserializationErrorHandler.s_area, DeserializationErrorHandler.s_layer, "Unknown xml element is a ConnectionInfo. Ignoring value.");
          return;
        }
      }
      TeamFoundationTracingService.TraceRaw(9500, TraceLevel.Warning, DeserializationErrorHandler.s_area, DeserializationErrorHandler.s_layer, "Unknown xml element '{0}'. Target type for deserialization: {1}. Type of object being deserialized: {2}", (object) e.Element.Name, (object) this.m_targetType, e.ObjectBeingDeserialized != null ? (object) e.ObjectBeingDeserialized.GetType().FullName : (object) string.Empty);
      this.AppendError(FrameworkResources.UnknownXmlElementError((object) e.Element.Name, (object) e.LineNumber, (object) e.LinePosition));
    }

    public void OnUnknownXmlAttribute(object sender, XmlAttributeEventArgs e)
    {
      if (string.Equals(e.Attr.Name, "xsi:type", StringComparison.Ordinal))
        return;
      TeamFoundationTracingService.TraceRaw(9501, TraceLevel.Warning, DeserializationErrorHandler.s_area, DeserializationErrorHandler.s_layer, "Unknown xml attribute '{0}'. Target type for deserialization: {1}. Type of object being deserialized: {2}", (object) e.Attr.Name, (object) this.m_targetType, e.ObjectBeingDeserialized != null ? (object) e.ObjectBeingDeserialized.GetType().FullName : (object) string.Empty);
      this.AppendError(FrameworkResources.UnknownXmlAttributeError((object) e.Attr.Name, (object) e.LineNumber, (object) e.LinePosition));
    }

    private void AppendError(string errorMessage)
    {
      if (this.Errors == null)
        this.Errors = new StringBuilder();
      if (this.Errors.Length > 0)
        this.Errors.Append("; ");
      this.Errors.Append(errorMessage);
    }
  }
}
