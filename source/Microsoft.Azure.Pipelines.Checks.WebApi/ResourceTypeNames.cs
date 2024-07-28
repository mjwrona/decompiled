// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.ResourceTypeNames
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [GenerateSpecificConstants(null)]
  public static class ResourceTypeNames
  {
    [GenerateConstant(null)]
    public const string ServiceEndpoint = "endpoint";
    [GenerateConstant(null)]
    public const string Queue = "queue";
    [GenerateConstant(null)]
    public const string SecureFile = "securefile";
    [GenerateConstant(null)]
    public const string VariableGroup = "variablegroup";
    [GenerateConstant(null)]
    public const string Environment = "environment";
    [GenerateConstant(null)]
    public const string AgentPool = "agentpool";
    [GenerateConstant(null)]
    public const string Repository = "repository";
    [GenerateConstant(null)]
    public const string PersistedStage = "persistedstage";
    public static readonly Dictionary<string, ResourceType> s_nameToValueMap = new Dictionary<string, ResourceType>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "endpoint",
        ResourceType.ServiceEndpoint
      },
      {
        "queue",
        ResourceType.Queue
      },
      {
        "securefile",
        ResourceType.SecureFile
      },
      {
        "variablegroup",
        ResourceType.VariableGroup
      },
      {
        "environment",
        ResourceType.Environment
      },
      {
        "agentpool",
        ResourceType.AgentPool
      },
      {
        "repository",
        ResourceType.Repository
      },
      {
        "persistedstage",
        ResourceType.PersistedStage
      }
    };
    public static readonly Dictionary<ResourceType, string> s_ValueToNameMap = new Dictionary<ResourceType, string>()
    {
      {
        ResourceType.ServiceEndpoint,
        "endpoint"
      },
      {
        ResourceType.Queue,
        "queue"
      },
      {
        ResourceType.SecureFile,
        "securefile"
      },
      {
        ResourceType.VariableGroup,
        "variablegroup"
      },
      {
        ResourceType.Environment,
        "environment"
      },
      {
        ResourceType.AgentPool,
        "agentpool"
      },
      {
        ResourceType.Repository,
        "repository"
      },
      {
        ResourceType.PersistedStage,
        "persistedstage"
      }
    };

    public static bool TryParse(string name, out ResourceType result)
    {
      result = (ResourceType) 0;
      return !string.IsNullOrEmpty(name) && ResourceTypeNames.s_nameToValueMap.TryGetValue(name, out result);
    }

    public static bool TryParse(ResourceType resourceType, out string result)
    {
      result = (string) null;
      if (resourceType == (ResourceType) 0)
        return false;
      result = ResourceTypeNames.s_ValueToNameMap[resourceType];
      return true;
    }

    public static string GetResourceTypeString(string typeName)
    {
      if (typeName != null)
      {
        switch (typeName.Length)
        {
          case 61:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPool")
              break;
            goto label_25;
          case 67:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.WebApi.EnvironmentInstance")
              return "environment";
            goto label_25;
          case 69:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentPoolReference")
              break;
            goto label_25;
          case 70:
            switch (typeName[51])
            {
              case 'A':
                if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.AgentQueueReference")
                  return "queue";
                goto label_25;
              case 'R':
                if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryReference")
                  return "repository";
                goto label_25;
              case 'S':
                if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference")
                  return "securefile";
                goto label_25;
              default:
                goto label_25;
            }
          case 71:
            switch (typeName[10])
            {
              case 'T':
                if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.EnvironmentReference")
                  return "environment";
                goto label_25;
              case 'V':
                if (typeName == "Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint")
                  return "endpoint";
                goto label_25;
              default:
                goto label_25;
            }
          case 73:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference")
              return "variablegroup";
            goto label_25;
          case 74:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.PersistedStageReference")
              return "persistedstage";
            goto label_25;
          case 75:
            if (typeName == "Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference")
              return "endpoint";
            goto label_25;
          default:
            goto label_25;
        }
        return "agentpool";
      }
label_25:
      return string.Empty;
    }
  }
}
