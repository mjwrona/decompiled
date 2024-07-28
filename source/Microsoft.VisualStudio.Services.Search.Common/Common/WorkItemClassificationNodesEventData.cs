// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.WorkItemClassificationNodesEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class WorkItemClassificationNodesEventData : ChangeEventData
  {
    private WorkItemClassificationNodesEventData()
    {
    }

    public WorkItemClassificationNodesEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public EventType EventType { get; set; }

    [DataMember]
    public int NodeId { get; set; }

    [DataMember]
    public ClassificationNodeType NodeType { get; set; }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("{0}:[{1}], {2}:[{3}], {4}:[{5}], {6}:[{7}]", (object) "ProjectId", (object) this.ProjectId, (object) "EventType", (object) this.EventType, (object) "NodeId", (object) this.NodeId, (object) this.NodeType, (object) this.NodeType));
  }
}
