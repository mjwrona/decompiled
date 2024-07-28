// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Utilities.ArtifactTraceabilityHelper
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.WebApi.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Utilities
{
  public static class ArtifactTraceabilityHelper
  {
    public static IDictionary<string, IList<string>> ArtifactCategoryToArtifactTypeMap = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Repository",
        (IList<string>) new List<string>()
        {
          "TfsVersionControl",
          "TfsGit",
          "Git",
          "GitHub",
          "GitHubEnterprise",
          "Bitbucket",
          "Svn"
        }
      },
      {
        "Build",
        (IList<string>) new List<string>() { "Jenkins" }
      },
      {
        "Pipeline",
        (IList<string>) new List<string>() { "Pipeline" }
      },
      {
        "Container",
        (IList<string>) new List<string>()
        {
          "AzureContainerRepository",
          "DockerHub"
        }
      },
      {
        "Package",
        (IList<string>) new List<string>()
      }
    };
    public static IDictionary<string, string> ArtifactTypeToArtifactCategoryMap = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    static ArtifactTraceabilityHelper()
    {
      foreach (KeyValuePair<string, IList<string>> categoryToArtifactType in (IEnumerable<KeyValuePair<string, IList<string>>>) ArtifactTraceabilityHelper.ArtifactCategoryToArtifactTypeMap)
      {
        foreach (string key in (IEnumerable<string>) categoryToArtifactType.Value)
          ArtifactTraceabilityHelper.ArtifactTypeToArtifactCategoryMap.Add(key, categoryToArtifactType.Key);
      }
    }

    public static void UpdateUniqueResourceIdentifier(
      IVssRequestContext requestContext,
      ArtifactTraceabilityData data)
    {
      try
      {
        if (data == null)
          throw new InvalidUniqueResourceIdentifierException(DeploymentResources.CannotCreateUniqueIdentifierInvalidInput());
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        if (data.ArtifactConnectionData != null && data.ArtifactConnectionData.Id != Guid.Empty)
          dictionary.Add("connection", data.ArtifactConnectionData.Id.ToString());
        dictionary.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) data.ResourceProperties);
        data.UniqueResourceIdentifier = UniqueResourceIdentiferHelper.GenerateUniqueResourceIdentifier(requestContext, data.ProjectId, data.ArtifactType, (IDictionary<string, string>) dictionary);
      }
      catch (Exception ex)
      {
        throw new InvalidUniqueResourceIdentifierException(DeploymentResources.CannotCreateUniqueIdentifierForArtifactType((object) data.ArtifactType, (object) ex.Message));
      }
    }

    public static RepositoryType GetRepositoryType(string strEnumValue)
    {
      RepositoryType result;
      return !Enum.TryParse<RepositoryType>(strEnumValue, true, out result) ? RepositoryType.None : result;
    }

    public static ArtifactCategory GetArtifactCategory(string strEnumValue)
    {
      ArtifactCategory result;
      return !Enum.TryParse<ArtifactCategory>(strEnumValue, true, out result) ? ArtifactCategory.None : result;
    }

    public static Dictionary<string, string> GetPropertyDictionary(string jsonString)
    {
      Dictionary<string, string> propertyDictionary = new Dictionary<string, string>();
      if (!string.IsNullOrWhiteSpace(jsonString))
        propertyDictionary = JsonUtility.FromString<Dictionary<string, string>>(jsonString);
      return propertyDictionary;
    }

    public static string ConvertContainerTypeFromACRToAzureContainerRepository(string type) => "ACR".Equals(type, StringComparison.OrdinalIgnoreCase) ? "AzureContainerRepository" : type;
  }
}
