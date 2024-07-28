// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ContinuousIntegrationTrigger
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public sealed class ContinuousIntegrationTrigger : BuildTrigger
  {
    [DataMember(Name = "BranchFilters", EmitDefaultValue = false)]
    private List<string> m_branchFilters;
    [DataMember(Name = "PathFilters", EmitDefaultValue = false)]
    private List<string> m_pathFilters;
    [DataMember(Name = "SettingsSourceType", EmitDefaultValue = false)]
    private int m_settingsSourceType;

    public ContinuousIntegrationTrigger()
      : this((ISecuredObject) null)
    {
    }

    internal ContinuousIntegrationTrigger(ISecuredObject securedObject)
      : base(DefinitionTriggerType.ContinuousIntegration, securedObject)
    {
      this.MaxConcurrentBuildsPerBranch = 1;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
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

    [DataMember]
    public bool BatchChanges { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int MaxConcurrentBuildsPerBranch { get; set; }

    public List<string> BranchFilters
    {
      get
      {
        if (this.m_branchFilters == null)
          this.m_branchFilters = new List<string>();
        return this.m_branchFilters;
      }
      internal set => this.m_branchFilters = value;
    }

    public List<string> PathFilters
    {
      get
      {
        if (this.m_pathFilters == null)
          this.m_pathFilters = new List<string>();
        return this.m_pathFilters;
      }
      internal set => this.m_pathFilters = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int? PollingInterval { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid PollingJobId { get; set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.m_settingsSourceType != 1)
        return;
      this.m_settingsSourceType = 0;
    }
  }
}
