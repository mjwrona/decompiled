// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RetentionPolicy
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class RetentionPolicy
  {
    [DataMember(Name = "Branches", EmitDefaultValue = false)]
    private List<string> m_branches;
    [DataMember(Name = "Artifacts", EmitDefaultValue = false)]
    private List<string> m_artifactsToDelete;
    [DataMember(Name = "ArtifactTypesToDelete", EmitDefaultValue = false)]
    private List<string> m_artifactTypesToDelete;
    private int m_daysToKeep;
    private int m_minimumToKeep;

    public RetentionPolicy()
    {
      this.DaysToKeep = 30;
      this.MinimumToKeep = 1;
      this.DeleteBuildRecord = true;
      this.DeleteTestResults = false;
    }

    public List<string> Branches
    {
      get
      {
        if (this.m_branches == null)
          this.m_branches = new List<string>();
        return this.m_branches;
      }
      set => this.m_branches = value;
    }

    [DataMember]
    public int DaysToKeep
    {
      get => this.m_daysToKeep;
      set
      {
        if (value < 0)
          this.m_daysToKeep = 0;
        else
          this.m_daysToKeep = value;
      }
    }

    [DataMember]
    public int MinimumToKeep
    {
      get => this.m_minimumToKeep;
      set
      {
        if (value < 0)
          this.m_minimumToKeep = 0;
        else
          this.m_minimumToKeep = value;
      }
    }

    [DataMember]
    public bool DeleteBuildRecord { get; set; }

    public List<string> ArtifactsToDelete
    {
      get
      {
        if (this.m_artifactsToDelete == null)
          this.m_artifactsToDelete = new List<string>();
        return this.m_artifactsToDelete;
      }
      set => this.m_artifactsToDelete = value;
    }

    public List<string> ArtifactTypesToDelete
    {
      get
      {
        if (this.m_artifactTypesToDelete == null)
          this.m_artifactTypesToDelete = new List<string>();
        return this.m_artifactTypesToDelete;
      }
      set => this.m_artifactTypesToDelete = value;
    }

    [DataMember]
    public bool DeleteTestResults { get; set; }

    public RetentionPolicy Clone() => new RetentionPolicy()
    {
      Branches = this.Branches.ConvertAll<string>((Converter<string, string>) (branch => branch)),
      DaysToKeep = this.DaysToKeep,
      MinimumToKeep = this.MinimumToKeep,
      DeleteBuildRecord = this.DeleteBuildRecord,
      ArtifactsToDelete = this.ArtifactsToDelete.ConvertAll<string>((Converter<string, string>) (artifact => artifact)),
      ArtifactTypesToDelete = this.ArtifactTypesToDelete.ConvertAll<string>((Converter<string, string>) (artifact => artifact)),
      DeleteTestResults = this.DeleteTestResults
    };
  }
}
