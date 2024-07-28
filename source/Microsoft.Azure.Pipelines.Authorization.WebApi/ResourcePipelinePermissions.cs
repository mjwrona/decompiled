// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.WebApi.ResourcePipelinePermissions
// Assembly: Microsoft.Azure.Pipelines.Authorization.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4807FD31-F2A4-4329-AA76-35B262BDA671
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.WebApi.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Authorization.WebApi
{
  [DataContract]
  public class ResourcePipelinePermissions
  {
    private List<PipelinePermission> m_pipelines;

    [DataMember(EmitDefaultValue = false)]
    public Resource Resource { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Permission AllPipelines { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<PipelinePermission> Pipelines
    {
      get => this.m_pipelines ?? (this.m_pipelines = new List<PipelinePermission>());
      set => this.m_pipelines = value;
    }
  }
}
