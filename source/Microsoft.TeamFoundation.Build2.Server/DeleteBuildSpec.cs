// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DeleteBuildSpec
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class DeleteBuildSpec
  {
    [DataMember(Name = "ArtifactsToDelete", EmitDefaultValue = false)]
    private List<string> m_artifactsToDelete;
    [DataMember(Name = "ArtifactTypesToDelete", EmitDefaultValue = false)]
    private List<string> m_artifactTypesToDelete;

    public DeleteBuildSpec() => this.DeleteBuildRecord = true;

    [DataMember]
    public int BuildId { get; set; }

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
    }

    public List<string> ArtifactTypesToDelete
    {
      get
      {
        if (this.m_artifactTypesToDelete == null)
          this.m_artifactTypesToDelete = new List<string>();
        return this.m_artifactTypesToDelete;
      }
    }

    public string DeletedReason { get; set; }
  }
}
