// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.RepositoryAnalysisService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class RepositoryAnalysisService : IRepositoryAnalysisService, IVssFrameworkService
  {
    private const string c_rootPath = "/";
    private const string c_primaryConfigurationFile = "azure-pipelines.yml";
    private const int c_defaultTemplateWeight = 1000;
    private const string c_layer = "RepositoryAnalysisService";
    private static readonly IReadOnlyList<string> s_wellKnownYamlNames = (IReadOnlyList<string>) new string[2]
    {
      "azure-pipelines.yml",
      ".azure-pipelines.yml"
    };
    private static readonly Regex s_configFileRegex = new Regex("^\\/?azure-pipelines-(?<number>[\\d]+)\\.yml$", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1.0));

    public string PrimaryConfigurationPath => "/azure-pipelines.yml";

    public void ServiceStart(IVssRequestContext context)
    {
    }

    public void ServiceEnd(IVssRequestContext context)
    {
    }

    public ConfigurationFile GetExistingConfigurationFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      string path)
    {
      using (new Tracer<RepositoryAnalysisService>(requestContext, TracePoints.RepositoryAnalysis.GetExistingConfigurationFileEnter, TracePoints.RepositoryAnalysis.GetExistingConfigurationFileLeave, nameof (GetExistingConfigurationFile)))
        return string.IsNullOrEmpty(path) ? this.SearchForConfigurationFile(requestContext, projectId, repositoryType, repository, connectionId, branch) : this.GetExistingConfigurationFileFromPath(requestContext, projectId, repositoryType, repository, connectionId, branch, path);
    }

    public IReadOnlyList<Template> GetSuggestedConfigurationFiles(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      IReadOnlyList<DetectedBuildFramework> detectedBuildFrameworks)
    {
      using (new Tracer<RepositoryAnalysisService>(requestContext, TracePoints.RepositoryAnalysis.GetSuggestedConfigurationFilesFromBuildFrameworksEnter, TracePoints.RepositoryAnalysis.GetSuggestedConfigurationFilesFromBuildFrameworksLeave, nameof (GetSuggestedConfigurationFiles)))
        return this.FindRecommendations(requestContext, repositoryContext, detectedBuildFrameworks);
    }

    public IEnumerable<string> FindExistingConfigurationFilePaths(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      Func<string, bool> validationFunction)
    {
      using (new Tracer<RepositoryAnalysisService>(requestContext, TracePoints.RepositoryAnalysis.FindExistingConfigurationFiles, TracePoints.RepositoryAnalysis.FindExistingConfigurationFiles, nameof (FindExistingConfigurationFilePaths)))
        return this.FindConfigurationFilePaths(requestContext, projectId, connectionId, repositoryType, repository, branch, validationFunction);
    }

    public string GetRecommendedConfigurationPath(
      IVssRequestContext requestContext,
      HashSet<string> files)
    {
      using (new Tracer<RepositoryAnalysisService>(requestContext, TracePoints.RepositoryAnalysis.GetRecommendedConfigurationPath, TracePoints.RepositoryAnalysis.GetRecommendedConfigurationPath, nameof (GetRecommendedConfigurationPath)))
      {
        if (files != null && files.Contains("/azure-pipelines.yml"))
        {
          int result;
          int valueOrDefault = files.Select<string, Match>((Func<string, Match>) (x => RepositoryAnalysisService.s_configFileRegex.Match(x))).Where<Match>((Func<Match, bool>) (x => x.Success)).Select<Match, string>((Func<Match, string>) (x => x.Groups["number"].Value)).Select<string, int?>((Func<string, int?>) (x => !int.TryParse(x, out result) ? new int?() : new int?(result))).Max().GetValueOrDefault();
          if (valueOrDefault >= 0 && valueOrDefault != int.MaxValue)
          {
            string configurationPath = string.Format("{0}azure-pipelines-{1}.yml", (object) "/", (object) (valueOrDefault + 1));
            if (!files.Contains(configurationPath))
              return configurationPath;
          }
        }
        return this.PrimaryConfigurationPath;
      }
    }

    private ConfigurationFile SearchForConfigurationFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch)
    {
      return RepositoryAnalysisService.s_wellKnownYamlNames.Select<string, ConfigurationFile>((Func<string, ConfigurationFile>) (fileName => this.GetExistingConfigurationFileFromPath(requestContext, projectId, repositoryType, repository, connectionId, branch, fileName))).FirstOrDefault<ConfigurationFile>((Func<ConfigurationFile, bool>) (f => f != null));
    }

    private ConfigurationFile GetExistingConfigurationFileFromPath(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      string path)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryType);
      try
      {
        FileContentData fileContentData = sourceProvider.GetFileContentData(requestContext, projectId, connectionId, repository, branch, path);
        if (fileContentData != null)
          return new ConfigurationFile(fileContentData.Path, fileContentData.Content, false, fileContentData.Url, fileContentData.ObjectId);
        requestContext.TraceError(TracePoints.RepositoryAnalysis.SearchForConfigurationFile, nameof (RepositoryAnalysisService), "Repository " + repository + " from source provider " + repositoryType + " on branch " + branch + " for file " + path + " returned null for content.");
      }
      catch (FileNotFoundException ex)
      {
        requestContext.TraceInfo(TracePoints.RepositoryAnalysis.SearchForConfigurationFile, nameof (RepositoryAnalysisService), "Could not find file " + path + " in repo " + repository);
      }
      return (ConfigurationFile) null;
    }

    private IReadOnlyList<Template> FindRecommendations(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      IReadOnlyList<DetectedBuildFramework> detectedBuildFrameworks)
    {
      IPipelineFragmentProviderService pipelineFragmentProviderService = requestContext.GetService<IPipelineFragmentProviderService>();
      List<Template> list = detectedBuildFrameworks.SelectMany<DetectedBuildFramework, Template>((Func<DetectedBuildFramework, IEnumerable<Template>>) (f => (IEnumerable<Template>) pipelineFragmentProviderService.GetTemplates(requestContext, repositoryContext, f))).ToList<Template>();
      Dictionary<string, Template> configFiles = this.GetConfigFiles(requestContext);
      if (list.Count > 0)
      {
        foreach (Template template in list)
          configFiles[template.Id] = template;
      }
      else
        this.RecommendDefaultConfig(requestContext, (IDictionary<string, Template>) configFiles);
      return (IReadOnlyList<Template>) configFiles.Values.ToList<Template>();
    }

    private IEnumerable<string> FindConfigurationFilePaths(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? connectionId,
      string repositoryType,
      string repository,
      string branch,
      Func<string, bool> validationFunction)
    {
      TreeAnalysis treeAnalysis = this.GetTreeAnalysis(requestContext, projectId, connectionId, repositoryType, repository, branch);
      return treeAnalysis == null ? Enumerable.Empty<string>() : treeAnalysis.NodeDictionary.SelectMany<KeyValuePair<string, IList<TreeNode>>, TreeNode>((Func<KeyValuePair<string, IList<TreeNode>>, IEnumerable<TreeNode>>) (kvp => (IEnumerable<TreeNode>) kvp.Value)).Where<TreeNode>((Func<TreeNode, bool>) (node => !node.IsDirectory && validationFunction(node.Name))).Select<TreeNode, string>((Func<TreeNode, string>) (node => node.Path));
    }

    private TreeAnalysis GetTreeAnalysis(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? connectionId,
      string repositoryType,
      string repository,
      string branch)
    {
      return requestContext.GetService<ITreeAnalysisProviderService>().GetTreeAnalysis(requestContext, projectId, connectionId, repositoryType, repository, branch);
    }

    private void RecommendDefaultConfig(
      IVssRequestContext requestContext,
      IDictionary<string, Template> configDictionary)
    {
      Template template;
      configDictionary.TryGetValue(TemplateIds.Pipelines.Empty, out template);
      if (template == null)
        requestContext.TraceError(TracePoints.RepositoryAnalysis.UpdateToRecommended, nameof (RepositoryAnalysisService), "Template " + TemplateIds.Pipelines.Empty + " could not be found");
      else
        template.RecommendedWeight = 1000;
    }

    private Dictionary<string, Template> GetConfigFiles(IVssRequestContext requestContext)
    {
      requestContext.GetService<IContributedFeatureService>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<ITemplatesService>().GetTemplates(vssRequestContext).ToDictionary<Template, string, Template>((Func<Template, string>) (x => x.Id), (Func<Template, Template>) (x => x));
    }
  }
}
