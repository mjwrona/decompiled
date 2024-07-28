// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DotNetCoreBuildFrameworkDetector
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DotNetCoreBuildFrameworkDetector : IBuildFrameworkDetector
  {
    private const string c_csprojExtension = ".csproj";
    private const string c_fsprojExtension = ".fsproj";
    public static readonly string Id = "dotnetcore";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<DotNetCoreBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.DotNetCoreTryDetectEnter, TracePoints.BuildFrameworkDetection.DotNetCoreTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (!DotNetCoreBuildFrameworkDetector.LooksLikeDotNetCore(treeAnalysis))
          return false;
        int maxProjectsToRead = DotNetCoreBuildFrameworkDetector.GetMaxProjectsToRead(requestContext, detectionType);
        List<DetectedBuildTarget> list = DotNetCoreBuildFrameworkDetector.GetRootProjects((IReadOnlyList<VsProjectFile>) DotNetCoreBuildFrameworkDetector.GetProjects(requestContext, treeAnalysis, fileContentsProvider, maxProjectsToRead).ToList<VsProjectFile>()).Select<VsProjectFile, (VsProjectFile, string)>((Func<VsProjectFile, (VsProjectFile, string)>) (project => (project, DotNetCoreBuildFrameworkDetector.GetProjectType(project)))).Where<(VsProjectFile, string)>((Func<(VsProjectFile, string), bool>) (t => !string.IsNullOrEmpty(t.projectType))).Select<(VsProjectFile, string), DetectedBuildTarget>((Func<(VsProjectFile, string), DetectedBuildTarget>) (t => new DetectedBuildTarget(t.projectType, t.project.Path.ToString(), (IReadOnlyDictionary<string, string>) DotNetCoreBuildFrameworkDetector.GetProjectSettings(t.project)))).ToList<DetectedBuildTarget>();
        if (list.Count == 0)
          return false;
        list.AddRange(DotNetCoreBuildFrameworkDetector.GetAzureFunctionAppBuildTargets(requestContext, fileContentsProvider, treeAnalysis));
        detectedBuildFramework = new DetectedBuildFramework(DotNetCoreBuildFrameworkDetector.Id, string.Empty, (IReadOnlyList<DetectedBuildTarget>) list);
        return true;
      }
    }

    private static bool LooksLikeDotNetCore(TreeAnalysis treeAnalysis) => treeAnalysis.FileTypes.Contains(".csproj") || treeAnalysis.FileTypes.Contains(".fsproj");

    private static Dictionary<string, string> GetProjectSettings(VsProjectFile project)
    {
      Dictionary<string, string> projectSettings = new Dictionary<string, string>();
      if (project.Id != Guid.Empty)
        projectSettings[DotNetCoreBuildFrameworkDetector.Settings.ProjectId] = project.Id.ToString();
      if (!string.IsNullOrEmpty(project.Name))
        projectSettings[DotNetCoreBuildFrameworkDetector.Settings.ProjectName] = project.Name;
      if (!string.IsNullOrEmpty(project.TargetFramework))
        projectSettings[DotNetCoreBuildFrameworkDetector.Settings.ProjectTargetFramework] = project.TargetFramework;
      return projectSettings;
    }

    private static int GetMaxProjectsToRead(
      IVssRequestContext requestContext,
      BuildFrameworkDetectionType detectionType)
    {
      return detectionType == BuildFrameworkDetectionType.Shallow ? 1 : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/DotNetCoreBuildFrameworkDetector/MaxProjectsToRead", 50);
    }

    private static IEnumerable<VsProjectFile> GetProjects(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      int maxProjectsToRead)
    {
      return treeAnalysis.GetMatchingFilesByExtension(".csproj", ".fsproj").Take<FilePath>(maxProjectsToRead).Select<FilePath, VsProjectFile>((Func<FilePath, VsProjectFile>) (projectPath => new MsBuildProjectParser(requestContext, fileContentsProvider).ParseProjectFile(projectPath)));
    }

    private static IEnumerable<VsProjectFile> GetRootProjects(IReadOnlyList<VsProjectFile> projects)
    {
      return projects.Where<VsProjectFile>((Func<VsProjectFile, bool>) (p => !IsReferenced(p)));

      bool IsReferenced(VsProjectFile project) => projects.Any<VsProjectFile>((Func<VsProjectFile, bool>) (p => p.ProjectReferences.Select<string, FilePath>((Func<string, FilePath>) (r => p.Path.Folder.AppendPath(r))).Any<FilePath>((Func<FilePath, bool>) (rPath => rPath == project.Path))));
    }

    private static string GetProjectType(VsProjectFile project)
    {
      HashSet<Guid> hashSet = project.ProjectTypes.ToHashSet<VsProjectType, Guid>((Func<VsProjectType, Guid>) (t => t.Type));
      string projectType = string.Empty;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.ASPNET_CORE))
        projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.AspNetCore;
      else if (hashSet.Contains(VsProjectType.WellKnownTypes.DOTNETCORE_TEST))
        projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.DotNetCoreTest;
      else if (hashSet.Contains(VsProjectType.WellKnownTypes.DOTNETCORE_WEB))
        projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.DotNetCoreWeb;
      else if (hashSet.Contains(VsProjectType.WellKnownTypes.DOTNETCORE))
      {
        projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.DotNetCore;
        if (hashSet.Contains(VsProjectType.WellKnownTypes.EXE))
          projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.Exe;
        else if (hashSet.Contains(VsProjectType.WellKnownTypes.FUNCTION))
          projectType = DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.Function;
      }
      return projectType;
    }

    private static IEnumerable<DetectedBuildTarget> GetAzureFunctionAppBuildTargets(
      IVssRequestContext requestContext,
      IFileContentsProvider fileContentsProvider,
      TreeAnalysis treeAnalysis)
    {
      return new AzureFunctionAppDetector().GetAzureFunctionApps(requestContext, treeAnalysis).Select<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>((Func<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>) (app => new DetectedBuildTarget(DotNetCoreBuildFrameworkDetector.Settings.WellKnownTypes.AzureFunctionApp, app.Host.ToString())));
    }

    public static class Settings
    {
      public static readonly string ProjectId = DotNetCoreBuildFrameworkDetector.Id + ".project.id";
      public static readonly string ProjectName = DotNetCoreBuildFrameworkDetector.Id + ".project.name";
      public static readonly string ProjectTargetFramework = DotNetCoreBuildFrameworkDetector.Id + ".project.targetFramework";
      public static readonly string WorkingDirectory = "workingDirectory";

      public static class WellKnownTypes
      {
        public static readonly string AspNetCore = nameof (AspNetCore);
        public static readonly string DotNetCore = nameof (DotNetCore);
        public static readonly string DotNetCoreTest = nameof (DotNetCoreTest);
        public static readonly string DotNetCoreWeb = nameof (DotNetCoreWeb);
        public static readonly string Function = nameof (Function);
        public static readonly string Exe = nameof (Exe);
        public static readonly string AzureFunctionApp = nameof (AzureFunctionApp);
      }
    }
  }
}
