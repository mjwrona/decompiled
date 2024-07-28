// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingStepGroup
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Name: {Name}, Steps: {Steps.Count}")]
  public class ServicingStepGroup
  {
    private readonly List<ServicingExecutionHandlerData> m_executionHandlers = new List<ServicingExecutionHandlerData>();
    private List<ServicingStep> m_steps;

    internal ServicingStepGroup() => this.m_steps = new List<ServicingStep>();

    public ServicingStepGroup(string name, List<ServicingStep> steps)
    {
      this.Name = name;
      this.m_steps = steps;
      if (this.Steps != null)
        return;
      this.m_steps = new List<ServicingStep>();
    }

    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlArrayItem(ElementName = "ExecutionHandler")]
    public List<ServicingExecutionHandlerData> ExecutionHandlers => this.m_executionHandlers;

    public List<ServicingStep> Steps => this.m_steps;

    internal static ServicingStepGroup FromXml(Stream servicingStepXmlStream) => (ServicingStepGroup) new XmlSerializer(typeof (ServicingStepGroup)).Deserialize(servicingStepXmlStream);

    [XmlIgnore]
    internal string Resource { get; set; }
  }
}
