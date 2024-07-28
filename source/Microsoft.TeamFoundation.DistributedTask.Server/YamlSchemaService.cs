// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.YamlSchemaService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class YamlSchemaService : IYamlSchemaService, IVssFrameworkService
  {
    private const string c_layer = "YamlSchemaService";
    private const string objectType = "object";
    private const string stringType = "string";
    private const string booleanType = "boolean";
    private const string integerType = "integer";
    private const string arrayType = "array";

    public object GetYamlSchema(IVssRequestContext requestContext, IList<TaskDefinition> tasks)
    {
      using (new MethodScope(requestContext, nameof (YamlSchemaService), nameof (GetYamlSchema)))
      {
        List<YamlSchemaService.JsonSchemaObject> jsonSchemaObjectList = new List<YamlSchemaService.JsonSchemaObject>()
        {
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/pipeline"
          },
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Pattern = "^$"
          }
        };
        return (object) new YamlSchemaService.YamlSchema()
        {
          Schema = "http://json-schema.org/draft-07/schema#",
          Id = "https://github.com/Microsoft/azure-pipelines-vscode/blob/master/local-schema.json",
          Comment = "v1.170.0",
          Title = "Pipeline schema",
          Description = "A pipeline definition",
          OneOf = jsonSchemaObjectList,
          Definitions = this.GetDefinitions(requestContext, tasks)
        };
      }
    }

    private object GetDefinitions(IVssRequestContext requestContext, IList<TaskDefinition> tasks) => (object) new
    {
      Pipeline = this.Pipeline(),
      StagesAtRoot = this.StagesAtRoot(),
      JobsAtRoot = this.JobsAtRoot(),
      PhasesAtRoot = this.PhasesAtRoot(),
      JobAtRoot = this.JobAtRoot(),
      PhaseAtRoot = this.PhaseAtRoot(),
      Stage = this.Stage(),
      Job = this.Job(),
      Phase = this.Phase(),
      Resources = this.Resources(),
      Pool = this.Pool(),
      Queue = this.Queue(),
      Strategy = this.Strategy(),
      Workspace = this.Workspace(),
      LegacyServer = this.LegacyServer(),
      Matrix = this.Matrix(),
      Script = this.Script(),
      Bash = this.Bash(),
      Powershell = this.Powershell(),
      Pwsh = this.Pwsh(),
      Checkout = this.Checkout(),
      TemplateReference = this.TemplateReference(),
      RepositoryReference = this.RepositoryReference(),
      ContainerReference = this.ContainerReference(),
      BranchFilter = this.BranchFilter(),
      BranchFilterArray = this.BranchFilterArray(),
      Trigger = this.Trigger(),
      PrTrigger = this.PrTrigger(),
      Parameters = this.Parameters(),
      TfvcMappings = this.TfvcMappings(),
      StepOrTemplateExpression = this.StepOrTemplateExpression(),
      Step = this.Step(),
      BooleanTemplateExpression = this.BooleanTemplateExpression(),
      BooleanTemplateMacroExpression = this.BooleanTemplateMacroExpression(),
      BooleanTemplateMacroRuntimeExpression = this.BooleanTemplateMacroRuntimeExpression(),
      ConditionTemplateMacroRuntimeExpression = this.ConditionTemplateMacroRuntimeExpression(),
      IntegerTemplateExpression = this.IntegerTemplateExpression(),
      IntegerTemplateMacroExpression = this.IntegerTemplateMacroExpression(),
      IntegerTemplateMacroRuntimeExpression = this.IntegerTemplateMacroRuntimeExpression(),
      TemplateMacroRuntimeExpression = this.TemplateMacroRuntimeExpression(),
      MacroExpression = this.MacroExpression(),
      RuntimeExpression = this.RuntimeExpression(),
      TemplateExpression = this.TemplateExpression(),
      FunctionExpression = this.FunctionExpression(),
      ConditionExpression = this.ConditionExpression(),
      StepInsertExpression = this.StepInsertExpression(),
      Task = this.GetTasks(requestContext, tasks),
      VmImage = this.VmImage()
    };

    private YamlSchemaService.JsonSchemaObject Pipeline() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AnyOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/jobsAtRoot"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/phasesAtRoot"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/jobAtRoot"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/phaseAtRoot"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject StagesAtRoot() => new YamlSchemaService.JsonSchemaObject()
    {
      AdditionalProperties = new bool?(false),
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "stages",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "array",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/stage"
            }
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            },
            Description = "Variables for the entire pipeline"
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pipeline name",
            Type = "string"
          }
        },
        {
          "appendCommitMessageToRunName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "append commit message to run name",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "trigger",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continuous integration triggers",
            Ref = "#/definitions/trigger"
          }
        },
        {
          "pr",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pull request triggers",
            Ref = "#/definitions/prTrigger"
          }
        },
        {
          "resources",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Containers and repositories used in the build",
            Ref = "#/definitions/resources"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject JobsAtRoot() => new YamlSchemaService.JsonSchemaObject()
    {
      AdditionalProperties = new bool?(false),
      Required = new List<string>() { "jobs" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "jobs",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Jobs which make up the pipeline",
            Type = "array",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              OneOf = new List<YamlSchemaService.JsonSchemaObject>()
              {
                new YamlSchemaService.JsonSchemaObject()
                {
                  Ref = "#/definitions/job"
                },
                new YamlSchemaService.JsonSchemaObject()
                {
                  Ref = "#/definitions/templateReference"
                }
              }
            }
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Variables for this multi-job pipeline",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Pipeline name"
          }
        },
        {
          "appendCommitMessageToRunName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "append commit message to run name",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "trigger",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continuous integration triggers",
            Ref = "#/definitions/trigger"
          }
        },
        {
          "pr",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pull request triggers",
            Ref = "#/definitions/prTrigger"
          }
        },
        {
          "resources",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Containers and repositories used in the build",
            Ref = "#/definitions/resources"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject PhasesAtRoot() => new YamlSchemaService.JsonSchemaObject()
    {
      AdditionalProperties = new bool?(false),
      Required = new List<string>() { "phases" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "phases",
          new YamlSchemaService.JsonSchemaObject()
          {
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use `jobs` instead",
            Description = "Phases which make up the pipeline",
            Type = "array",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/phase"
            }
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Variables for this multi-phase pipeline",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Pipeline name"
          }
        },
        {
          "appendCommitMessageToRunName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "append commit message to run name",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "trigger",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continuous integration triggers",
            Ref = "#/definitions/trigger"
          }
        },
        {
          "pr",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pull request triggers",
            Ref = "#/definitions/prTrigger"
          }
        },
        {
          "resources",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Containers and repositories used in the build",
            Ref = "#/definitions/resources"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject JobAtRoot() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "pool",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pool where this job will run",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/pool"
              }
            }
          }
        },
        {
          "server",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression",
            Description = "True if this is an agent-less job (runs on server)",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool:server instead"
          }
        },
        {
          "strategy",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Execution strategy for this job",
            Ref = "#/definitions/strategy"
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Job-specific variables",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "steps",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "array",
            Description = "A list of steps to run in this job",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/stepOrTemplateExpression"
            }
          }
        },
        {
          "container",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Container resource name"
          }
        },
        {
          "workspace",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/workspace"
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Pipeline name"
          }
        },
        {
          "trigger",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continuous integration triggers",
            Ref = "#/definitions/trigger"
          }
        },
        {
          "pr",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pull request triggers",
            Ref = "#/definitions/prTrigger"
          }
        },
        {
          "resources",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Containers and repositories used in the build",
            Ref = "#/definitions/resources"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Matrix() => new YamlSchemaService.JsonSchemaObject()
    {
      Description = "List of permutations of variable values to run",
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          MinProperties = new int?(1),
          PatternProperties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "^[A-Za-z0-9_]+$",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "object",
                Description = "Variable-value pair to pass in this matrix instance"
              }
            }
          }
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/runtimeExpression"
        }
      }
    };

    private Dictionary<string, YamlSchemaService.JsonSchemaObject> GetScriptingProperties() => new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
    {
      {
        "displayName",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Description = "Human-readable name for the step"
        }
      },
      {
        "name",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Description = "ID of the step",
          Pattern = "^[_A-Za-z0-9]*$"
        }
      },
      {
        "failOnStderr",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Fail the task if output is sent to Stderr?",
          Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
        }
      },
      {
        "workingDirectory",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Description = "Start the script with this working directory"
        }
      },
      {
        "condition",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Evaluate this condition expression to determine whether to run this script",
          Ref = "#/definitions/conditionTemplateMacroRuntimeExpression"
        }
      },
      {
        "continueOnError",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Continue running the parent job even on failure?",
          Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
        }
      },
      {
        "enabled",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Run this script when the job runs?",
          Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
        }
      },
      {
        "timeoutInMinutes",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Time to wait for this script to complete before the server kills it",
          Ref = "#/definitions/integerTemplateMacroExpression"
        }
      },
      {
        "retryCountOnTaskFailure",
        new YamlSchemaService.JsonSchemaObject()
        {
          Description = "Number of retries in case attempt of task execution failed",
          Ref = "#/definitions/integerTemplateMacroExpression"
        }
      },
      {
        "env",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          Description = "Variables to map into the process's environment"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject ConstructScriptingSchemaObject(
      string type,
      Dictionary<string, YamlSchemaService.JsonSchemaObject> properties)
    {
      return new YamlSchemaService.JsonSchemaObject()
      {
        Type = "object",
        AdditionalProperties = new bool?(false),
        Required = new List<string>() { type },
        FirstProperty = new List<string>() { type },
        Properties = properties
      };
    }

    private YamlSchemaService.JsonSchemaObject Script()
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> scriptingProperties = this.GetScriptingProperties();
      scriptingProperties.Add("script", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "An inline script"
      });
      return this.ConstructScriptingSchemaObject("script", scriptingProperties);
    }

    private YamlSchemaService.JsonSchemaObject Bash()
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> scriptingProperties = this.GetScriptingProperties();
      scriptingProperties.Add("bash", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "An inline script"
      });
      return this.ConstructScriptingSchemaObject("bash", scriptingProperties);
    }

    private YamlSchemaService.JsonSchemaObject Powershell()
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> scriptingProperties = this.GetScriptingProperties();
      scriptingProperties.Add("errorActionPreference", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Strategy for dealing with script errors",
        OneOf = new List<YamlSchemaService.JsonSchemaObject>()
        {
          new YamlSchemaService.JsonSchemaObject()
          {
            Enum = new List<string>()
            {
              "stop",
              "continue",
              "silentlyContinue"
            },
            IgnoreCase = "value"
          },
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/templateMacroRuntimeExpression"
          }
        }
      });
      scriptingProperties.Add("ignoreLASTEXITCODE", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Check the final exit code of the script to determine whether the step succeeded?",
        Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
      });
      scriptingProperties.Add("powershell", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "Inline PowerShell or reference to a PowerShell file"
      });
      return this.ConstructScriptingSchemaObject("powershell", scriptingProperties);
    }

    private YamlSchemaService.JsonSchemaObject Pwsh()
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> scriptingProperties = this.GetScriptingProperties();
      scriptingProperties.Add("errorActionPreference", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Strategy for dealing with script errors",
        OneOf = new List<YamlSchemaService.JsonSchemaObject>()
        {
          new YamlSchemaService.JsonSchemaObject()
          {
            Enum = new List<string>()
            {
              "stop",
              "continue",
              "silentlyContinue"
            },
            IgnoreCase = "value"
          },
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/templateMacroRuntimeExpression"
          }
        }
      });
      scriptingProperties.Add("ignoreLASTEXITCODE", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Check the final exit code of the script to determine whether the step succeeded?",
        Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
      });
      scriptingProperties.Add("pwsh", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "Inline PowerShell or reference to a PowerShell file"
      });
      return this.ConstructScriptingSchemaObject("pwsh", scriptingProperties);
    }

    private YamlSchemaService.JsonSchemaObject TemplateReference() => this.ConstructScriptingSchemaObject("template", new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
    {
      {
        "template",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Description = "A URL to a step template"
        }
      },
      {
        "parameters",
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          Description = "Step-specific parameters"
        }
      }
    });

    private YamlSchemaService.JsonSchemaObject RepositoryReference() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Required = new List<string>() { "repository" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "repository",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "ID for the external repository",
            Pattern = "^[A-Za-z0-9_.]+$"
          }
        },
        {
          "type",
          new YamlSchemaService.JsonSchemaObject()
          {
            Enum = new List<string>()
            {
              "git",
              "github",
              "tfsgit",
              "tfsversioncontrol"
            },
            Description = "Type of external repository",
            IgnoreCase = "value"
          }
        },
        {
          "endpoint",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "ID of the service endpoint connecting to this repository"
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Identity and repository name",
            Examples = new List<string>() { "contoso/foo" }
          }
        },
        {
          "ref",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Refname to retrieve",
            Examples = new List<string>()
            {
              "refs/heads/master",
              "refs/tags/lkg"
            }
          }
        },
        {
          "clean",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Scorch the repo before fetching?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "fetchDepth",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Depth of Git graph to fetch",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "lfs",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Fetch and checkout Git LFS objects?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "mappings",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Workspace mappings for TFVC",
            Type = "array",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/tfvcMappings"
            }
          }
        },
        {
          "submodules",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Fetch and checkout submodules?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "checkoutOptions",
          new YamlSchemaService.JsonSchemaObject()
          {
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This location is deprecated, `checkoutOptions` should be a peer of the `repository` keyword.",
            Type = "object"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject ContainerReference() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Required = new List<string>() { "container", "image" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "container",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "ID for the container",
            Pattern = "^[A-Za-z0-9_]+$"
          }
        },
        {
          "image",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Container image tag",
            Examples = new List<string>()
            {
              "ubuntu:16.04",
              "windows:1803"
            }
          }
        },
        {
          "endpoint",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "ID of the service endpoint connecting to a private container registry"
          }
        },
        {
          "options",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Options to pass into container host"
          }
        },
        {
          "localImage",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression",
            Description = "Build the image locally?"
          }
        },
        {
          "env",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "object",
            Description = "Variables to map into the container's environment"
          }
        },
        {
          "type",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Container type"
          }
        },
        {
          "registry",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject BranchFilter() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Description = "branch name or prefix filter",
      Pattern = "^[^\\/~\\^\\: \\[\\]\\*\\\\]+(\\/[^\\/~\\^\\: \\[\\]\\*\\\\]+)*(\\/\\*)?$"
    };

    private YamlSchemaService.JsonSchemaObject BranchFilterArray() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "array",
      Items = new YamlSchemaService.JsonSchemaObject()
      {
        Ref = "#/definitions/branchFilter"
      }
    };

    private YamlSchemaService.JsonSchemaObject Trigger() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Pattern = "^none$"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/branchFilterArray"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "batch",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "boolean",
                Description = "Whether to batch changes per branch"
              }
            },
            {
              "branches",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "object",
                Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                {
                  {
                    "include",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Ref = "#/definitions/branchFilterArray"
                    }
                  },
                  {
                    "exclude",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Ref = "#/definitions/branchFilterArray"
                    }
                  }
                }
              }
            },
            {
              "paths",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "object",
                Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                {
                  {
                    "include",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Type = "array",
                      Items = new YamlSchemaService.JsonSchemaObject()
                      {
                        Type = "string"
                      }
                    }
                  },
                  {
                    "exclude",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Type = "array",
                      Items = new YamlSchemaService.JsonSchemaObject()
                      {
                        Type = "string"
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject PrTrigger() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "string",
          Pattern = "^none$"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/branchFilterArray"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "autoCancel",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "boolean",
                Description = "Whether to cancel running PR builds when a new commit lands in the branch"
              }
            },
            {
              "branches",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "object",
                Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                {
                  {
                    "include",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Ref = "#/definitions/branchFilterArray"
                    }
                  },
                  {
                    "exclude",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Ref = "#/definitions/branchFilterArray"
                    }
                  }
                }
              }
            },
            {
              "paths",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "object",
                Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                {
                  {
                    "include",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Type = "array",
                      Items = new YamlSchemaService.JsonSchemaObject()
                      {
                        Type = "string"
                      }
                    }
                  },
                  {
                    "exclude",
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Type = "array",
                      Items = new YamlSchemaService.JsonSchemaObject()
                      {
                        Type = "string"
                      }
                    }
                  }
                }
              }
            },
            {
              "drafts",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "boolean",
                Description = "Whether to start a run when a draft PR is created"
              }
            }
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Parameters() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object"
    };

    private YamlSchemaService.JsonSchemaObject TfvcMappings() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "localPath",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "On-disk path"
          }
        },
        {
          "serverPath",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "TFVC server-side path"
          }
        },
        {
          "cloak",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Cloak this path?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject PhaseAtRoot() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      DeprecationMessage = "This option is deprecated, use `job` (inside `jobs`) instead",
      DoNotSuggest = new bool?(true),
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "queue",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Queue where this phase will run",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool instead",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/queue"
              }
            }
          }
        },
        {
          "server",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "True if this is an agent-less phase (runs on server)",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool:server instead",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
              },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/legacyServer"
              }
            }
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Phase-specific variables",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "steps",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "array",
            Description = "A list of steps to run in this phase",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/stepOrTemplateExpression"
            }
          }
        },
        {
          "parameters",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Parameters used in a pipeline template",
            Type = "object"
          }
        },
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pipeline name",
            Type = "string"
          }
        },
        {
          "appendCommitMessageToRunName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "append commit message to run name",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "trigger",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continuous integration triggers",
            Ref = "#/definitions/trigger"
          }
        },
        {
          "pr",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pull request triggers",
            Ref = "#/definitions/prTrigger"
          }
        },
        {
          "resources",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Containers and repositories used in the build",
            Ref = "#/definitions/resources"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Stage() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false)
    };

    private YamlSchemaService.JsonSchemaObject Job() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      FirstProperty = new List<string>() { "job" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "pool",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Pool where this job will run",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/pool"
              }
            }
          }
        },
        {
          "server",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression",
            Description = "True if this is an agent-less job (runs on server)",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool:server instead"
          }
        },
        {
          "strategy",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Execution strategy for this job",
            Ref = "#/definitions/strategy"
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Job-specific variables",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "steps",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "array",
            Description = "A list of steps to run in this job",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/stepOrTemplateExpression"
            }
          }
        },
        {
          "container",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Container resource name"
          }
        },
        {
          "workspace",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/workspace"
          }
        },
        {
          "job",
          new YamlSchemaService.JsonSchemaObject()
          {
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "string",
                Description = "ID of the job",
                Pattern = "^[_A-Za-z0-9]*$"
              },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "integer",
                Description = "ID of the job"
              },
              new YamlSchemaService.JsonSchemaObject() { Type = "null" }
            }
          }
        },
        {
          "continueOnError",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continue running this job even on failure?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "displayName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Human-readable name of the job"
          }
        },
        {
          "condition",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Evaluate this condition expression to determine whether to run this job",
            Ref = "#/definitions/conditionTemplateMacroRuntimeExpression"
          }
        },
        {
          "dependsOn",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Any jobs which must complete before this one",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "string",
                  UniqueItems = new bool?(true)
                }
              }
            }
          }
        },
        {
          "timeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait before cancelling the job",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "cancelTimeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait for the job to cancel before forcibly terminating it",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Phase() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      DoNotSuggest = new bool?(true),
      DeprecationMessage = "This option is deprecated, use `job` (inside `jobs`) instead",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "queue",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Queue where this phase will run",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool instead",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/queue"
              }
            }
          }
        },
        {
          "server",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "True if this is an agent-less phase (runs on server)",
            DoNotSuggest = new bool?(true),
            DeprecationMessage = "This option is deprecated, use pool:server instead",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
              },
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/legacyServer"
              }
            }
          }
        },
        {
          "variables",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Phase-specific variables",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "object" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "object",
                  OneOf = new List<YamlSchemaService.JsonSchemaObject>()
                  {
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "name",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        },
                        {
                          "value",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    },
                    new YamlSchemaService.JsonSchemaObject()
                    {
                      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
                      {
                        {
                          "group",
                          new YamlSchemaService.JsonSchemaObject() { Type = "string" }
                        }
                      },
                      AdditionalProperties = new bool?(false)
                    }
                  }
                }
              }
            }
          }
        },
        {
          "steps",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "array",
            Description = "A list of steps to run in this phase",
            Items = new YamlSchemaService.JsonSchemaObject()
            {
              Ref = "#/definitions/stepOrTemplateExpression"
            }
          }
        },
        {
          "parameters",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Parameters used in a pipeline template",
            Type = "object"
          }
        },
        {
          "phase",
          new YamlSchemaService.JsonSchemaObject()
          {
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "string",
                Description = "ID of the phase",
                Pattern = "^[_A-Za-z0-9]*$"
              },
              new YamlSchemaService.JsonSchemaObject() { Type = "null" }
            }
          }
        },
        {
          "displayName",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Human-readable name of the phase"
          }
        },
        {
          "dependsOn",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Any phases which must complete before this one",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "string",
                  UniqueItems = new bool?(true)
                }
              }
            }
          }
        },
        {
          "condition",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Evaluate this condition expression to determine whether to run this phase",
            Ref = "#/definitions/conditionTemplateMacroRuntimeExpression"
          }
        },
        {
          "continueOnError",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Continue running this phase even on failure?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "template",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Reference to a template for this phase"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Resources() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "object",
          AdditionalProperties = new bool?(false),
          Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "containers",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Description = "List of container images",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Ref = "#/definitions/containerReference"
                }
              }
            },
            {
              "repositories",
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Description = "List of external repositories",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Ref = "#/definitions/repositoryReference"
                }
              }
            }
          }
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "array",
          DoNotSuggest = new bool?(true),
          DeprecationMessage = "This option is deprecated, use `repositories` or `containers` instead"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Pool() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Description = "Pool details",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Name of a pool"
          }
        },
        {
          "vmImage",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "For the Azure Pipelines pool, the name of the VM image to use",
            Ref = "#/definitions/vmImage"
          }
        },
        {
          "demands",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "List of demands (for a private pool)",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "string"
                }
              }
            }
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Queue() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      DoNotSuggest = new bool?(true),
      DeprecationMessage = "This option is deprecated, use `pool` under `jobs` instead",
      Description = "Queue details",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "name",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Name of a queue"
          }
        },
        {
          "demands",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "List of demands (for a private queue)",
            OneOf = new List<YamlSchemaService.JsonSchemaObject>()
            {
              new YamlSchemaService.JsonSchemaObject() { Type = "string" },
              new YamlSchemaService.JsonSchemaObject()
              {
                Type = "array",
                Items = new YamlSchemaService.JsonSchemaObject()
                {
                  Type = "string"
                }
              }
            }
          }
        },
        {
          "timeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait before cancelling the phase",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "cancelTimeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait for the phase to cancel before forcibly terminating it",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "parallel",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Maximum number of parallel agent executions",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "matrix",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/matrix"
          }
        },
        {
          "container",
          new YamlSchemaService.JsonSchemaObject()
          {
            Type = "string",
            Description = "Container resource name"
          }
        },
        {
          "workspace",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/workspace"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Strategy() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AnyOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          AdditionalProperties = new bool?(false),
          Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "matrix",
              new YamlSchemaService.JsonSchemaObject()
              {
                Ref = "#/definitions/matrix"
              }
            },
            {
              "maxParallel",
              new YamlSchemaService.JsonSchemaObject()
              {
                Description = "Maximum number of jobs running in parallel",
                Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
              }
            }
          }
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          AdditionalProperties = new bool?(false),
          Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
          {
            {
              "parallel",
              new YamlSchemaService.JsonSchemaObject()
              {
                Description = "Run the job this many times",
                Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
              }
            },
            {
              "maxParallel",
              new YamlSchemaService.JsonSchemaObject()
              {
                Description = "Maximum number of jobs running in parallel",
                Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
              }
            }
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Workspace() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      Description = "Workspace settings",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "clean",
          new YamlSchemaService.JsonSchemaObject()
          {
            IgnoreCase = "value",
            Description = "Clean source?",
            Enum = new List<string>()
            {
              "outputs",
              "resources",
              "all"
            }
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject LegacyServer() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Description = "Server job details",
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "timeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait before cancelling the job",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "cancelTimeoutInMinutes",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Time to wait for the job to cancel before forcibly terminating it",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "parallel",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Maximum number of parallel agent executions",
            Ref = "#/definitions/integerTemplateMacroRuntimeExpression"
          }
        },
        {
          "matrix",
          new YamlSchemaService.JsonSchemaObject()
          {
            Ref = "#/definitions/matrix"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Checkout() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "object",
      AdditionalProperties = new bool?(false),
      Required = new List<string>() { "checkout" },
      FirstProperty = new List<string>() { "checkout" },
      Properties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>()
      {
        {
          "checkout",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Whether or not to check out the repository containing this pipeline definition",
            IgnoreCase = "value",
            Enum = new List<string>() { "self", "none" }
          }
        },
        {
          "clean",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Start from a clean, freshly-fetched workdir?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "fetchDepth",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Depth of Git graph to fetch",
            Ref = "#/definitions/integerTemplateMacroExpression"
          }
        },
        {
          "lfs",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Fetch Git-LFS objects?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "submodules",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Check out Git submodules?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "persistCredentials",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Keep credentials available for later use?",
            Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
          }
        },
        {
          "condition",
          new YamlSchemaService.JsonSchemaObject()
          {
            Description = "Is this step enabled?",
            Ref = "#/definitions/conditionTemplateMacroRuntimeExpression"
          }
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject StepOrTemplateExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/step"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/stepInsertExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject Step() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/script"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/bash"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/powershell"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/pwsh"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/checkout"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateReference"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/task"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject BooleanTemplateExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "boolean"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject BooleanTemplateMacroExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "boolean"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject BooleanTemplateMacroRuntimeExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "boolean"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/runtimeExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject ConditionTemplateMacroRuntimeExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "boolean"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/runtimeExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/conditionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject IntegerTemplateExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "integer"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject IntegerTemplateMacroExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "integer"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject IntegerTemplateMacroRuntimeExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Type = "integer"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/runtimeExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject TemplateMacroRuntimeExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      OneOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/runtimeExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/macroExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/templateExpression"
        },
        new YamlSchemaService.JsonSchemaObject()
        {
          Ref = "#/definitions/functionExpression"
        }
      }
    };

    private YamlSchemaService.JsonSchemaObject MacroExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Pattern = "^\\$\\(.*\\)$"
    };

    private YamlSchemaService.JsonSchemaObject RuntimeExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Pattern = "^\\$\\[.*\\]$"
    };

    private YamlSchemaService.JsonSchemaObject TemplateExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Pattern = "^\\${{.*}}$"
    };

    private string CreateExpressionMatcher(string allowablePrefixes) => string.Format("^{0}\\([a-z0-9\\, \\(\\)'\"\\[\\]\\./_]*\\)$", (object) allowablePrefixes);

    private YamlSchemaService.JsonSchemaObject FunctionExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Pattern = this.CreateExpressionMatcher("(and|coalesce|contains|endsWith|eq|format|gt|ge|lt|le|in|not|ne|notIn|or|startsWith|xor)"),
      IgnoreCase = "value"
    };

    private YamlSchemaService.JsonSchemaObject ConditionExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Pattern = this.CreateExpressionMatcher("(always|failed|canceled|succeded|succeededOrFailed)"),
      IgnoreCase = "value"
    };

    private YamlSchemaService.JsonSchemaObject StepInsertExpression() => new YamlSchemaService.JsonSchemaObject()
    {
      Type = "string",
      Description = "Conditionally insert one or more steps",
      Pattern = "^\\${{.*}}$"
    };

    private YamlSchemaService.JsonSchemaObject VmImage() => new YamlSchemaService.JsonSchemaObject()
    {
      AnyOf = new List<YamlSchemaService.JsonSchemaObject>()
      {
        new YamlSchemaService.JsonSchemaObject()
        {
          Enum = new List<string>()
          {
            "ubuntu-16.04",
            "ubuntu-latest",
            "vs2015-win2012r2",
            "vs2017-win2016",
            "windows-2019",
            "windows-latest",
            "win1803",
            "macos-10.13",
            "macos-10.14",
            "macos-latest"
          },
          IgnoreCase = "value"
        },
        new YamlSchemaService.JsonSchemaObject() { Type = "string" }
      }
    };

    private YamlSchemaService.JsonSchemaObject GetTasks(
      IVssRequestContext requestContext,
      IList<TaskDefinition> tasks)
    {
      List<YamlSchemaService.JsonSchemaObject> jsonSchemaObjectList = new List<YamlSchemaService.JsonSchemaObject>();
      foreach (TaskDefinition task in (IEnumerable<TaskDefinition>) tasks)
      {
        YamlSchemaService.JsonSchemaObject jsonSchemaObject = new YamlSchemaService.JsonSchemaObject()
        {
          FirstProperty = new List<string>() { "task" },
          Required = new List<string>() { "task", "inputs" },
          Properties = this.GetTaskProperties(requestContext, task)
        };
        jsonSchemaObjectList.Add(jsonSchemaObject);
      }
      return new YamlSchemaService.JsonSchemaObject()
      {
        Type = "object",
        AnyOf = jsonSchemaObjectList,
        Properties = this.GetTasksProperties(tasks),
        FirstProperty = new List<string>() { "task" },
        AdditionalProperties = new bool?(false)
      };
    }

    private string EscapeSpecialCharacters(string pattern) => new Regex("([\\^\\$\\.\\|\\?\\*\\+\\(\\)\\[\\{\\\\}\\]])", RegexOptions.Compiled).Replace(pattern, "\\$1");

    private Dictionary<string, YamlSchemaService.JsonSchemaObject> GetTaskProperties(
      IVssRequestContext requestContext,
      TaskDefinition task)
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> taskProperties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>();
      YamlSchemaService.JsonSchemaObject jsonSchemaObject1 = new YamlSchemaService.JsonSchemaObject()
      {
        Pattern = string.Format("^{0}@{1}$", (object) this.EscapeSpecialCharacters(task.Name), (object) task.Version.Major),
        Description = task.FriendlyName + "\n\n" + task.Description,
        IgnoreCase = "value"
      };
      taskProperties.Add(nameof (task), jsonSchemaObject1);
      Dictionary<string, YamlSchemaService.JsonSchemaObject> dictionary = new Dictionary<string, YamlSchemaService.JsonSchemaObject>();
      List<string> stringList1 = new List<string>();
      foreach (TaskInputDefinition input in (IEnumerable<TaskInputDefinition>) task.Inputs)
      {
        YamlSchemaService.JsonSchemaObject jsonSchemaObject2 = new YamlSchemaService.JsonSchemaObject()
        {
          Description = input.Label,
          IgnoreCase = "key"
        };
        if (input.Options.Count > 0)
        {
          List<string> stringList2 = new List<string>();
          foreach (string key in input.Options.Keys)
            stringList2.Add(key);
          jsonSchemaObject2.Enum = stringList2;
          jsonSchemaObject2.IgnoreCase = "all";
        }
        else
          jsonSchemaObject2.Type = string.Compare(input.InputType, "boolean", true) == 0 || string.Compare(input.InputType, "bool", true) == 0 ? "boolean" : (string.Compare(input.InputType, "integer", true) == 0 || string.Compare(input.InputType, "int", true) == 0 ? "integer" : "string");
        string key1 = input.Name;
        if (input.Aliases.Count > 0)
        {
          jsonSchemaObject2.Aliases = (List<string>) input.Aliases;
          string str = string.Copy(jsonSchemaObject2.Aliases[0]);
          jsonSchemaObject2.Aliases[0] = string.Copy(input.Name);
          key1 = str;
        }
        try
        {
          dictionary.Add(key1, jsonSchemaObject2);
        }
        catch (Exception ex)
        {
          requestContext.TraceException("ResourceService", ex);
        }
        if (input.Required && string.IsNullOrEmpty(input.VisibleRule) && string.IsNullOrEmpty(input.DefaultValue))
          stringList1.Add(key1);
      }
      YamlSchemaService.JsonSchemaObject jsonSchemaObject3 = new YamlSchemaService.JsonSchemaObject()
      {
        Properties = dictionary,
        Required = stringList1,
        AdditionalProperties = new bool?(false),
        Description = task.FriendlyName + " inputs"
      };
      taskProperties.Add("inputs", jsonSchemaObject3);
      return taskProperties;
    }

    private Dictionary<string, YamlSchemaService.JsonSchemaObject> GetTasksProperties(
      IList<TaskDefinition> tasks)
    {
      Dictionary<string, YamlSchemaService.JsonSchemaObject> tasksProperties = new Dictionary<string, YamlSchemaService.JsonSchemaObject>();
      List<YamlSchemaService.JsonSchemaObject> jsonSchemaObjectList = new List<YamlSchemaService.JsonSchemaObject>();
      foreach (TaskDefinition task in (IEnumerable<TaskDefinition>) tasks)
      {
        YamlSchemaService.JsonSchemaObject jsonSchemaObject = new YamlSchemaService.JsonSchemaObject()
        {
          Enum = new List<string>()
          {
            task.Name + "@" + task.Version.Major.ToString()
          },
          Description = task.Description,
          IgnoreCase = "value"
        };
        jsonSchemaObjectList.Add(jsonSchemaObject);
      }
      tasksProperties.Add("task", new YamlSchemaService.JsonSchemaObject()
      {
        OneOf = jsonSchemaObjectList
      });
      tasksProperties.Add("displayName", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "Human-readable name for the task"
      });
      tasksProperties.Add("name", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "string",
        Description = "ID of the task instance",
        Pattern = "^[_A-Za-z0-9]*$"
      });
      tasksProperties.Add("condition", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Evaluate this condition expression to determine whether to run this task",
        Ref = "#/definitions/conditionTemplateMacroRuntimeExpression"
      });
      tasksProperties.Add("continueOnError", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "boolean",
        Description = "Continue running the parent job even on failure?"
      });
      tasksProperties.Add("enabled", new YamlSchemaService.JsonSchemaObject()
      {
        Description = "Run this task when the job runs?",
        Ref = "#/definitions/booleanTemplateMacroRuntimeExpression"
      });
      tasksProperties.Add("timeoutInMinutes", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "integer",
        Description = "Time to wait for this task to complete before the server kills it"
      });
      tasksProperties.Add("retryCountOnTaskFailure", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "integer",
        Description = "Number of retries in case attempt of task execution failed"
      });
      tasksProperties.Add("inputs", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "object",
        Description = "Task-specific inputs"
      });
      tasksProperties.Add("env", new YamlSchemaService.JsonSchemaObject()
      {
        Type = "object",
        Description = "Variables to map into the process's environment"
      });
      return tasksProperties;
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private class YamlSchema
    {
      [JsonProperty("$schema")]
      public string Schema { get; set; }

      [JsonProperty("$id")]
      public string Id { get; set; }

      [JsonProperty("$comment")]
      public string Comment { get; set; }

      public string Title { get; set; }

      public string Description { get; set; }

      public List<YamlSchemaService.JsonSchemaObject> OneOf { get; set; }

      public object Definitions { get; set; }
    }

    private class JsonSchemaObject
    {
      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Type { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Description { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, YamlSchemaService.JsonSchemaObject> Properties { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public bool? AdditionalProperties { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string DeprecationMessage { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public bool? DoNotSuggest { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string IgnoreCase { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Pattern { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<string> Enum { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<string> Aliases { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<string> FirstProperty { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<YamlSchemaService.JsonSchemaObject> OneOf { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<YamlSchemaService.JsonSchemaObject> AnyOf { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<string> Required { get; set; }

      [JsonProperty(PropertyName = "$ref", NullValueHandling = NullValueHandling.Ignore)]
      public string Ref { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public YamlSchemaService.JsonSchemaObject Items { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public bool? UniqueItems { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public int? MinProperties { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public Dictionary<string, YamlSchemaService.JsonSchemaObject> PatternProperties { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public List<string> Examples { get; set; }
    }
  }
}
