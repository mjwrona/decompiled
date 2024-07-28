// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperation
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Operation: {Name}, Groups: {Groups.Count}, Steps: {StepCount}, Target: {Target}")]
  public class ServicingOperation
  {
    private readonly List<ServicingExecutionHandlerData> m_executionHandlers = new List<ServicingExecutionHandlerData>();
    private List<ServicingStepGroup> m_groups = new List<ServicingStepGroup>();

    [XmlAttribute("name")]
    public string Name { get; set; }

    public List<ServicingStepGroup> Groups => this.m_groups;

    [XmlIgnore]
    public int StepCount => this.Groups.Aggregate<ServicingStepGroup, int>(0, (Func<int, ServicingStepGroup, int>) ((count, stepGroup) => count + stepGroup.Steps.Count));

    public ServicingOperationTarget Target { get; set; }

    [XmlArrayItem(ElementName = "ExecutionHandler")]
    public List<ServicingExecutionHandlerData> ExecutionHandlers => this.m_executionHandlers;

    [XmlIgnore]
    internal string Resource { get; set; }
  }
}
