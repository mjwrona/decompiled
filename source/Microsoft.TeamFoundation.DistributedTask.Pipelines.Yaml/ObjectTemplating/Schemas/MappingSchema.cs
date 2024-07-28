// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.MappingSchema
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class MappingSchema : Schema
  {
    internal MappingSchema()
    {
    }

    internal MappingSchema(MappingToken mapping)
    {
      for (int index = 1; index < mapping.Count; ++index)
      {
        KeyValuePair<ScalarToken, TemplateToken> keyValuePair = mapping[index];
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "mapping schema key");
        string str1 = literal.Value;
        if (str1 != null)
        {
          switch (str1.Length)
          {
            case 4:
              switch (str1[0])
              {
                case 'd':
                  if (str1 == "data")
                  {
                    this.Data = TemplateUtil.AssertMapping(keyValuePair.Value, "data");
                    continue;
                  }
                  break;
                case 'l':
                  if (str1 == "load")
                  {
                    this.Load = TemplateUtil.AssertLiteral(keyValuePair.Value, "load").Value;
                    continue;
                  }
                  break;
              }
              break;
            case 8:
              switch (str1[0])
              {
                case 'f':
                  if (str1 == "firstKey")
                  {
                    this.FirstKey = TemplateUtil.AssertLiteral(keyValuePair.Value, "firstKey").Value;
                    continue;
                  }
                  break;
                case 'i':
                  if (str1 == "inherits")
                  {
                    this.Inherits = TemplateUtil.AssertLiteral(keyValuePair.Value, "inherits").Value;
                    continue;
                  }
                  break;
              }
              break;
            case 9:
              if (str1 == "transform")
              {
                this.Transform = keyValuePair.Value;
                continue;
              }
              break;
            case 10:
              if (str1 == "properties")
              {
                using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair.Value, "properties").GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    MappingToken mappingToken = TemplateUtil.AssertMapping(enumerator.Current, "name");
                    TemplateUtil.AssertNotEmpty(mappingToken, "properties");
                    string str2 = TemplateUtil.AssertLiteral(mappingToken[0].Value, "name first value").Value;
                    PropertyValue propertyValue = new PropertyValue(mappingToken, str2);
                    this.Properties.Add(str2, propertyValue);
                  }
                  continue;
                }
              }
              else
                break;
            case 12:
              if (str1 == "looseKeyType")
              {
                this.LooseKeyType = TemplateUtil.AssertLiteral(keyValuePair.Value, "looseKeyType").Value;
                continue;
              }
              break;
            case 14:
              if (str1 == "looseValueType")
              {
                this.LooseValueType = TemplateUtil.AssertLiteral(keyValuePair.Value, "looseValueType").Value;
                continue;
              }
              break;
          }
        }
        TemplateUtil.AssertUnexpectedValue(literal, "mapping schema key");
      }
    }

    internal string FirstKey { get; set; }

    internal string Inherits { get; set; }

    internal string LooseKeyType { get; set; }

    internal string LooseValueType { get; set; }

    internal Dictionary<string, PropertyValue> Properties { get; } = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) StringComparer.Ordinal);

    internal override SchemaType Type => SchemaType.Mapping;

    internal override void Validate(TemplateSchema templateSchema)
    {
      base.Validate(templateSchema);
      if (!string.IsNullOrEmpty(this.FirstKey) && !this.Properties.TryGetValue(this.FirstKey, out PropertyValue _))
        throw new ArgumentException("Property 'firstKey' is defined but does not exist in the 'properties' mapping");
      if (!string.IsNullOrEmpty(this.LooseKeyType))
      {
        templateSchema.GetDefinition(this.LooseKeyType);
        if (string.IsNullOrEmpty(this.LooseValueType))
          throw new ArgumentException("Property 'looseKeyType' is defined but 'looseValueType' is not defined");
        templateSchema.GetDefinition(this.LooseValueType);
      }
      else if (!string.IsNullOrEmpty(this.LooseValueType))
        throw new ArgumentException("Property 'looseValueType' is defined but 'looseKeyType' is not defined");
      foreach (PropertyValue propertyValue in this.Properties.Values)
        templateSchema.GetDefinition(propertyValue.Type);
      if (string.IsNullOrEmpty(this.Inherits))
        return;
      Definition definition = templateSchema.GetDefinition(this.Inherits);
      Dictionary<string, string> dictionary = !definition.AllowExpressionsInSubTree ? definition.Events : throw new NotSupportedException("Property 'allowExpressionsInSubTree' is not supported on inhertied definitions");
      // ISSUE: explicit non-virtual call
      if ((dictionary != null ? (__nonvirtual (dictionary.Count) > 0 ? 1 : 0) : 0) != 0)
        throw new NotSupportedException("Property 'events' is not supported on inherited definitions");
      if (definition.Schemas.Count != 1)
        throw new NotSupportedException(string.Format("Expected exactly 1 schema on inherited definition. Actual '{0}'", (object) definition.Schemas.Count));
      if (!(definition.Schemas[0] is MappingSchema schema))
        throw new NotSupportedException("Only mapping schemas are supported for inherited definitions");
      if (!string.IsNullOrEmpty(schema.FirstKey))
        throw new NotSupportedException("Property 'firstKey' is not supported on inherited schemas");
      if (!string.IsNullOrEmpty(schema.Inherits))
        throw new NotSupportedException("Property 'inherits' is not supported on inherited schemas");
      if (!string.IsNullOrEmpty(schema.Load))
        throw new NotSupportedException("Property 'load' is not supported on inherited schemas");
      if (!string.IsNullOrEmpty(schema.LooseKeyType))
        throw new NotSupportedException("Property 'looseKeyType' is not supported on inherited schemas");
      if (!string.IsNullOrEmpty(schema.LooseValueType))
        throw new NotSupportedException("Property 'looseValueType' is not supported on inherited schemas");
      if (schema.Transform != null)
        throw new NotSupportedException("Property 'transform' is not supported on inherited schemas");
    }
  }
}
