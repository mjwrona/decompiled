// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Definition
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class Definition
  {
    internal Definition()
    {
    }

    internal Definition(TemplateToken value)
    {
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair1 in TemplateUtil.AssertMapping(value, "definition"))
      {
        LiteralToken literal1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair1.Key, "definition key");
        switch (literal1.Value)
        {
          case "allowExpressionsInSubTree":
            LiteralToken literal2 = TemplateUtil.AssertLiteral(keyValuePair1.Value, "allowExpressionsInSubTree");
            switch (literal2.Value)
            {
              case "true":
                this.AllowExpressionsInSubTree = true;
                continue;
              case "false":
                continue;
              default:
                TemplateUtil.AssertUnexpectedValue(literal2, "allowExpressionsInSubTree");
                continue;
            }
          case "events":
            Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair2 in TemplateUtil.AssertMapping(keyValuePair1.Value, "events"))
            {
              LiteralToken literalToken1 = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair2.Key, "events key");
              LiteralToken literalToken2 = TemplateUtil.AssertLiteral(keyValuePair2.Value, "events value");
              dictionary[literalToken1.Value] = literalToken2.Value;
            }
            this.Events = dictionary;
            continue;
          case "schemas":
            using (IEnumerator<TemplateToken> enumerator = TemplateUtil.AssertSequence(keyValuePair1.Value, "schemas").GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                MappingToken mapping = TemplateUtil.AssertMapping(enumerator.Current, "schema");
                TemplateUtil.AssertNotEmpty(mapping, "schema");
                LiteralToken literal3 = TemplateUtil.AssertLiteral((TemplateToken) mapping[0].Key, "schema first key");
                if (!string.Equals(literal3.Value, "structure", StringComparison.Ordinal))
                  TemplateUtil.AssertUnexpectedValue(literal3, "schema first key");
                LiteralToken literal4 = TemplateUtil.AssertLiteral(mapping[0].Value, "schema first value");
                switch (literal4.Value)
                {
                  case "scalar":
                    this.Schemas.Add((Schema) new ScalarSchema(mapping));
                    continue;
                  case "sequence":
                    this.Schemas.Add((Schema) new SequenceSchema(mapping));
                    continue;
                  case "mapping":
                    this.Schemas.Add((Schema) new MappingSchema(mapping));
                    continue;
                  default:
                    TemplateUtil.AssertUnexpectedValue(literal4, "schema first value");
                    throw new ArgumentException();
                }
              }
              continue;
            }
          case "data":
            this.Data = TemplateUtil.AssertMapping(keyValuePair1.Value, "data");
            continue;
          default:
            TemplateUtil.AssertUnexpectedValue(literal1, "definition key");
            continue;
        }
      }
    }

    internal bool AllowExpressionsInSubTree { get; set; }

    internal Dictionary<string, string> Events { get; set; }

    internal List<Schema> Schemas { get; } = new List<Schema>();

    internal MappingToken Data { get; set; }

    internal void Validate(TemplateSchema templateSchema)
    {
      if (this.Schemas.Count == 0)
        throw new ArgumentException("At least one schema must be defined");
      bool flag1 = false;
      bool flag2 = false;
      bool flag3 = false;
      List<MappingSchema> source = (List<MappingSchema>) null;
      SequenceSchema sequenceSchema1 = (SequenceSchema) null;
      foreach (Schema schema in this.Schemas)
      {
        schema.Validate(templateSchema);
        if (schema is MappingSchema mappingSchema)
        {
          if (source == null)
            source = new List<MappingSchema>();
          source.Add(mappingSchema);
          if (!string.IsNullOrEmpty(mappingSchema.FirstKey))
            flag1 = true;
          else
            flag2 = true;
          if (!string.IsNullOrEmpty(mappingSchema.LooseKeyType))
            flag3 = true;
        }
        else if (schema is SequenceSchema sequenceSchema2)
          sequenceSchema1 = sequenceSchema1 == null ? sequenceSchema2 : throw new ArgumentException("More than one 'sequence' schema cannot be defined");
      }
      // ISSUE: explicit non-virtual call
      if (source == null || __nonvirtual (source.Count) <= 1)
        return;
      if (flag1)
      {
        if (flag2)
          throw new ArgumentException("A definition cannot have a mix of mapping schemas with 'firstKey' set and unset");
        foreach (List<MappingSchema> mappingSchemaList in source.GroupBy<MappingSchema, string>((Func<MappingSchema, string>) (x => x.FirstKey), (IEqualityComparer<string>) StringComparer.Ordinal).Select<IGrouping<string, MappingSchema>, List<MappingSchema>>((Func<IGrouping<string, MappingSchema>, List<MappingSchema>>) (x => x.ToList<MappingSchema>())).Where<List<MappingSchema>>((Func<List<MappingSchema>, bool>) (x => x.Count > 1)))
        {
          bool flag4 = false;
          Dictionary<string, PropertyValue> dictionary = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) StringComparer.Ordinal);
          foreach (MappingSchema mappingSchema in mappingSchemaList)
          {
            if (!string.IsNullOrEmpty(mappingSchema.LooseKeyType))
              flag4 = true;
            foreach (KeyValuePair<string, PropertyValue> mergedProperty in Definition.GetMergedProperties(templateSchema, mappingSchema))
            {
              PropertyValue propertyValue;
              if (dictionary.TryGetValue(mergedProperty.Key, out propertyValue))
              {
                if (!string.Equals(propertyValue.Type, mergedProperty.Value.Type, StringComparison.Ordinal))
                  throw new ArgumentException("Property '" + mergedProperty.Key + "' is defined for two mappings with the same firstKey but with different types");
              }
              else
                dictionary.Add(mergedProperty.Key, mergedProperty.Value);
            }
          }
          if (flag4)
            throw new ArgumentException("Property 'looseKeyType' cannot be set for two mappings with the same firstKey");
        }
      }
      else
      {
        if (flag3)
          throw new ArgumentException("Property 'looseKeyType' cannot be set for two mappings without a firstKey");
        Dictionary<string, PropertyValue> dictionary = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) StringComparer.Ordinal);
        foreach (MappingSchema mappingSchema in source)
        {
          foreach (KeyValuePair<string, PropertyValue> mergedProperty in Definition.GetMergedProperties(templateSchema, mappingSchema))
          {
            PropertyValue propertyValue;
            if (dictionary.TryGetValue(mergedProperty.Key, out propertyValue))
            {
              if (!string.Equals(propertyValue.Type, mergedProperty.Value.Type, StringComparison.Ordinal))
                throw new ArgumentException("Property '" + mergedProperty.Key + "' is defined for two mappings with the same firstKey but with different types");
            }
            else
              dictionary.Add(mergedProperty.Key, mergedProperty.Value);
          }
        }
      }
    }

    private static IEnumerable<KeyValuePair<string, PropertyValue>> GetMergedProperties(
      TemplateSchema templateSchema,
      MappingSchema mappingSchema)
    {
      foreach (KeyValuePair<string, PropertyValue> property in mappingSchema.Properties)
        yield return property;
      if (!string.IsNullOrEmpty(mappingSchema.Inherits))
      {
        MappingSchema mappingSchema1 = templateSchema.GetDefinition(mappingSchema.Inherits).Schemas.Cast<MappingSchema>().Single<MappingSchema>();
        if (!string.IsNullOrEmpty(mappingSchema1.Inherits))
          throw new NotSupportedException("Multiple levels of inheritance is not supported");
        foreach (KeyValuePair<string, PropertyValue> property in mappingSchema1.Properties)
        {
          if (!mappingSchema.Properties.TryGetValue(property.Key, out PropertyValue _))
            yield return property;
        }
      }
    }
  }
}
