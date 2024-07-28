// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.CloudRepoSourceContext
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class CloudRepoSourceContext
  {
    [DataMember(EmitDefaultValue = false)]
    public RepoId RepoId;
    private object revision;

    [DataMember(EmitDefaultValue = false)]
    public string RevisionId
    {
      get => this.RevisionCase != CloudRepoSourceContext.RevisionOneofCase.RevisionId ? "" : (string) this.revision;
      set
      {
        this.revision = (object) value;
        this.RevisionCase = value == null ? CloudRepoSourceContext.RevisionOneofCase.None : CloudRepoSourceContext.RevisionOneofCase.RevisionId;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public AliasContext AliasContext
    {
      get => this.RevisionCase != CloudRepoSourceContext.RevisionOneofCase.AliasContext ? (AliasContext) null : (AliasContext) this.revision;
      set
      {
        this.revision = (object) value;
        this.RevisionCase = value == null ? CloudRepoSourceContext.RevisionOneofCase.None : CloudRepoSourceContext.RevisionOneofCase.AliasContext;
      }
    }

    public CloudRepoSourceContext.RevisionOneofCase RevisionCase { get; private set; }

    public enum RevisionOneofCase
    {
      None = 0,
      RevisionId = 2,
      AliasContext = 3,
    }
  }
}
