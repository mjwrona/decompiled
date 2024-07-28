// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders.BuildProvider
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.BuildProviders
{
  internal class BuildProvider : IBuildProvider
  {
    public string GetLatestVersion(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint)
    {
      return BuildProvider.GetLatestValueAndDisplayValue(requestContext, projectId, build, endpoint).version;
    }

    public (string version, string versionName) GetLatestVersionAndName(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint)
    {
      (string version, string versionName) valueAndDisplayValue = BuildProvider.GetLatestValueAndDisplayValue(requestContext, projectId, build, endpoint);
      return (valueAndDisplayValue.version, valueAndDisplayValue.versionName);
    }

    private static (string version, string versionName) GetLatestValueAndDisplayValue(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint)
    {
      string str1 = string.Empty;
      string str2 = string.Empty;
      string typeId = build.Properties.Get<string>(BuildPropertyNames.Type);
      IArtifactType artifactType = requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, typeId);
      if (artifactType != null)
      {
        IDictionary<string, string> resourceInputs = build.GetResourceInputs();
        resourceInputs["connection"] = endpoint.Id.ToString();
        ArtifactTaskInputMapper.PopulateMappedTaskInputs(artifactType, resourceInputs);
        InputValue latestVersion = artifactType.GetLatestVersion(requestContext, resourceInputs, new ProjectInfo()
        {
          Id = projectId
        });
        str1 = latestVersion?.Value ?? string.Empty;
        str2 = latestVersion?.DisplayValue ?? string.Empty;
      }
      return (str1, str2);
    }

    public void Validate(
      IVssRequestContext requestContext,
      Guid projectId,
      BuildResource build,
      ServiceEndpoint endpoint)
    {
      string typeId = build.Properties.Get<string>(BuildPropertyNames.Type);
      IArtifactType artifactType = !string.IsNullOrEmpty(typeId) ? requestContext.GetService<IArtifactService>().GetArtifactType(requestContext, typeId) : throw new ResourceValidationException(TaskResources.BuildResourceTypeRequired(), BuildPropertyNames.Type);
      string str = artifactType != null ? artifactType.ArtifactType : throw new ResourceValidationException(TaskResources.CannotFindBuildResourceExtension((object) typeId), BuildPropertyNames.Type);
      if ((str != null ? (!str.Equals("Build", StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 1) != 0)
        throw new ResourceValidationException(TaskResources.CannotUseNonBuildTypeArtifact((object) typeId), BuildPropertyNames.Type);
      if (build.Endpoint == null)
        throw new ResourceValidationException(TaskResources.BuildResourceConnectionInputRequired(), "Endpoint");
      if (endpoint == null)
        throw new ResourceValidationException(TaskResources.BuildResourceValidConnectionInputRequired((object) build.Endpoint.Name), "Endpoint");
      if (string.IsNullOrEmpty(build.Properties.Get<string>(PipelinePropertyNames.Source)))
        throw new ResourceValidationException(TaskResources.BuildResourceSourceInputRequired(), PipelinePropertyNames.Source);
      IList<string> validYamlInputs = artifactType.GetValidYamlInputs();
      validYamlInputs.AddRange<string, IList<string>>((IEnumerable<string>) new string[5]
      {
        BuildPropertyNames.Type,
        BuildPropertyNames.Branch,
        BuildPropertyNames.Version,
        BuildPropertyNames.Source,
        BuildPropertyNames.Connection
      });
      foreach (string key in (IEnumerable<string>) build.Properties.Items.Keys)
      {
        string propertyName = key;
        if (!validYamlInputs.Any<string>((Func<string, bool>) (input => string.Equals(input, propertyName, StringComparison.OrdinalIgnoreCase))))
          throw new ResourceValidationException(TaskResources.BuildResourceContainsInvalidInput((object) build.Alias, (object) propertyName), propertyName);
      }
    }
  }
}
