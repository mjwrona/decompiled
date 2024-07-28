// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.ScalarSchema
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class ScalarSchema : Schema
  {
    internal ScalarSchema()
    {
    }

    internal ScalarSchema(MappingToken mapping)
    {
      for (int index = 1; index < mapping.Count; ++index)
      {
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mapping[index];
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "scalar schema key");
        switch (literal.Value)
        {
          case "constant":
            this.Constant = TemplateUtil.AssertLiteral(keyValuePair.Value, "constant").Value;
            break;
          case "ignoreCase":
            this.IgnoreCase = ScalarSchema.ConvertToBoolean(keyValuePair.Value, "ignoreCase");
            break;
          case "requireNonEmpty":
            this.RequireNonEmpty = ScalarSchema.ConvertToBoolean(keyValuePair.Value, "requireNonEmpty");
            break;
          case "transform":
            this.Transform = keyValuePair.Value;
            break;
          case "data":
            this.Data = TemplateUtil.AssertMapping(keyValuePair.Value, "data");
            break;
          default:
            TemplateUtil.AssertUnexpectedValue(literal, "scalar schema key");
            throw new ArgumentException();
        }
      }
    }

    internal string Constant { get; set; }

    internal bool IgnoreCase { get; set; }

    internal bool RequireNonEmpty { get; set; }

    internal override SchemaType Type => SchemaType.Scalar;

    internal bool IsMatch(LiteralToken literal)
    {
      if (!string.IsNullOrEmpty(this.Constant))
      {
        StringComparison comparisonType = this.IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (string.Equals(this.Constant, literal.Value, comparisonType))
          return true;
      }
      else if (!this.RequireNonEmpty || !string.IsNullOrEmpty(literal.Value))
        return true;
      return false;
    }

    internal override void Validate(TemplateSchema templateSchema)
    {
      base.Validate(templateSchema);
      if (!string.IsNullOrEmpty(this.Constant) && this.RequireNonEmpty)
        throw new ArgumentException(string.Format("Properties '{0}' and '{1}' cannot both be set", (object) this.Constant, (object) this.RequireNonEmpty));
    }

    private static bool ConvertToBoolean(TemplateToken value, string objectDescription)
    {
      LiteralToken literal = TemplateUtil.AssertLiteral(value, objectDescription);
      switch (literal.Value)
      {
        case "true":
          return true;
        case "false":
          return false;
        default:
          TemplateUtil.AssertUnexpectedValue(literal, objectDescription);
          throw new ArgumentException();
      }
    }
  }
}
