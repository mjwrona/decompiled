// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.CacheValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  [DataContract]
  internal sealed class CacheValue
  {
    [DataMember(Name = "Errors", EmitDefaultValue = false)]
    private PipelineValidationErrors m_errors;

    public CacheValue(TemplateToken obj, int objBytes, PipelineValidationErrors errors)
    {
      this.Obj = obj;
      this.ObjBytes = objBytes;
      this.m_errors = errors;
    }

    private CacheValue()
    {
    }

    internal PipelineValidationErrors Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new PipelineValidationErrors();
        return this.m_errors;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    internal TemplateToken Obj { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    internal int ObjBytes { get; private set; }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      PipelineValidationErrors errors = this.m_errors;
      if ((errors != null ? (errors.Count == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_errors = (PipelineValidationErrors) null;
    }
  }
}
