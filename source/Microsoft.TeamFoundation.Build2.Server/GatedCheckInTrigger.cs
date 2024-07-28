// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GatedCheckInTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class GatedCheckInTrigger : BuildTrigger
  {
    [DataMember(Name = "PathFilters", EmitDefaultValue = false)]
    private List<string> m_pathFilters;

    public GatedCheckInTrigger()
      : base(DefinitionTriggerType.GatedCheckIn)
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public bool RunContinuousIntegration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool UseWorkspaceMappings { get; set; }

    public List<string> PathFilters
    {
      get
      {
        if (this.m_pathFilters == null)
          this.m_pathFilters = new List<string>();
        return this.m_pathFilters;
      }
      set
      {
        if (value == null)
          return;
        this.m_pathFilters = new List<string>((IEnumerable<string>) value);
      }
    }

    public override BuildTrigger Clone()
    {
      GatedCheckInTrigger trigger = new GatedCheckInTrigger();
      this.CloneInternal((BuildTrigger) trigger);
      return (BuildTrigger) trigger;
    }

    protected override BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      base.CloneInternal(trigger);
      GatedCheckInTrigger gatedCheckInTrigger = trigger as GatedCheckInTrigger;
      gatedCheckInTrigger.RunContinuousIntegration = this.RunContinuousIntegration;
      gatedCheckInTrigger.UseWorkspaceMappings = this.UseWorkspaceMappings;
      gatedCheckInTrigger.PathFilters = this.PathFilters.ConvertAll<string>((Converter<string, string>) (filter => filter));
      return trigger;
    }
  }
}
