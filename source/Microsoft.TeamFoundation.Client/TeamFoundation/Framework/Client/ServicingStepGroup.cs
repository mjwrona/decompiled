// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.ServicingStepGroup
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [DebuggerDisplay("Name: {Name}, Steps: {Steps.Length}, Subsystem: {Subsystem}")]
  public sealed class ServicingStepGroup
  {
    internal ServicingExecutionHandlerData[] m_executionHandlers = Helper.ZeroLengthArrayOfServicingExecutionHandlerData;
    private string m_name;
    internal ServicingStep[] m_steps = Helper.ZeroLengthArrayOfServicingStep;
    private string m_subsystemAttribute;

    public ServicingStepGroup(string name, string subsystem, IEnumerable<ServicingStep> steps)
    {
      this.Name = name;
      this.SubsystemAttribute = subsystem;
      if (steps == null)
        return;
      if (steps is ServicingStep[])
        this.Steps = (ServicingStep[]) steps;
      if (steps is List<ServicingStep>)
        this.Steps = ((List<ServicingStep>) steps).ToArray();
      else
        this.Steps = new List<ServicingStep>(steps).ToArray();
    }

    private ServicingStepGroup()
    {
    }

    public ServicingExecutionHandlerData[] ExecutionHandlers
    {
      get => (ServicingExecutionHandlerData[]) this.m_executionHandlers.Clone();
      set => this.m_executionHandlers = value;
    }

    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    public ServicingStep[] Steps
    {
      get => (ServicingStep[]) this.m_steps.Clone();
      set => this.m_steps = value;
    }

    public string SubsystemAttribute
    {
      get => this.m_subsystemAttribute;
      set => this.m_subsystemAttribute = value;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static ServicingStepGroup FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      ServicingStepGroup servicingStepGroup = new ServicingStepGroup();
      bool isEmptyElement = reader.IsEmptyElement;
      if (reader.HasAttributes)
      {
        while (reader.MoveToNextAttribute())
        {
          switch (reader.Name)
          {
            case "name":
              servicingStepGroup.m_name = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            case "subsystem":
              servicingStepGroup.m_subsystemAttribute = XmlUtility.StringFromXmlAttribute(reader);
              continue;
            default:
              continue;
          }
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
              servicingStepGroup.m_executionHandlers = XmlUtility.ArrayOfObjectFromXml<ServicingExecutionHandlerData>(serviceProvider, reader, "ExecutionHandler", false, ServicingStepGroup.\u003C\u003EO.\u003C0\u003E__FromXml ?? (ServicingStepGroup.\u003C\u003EO.\u003C0\u003E__FromXml = new Func<IServiceProvider, XmlReader, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.FromXml)));
              continue;
            case "Steps":
              servicingStepGroup.m_steps = Helper.ArrayOfServicingStepFromXml(serviceProvider, reader, false);
              continue;
            default:
              reader.ReadOuterXml();
              continue;
          }
        }
        reader.ReadEndElement();
      }
      return servicingStepGroup;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("ServicingStepGroup instance " + this.GetHashCode().ToString());
      stringBuilder.AppendLine("  ExecutionHandlers: " + Helper.ArrayToString<ServicingExecutionHandlerData>(this.m_executionHandlers));
      stringBuilder.AppendLine("  Name: " + this.m_name);
      stringBuilder.AppendLine("  Steps: " + Helper.ArrayToString<ServicingStep>(this.m_steps));
      stringBuilder.AppendLine("  SubsystemAttribute: " + this.m_subsystemAttribute);
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void ToXml(XmlWriter writer, string element)
    {
      writer.WriteStartElement(element);
      if (this.m_name != null)
        XmlUtility.ToXmlAttribute(writer, "name", this.m_name);
      if (this.m_subsystemAttribute != null)
        XmlUtility.ToXmlAttribute(writer, "subsystem", this.m_subsystemAttribute);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      XmlUtility.ArrayOfObjectToXml<ServicingExecutionHandlerData>(writer, this.m_executionHandlers, "ExecutionHandlers", "ExecutionHandler", false, false, ServicingStepGroup.\u003C\u003EO.\u003C1\u003E__ToXml ?? (ServicingStepGroup.\u003C\u003EO.\u003C1\u003E__ToXml = new Action<XmlWriter, string, ServicingExecutionHandlerData>(ServicingExecutionHandlerData.ToXml)));
      Helper.ToXml(writer, "Steps", this.m_steps, false, false);
      writer.WriteEndElement();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, ServicingStepGroup obj) => obj.ToXml(writer, element);
  }
}
