// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.TemplateSchema
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas
{
  internal sealed class TemplateSchema
  {
    private static TemplateSchema s_schema;

    internal TemplateSchema()
    {
      Definition definition1 = new Definition();
      ScalarSchema scalarSchema1 = new ScalarSchema();
      definition1.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema1);
      this.Definitions.Add("string", definition1);
      Definition definition2 = new Definition();
      SequenceSchema sequenceSchema1 = new SequenceSchema()
      {
        ItemType = "any"
      };
      definition2.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema1);
      this.Definitions.Add("sequence", definition2);
      Definition definition3 = new Definition();
      MappingSchema mappingSchema1 = new MappingSchema()
      {
        LooseKeyType = "string",
        LooseValueType = "any"
      };
      definition3.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema1);
      this.Definitions.Add("mapping", definition3);
      Definition definition4 = new Definition();
      ScalarSchema scalarSchema2 = new ScalarSchema();
      definition4.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema2);
      SequenceSchema sequenceSchema2 = new SequenceSchema()
      {
        ItemType = "any"
      };
      definition4.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema2);
      MappingSchema mappingSchema2 = new MappingSchema()
      {
        LooseKeyType = "string",
        LooseValueType = "any"
      };
      definition4.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema2);
      this.Definitions.Add("any", definition4);
    }

    private TemplateSchema(TemplateToken value)
      : this()
    {
      foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair1 in TemplateUtil.AssertMapping(value, "templateSchema"))
      {
        LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair1.Key, "templateSchema key");
        if (string.Equals(literal.Value, "definitions", StringComparison.Ordinal))
        {
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair2 in TemplateUtil.AssertMapping(keyValuePair1.Value, "definitions"))
            this.Definitions.Add(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair2.Key, "definitions key").Value, new Definition(keyValuePair2.Value));
        }
        else if (string.Equals(literal.Value, "transforms", StringComparison.Ordinal))
        {
          foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair3 in TemplateUtil.AssertMapping(keyValuePair1.Value, "transforms"))
            this.Transforms.Add(TemplateUtil.AssertLiteral((TemplateToken) keyValuePair3.Key, "transform key").Value, keyValuePair3.Value);
        }
        else
          TemplateUtil.AssertUnexpectedValue(literal, "templateSchema key");
      }
    }

    internal Dictionary<string, Definition> Definitions { get; } = new Dictionary<string, Definition>((IEqualityComparer<string>) StringComparer.Ordinal);

    internal Dictionary<string, TemplateToken> Transforms { get; set; } = new Dictionary<string, TemplateToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static TemplateSchema Load(
      TemplateContext context,
      IObjectReader objectReader,
      int? fileId)
    {
      bool includeFileContentInErrors = true;
      TemplateToken templateToken = TemplateReader.Read(context, "templateSchema", objectReader, fileId, TemplateSchema.Schema, includeFileContentInErrors, out int _);
      if (context.Errors.Count > 0)
        throw new PipelineValidationException((IEnumerable<PipelineValidationError>) context.Errors);
      TemplateSchema templateSchema = new TemplateSchema(templateToken);
      templateSchema.Validate();
      return templateSchema;
    }

    internal Definition GetDefinition(string type)
    {
      Definition definition;
      if (this.Definitions.TryGetValue(type, out definition))
        return definition;
      throw new ArgumentException("Schema definition '" + type + "' not found");
    }

    internal string GetProperty(MappingSchema schema, string name)
    {
      string type;
      if (this.TryGetProperty(schema, name, out type))
        return type;
      throw new ArgumentException("Property '" + name + "' not found");
    }

    internal bool HasProperties(MappingSchema schema)
    {
      for (int index = 0; index < 10; ++index)
      {
        if (schema.Properties.Count > 0)
          return true;
        if (string.IsNullOrEmpty(schema.Inherits))
          return false;
        schema = this.GetDefinition(schema.Inherits).Schemas.Single<Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema>() as MappingSchema;
      }
      throw new InvalidOperationException("Inheritance depth exceeded 10");
    }

    internal bool TryGetProperty(MappingSchema schema, string name, out string type)
    {
      for (int index = 0; index < 10; ++index)
      {
        PropertyValue propertyValue;
        if (schema.Properties.TryGetValue(name, out propertyValue))
        {
          type = propertyValue.Type;
          return true;
        }
        if (string.IsNullOrEmpty(schema.Inherits))
        {
          type = (string) null;
          return false;
        }
        schema = this.GetDefinition(schema.Inherits).Schemas.Single<Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema>() as MappingSchema;
      }
      throw new InvalidOperationException("Inheritance depth exceeded 10");
    }

    internal bool TryMatchFirstKey(
      List<MappingSchema> schemas,
      string firstKey,
      out string firstValueType,
      out string looseKeyType,
      out string looseValueType)
    {
      bool flag = false;
      looseKeyType = (string) null;
      looseValueType = (string) null;
      int index = 0;
      while (index < schemas.Count)
      {
        MappingSchema schema = schemas[index];
        if (string.Equals(firstKey, schema.FirstKey, StringComparison.Ordinal))
        {
          if (!flag)
            flag = true;
          ++index;
          if (!string.IsNullOrEmpty(schema.LooseKeyType))
          {
            looseKeyType = schema.LooseKeyType;
            looseValueType = schema.LooseValueType;
          }
        }
        else
          schemas.RemoveAt(index);
      }
      firstValueType = flag ? this.GetProperty(schemas[0], firstKey) : (string) null;
      return flag;
    }

    internal bool TryMatchKey(List<MappingSchema> schemas, string key, out string valueType)
    {
      valueType = (string) null;
      bool flag = false;
      for (int index = 0; index < schemas.Count; ++index)
      {
        string type;
        if (this.TryGetProperty(schemas[index], key, out type))
        {
          if (valueType == null)
            valueType = type;
        }
        else
          flag = true;
      }
      if (valueType == null)
        return false;
      if (flag)
      {
        int index = 0;
        while (index < schemas.Count)
        {
          if (this.TryGetProperty(schemas[index], key, out string _))
            ++index;
          else
            schemas.RemoveAt(index);
        }
      }
      return true;
    }

    private static TemplateSchema Schema
    {
      get
      {
        if (TemplateSchema.s_schema == null)
        {
          TemplateSchema templateSchema = new TemplateSchema();
          templateSchema.Definitions.Add("templateSchema", new Definition()
          {
            Schemas = {
              (Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) new MappingSchema()
              {
                Properties = {
                  {
                    "definitions",
                    new PropertyValue("definitions")
                  },
                  {
                    "transforms",
                    new PropertyValue("transforms")
                  }
                }
              }
            }
          });
          Definition definition1 = new Definition();
          MappingSchema mappingSchema1 = new MappingSchema()
          {
            LooseKeyType = "nonEmptyString",
            LooseValueType = "definition"
          };
          definition1.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema1);
          templateSchema.Definitions.Add("definitions", definition1);
          templateSchema.Definitions.Add("definition", new Definition()
          {
            Schemas = {
              (Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) new MappingSchema()
              {
                Properties = {
                  {
                    "allowExpressionsInSubTree",
                    new PropertyValue("true")
                  },
                  {
                    "data",
                    new PropertyValue("mapping")
                  },
                  {
                    "schemas",
                    new PropertyValue("schemas")
                  }
                }
              }
            }
          });
          Definition definition2 = new Definition();
          SequenceSchema sequenceSchema1 = new SequenceSchema()
          {
            ItemType = "schema"
          };
          definition2.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema1);
          templateSchema.Definitions.Add("schemas", definition2);
          Definition definition3 = new Definition();
          MappingSchema mappingSchema2 = new MappingSchema()
          {
            FirstKey = "structure"
          };
          mappingSchema2.Properties.Add("structure", new PropertyValue("schemaType"));
          mappingSchema2.Properties.Add("inherits", new PropertyValue("nonEmptyString"));
          mappingSchema2.Properties.Add("firstKey", new PropertyValue("nonEmptyString"));
          mappingSchema2.Properties.Add("properties", new PropertyValue("properties"));
          mappingSchema2.Properties.Add("looseKeyType", new PropertyValue("nonEmptyString"));
          mappingSchema2.Properties.Add("looseValueType", new PropertyValue("nonEmptyString"));
          mappingSchema2.Properties.Add("transform", new PropertyValue("schemaTransform"));
          mappingSchema2.Properties.Add("load", new PropertyValue("nonEmptyString"));
          mappingSchema2.Properties.Add("data", new PropertyValue("mapping"));
          definition3.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema2);
          MappingSchema mappingSchema3 = new MappingSchema()
          {
            FirstKey = "structure"
          };
          mappingSchema3.Properties.Add("structure", new PropertyValue("schemaType"));
          mappingSchema3.Properties.Add("itemType", new PropertyValue("nonEmptyString"));
          mappingSchema3.Properties.Add("transform", new PropertyValue("schemaTransform"));
          mappingSchema3.Properties.Add("load", new PropertyValue("nonEmptyString"));
          mappingSchema3.Properties.Add("data", new PropertyValue("mapping"));
          definition3.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema3);
          MappingSchema mappingSchema4 = new MappingSchema()
          {
            FirstKey = "structure"
          };
          mappingSchema4.Properties.Add("structure", new PropertyValue("schemaType"));
          mappingSchema4.Properties.Add("constant", new PropertyValue("nonEmptyString"));
          mappingSchema4.Properties.Add("ignoreCase", new PropertyValue("boolean"));
          mappingSchema4.Properties.Add("requireNonEmpty", new PropertyValue("boolean"));
          mappingSchema4.Properties.Add("transform", new PropertyValue("schemaTransform"));
          mappingSchema4.Properties.Add("load", new PropertyValue("nonEmptyString"));
          mappingSchema4.Properties.Add("data", new PropertyValue("mapping"));
          definition3.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema4);
          templateSchema.Definitions.Add("schema", definition3);
          Definition definition4 = new Definition();
          SequenceSchema sequenceSchema2 = new SequenceSchema()
          {
            ItemType = "name"
          };
          definition4.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema2);
          templateSchema.Definitions.Add("properties", definition4);
          Definition definition5 = new Definition();
          MappingSchema mappingSchema5 = new MappingSchema()
          {
            FirstKey = "name"
          };
          mappingSchema5.Properties.Add("name", new PropertyValue("nonEmptyString"));
          mappingSchema5.Properties.Add("type", new PropertyValue("nonEmptyString"));
          mappingSchema5.Properties.Add("data", new PropertyValue("mapping"));
          definition5.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema5);
          templateSchema.Definitions.Add("name", definition5);
          Definition definition6 = new Definition();
          ScalarSchema scalarSchema1 = new ScalarSchema()
          {
            Constant = "scalar"
          };
          definition6.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema1);
          ScalarSchema scalarSchema2 = new ScalarSchema()
          {
            Constant = "sequence"
          };
          definition6.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema2);
          ScalarSchema scalarSchema3 = new ScalarSchema()
          {
            Constant = "mapping"
          };
          definition6.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema3);
          templateSchema.Definitions.Add("schemaType", definition6);
          Definition definition7 = new Definition()
          {
            AllowExpressionsInSubTree = true
          };
          ScalarSchema scalarSchema4 = new ScalarSchema();
          definition7.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema4);
          SequenceSchema sequenceSchema3 = new SequenceSchema()
          {
            ItemType = "any"
          };
          definition7.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema3);
          MappingSchema mappingSchema6 = new MappingSchema()
          {
            LooseKeyType = "string",
            LooseValueType = "any"
          };
          definition7.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema6);
          templateSchema.Definitions.Add("schemaTransform", definition7);
          Definition definition8 = new Definition();
          MappingSchema mappingSchema7 = new MappingSchema()
          {
            LooseKeyType = "nonEmptyString",
            LooseValueType = "transform"
          };
          definition8.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema7);
          templateSchema.Definitions.Add("transforms", definition8);
          Definition definition9 = new Definition()
          {
            AllowExpressionsInSubTree = true
          };
          ScalarSchema scalarSchema5 = new ScalarSchema();
          definition9.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema5);
          SequenceSchema sequenceSchema4 = new SequenceSchema()
          {
            ItemType = "any"
          };
          definition9.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) sequenceSchema4);
          MappingSchema mappingSchema8 = new MappingSchema()
          {
            LooseKeyType = "string",
            LooseValueType = "any"
          };
          definition9.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) mappingSchema8);
          templateSchema.Definitions.Add("transform", definition9);
          Definition definition10 = new Definition();
          ScalarSchema scalarSchema6 = new ScalarSchema()
          {
            Constant = "false"
          };
          definition10.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema6);
          ScalarSchema scalarSchema7 = new ScalarSchema()
          {
            Constant = "true"
          };
          definition10.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema7);
          templateSchema.Definitions.Add("boolean", definition10);
          Definition definition11 = new Definition();
          ScalarSchema scalarSchema8 = new ScalarSchema()
          {
            RequireNonEmpty = true
          };
          definition11.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema8);
          templateSchema.Definitions.Add("nonEmptyString", definition11);
          Definition definition12 = new Definition();
          ScalarSchema scalarSchema9 = new ScalarSchema()
          {
            Constant = "true"
          };
          definition12.Schemas.Add((Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas.Schema) scalarSchema9);
          templateSchema.Definitions.Add("true", definition12);
          templateSchema.Validate();
          Interlocked.CompareExchange<TemplateSchema>(ref TemplateSchema.s_schema, templateSchema, (TemplateSchema) null);
        }
        return TemplateSchema.s_schema;
      }
    }

    private void Validate()
    {
      foreach (Definition definition in this.Definitions.Values)
        definition.Validate(this);
    }
  }
}
