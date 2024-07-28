// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingExecutionHandlerData
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  public sealed class ServicingExecutionHandlerData
  {
    private string m_handlerType;

    private ServicingExecutionHandlerData()
    {
    }

    public string HandlerType
    {
      get => this.m_handlerType;
      set => this.m_handlerType = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingExecutionHandlerData FromXml(
      IServiceProvider serviceProvider,
      XmlReader reader)
    {
      ServicingExecutionHandlerData executionHandlerData = new ServicingExecutionHandlerData();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "type")
            executionHandlerData.m_handlerType = XmlUtility.StringFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          string name = reader.Name;
          reader.ReadOuterXml();
        }
        reader.ReadEndElement();
      }
      return executionHandlerData;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingExecutionHandlerData instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  HandlerType: " + this.m_handlerType);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_handlerType != null)
        XmlUtility.ToXmlAttribute(writer, "type", this.m_handlerType);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingExecutionHandlerData obj) => obj.ToXml(writer, element);
  }
}
