// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingOperation
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [DebuggerDisplay("Name: {Name}, Groups: {Groups.Count}, Steps: {StepCount}")]
  public sealed class ServicingOperation
  {
    internal ServicingExecutionHandlerData[] m_executionHandlers = Helper.ZeroLengthArrayOfServicingExecutionHandlerData;
    internal ServicingStepGroup[] m_groups = Helper.ZeroLengthArrayOfServicingStepGroup;
    private string m_name;

    [XmlIgnore]
    public int StepCount => ((IEnumerable<ServicingStepGroup>) this.Groups).Aggregate<ServicingStepGroup, int>(0, (Func<int, ServicingStepGroup, int>) ((count, stepGroup) => count + stepGroup.Steps.Length));

    private ServicingOperation()
    {
    }

    public ServicingExecutionHandlerData[] ExecutionHandlers
    {
      get => (ServicingExecutionHandlerData[]) this.m_executionHandlers.Clone();
      set => this.m_executionHandlers = value;
    }

    public ServicingStepGroup[] Groups
    {
      get => (ServicingStepGroup[]) this.m_groups.Clone();
      set => this.m_groups = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingOperation FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServicingOperation servicingOperation = new ServicingOperation();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          if (reader.Name == "name")
            servicingOperation.m_name = XmlUtility.StringFromXmlAttribute(reader);
        }
      }
      reader.Read();
      if (!isEmptyElement)
      {
        while (reader.NodeType == XmlNodeType.Element)
        {
          switch (reader.Name)
          {
            case "ExecutionHandlers":
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              servicingOperation.m_executionHandlers = XmlUtility.ArrayOfObjectFromXml<ServicingExecutionHandlerData>(serviceProvider, reader, "ExecutionHandler", false, ServicingOperation.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ServicingOperation.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.FromXml)));
              continue;
            case "Groups":
              servicingOperation.m_groups = Helper.ArrayOfServicingStepGroupFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return servicingOperation;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingOperation instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ExecutionHandlers: " + Helper.ArrayToString<ServicingExecutionHandlerData>(this.m_executionHandlers));
      stringBuilder.AppendLine("  Groups: " + Helper.ArrayToString<ServicingStepGroup>(this.m_groups));
      stringBuilder.AppendLine("  Name: " + this.m_name);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingExecutionHandlerData>(writer, this.m_executionHandlers, "ExecutionHandlers", "ExecutionHandler", false, false, ServicingOperation.\u003C\u003EO.\u003C1\u003E__ToXml ?? (ServicingOperation.\u003C\u003EO.\u003C1\u003E__ToXml = new Action<XmlWriter, string, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.ToXml)));
      Helper.ToXml(writer, "Groups", this.m_groups, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingOperation obj) => obj.ToXml(writer, element);
  }
}
