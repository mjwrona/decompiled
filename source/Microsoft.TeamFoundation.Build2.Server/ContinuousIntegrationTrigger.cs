// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ContinuousIntegrationTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class ContinuousIntegrationTrigger : FilteredBuildTrigger
  {
    public ContinuousIntegrationTrigger()
      : base(DefinitionTriggerType.ContinuousIntegration)
    {
      this.MaxConcurrentBuildsPerBranch = 1;
    }

    [DataMember]
    public bool BatchChanges { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MaxConcurrentBuildsPerBranch { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PollingInterval { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid PollingJobId { get; set; }

    public override BuildTrigger Clone()
    {
      ContinuousIntegrationTrigger trigger = new ContinuousIntegrationTrigger();
      this.CloneInternal((BuildTrigger) trigger);
      return (BuildTrigger) trigger;
    }

    protected override BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      base.CloneInternal(trigger);
      ContinuousIntegrationTrigger integrationTrigger = trigger as ContinuousIntegrationTrigger;
      integrationTrigger.BatchChanges = this.BatchChanges;
      integrationTrigger.MaxConcurrentBuildsPerBranch = this.MaxConcurrentBuildsPerBranch;
      integrationTrigger.PollingInterval = this.PollingInterval;
      integrationTrigger.PollingJobId = this.PollingJobId;
      return trigger;
    }
  }
}
