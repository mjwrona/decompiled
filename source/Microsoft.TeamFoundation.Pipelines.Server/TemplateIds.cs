// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TemplateIds
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class TemplateIds
  {
    public static readonly string Android = "android";
    public static readonly string Ant = "ant";
    public static readonly string Aspnet = "aspnet";
    public static readonly string AspnetCore = "aspnetcore";
    public static readonly string AspnetCoreNetFramework = "aspnetcorenetframework";
    public static readonly string Go = "go";
    public static readonly string Gradle = "gradle";
    public static readonly string Html = "html";
    public static readonly string Maven = "maven";
    public static readonly string NetDesktop = "netdesktop";
    public static readonly string NodeJs = "nodejs";
    public static readonly string NodeJsWithAngular = "nodejswithangular";
    public static readonly string NodeJsWithGrunt = "nodejswithgrunt";
    public static readonly string NodeJsWithGulp = "nodejswithgulp";
    public static readonly string NodeJsWithWebpack = "nodejswithwebpack";
    public static readonly string Php = "php";
    public static readonly string PythonDjango = "pythondjango";
    public static readonly string PythonPackage = "pythonpackage";
    public static readonly string Ruby = "ruby";
    public static readonly string UniversalWindowsPlatform = "universalwindowsplatform";
    public static readonly string Xcode = "xcode";

    public static class Pipelines
    {
      public static readonly string AspnetCoreFunctionAppToWindowsOnAzure = "aspnetcorefunctionapptowindowsonazure";
      public static readonly string DockerBuild = "dockerbuild";
      public static readonly string DeployToExistingK8s = "deploytoexistingkubernetescluster";
      public static readonly string DockerContainer = "dockercontainer";
      public static readonly string DockerContainerFunctionApp = "dockercontainerfunctionapp";
      public static readonly string DockerContainerToAcr = "dockercontainertoacr";
      public static readonly string DockerContainerToAks = "dockercontainertoaks";
      public static readonly string DockerContainerWebapp = "dockercontainerwebapp";
      public static readonly string Empty = "empty";
      public static readonly string Gcc = "gcc";
      public static readonly string JekyllContainer = "jekyllcontainer";
      public static readonly string MavenWebAppToLinuxOnAzure = "mavenwebapptolinuxonazure";
      public static readonly string PythonFunctionAppToLinuxOnAzure = "pythonfunctionapptolinuxonazure";
      public static readonly string NodeJsExpressWebAppToLinuxOnAzure = "nodejsexpresswebapptolinuxonazure";
      public static readonly string NodeJsFunctionAppToLinuxOnAzure = "nodejsfunctionapptolinuxonazure";
      public static readonly string NodeJsReactWebAppToLinuxOnAzure = "nodejsreactwebapptolinuxonazure";
      public static readonly string NodeJsWithReact = "nodejswithreact";
      public static readonly string NodeJsWithVue = "nodejswithvue";
      public static readonly string PhpWebAppToLinuxOnAzure = "phpwebapptolinuxonazure";
      public static readonly string PowershellFunctionAppToWindowsOnAzure = "powershellfunctionapptowindowsonazure";
      public static readonly string PythonToLinuxWebAppOnAzure = "pythontolinuxwebapponazure";
      public static readonly string XamarinAndroid = "xamarinandroid";
      public static readonly string XamarinIos = "xamarinios";

      public static IReadOnlyList<string> Ids { get; } = (IReadOnlyList<string>) new string[23]
      {
        TemplateIds.Pipelines.AspnetCoreFunctionAppToWindowsOnAzure,
        TemplateIds.Pipelines.DockerBuild,
        TemplateIds.Pipelines.DeployToExistingK8s,
        TemplateIds.Pipelines.DockerContainer,
        TemplateIds.Pipelines.DockerContainerFunctionApp,
        TemplateIds.Pipelines.DockerContainerToAcr,
        TemplateIds.Pipelines.DockerContainerToAks,
        TemplateIds.Pipelines.DockerContainerWebapp,
        TemplateIds.Pipelines.Empty,
        TemplateIds.Pipelines.Gcc,
        TemplateIds.Pipelines.JekyllContainer,
        TemplateIds.Pipelines.MavenWebAppToLinuxOnAzure,
        TemplateIds.Pipelines.PythonFunctionAppToLinuxOnAzure,
        TemplateIds.Pipelines.NodeJsExpressWebAppToLinuxOnAzure,
        TemplateIds.Pipelines.NodeJsFunctionAppToLinuxOnAzure,
        TemplateIds.Pipelines.NodeJsReactWebAppToLinuxOnAzure,
        TemplateIds.Pipelines.NodeJsWithReact,
        TemplateIds.Pipelines.NodeJsWithVue,
        TemplateIds.Pipelines.PhpWebAppToLinuxOnAzure,
        TemplateIds.Pipelines.PowershellFunctionAppToWindowsOnAzure,
        TemplateIds.Pipelines.PythonToLinuxWebAppOnAzure,
        TemplateIds.Pipelines.XamarinAndroid,
        TemplateIds.Pipelines.XamarinIos
      };
    }

    public static class Workflow
    {
      public static readonly string Blank = "blank";
      public static readonly string C_Cpp = "ccpp";
      public static readonly string Clojure = "clojure";
      public static readonly string Crystal = "crystal";
      public static readonly string Dart = "dart";
      public static readonly string DockerImage = "dockerimage";
      public static readonly string Elixir = "elixir";
      public static readonly string Erlang = "erlang";
      public static readonly string Haskell = "haskell";
      public static readonly string Jekyll = "jekyll";
      public static readonly string Rust = "rust";
      public static readonly string Xamarin = "xamarin";

      public static IReadOnlyList<string> Ids { get; } = (IReadOnlyList<string>) new string[12]
      {
        TemplateIds.Workflow.Blank,
        TemplateIds.Workflow.C_Cpp,
        TemplateIds.Workflow.Clojure,
        TemplateIds.Workflow.Crystal,
        TemplateIds.Workflow.Dart,
        TemplateIds.Workflow.DockerImage,
        TemplateIds.Workflow.Elixir,
        TemplateIds.Workflow.Erlang,
        TemplateIds.Workflow.Haskell,
        TemplateIds.Workflow.Jekyll,
        TemplateIds.Workflow.Rust,
        TemplateIds.Workflow.Xamarin
      };
    }
  }
}
