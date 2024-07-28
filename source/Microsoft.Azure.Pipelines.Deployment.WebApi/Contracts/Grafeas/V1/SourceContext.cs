// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1.SourceContext
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi.Contracts.Grafeas.V1
{
  [DataContract]
  public class SourceContext
  {
    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> Labels;
    private object context;

    [DataMember(EmitDefaultValue = false)]
    public CloudRepoSourceContext CloudRepo
    {
      get => this.ContextCase != SourceContext.ContextOneofCase.CloudRepo ? (CloudRepoSourceContext) null : (CloudRepoSourceContext) this.context;
      set
      {
        this.context = (object) value;
        this.ContextCase = value == null ? SourceContext.ContextOneofCase.None : SourceContext.ContextOneofCase.CloudRepo;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public GerritSourceContext Gerrit
    {
      get => this.ContextCase != SourceContext.ContextOneofCase.Gerrit ? (GerritSourceContext) null : (GerritSourceContext) this.context;
      set
      {
        this.context = (object) value;
        this.ContextCase = value == null ? SourceContext.ContextOneofCase.None : SourceContext.ContextOneofCase.Gerrit;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public GitSourceContext Git
    {
      get => this.ContextCase != SourceContext.ContextOneofCase.Git ? (GitSourceContext) null : (GitSourceContext) this.context;
      set
      {
        this.context = (object) value;
        this.ContextCase = value == null ? SourceContext.ContextOneofCase.None : SourceContext.ContextOneofCase.Git;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public SourceContext.ContextOneofCase ContextCase { get; private set; }

    [DataContract]
    public enum ContextOneofCase
    {
      None,
      CloudRepo,
      Gerrit,
      Git,
    }
  }
}
