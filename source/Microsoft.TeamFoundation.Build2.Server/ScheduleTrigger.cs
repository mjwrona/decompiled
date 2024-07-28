// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ScheduleTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class ScheduleTrigger : BuildTrigger
  {
    [DataMember(Name = "Schedules", EmitDefaultValue = false)]
    private List<Schedule> m_schedules;

    public ScheduleTrigger()
      : base(DefinitionTriggerType.Schedule)
    {
    }

    public List<Schedule> Schedules
    {
      get
      {
        if (this.m_schedules == null)
          this.m_schedules = new List<Schedule>();
        return this.m_schedules;
      }
      set
      {
        if (value == null)
          return;
        this.m_schedules = new List<Schedule>((IEnumerable<Schedule>) value);
      }
    }

    public override BuildTrigger Clone()
    {
      ScheduleTrigger trigger = new ScheduleTrigger();
      this.CloneInternal((BuildTrigger) trigger);
      return (BuildTrigger) trigger;
    }

    protected override BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      base.CloneInternal(trigger);
      (trigger as ScheduleTrigger).Schedules = this.Schedules.ConvertAll<Schedule>((Converter<Schedule, Schedule>) (schedule => schedule.Clone()));
      return trigger;
    }
  }
}
