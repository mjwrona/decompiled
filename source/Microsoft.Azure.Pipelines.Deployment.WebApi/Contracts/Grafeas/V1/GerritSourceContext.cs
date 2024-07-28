// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.GerritSourceContext
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class GerritSourceContext
  {
    [DataMember(EmitDefaultValue = false)]
    public string HostUri;
    [DataMember(EmitDefaultValue = false)]
    public string GerritProject;
    private object revision;

    [DataMember(EmitDefaultValue = false)]
    public string RevisionId
    {
      get => this.RevisionCase != GerritSourceContext.RevisionOneofCase.RevisionId ? "" : (string) this.revision;
      set
      {
        this.revision = (object) value;
        this.RevisionCase = value == null ? GerritSourceContext.RevisionOneofCase.None : GerritSourceContext.RevisionOneofCase.RevisionId;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public AliasContext AliasContext
    {
      get => this.RevisionCase != GerritSourceContext.RevisionOneofCase.AliasContext ? (AliasContext) null : (AliasContext) this.revision;
      set
      {
        this.revision = (object) value;
        this.RevisionCase = value == null ? GerritSourceContext.RevisionOneofCase.None : GerritSourceContext.RevisionOneofCase.AliasContext;
      }
    }

    public GerritSourceContext.RevisionOneofCase RevisionCase { get; private set; }

    public enum RevisionOneofCase
    {
      None = 0,
      RevisionId = 3,
      AliasContext = 4,
    }
  }
}
