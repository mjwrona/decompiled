// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.MsBuildBuildFrameworkDetector
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
  public class MsBuildBuildFrameworkDetector : IBuildFrameworkDetector
  {
    private const string c_solutionExtension = ".sln";
    private const string c_csprojExtension = ".csproj";
    private const string c_vbprojExtension = ".vbproj";
    private const string c_fsprojExtension = ".fsproj";
    public static readonly string Id = "msbuild";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<MsBuildBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.MsBuildTryDetectEnter, TracePoints.BuildFrameworkDetection.MsBuildTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (MsBuildBuildFrameworkDetector.HasSolutionFile(treeAnalysis))
        {
          FilePath solutionPath = treeAnalysis.GetMatchingFilesByExtension(".sln").FirstOrDefault<FilePath>();
          IEnumerable<VsProjectFile> solutionFile = new MsBuildProjectParser(requestContext, fileContentsProvider).ParseSolutionFile(solutionPath, detectionType == BuildFrameworkDetectionType.Shallow);
          List<DetectedBuildTarget> functionAppBuildTargets = MsBuildBuildFrameworkDetector.GetAzureFunctionAppBuildTargets(requestContext, treeAnalysis);
          functionAppBuildTargets.Add(new DetectedBuildTarget(MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.SolutionFile, solutionPath.ToString(), (IReadOnlyDictionary<string, string>) new Dictionary<string, string>()));
          int maxProjectsToRead = MsBuildBuildFrameworkDetector.GetMaxProjectsToRead(requestContext);
          detectedBuildFramework = MsBuildBuildFrameworkDetector.GetFramework(solutionFile, functionAppBuildTargets, maxProjectsToRead);
        }
        else if (MsBuildBuildFrameworkDetector.LooksLikeMsBuild(treeAnalysis))
        {
          FilePath projectPath = treeAnalysis.GetMatchingFilesByExtension(".csproj", ".vbproj", ".fsproj").FirstOrDefault<FilePath>();
          VsProjectFile projectFile = new MsBuildProjectParser(requestContext, fileContentsProvider).ParseProjectFile(projectPath);
          List<DetectedBuildTarget> functionAppBuildTargets = MsBuildBuildFrameworkDetector.GetAzureFunctionAppBuildTargets(requestContext, treeAnalysis);
          detectedBuildFramework = MsBuildBuildFrameworkDetector.GetFramework((IEnumerable<VsProjectFile>) new VsProjectFile[1]
          {
            projectFile
          }, functionAppBuildTargets, 1);
        }
        return detectedBuildFramework != null;
      }
    }

    private static List<DetectedBuildTarget> GetAzureFunctionAppBuildTargets(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis)
    {
      return new AzureFunctionAppDetector().GetAzureFunctionApps(requestContext, treeAnalysis).Select<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>((Func<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>) (app => new DetectedBuildTarget(MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.AzureFunctionApp, app.Host.ToString()))).ToList<DetectedBuildTarget>();
    }

    private static bool HasSolutionFile(TreeAnalysis treeAnalysis) => treeAnalysis.FileTypes.Contains(".sln");

    private static bool LooksLikeMsBuild(TreeAnalysis treeAnalysis) => treeAnalysis.FileTypes.Contains(".csproj") || treeAnalysis.FileTypes.Contains(".vbproj") || treeAnalysis.FileTypes.Contains(".fsproj");

    private static int GetMaxProjectsToRead(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/MsBuildBuildFrameworkDetector/MaxProjectsToRead", 50);

    private static DetectedBuildFramework GetFramework(
      IEnumerable<VsProjectFile> projectFiles,
      List<DetectedBuildTarget> buildTargets,
      int maxProjectsToRead)
    {
      List<(VsProjectFile, string)> list = projectFiles.Take<VsProjectFile>(maxProjectsToRead).Select<VsProjectFile, (VsProjectFile, string)>((Func<VsProjectFile, (VsProjectFile, string)>) (project => (project, MsBuildBuildFrameworkDetector.GetWellKnownType(project)))).Where<(VsProjectFile, string)>((Func<(VsProjectFile, string), bool>) (t => t.projectType != null)).ToList<(VsProjectFile, string)>();
      string version = list.Select<(VsProjectFile, string), string>((Func<(VsProjectFile, string), string>) (t => t.project.ToolsVersion)).LastOrDefault<string>();
      buildTargets.AddRange(list.Select<(VsProjectFile, string), DetectedBuildTarget>((Func<(VsProjectFile, string), DetectedBuildTarget>) (t => new DetectedBuildTarget(t.projectType, t.project.Path.ToString(), (IReadOnlyDictionary<string, string>) MsBuildBuildFrameworkDetector.GetProjectSettings(t.project)))));
      return new DetectedBuildFramework(MsBuildBuildFrameworkDetector.Id, version, (IReadOnlyList<DetectedBuildTarget>) buildTargets);
    }

    private static Dictionary<string, string> GetProjectSettings(VsProjectFile projectFile)
    {
      Dictionary<string, string> projectSettings = new Dictionary<string, string>();
      if (!string.IsNullOrEmpty(projectFile.TargetFramework))
        projectSettings[MsBuildBuildFrameworkDetector.Settings.TargetFramework] = projectFile.TargetFramework;
      if (projectFile.Id != Guid.Empty)
        projectSettings[MsBuildBuildFrameworkDetector.Settings.ProjectId] = projectFile.Id.ToString();
      if (!string.IsNullOrEmpty(projectFile.Name))
        projectSettings[MsBuildBuildFrameworkDetector.Settings.ProjectName] = projectFile.Name;
      return projectSettings;
    }

    private static string GetWellKnownType(VsProjectFile projectFile)
    {
      HashSet<Guid> hashSet = projectFile.ProjectTypes.ToHashSet<VsProjectType, Guid>((Func<VsProjectType, Guid>) (t => t.Type));
      if (hashSet.Contains(VsProjectType.WellKnownTypes.ASPNET_CORE))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.AspNetCore;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.Xamarin_Android))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.XamarinAndroid;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.Xamarin_iOS))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.XamarinIos;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.UniversalWindowsClassLibrary))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.UniversalWindowsPlatform;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.EXE))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.Exe;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.FUNCTION))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.Function;
      if (hashSet.Contains(VsProjectType.WellKnownTypes.WebApplication))
        return MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.WebApp;
      return hashSet.Contains(VsProjectType.WellKnownTypes.WebSite) ? MsBuildBuildFrameworkDetector.Settings.WellKnownTypes.WebSite : (string) null;
    }

    public static class Settings
    {
      public static readonly string ProjectId = MsBuildBuildFrameworkDetector.Id + ".project.id";
      public static readonly string ProjectName = MsBuildBuildFrameworkDetector.Id + ".project.name";
      public static readonly string TargetFramework = MsBuildBuildFrameworkDetector.Id + ".project.targetFramework";

      public static class WellKnownTypes
      {
        public static readonly string WebApp = nameof (WebApp);
        public static readonly string AspNetCore = nameof (AspNetCore);
        public static readonly string WebSite = nameof (WebSite);
        public static readonly string Function = nameof (Function);
        public static readonly string Exe = nameof (Exe);
        public static readonly string SolutionFile = nameof (SolutionFile);
        public static readonly string UniversalWindowsPlatform = nameof (UniversalWindowsPlatform);
        public static readonly string XamarinAndroid = nameof (XamarinAndroid);
        public static readonly string XamarinIos = nameof (XamarinIos);
        public static readonly string AzureFunctionApp = nameof (AzureFunctionApp);
      }
    }
  }
}
