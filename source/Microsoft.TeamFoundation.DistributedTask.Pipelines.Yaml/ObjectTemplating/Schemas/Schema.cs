// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal abstract class Schema
  {
    internal string Load { get; set; }

    internal TemplateToken Transform { get; set; }

    internal abstract SchemaType Type { get; }

    internal MappingToken Data { get; set; }

    internal virtual void Validate(TemplateSchema templateSchema)
    {
      if (this.Transform != null && !string.IsNullOrEmpty(this.Load))
        throw new ArgumentException("Properties 'transform' and 'load' cannot both be set");
    }
  }
}
