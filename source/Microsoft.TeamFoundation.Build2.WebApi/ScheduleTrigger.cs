// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ScheduleTrigger
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class ScheduleTrigger : BuildTrigger
  {
    [DataMember(Name = "Schedules", EmitDefaultValue = false)]
    private List<Schedule> m_schedules;

    public ScheduleTrigger()
      : this((ISecuredObject) null)
    {
    }

    internal ScheduleTrigger(ISecuredObject securedObject)
      : base(DefinitionTriggerType.Schedule, securedObject)
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
      set => this.m_schedules = value;
    }
  }
}
