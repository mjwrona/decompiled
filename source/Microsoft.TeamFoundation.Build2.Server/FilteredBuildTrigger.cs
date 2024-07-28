// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.FilteredBuildTrigger
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public abstract class FilteredBuildTrigger : BuildTrigger
  {
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;
    [DataMember(Name = "PathFilters", EmitDefaultValue = false)]
    private List<string> m_pathFilters;
    [DataMember(Name = "SettingsSourceType", EmitDefaultValue = false)]
    private int m_settingsSourceType;

    protected FilteredBuildTrigger(DefinitionTriggerType triggerType)
      : base(triggerType)
    {
    }

    public int SettingsSourceType
    {
      get
      {
        if (this.m_settingsSourceType == 0)
          this.m_settingsSourceType = 1;
        return this.m_settingsSourceType;
      }
      set => this.m_settingsSourceType = value;
    }

    public List<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return this.m_branchFilters;
      }
      set
      {
        if (value == null)
          return;
        this.m_branchFilters = new List<string>((IEnumerable<string>) value);
      }
    }

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

    protected override BuildTrigger CloneInternal(BuildTrigger trigger)
    {
      base.CloneInternal(trigger);
      FilteredBuildTrigger filteredBuildTrigger = trigger as FilteredBuildTrigger;
      filteredBuildTrigger.SettingsSourceType = this.SettingsSourceType;
      filteredBuildTrigger.BranchFilters = this.BranchFilters.ConvertAll<string>((Converter<string, string>) (filter => filter));
      filteredBuildTrigger.PathFilters = this.PathFilters.ConvertAll<string>((Converter<string, string>) (filter => filter));
      return trigger;
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.m_settingsSourceType != 1)
        return;
      this.m_settingsSourceType = 0;
    }
  }
}
