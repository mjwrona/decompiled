// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DockerPipelineFragmentProvider
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DockerPipelineFragmentProvider : PipelineFragmentProviderBase
  {
    private const string c_servicePortPropertyName = "servicePort";
    private const string c_dockerfilePathPropertyName = "dockerfilePath";
    private const string c_buildContextPathPropertyName = "buildContextPath";
    private const string c_imageRepositoryPropertyName = "imageRepository";
    private static readonly string[] m_azureFunctionBaseImages = new string[5]
    {
      "azure-functions/base",
      "azure-functions/dotnet",
      "azure-functions/node",
      "azure-functions/python",
      "azure-functions/powershell"
    };

    protected override IEnumerable<Template> GetTemplatesInternal(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      DetectedBuildFramework detectedBuildFramework)
    {
      List<string> suggestedTemplateIds = DockerPipelineFragmentProvider.GetSuggestedTemplateIds(requestContext);
      if (DockerPipelineFragmentProvider.ContainsAzureFunctionBaseImage(detectedBuildFramework) && suggestedTemplateIds.Contains(TemplateIds.Pipelines.DockerContainerFunctionApp))
      {
        suggestedTemplateIds.Remove(TemplateIds.Pipelines.DockerContainerFunctionApp);
        suggestedTemplateIds.Insert(0, TemplateIds.Pipelines.DockerContainerFunctionApp);
      }
      List<Template> list = suggestedTemplateIds.Select<string, Template>((Func<string, Template>) (id => this.GetTemplate(requestContext, id))).ToList<Template>();
      if (list.Count > 0)
      {
        string firstDockerfilePath = DockerPipelineFragmentProvider.GetFirstDockerfilePath(detectedBuildFramework);
        string buildContext = DockerPipelineFragmentProvider.GetBuildContext(firstDockerfilePath);
        IList<string> exposedPorts = DockerPipelineFragmentProvider.GetExposedPorts(detectedBuildFramework);
        string repositoryName = DockerPipelineFragmentProvider.GetRepositoryName(requestContext, repositoryContext);
        foreach (Template template in list)
        {
          DockerPipelineFragmentProvider.AssignPropertyValue(template, "dockerfilePath", firstDockerfilePath);
          DockerPipelineFragmentProvider.AssignPropertyValue(template, "buildContextPath", buildContext);
          DockerPipelineFragmentProvider.AssignPropertyValue(template, "imageRepository", repositoryName);
          if (exposedPorts.Any<string>())
          {
            DockerPipelineFragmentProvider.AssignPropertyPossibleValues(template, "servicePort", exposedPorts);
            DockerPipelineFragmentProvider.AssignPropertyValue(template, "servicePort", exposedPorts.First<string>());
          }
        }
      }
      return (IEnumerable<Template>) list;
    }

    private static List<string> GetSuggestedTemplateIds(IVssRequestContext requestContext) => new List<string>()
    {
      TemplateIds.Pipelines.DockerBuild,
      TemplateIds.Pipelines.DockerContainer,
      TemplateIds.Pipelines.DeployToExistingK8s
    };

    protected override bool IsSupportedFramework(DetectedBuildFramework detectedBuildFramework) => string.Equals(detectedBuildFramework.Id, DockerBuildFrameworkDetector.Id, StringComparison.OrdinalIgnoreCase);

    private static bool ContainsAzureFunctionBaseImage(DetectedBuildFramework detectedBuildFramework) => detectedBuildFramework.BuildTargets.Where<DetectedBuildTarget>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType = new Func<DetectedBuildTarget, bool>(DockerPipelineFragmentProvider.IsDockerfileType))).Where<DetectedBuildTarget>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C1\u003E__HasBaseImages ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C1\u003E__HasBaseImages = new Func<DetectedBuildTarget, bool>(DockerPipelineFragmentProvider.HasBaseImages))).Select<DetectedBuildTarget, string>((Func<DetectedBuildTarget, string>) (t => t.Settings[DockerBuildFrameworkDetector.Settings.BaseImages])).SelectMany<string, string>((Func<string, IEnumerable<string>>) (images => (IEnumerable<string>) images.Split(new char[1]
    {
      ','
    }, StringSplitOptions.RemoveEmptyEntries))).Any<string>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C2\u003E__IsAzureFunctionBaseImage ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C2\u003E__IsAzureFunctionBaseImage = new Func<string, bool>(DockerPipelineFragmentProvider.IsAzureFunctionBaseImage)));

    private static bool IsAzureFunctionBaseImage(string imageName) => ((IEnumerable<string>) DockerPipelineFragmentProvider.m_azureFunctionBaseImages).Any<string>((Func<string, bool>) (azureImage => imageName.Equals(azureImage, StringComparison.OrdinalIgnoreCase) || imageName.EndsWith("/" + azureImage, StringComparison.OrdinalIgnoreCase)));

    private static void AssignPropertyValue(Template template, string name, string value)
    {
      TemplateParameterDefinition parameterDefinition = template.Parameters.FirstOrDefault<TemplateParameterDefinition>((Func<TemplateParameterDefinition, bool>) (p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)));
      if (parameterDefinition == null || string.IsNullOrEmpty(value))
        return;
      parameterDefinition.DefaultValue = value;
    }

    private static void AssignPropertyPossibleValues(
      Template template,
      string name,
      IList<string> values)
    {
      TemplateParameterDefinition parameterDefinition = template.Parameters.FirstOrDefault<TemplateParameterDefinition>((Func<TemplateParameterDefinition, bool>) (p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase)));
      if (parameterDefinition == null || values == null || values.Count <= 0)
        return;
      parameterDefinition.PossibleValues = values;
    }

    private static string GetFirstDockerfilePath(DetectedBuildFramework detectedBuildFramework)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      string str = detectedBuildFramework.BuildTargets.Where<DetectedBuildTarget>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType = new Func<DetectedBuildTarget, bool>(DockerPipelineFragmentProvider.IsDockerfileType))).Select<DetectedBuildTarget, string>((Func<DetectedBuildTarget, string>) (t => t.Path)).FirstOrDefault<string>();
      if (string.IsNullOrEmpty(str))
        return (string) null;
      if (!str.StartsWith("/", StringComparison.Ordinal))
        str = "/" + str;
      return "$(Build.SourcesDirectory)" + str;
    }

    private static string GetRepositoryName(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext)
    {
      if (repositoryContext == null)
        return "";
      string str = repositoryContext.RepositoryName;
      if (string.IsNullOrEmpty(str))
        str = DockerPipelineFragmentProvider.FetchRepositoryName(requestContext, repositoryContext) ?? repositoryContext.RepositoryId ?? string.Empty;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return string.Concat<char>(str.ToLowerInvariant().Where<char>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C3\u003E__IsLetter ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C3\u003E__IsLetter = new Func<char, bool>(char.IsLetter))));
    }

    private static string FetchRepositoryName(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext)
    {
      return requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryContext.SourceProvider, false)?.GetUserRepository(requestContext, repositoryContext.ProjectId, repositoryContext.ConnectionId, repositoryContext.RepositoryId)?.Name;
    }

    private static string GetBuildContext(string dockerfilePath)
    {
      if (string.IsNullOrEmpty(dockerfilePath))
        return (string) null;
      int length = dockerfilePath.LastIndexOf('/');
      return length <= 0 ? (string) null : dockerfilePath.Substring(0, length);
    }

    private static IList<string> GetExposedPorts(DetectedBuildFramework detectedBuildFramework)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      List<string> list = detectedBuildFramework.BuildTargets.Where<DetectedBuildTarget>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C0\u003E__IsDockerfileType = new Func<DetectedBuildTarget, bool>(DockerPipelineFragmentProvider.IsDockerfileType))).Where<DetectedBuildTarget>(DockerPipelineFragmentProvider.\u003C\u003EO.\u003C4\u003E__HasExposedPorts ?? (DockerPipelineFragmentProvider.\u003C\u003EO.\u003C4\u003E__HasExposedPorts = new Func<DetectedBuildTarget, bool>(DockerPipelineFragmentProvider.HasExposedPorts))).Select<DetectedBuildTarget, string>((Func<DetectedBuildTarget, string>) (t => t.Settings[DockerBuildFrameworkDetector.Settings.ExposedPorts])).SelectMany<string, string>((Func<string, IEnumerable<string>>) (ports => (IEnumerable<string>) ports.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries))).OrderBy<string, string>((Func<string, string>) (port => port), (IComparer<string>) new NaturalStringComparer(StringComparison.Ordinal)).ToList<string>();
      if (list.Count == 0)
        list.Add("80");
      return (IList<string>) list;
    }

    private static bool IsDockerfileType(DetectedBuildTarget target) => string.Equals(target.Type, DockerBuildFrameworkDetector.Settings.WellKnownTypes.Dockerfile, StringComparison.OrdinalIgnoreCase);

    private static bool HasExposedPorts(DetectedBuildTarget target) => target.Settings.ContainsKey(DockerBuildFrameworkDetector.Settings.ExposedPorts);

    private static bool HasBaseImages(DetectedBuildTarget target) => target.Settings.ContainsKey(DockerBuildFrameworkDetector.Settings.BaseImages);
  }
}
