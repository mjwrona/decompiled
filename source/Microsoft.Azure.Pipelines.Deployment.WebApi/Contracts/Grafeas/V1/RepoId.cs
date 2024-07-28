// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.RepoId
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class RepoId
  {
    private object id;

    [DataMember(EmitDefaultValue = false)]
    public ProjectRepoId ProjectRepoId
    {
      get => this.IdCase != RepoId.IdOneofCase.ProjectRepoId ? (ProjectRepoId) null : (ProjectRepoId) this.id;
      set
      {
        this.id = (object) value;
        this.IdCase = value == null ? RepoId.IdOneofCase.None : RepoId.IdOneofCase.ProjectRepoId;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Uid
    {
      get => this.IdCase != RepoId.IdOneofCase.Uid ? "" : (string) this.id;
      set => this.IdCase = RepoId.IdOneofCase.Uid;
    }

    [DataMember(EmitDefaultValue = false)]
    public RepoId.IdOneofCase IdCase { get; private set; }

    public enum IdOneofCase
    {
      None,
      ProjectRepoId,
      Uid,
    }
  }
}
