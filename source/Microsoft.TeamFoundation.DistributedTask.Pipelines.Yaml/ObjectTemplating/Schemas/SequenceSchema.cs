// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.SequenceSchema
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class SequenceSchema : Schema
  {
    internal SequenceSchema()
    {
    }

    internal SequenceSchema(MappingToken mapping)
    {
      for (int index = 1; index < mapping.Count; ++index)
      {
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mapping[index];
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "sequence schema key");
        switch (literal.Value)
        {
          case "itemType":
            this.ItemType = TemplateUtil.AssertLiteral(keyValuePair.Value, "itemType").Value;
            break;
          case "transform":
            this.Transform = keyValuePair.Value;
            break;
          case "data":
            this.Data = TemplateUtil.AssertMapping(keyValuePair.Value, "data");
            break;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "sequence schema key");
            throw new ArgumentException();
        }
      }
    }

    internal string ItemType { get; set; }

    internal override SchemaType Type => SchemaType.Sequence;

    internal override void Validate(TemplateSchema templateSchema)
    {
      base.Validate(templateSchema);
      templateSchema.GetDefinition(this.ItemType);
    }
  }
}
