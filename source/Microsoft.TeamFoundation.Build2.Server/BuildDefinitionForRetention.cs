// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionForRetention
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionForRetention
  {
    private BuildRepository m_repository;
    private List<RetentionPolicy> m_retentionRules;

    public int DefinitionId { get; set; }

    public string DefinitionName { get; set; }

    public int ProcessType { get; set; }

    public Guid ProjectId { get; set; }

    public string RepositoryString { get; set; }

    public bool Deleted { get; set; }

    public string RetentionRulesString { get; set; }

    public BuildRepository Repository
    {
      get
      {
        if (this.m_repository == null)
          this.SummonBuildRepository();
        return this.m_repository;
      }
      set => this.m_repository = value;
    }

    private void SummonBuildRepository()
    {
      if (string.IsNullOrEmpty(this.RepositoryString))
        return;
      this.m_repository = JsonUtility.FromString<BuildRepository>(this.RepositoryString);
    }

    public List<RetentionPolicy> RetentionRules
    {
      get
      {
        if (this.m_retentionRules == null)
          this.SummonRetentionRules();
        return this.m_retentionRules;
      }
      set => this.m_retentionRules = value;
    }

    private void SummonRetentionRules()
    {
      if (!string.IsNullOrEmpty(this.RetentionRulesString))
        this.m_retentionRules = JsonUtility.FromString<List<RetentionPolicy>>(this.RetentionRulesString);
      else
        this.m_retentionRules = new List<RetentionPolicy>();
    }
  }
}
