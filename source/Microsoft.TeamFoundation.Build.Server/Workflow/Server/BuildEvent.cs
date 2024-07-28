// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Workflow.Server.BuildEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.Workflow.Server
{
  [DataContract(Namespace = "http://www.tempuri.org/Microsoft/TeamFoundation/Build/07")]
  public sealed class BuildEvent
  {
    [DataMember(Name = "InstanceId", IsRequired = true, EmitDefaultValue = true)]
    private Guid m_instanceId = Guid.Empty;
    [DataMember(Name = "EventType", IsRequired = true, EmitDefaultValue = true)]
    private string m_eventType;
    [DataMember(Name = "Activity", EmitDefaultValue = true)]
    private string m_activity;
    [DataMember(Name = "ContextId", EmitDefaultValue = true)]
    private Guid m_contextId = Guid.Empty;

    private BuildEvent()
    {
    }

    public override string ToString() => "BuildEvent instance " + this.GetHashCode().ToString() + "\r\n  InstanceId: " + this.m_instanceId.ToString() + "\r\n  EventType: " + this.m_eventType + "\r\n  Activity: " + this.m_activity + "\r\n  ContextId: " + this.m_contextId.ToString() + "\r\n";
  }
}
