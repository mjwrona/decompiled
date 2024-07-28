// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.PropertyValue
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class PropertyValue
  {
    internal PropertyValue()
    {
    }

    internal PropertyValue(string type) => this.Type = type;

    internal PropertyValue(MappingToken property, string propertyName)
    {
      TemplateUtil.AssertNotEmpty(property, "name");
      LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) property[0].Key, "name first key");
      if (!string.Equals(literal1.Value, "name", StringComparison.Ordinal))
        TemplateUtil.AssertUnexpectedValue(literal1, "name first key");
      for (int index = 1; index < property.Count; ++index)
      {
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = property[index];
        LiteralToken literal2 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "name mapping schema key");
        switch (literal2.Value)
        {
          case "type":
            this.Type = TemplateUtil.AssertLiteral(keyValuePair.Value, "type").Value;
            break;
          case "data":
            this.Data = TemplateUtil.AssertMapping(keyValuePair.Value, "data");
            break;
          default:
            TemplateUtil.AssertUnexpectedValue(literal2, "name mapping schema key");
            break;
        }
      }
      if (string.IsNullOrEmpty(this.Type))
        throw new ArgumentException("Unexpected property without type encountered: '" + propertyName + "'");
    }

    internal string Type { get; set; }

    internal MappingToken Data { get; set; }
  }
}
