// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.TemplateSchemaConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Schemas;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal sealed class TemplateSchemaConverter
  {
    private const string c_schemaFileName = "__built-in-schema.yml";
    private static Regex[] s_exclude = new Regex[2]
    {
      new Regex("^task$", RegexOptions.Compiled),
      new Regex("^wf(-.*)?$", RegexOptions.Compiled)
    };
    private Dictionary<string, Definition> m_yamlDefinitions;
    private TemplateSchema m_schema;
    private ITraceWriter m_writer;
    private const string c_objectType = "object";
    private const string c_stringType = "string";
    private const string c_booleanType = "boolean";
    private const string c_integerType = "integer";

    internal TemplateSchemaConverter()
    {
    }

    internal object GetYamlSchema(
      ITraceWriter writer,
      TemplateSchema schema,
      IList<TaskDefinition> majorVersionTasks,
      ParseOptions parseOptions,
      bool validateTaskNames = true,
      IList<TaskDefinition> allTasks = null,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates = null)
    {
      this.m_writer = writer != null ? writer : throw new ArgumentException("TraceWriter cannot be null");
      this.m_schema = schema;
      List<TemplateSchemaConverter.JsonSchemaDefinition> schemaDefinitionList = new List<TemplateSchemaConverter.JsonSchemaDefinition>()
      {
        new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Ref = "#/definitions/pipeline"
        },
        new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Type = "string",
          Pattern = "^$"
        }
      };
      return (object) new TemplateSchemaConverter.SchemaObject()
      {
        Schema = "http://json-schema.org/draft-07/schema#",
        Id = "https://github.com/Microsoft/azure-pipelines-vscode/blob/main/service-schema.json",
        Comment = "v1.183.0",
        Title = "Pipeline schema",
        Description = "A pipeline definition",
        OneOf = schemaDefinitionList,
        Definitions = (object) this.GetYamlDefinitions(majorVersionTasks, allTasks, validateTaskNames, parseOptions, templates)
      };
    }

    internal Dictionary<string, object> GetYamlDefinitions(
      IList<TaskDefinition> majorVersionTasks,
      IList<TaskDefinition> allTasks,
      bool validateTaskNames,
      ParseOptions parseOptions,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates = null)
    {
      this.m_yamlDefinitions = this.m_schema.Definitions;
      Dictionary<string, object> definitions = this.GetDefinitions(this.m_schema, parseOptions);
      if (majorVersionTasks == null)
        majorVersionTasks = (IList<TaskDefinition>) new List<TaskDefinition>();
      if (allTasks == null)
        allTasks = (IList<TaskDefinition>) new List<TaskDefinition>();
      definitions["task"] = this.GetTaskSchema(majorVersionTasks, allTasks, validateTaskNames);
      this.AddTemplates((IDictionary<string, object>) definitions, templates);
      return definitions;
    }

    private Dictionary<string, object> GetDefinitions(
      TemplateSchema yamlSchema,
      ParseOptions parseOptions)
    {
      Dictionary<string, object> definitions = new Dictionary<string, object>();
      foreach (KeyValuePair<string, Definition> yamlDefinition in this.m_yamlDefinitions)
      {
        string definitionName = yamlDefinition.Key;
        if (!((IEnumerable<Regex>) TemplateSchemaConverter.s_exclude).Any<Regex>((Func<Regex, bool>) (x => x.IsMatch(definitionName))))
        {
          Definition definition = yamlDefinition.Value;
          if (!definitions.ContainsKey(definitionName.ToLower()))
            definitions = this.ParseDefinition(definitionName, definition, definitions, parseOptions);
        }
      }
      return definitions;
    }

    private Dictionary<string, object> ParseDefinition(
      string definitionName,
      Definition definition,
      Dictionary<string, object> definitions,
      ParseOptions parseOptions)
    {
      TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition;
      if (definition.Schemas.Count == 1)
      {
        if (definition.Data != null)
          throw new ArgumentException("Schema definitions with only one schema field should have data at the schema level, not the definition level", definitionName);
        schemaDefinition = this.ParseSchema(definition.Schemas[0], definitions, parseOptions);
      }
      else
      {
        if (definition.Schemas.Count <= 1)
          throw new ArgumentException("All schema definitions must have at least one schema field.", definitionName);
        schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition(definition.Data);
        List<TemplateSchemaConverter.JsonSchemaDefinition> schemaDefinitionList = new List<TemplateSchemaConverter.JsonSchemaDefinition>();
        foreach (Schema schema in definition.Schemas)
          schemaDefinitionList.Add(this.ParseSchema(schema, definitions, parseOptions));
        schemaDefinition.AnyOf = schemaDefinitionList;
      }
      definitions.Add(definitionName, (object) schemaDefinition);
      return definitions;
    }

    private TemplateSchemaConverter.JsonSchemaDefinition ParseSchema(
      Schema schema,
      Dictionary<string, object> definitions,
      ParseOptions parseOptions)
    {
      switch (schema.Type)
      {
        case SchemaType.Scalar:
          return this.ParseScalarSchema(schema as ScalarSchema, definitions);
        case SchemaType.Sequence:
          return this.ParseSequenceSchema(schema as SequenceSchema, definitions);
        case SchemaType.Mapping:
          return this.ParseMappingSchema(schema as MappingSchema, definitions, parseOptions);
        default:
          throw new ArgumentException("Schema type must be either Scalar, Sequence, or Mapping");
      }
    }

    private TemplateSchemaConverter.JsonSchemaDefinition ParseScalarSchema(
      ScalarSchema scalar,
      Dictionary<string, object> definitions)
    {
      TemplateSchemaConverter.JsonSchemaDefinition scalarSchema = new TemplateSchemaConverter.JsonSchemaDefinition(scalar.Data);
      scalarSchema.Type = "string";
      if (scalar.IgnoreCase)
        scalarSchema.IgnoreCase = "value";
      if (!string.IsNullOrEmpty(scalar.Constant))
        scalarSchema.Pattern = string.Format("^{0}$", (object) scalar.Constant);
      return scalarSchema;
    }

    private TemplateSchemaConverter.JsonSchemaDefinition ParseMappingSchema(
      MappingSchema mapping,
      Dictionary<string, object> definitions,
      ParseOptions parseOptions)
    {
      TemplateSchemaConverter.JsonSchemaDefinition mappingSchema = new TemplateSchemaConverter.JsonSchemaDefinition(mapping.Data);
      mappingSchema.Type = "object";
      List<string> stringList = new List<string>();
      if (!string.IsNullOrEmpty(mapping.FirstKey) && !string.IsNullOrEmpty(mapping.Inherits) && mapping.FirstKey.ToLower() == "task" && mapping.Inherits.ToLower() == "taskbase")
      {
        mappingSchema.Ref = "#/definitions/task";
        return mappingSchema;
      }
      if (mapping.Properties.Count > 0)
      {
        mappingSchema.AdditionalProperties = new bool?(false);
        mappingSchema.Properties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
        foreach (KeyValuePair<string, PropertyValue> property in mapping.Properties)
        {
          TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
          {
            Ref = "#/definitions/" + property.Value.Type
          };
          MappingToken data = property.Value.Data;
          if (data != null)
          {
            foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in data)
            {
              LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "properties data key");
              switch (literal.Value)
              {
                case "description":
                  schemaDefinition.Description = TemplateUtil.AssertLiteral(keyValuePair.Value, "description").Value;
                  continue;
                case "deprecationMessage":
                  schemaDefinition.DeprecationMessage = TemplateUtil.AssertLiteral(keyValuePair.Value, "deprecationMessage").Value;
                  schemaDefinition.DoNotSuggest = new bool?(true);
                  continue;
                case "required":
                  if (Convert.ToBoolean(TemplateUtil.AssertLiteral(keyValuePair.Value, "required").Value))
                  {
                    stringList.Add(property.Key);
                    continue;
                  }
                  continue;
                case "examples":
                  string commaSeperatedOptions1 = TemplateUtil.AssertLiteral(keyValuePair.Value, "enum").Value;
                  schemaDefinition.Examples = this.ParseOptionsToList(commaSeperatedOptions1);
                  continue;
                case "enum":
                  string commaSeperatedOptions2 = TemplateUtil.AssertLiteral(keyValuePair.Value, "enum").Value;
                  schemaDefinition.Enum = this.ParseOptionsToList(commaSeperatedOptions2);
                  continue;
                default:
                  TemplateUtil.AssertUnexpectedValue(literal, "sequence schema key");
                  throw new ArgumentException();
              }
            }
          }
          mappingSchema.Properties.Add(property.Key, schemaDefinition);
        }
      }
      if (!string.IsNullOrEmpty(mapping.Inherits))
      {
        Definition definition;
        if (!this.m_yamlDefinitions.TryGetValue(mapping.Inherits, out definition))
          throw new ArgumentException("The inherited definition could not be found", mapping.Inherits);
        MappingSchema schema = definition.Schemas[0] as MappingSchema;
        if (schema.Properties.Count > 0)
        {
          if (mappingSchema.Properties == null)
            mappingSchema.Properties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
          foreach (KeyValuePair<string, PropertyValue> property in schema.Properties)
          {
            if (!mappingSchema.Properties.ContainsKey(property.Key))
            {
              TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
              {
                Ref = "#/definitions/" + property.Value.Type
              };
              MappingToken data = property.Value.Data;
              if (data != null)
              {
                foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in data)
                {
                  LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "properties data key");
                  switch (literal.Value)
                  {
                    case "description":
                      schemaDefinition.Description = TemplateUtil.AssertLiteral(keyValuePair.Value, "description").Value;
                      continue;
                    case "deprecationMessage":
                      schemaDefinition.DeprecationMessage = TemplateUtil.AssertLiteral(keyValuePair.Value, "deprecationMessage").Value;
                      schemaDefinition.DoNotSuggest = new bool?(true);
                      continue;
                    case "required":
                      if (Convert.ToBoolean(TemplateUtil.AssertLiteral(keyValuePair.Value, "required").Value))
                      {
                        stringList.Add(property.Key);
                        continue;
                      }
                      continue;
                    case "examples":
                      string commaSeperatedOptions3 = TemplateUtil.AssertLiteral(keyValuePair.Value, "enum").Value;
                      schemaDefinition.Examples = this.ParseOptionsToList(commaSeperatedOptions3);
                      continue;
                    case "enum":
                      string commaSeperatedOptions4 = TemplateUtil.AssertLiteral(keyValuePair.Value, "enum").Value;
                      schemaDefinition.Enum = this.ParseOptionsToList(commaSeperatedOptions4);
                      continue;
                    default:
                      TemplateUtil.AssertUnexpectedValue(literal, "sequence schema key");
                      throw new ArgumentException();
                  }
                }
              }
              mappingSchema.Properties.Add(property.Key, schemaDefinition);
            }
          }
        }
      }
      if (!string.IsNullOrEmpty(mapping.Inherits) && mapping.Inherits.ToLower() == "pipelinebase" && mappingSchema.Properties == null)
        mappingSchema.Properties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      if (stringList.Count > 0)
        mappingSchema.Required = stringList;
      if (mapping.LooseKeyType != null && mapping.LooseValueType != null)
        mappingSchema.AdditionalProperties = new bool?(true);
      if (!string.IsNullOrEmpty(mapping.FirstKey))
        mappingSchema.FirstProperty = new List<string>()
        {
          mapping.FirstKey
        };
      return mappingSchema;
    }

    private TemplateSchemaConverter.JsonSchemaDefinition ParseSequenceSchema(
      SequenceSchema sequence,
      Dictionary<string, object> definitions)
    {
      return new TemplateSchemaConverter.JsonSchemaDefinition(sequence.Data)
      {
        Type = "array",
        Items = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Ref = "#/definitions/" + sequence.ItemType
        }
      };
    }

    private object GetTaskSchema(
      IList<TaskDefinition> majorVersionTasks,
      IList<TaskDefinition> allTasks,
      bool validateTaskNames)
    {
      List<TemplateSchemaConverter.JsonSchemaDefinition> taskSchemas = new List<TemplateSchemaConverter.JsonSchemaDefinition>();
      this.AddTasksToSchema(ref taskSchemas, majorVersionTasks, true);
      this.AddTasksToSchema(ref taskSchemas, allTasks, false);
      if (!validateTaskNames)
      {
        Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> dictionary = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>()
        {
          {
            "task",
            new TemplateSchemaConverter.JsonSchemaDefinition()
            {
              Type = "string"
            }
          },
          {
            "inputs",
            new TemplateSchemaConverter.JsonSchemaDefinition()
            {
              AdditionalProperties = new bool?(true)
            }
          }
        };
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          FirstProperty = new List<string>() { "task" },
          Required = new List<string>() { "task" },
          Properties = dictionary
        };
        taskSchemas.Add(schemaDefinition);
      }
      return (object) new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "object",
        AnyOf = taskSchemas,
        Properties = this.GetTasksSchemaProperties(majorVersionTasks, allTasks, validateTaskNames),
        FirstProperty = new List<string>() { "task" },
        AdditionalProperties = new bool?(false)
      };
    }

    private void AddTasksToSchema(
      ref List<TemplateSchemaConverter.JsonSchemaDefinition> taskSchemas,
      IList<TaskDefinition> tasks,
      bool majorVersionOnly)
    {
      foreach (TaskDefinition task in (IEnumerable<TaskDefinition>) tasks)
      {
        List<string> stringList = new List<string>()
        {
          "task"
        };
        if (task.Inputs.Any<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (input => this.InputAlwaysRequired(input))))
          stringList.Add("inputs");
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          FirstProperty = new List<string>() { "task" },
          Required = stringList,
          Properties = this.GetTaskSchemaProperties(task, majorVersionOnly),
          DoNotSuggest = new bool?(!majorVersionOnly)
        };
        if (task.Deprecated)
        {
          schemaDefinition.DoNotSuggest = new bool?(true);
          schemaDefinition.DeprecationMessage = string.Format("{0} is deprecated - {1}", (object) task.Name, (object) task.Description);
        }
        taskSchemas.Add(schemaDefinition);
      }
    }

    private string EscapeSpecialCharacters(string pattern) => new Regex("([\\^\\$\\.\\|\\?\\*\\+\\(\\)\\[\\{\\\\}\\]])", RegexOptions.Compiled).Replace(pattern, "\\$1");

    private Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> GetTaskSchemaProperties(
      TaskDefinition task,
      bool majorVersionOnly)
    {
      Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> schemaProperties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition1 = new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Pattern = string.Format("^{0}@{1}$", (object) this.EscapeSpecialCharacters(task.Name), majorVersionOnly ? (object) task.Version.Major.ToString() : (object) task.Version.ToString()),
        Description = string.Format("{0}\n\n{1}", (object) task.FriendlyName, (object) task.Description),
        IgnoreCase = "value"
      };
      schemaProperties.Add(nameof (task), schemaDefinition1);
      Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> dictionary = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      List<string> stringList1 = new List<string>();
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) task.Inputs)
      {
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition2 = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Description = input.Label,
          IgnoreCase = "key"
        };
        if (input.Options != null && input.Options.Count > 0)
        {
          List<string> stringList2 = new List<string>();
          foreach (string key in input.Options.Keys)
            stringList2.Add(key);
          schemaDefinition2.Enum = stringList2;
          schemaDefinition2.IgnoreCase = "all";
        }
        else
          schemaDefinition2.Type = string.Compare(input.InputType, "boolean", true) == 0 || string.Compare(input.InputType, "bool", true) == 0 ? "boolean" : (string.Compare(input.InputType, "integer", true) == 0 || string.Compare(input.InputType, "int", true) == 0 ? "integer" : "string");
        string key1 = input.Name;
        if (input.Aliases.Count > 0)
        {
          schemaDefinition2.Aliases = (List<string>) input.Aliases;
          string str = string.Copy(schemaDefinition2.Aliases[0]);
          schemaDefinition2.Aliases[0] = string.Copy(input.Name);
          key1 = str;
        }
        try
        {
          dictionary.Add(key1, schemaDefinition2);
        }
        catch (Exception ex)
        {
          this.m_writer.Error(ex.Message);
        }
        if (this.InputAlwaysRequired(input))
          stringList1.Add(key1);
      }
      TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition3 = new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Properties = dictionary,
        Required = stringList1,
        AdditionalProperties = new bool?(false),
        Description = string.Format("{0} inputs", (object) task.FriendlyName)
      };
      schemaProperties.Add("inputs", schemaDefinition3);
      return schemaProperties;
    }

    private bool InputAlwaysRequired(TaskInputDefinition input) => input.Required && string.IsNullOrEmpty(input.VisibleRule) && string.IsNullOrEmpty(input.DefaultValue);

    private Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> GetTasksSchemaProperties(
      IList<TaskDefinition> tasks,
      IList<TaskDefinition> allTasks,
      bool validateTaskNames)
    {
      Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> schemaProperties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      List<TemplateSchemaConverter.JsonSchemaDefinition> anyOf = new List<TemplateSchemaConverter.JsonSchemaDefinition>();
      this.AddTasksToJsonSchemaDefinition(ref anyOf, tasks, true);
      this.AddTasksToJsonSchemaDefinition(ref anyOf, allTasks, false);
      if (!validateTaskNames)
        anyOf.Add(new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Type = "string"
        });
      schemaProperties.Add("task", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        AnyOf = anyOf
      });
      schemaProperties.Add("displayName", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "string",
        Description = "Human-readable name for the task"
      });
      schemaProperties.Add("name", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "string",
        Description = "ID of the task instance",
        Pattern = "^[_A-Za-z0-9]*$"
      });
      schemaProperties.Add("condition", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Description = "Evaluate this condition expression to determine whether to run this task",
        Type = "string"
      });
      schemaProperties.Add("continueOnError", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "boolean",
        Description = "Continue running the parent job even on failure?"
      });
      schemaProperties.Add("enabled", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Description = "Run this task when the job runs?",
        Type = "string"
      });
      schemaProperties.Add("retryCountOnTaskFailure", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "integer",
        Description = "Number of retries if the task fails"
      });
      schemaProperties.Add("timeoutInMinutes", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "integer",
        Description = "Time to wait for this task to complete before the server kills it"
      });
      schemaProperties.Add("inputs", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "object",
        Description = "Task-specific inputs"
      });
      schemaProperties.Add("env", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "object",
        Description = "Variables to map into the process's environment"
      });
      return schemaProperties;
    }

    private void AddTasksToJsonSchemaDefinition(
      ref List<TemplateSchemaConverter.JsonSchemaDefinition> anyOf,
      IList<TaskDefinition> tasks,
      bool majorVersionOnly)
    {
      foreach (TaskDefinition task in (IEnumerable<TaskDefinition>) tasks)
      {
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Enum = new List<string>()
          {
            majorVersionOnly ? task.Name + "@" + task.Version.Major.ToString() : task.Name + "@" + task.Version.ToString()
          },
          Description = task.Description,
          IgnoreCase = "value",
          DoNotSuggest = new bool?(!majorVersionOnly)
        };
        if (task.Deprecated)
        {
          schemaDefinition.DoNotSuggest = new bool?(true);
          schemaDefinition.DeprecationMessage = string.Format("{0} is deprecated - {1}", (object) task.Name, (object) task.Description);
        }
        anyOf.Add(schemaDefinition);
      }
    }

    private List<string> ParseOptionsToList(string commaSeperatedOptions)
    {
      List<string> optionsToList = new List<string>();
      string str1 = commaSeperatedOptions;
      char[] chArray = new char[1]{ ',' };
      foreach (string str2 in str1.Split(chArray))
        optionsToList.Add(str2);
      return optionsToList;
    }

    private void AddTemplates(
      IDictionary<string, object> definitions,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates)
    {
      if (templates == null || templates.Count <= 0)
        return;
      this.OverrideLoadTemplate(definitions, templates, "step");
      this.OverrideLoadTemplate(definitions, templates, "job");
      this.OverrideLoadTemplate(definitions, templates, "stage");
      this.OverrideLoadTemplate(definitions, templates, "variable");
      definitions["extends"] = (object) this.GetTemplateSchema(templates);
    }

    private void OverrideLoadTemplate(
      IDictionary<string, object> definitions,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates,
      string name)
    {
      if (!(definitions[name] is TemplateSchemaConverter.JsonSchemaDefinition definition1) || !definition1.AnyOf.Where<TemplateSchemaConverter.JsonSchemaDefinition>((Func<TemplateSchemaConverter.JsonSchemaDefinition, bool>) (x => x.FirstProperty != null && x.FirstProperty.First<string>().Equals("template"))).Any<TemplateSchemaConverter.JsonSchemaDefinition>())
        return;
      TemplateSchemaConverter.JsonSchemaDefinition definition2 = definitions[name] as TemplateSchemaConverter.JsonSchemaDefinition;
      definition2.AnyOf.Remove(definition2.AnyOf.Where<TemplateSchemaConverter.JsonSchemaDefinition>((Func<TemplateSchemaConverter.JsonSchemaDefinition, bool>) (x => x.FirstProperty != null && x.FirstProperty.First<string>().Equals("template"))).First<TemplateSchemaConverter.JsonSchemaDefinition>());
      definition2.AnyOf.Add(this.GetTemplateSchema(templates));
    }

    private TemplateSchemaConverter.JsonSchemaDefinition GetTemplateSchema(
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates)
    {
      List<TemplateSchemaConverter.JsonSchemaDefinition> taskSchemas = new List<TemplateSchemaConverter.JsonSchemaDefinition>();
      this.AddTemplatesToSchema(taskSchemas, templates);
      return new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "object",
        AnyOf = taskSchemas,
        Properties = this.GetTemplatesSchemaProperties(templates),
        FirstProperty = new List<string>() { "template" },
        AdditionalProperties = new bool?(false)
      };
    }

    private void AddTemplatesToSchema(
      List<TemplateSchemaConverter.JsonSchemaDefinition> taskSchemas,
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates)
    {
      foreach (KeyValuePair<YamlTemplateLocation, IList<TemplateParameter>> template in templates)
      {
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          FirstProperty = new List<string>() { "template" },
          Required = new List<string>(),
          Properties = this.GetTemplateSchemaProperties(template),
          DoNotSuggest = new bool?(false)
        };
        taskSchemas.Add(schemaDefinition);
      }
    }

    private Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> GetTemplatesSchemaProperties(
      Dictionary<YamlTemplateLocation, IList<TemplateParameter>> templates)
    {
      Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> schemaProperties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      List<TemplateSchemaConverter.JsonSchemaDefinition> anyOf = new List<TemplateSchemaConverter.JsonSchemaDefinition>();
      foreach (KeyValuePair<YamlTemplateLocation, IList<TemplateParameter>> template in templates)
        this.AddTemplateToJsonSchemaDefinition(template.Key.RepositoryAlias.Equals("self") ? template.Key.Path : string.Format("{0}@{1}", (object) template.Key.Path, (object) template.Key.RepositoryAlias), anyOf);
      schemaProperties.Add("template", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        AnyOf = anyOf
      });
      schemaProperties.Add("parameters", new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Type = "object",
        Description = "Template parameter inputs"
      });
      return schemaProperties;
    }

    private void AddTemplateToJsonSchemaDefinition(
      string name,
      List<TemplateSchemaConverter.JsonSchemaDefinition> anyOf)
    {
      TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
      {
        Enum = new List<string>() { name },
        Description = name,
        IgnoreCase = "value",
        DoNotSuggest = new bool?(false)
      };
      anyOf.Add(schemaDefinition);
    }

    private Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> GetTemplateSchemaProperties(
      KeyValuePair<YamlTemplateLocation, IList<TemplateParameter>> templateValuePair)
    {
      Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> dictionary = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
      string str1 = templateValuePair.Key.RepositoryAlias.Equals("self") ? templateValuePair.Key.Path : string.Format("{0}@{1}", (object) templateValuePair.Key.Path, (object) templateValuePair.Key.RepositoryAlias);
      foreach (TemplateParameter templateParameter in (IEnumerable<TemplateParameter>) templateValuePair.Value)
      {
        TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
        {
          Description = templateParameter.Name,
          IgnoreCase = "key"
        };
        IList<JToken> values = templateParameter.Values;
        if ((values != null ? (values.Count > 0 ? 1 : 0) : 0) != 0)
        {
          List<string> stringList = new List<string>();
          foreach (JToken jtoken in (IEnumerable<JToken>) templateParameter.Values)
          {
            string str2 = (string) jtoken;
            stringList.Add(str2.ToString());
          }
          schemaDefinition.Enum = stringList;
        }
        else
        {
          switch (templateParameter.Type)
          {
            case TemplateParameterType.Number:
              schemaDefinition.Type = "integer";
              break;
            case TemplateParameterType.Boolean:
              schemaDefinition.Type = "boolean";
              break;
            default:
              schemaDefinition.Type = "string";
              break;
          }
        }
        dictionary.Add(templateParameter.Name, schemaDefinition);
      }
      return new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>()
      {
        {
          "template",
          new TemplateSchemaConverter.JsonSchemaDefinition()
          {
            Pattern = str1,
            Description = "Template YAML File",
            IgnoreCase = "value"
          }
        },
        {
          "parameters",
          new TemplateSchemaConverter.JsonSchemaDefinition()
          {
            Properties = dictionary,
            AdditionalProperties = new bool?(false),
            Description = "Parameters for Template"
          }
        }
      };
    }

    [DataContract]
    internal class SchemaObject
    {
      [DataMember(Name = "$schema")]
      public string Schema { get; set; }

      [DataMember(Name = "$id")]
      public string Id { get; set; }

      [DataMember(Name = "$comment")]
      public string Comment { get; set; }

      [DataMember]
      public string Title { get; set; }

      [DataMember]
      public string Description { get; set; }

      [DataMember]
      public List<TemplateSchemaConverter.JsonSchemaDefinition> OneOf { get; set; }

      [DataMember]
      public object Definitions { get; set; }
    }

    [DataContract]
    internal class JsonSchemaDefinition
    {
      internal JsonSchemaDefinition()
      {
      }

      internal JsonSchemaDefinition(MappingToken data)
      {
        if (data == null)
          return;
        foreach (KeyValuePair<ScalarToken, TemplateToken> keyValuePair in data)
        {
          LiteralToken literal = TemplateUtil.AssertLiteral((TemplateToken) keyValuePair.Key, "data key");
          switch (literal.Value)
          {
            case "description":
              this.Description = TemplateUtil.AssertLiteral(keyValuePair.Value, "description").Value;
              continue;
            case "deprecationMessage":
              this.DeprecationMessage = TemplateUtil.AssertLiteral(keyValuePair.Value, "deprecationMessage").Value;
              continue;
            case "minProperties":
              this.MinProperties = new int?(Convert.ToInt32(TemplateUtil.AssertLiteral(keyValuePair.Value, "minProperties").Value));
              continue;
            case "pattern":
              this.Pattern = TemplateUtil.AssertLiteral(keyValuePair.Value, "pattern").Value;
              continue;
            case "patternProperties":
              MappingToken mappingToken = TemplateUtil.AssertMapping(keyValuePair.Value, "patternProperties");
              this.PatternProperties = new Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition>();
              using (IEnumerator<KeyValuePair<ScalarToken, TemplateToken>> enumerator = mappingToken.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  KeyValuePair<ScalarToken, TemplateToken> current = enumerator.Current;
                  string key = TemplateUtil.AssertLiteral((TemplateToken) current.Key, "patternProperties key").Value;
                  string str = TemplateUtil.AssertLiteral(current.Value, "patternProperties value").Value;
                  TemplateSchemaConverter.JsonSchemaDefinition schemaDefinition = new TemplateSchemaConverter.JsonSchemaDefinition()
                  {
                    Ref = "#/definitions/" + str
                  };
                  this.PatternProperties.Add(key, schemaDefinition);
                }
                continue;
              }
            case "uniqueItems":
              this.UniqueItems = new bool?(Convert.ToBoolean(TemplateUtil.AssertLiteral(keyValuePair.Value, "uniqueItems").Value));
              continue;
            default:
              TemplateUtil.AssertUnexpectedValue(literal, "sequence schema key");
              throw new ArgumentException();
          }
        }
      }

      [DataMember(EmitDefaultValue = false)]
      internal string Type { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal string Description { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> Properties { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal bool? AdditionalProperties { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal string DeprecationMessage { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal bool? DoNotSuggest { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal string IgnoreCase { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal string Pattern { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<string> Enum { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<string> Aliases { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<string> FirstProperty { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<TemplateSchemaConverter.JsonSchemaDefinition> OneOf { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<TemplateSchemaConverter.JsonSchemaDefinition> AnyOf { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<string> Required { get; set; }

      [DataMember(Name = "$ref", EmitDefaultValue = false)]
      internal string Ref { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal TemplateSchemaConverter.JsonSchemaDefinition Items { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal bool? UniqueItems { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal int? MinProperties { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal Dictionary<string, TemplateSchemaConverter.JsonSchemaDefinition> PatternProperties { get; set; }

      [DataMember(EmitDefaultValue = false)]
      internal List<string> Examples { get; set; }
    }
  }
}
