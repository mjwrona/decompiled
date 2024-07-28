// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Helpers.TemplateRules
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Helpers
{
  public static class TemplateRules
  {
    private static readonly IReadOnlyDictionary<string, int> s_templateMatchQuality = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>()
    {
      {
        TemplateIds.Pipelines.DockerBuild,
        36000
      },
      {
        TemplateIds.Pipelines.DockerContainer,
        35000
      },
      {
        TemplateIds.Workflow.DockerImage,
        34900
      },
      {
        TemplateIds.Pipelines.DeployToExistingK8s,
        34000
      },
      {
        TemplateIds.Pipelines.DockerContainerToAks,
        31000
      },
      {
        TemplateIds.Pipelines.DockerContainerWebapp,
        30000
      },
      {
        TemplateIds.Pipelines.DockerContainerFunctionApp,
        30000
      },
      {
        TemplateIds.Pipelines.DockerContainerToAcr,
        30000
      },
      {
        TemplateIds.AspnetCore,
        29000
      },
      {
        TemplateIds.Aspnet,
        28000
      },
      {
        TemplateIds.Maven,
        27000
      },
      {
        TemplateIds.Pipelines.MavenWebAppToLinuxOnAzure,
        26500
      },
      {
        TemplateIds.NodeJs,
        26000
      },
      {
        TemplateIds.Pipelines.NodeJsExpressWebAppToLinuxOnAzure,
        25500
      },
      {
        TemplateIds.Pipelines.NodeJsFunctionAppToLinuxOnAzure,
        25250
      },
      {
        TemplateIds.Pipelines.AspnetCoreFunctionAppToWindowsOnAzure,
        25250
      },
      {
        TemplateIds.Pipelines.PythonFunctionAppToLinuxOnAzure,
        25250
      },
      {
        TemplateIds.NodeJsWithAngular,
        25000
      },
      {
        TemplateIds.AspnetCoreNetFramework,
        24000
      },
      {
        TemplateIds.Pipelines.NodeJsWithReact,
        23000
      },
      {
        TemplateIds.Pipelines.NodeJsReactWebAppToLinuxOnAzure,
        22500
      },
      {
        TemplateIds.PythonPackage,
        22000
      },
      {
        TemplateIds.Php,
        21000
      },
      {
        TemplateIds.Pipelines.PhpWebAppToLinuxOnAzure,
        20500
      },
      {
        TemplateIds.Gradle,
        20000
      },
      {
        TemplateIds.Html,
        19000
      },
      {
        TemplateIds.UniversalWindowsPlatform,
        18000
      },
      {
        TemplateIds.Xcode,
        17000
      },
      {
        TemplateIds.Pipelines.XamarinAndroid,
        16000
      },
      {
        TemplateIds.Workflow.Xamarin,
        15900
      },
      {
        TemplateIds.Pipelines.Gcc,
        15000
      },
      {
        TemplateIds.Workflow.C_Cpp,
        14900
      },
      {
        TemplateIds.Android,
        14000
      },
      {
        TemplateIds.Go,
        13000
      },
      {
        TemplateIds.Pipelines.NodeJsWithVue,
        12000
      },
      {
        TemplateIds.NodeJsWithWebpack,
        11000
      },
      {
        TemplateIds.NodeJsWithGulp,
        10000
      },
      {
        TemplateIds.Pipelines.XamarinIos,
        9000
      },
      {
        TemplateIds.PythonDjango,
        8000
      },
      {
        TemplateIds.Pipelines.PythonToLinuxWebAppOnAzure,
        7750
      },
      {
        TemplateIds.Ant,
        7000
      },
      {
        TemplateIds.Ruby,
        6000
      },
      {
        TemplateIds.Pipelines.JekyllContainer,
        5000
      },
      {
        TemplateIds.Workflow.Jekyll,
        4900
      },
      {
        TemplateIds.NodeJsWithGrunt,
        4000
      },
      {
        TemplateIds.NetDesktop,
        3000
      }
    };
    private static readonly IReadOnlyDictionary<string, Func<TreeAnalysis, bool>> s_rulesDictionary;

    public static IEnumerable<string> GetRuleIds() => TemplateRules.s_rulesDictionary.Keys;

    public static int GetMatchQuality(string ruleName, TreeAnalysis treeAnalysis) => !TemplateRules.s_rulesDictionary[ruleName](treeAnalysis) ? 0 : TemplateRules.s_templateMatchQuality[ruleName];

    static TemplateRules()
    {
      IList<TreeNode> source;
      TemplateRules.s_rulesDictionary = (IReadOnlyDictionary<string, Func<TreeAnalysis, bool>>) new Dictionary<string, Func<TreeAnalysis, bool>>()
      {
        {
          TemplateIds.Workflow.DockerImage,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile") || treeAnalysis.NodeDictionary.ContainsKey("docker-compose.yml"))
        },
        {
          TemplateIds.Pipelines.DockerContainer,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile") || treeAnalysis.NodeDictionary.ContainsKey("docker-compose.yml"))
        },
        {
          TemplateIds.Pipelines.DockerBuild,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Pipelines.DeployToExistingK8s,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Pipelines.DockerContainerToAcr,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Pipelines.DockerContainerToAks,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Pipelines.DockerContainerFunctionApp,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Pipelines.DockerContainerWebapp,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Dockerfile"))
        },
        {
          TemplateIds.Ant,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("build.xml"))
        },
        {
          TemplateIds.Android,
          (Func<TreeAnalysis, bool>) (treeAnalysis => (treeAnalysis.NodeDictionary.ContainsKey("gradlew") || treeAnalysis.NodeDictionary.ContainsKey("gradlew.bat")) && treeAnalysis.NodeDictionary.ContainsKey("AndroidManifest.xml"))
        },
        {
          TemplateIds.Gradle,
          (Func<TreeAnalysis, bool>) (treeAnalysis => (treeAnalysis.NodeDictionary.ContainsKey("gradlew") || treeAnalysis.NodeDictionary.ContainsKey("gradlew.bat")) && !treeAnalysis.NodeDictionary.ContainsKey("AndroidManifest.xml"))
        },
        {
          TemplateIds.Pipelines.Gcc,
          (Func<TreeAnalysis, bool>) (treeAnalysis =>
          {
            if (!treeAnalysis.NodeDictionary.ContainsKey("Makefile"))
              return false;
            return treeAnalysis.FileTypes.Contains(".c") || treeAnalysis.FileTypes.Contains(".cpp") || treeAnalysis.FileTypes.Contains(".h") || treeAnalysis.FileTypes.Contains(".hpp");
          })
        },
        {
          TemplateIds.Workflow.C_Cpp,
          (Func<TreeAnalysis, bool>) (treeAnalysis =>
          {
            if (!treeAnalysis.NodeDictionary.ContainsKey("Makefile"))
              return false;
            return treeAnalysis.FileTypes.Contains(".c") || treeAnalysis.FileTypes.Contains(".cpp") || treeAnalysis.FileTypes.Contains(".h") || treeAnalysis.FileTypes.Contains(".hpp");
          })
        },
        {
          TemplateIds.Go,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Gopkg.lock") || treeAnalysis.NodeDictionary.ContainsKey("Godeps") || treeAnalysis.NodeDictionary.ContainsKey("Godeps.json") || treeAnalysis.NodeDictionary.ContainsKey("glide.yaml") || treeAnalysis.FileTypes.Contains(".go"))
        },
        {
          TemplateIds.Maven,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("pom.xml") || treeAnalysis.NodeDictionary.ContainsKey("pom.atom") || treeAnalysis.NodeDictionary.ContainsKey("pom.clj") || treeAnalysis.NodeDictionary.ContainsKey("pom.groovy") || treeAnalysis.NodeDictionary.ContainsKey("pom.rb") || treeAnalysis.NodeDictionary.ContainsKey("pom.scala") || treeAnalysis.NodeDictionary.ContainsKey("pom.yaml"))
        },
        {
          TemplateIds.Pipelines.MavenWebAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("pom.xml") || treeAnalysis.NodeDictionary.ContainsKey("pom.atom") || treeAnalysis.NodeDictionary.ContainsKey("pom.clj") || treeAnalysis.NodeDictionary.ContainsKey("pom.groovy") || treeAnalysis.NodeDictionary.ContainsKey("pom.rb") || treeAnalysis.NodeDictionary.ContainsKey("pom.scala") || treeAnalysis.NodeDictionary.ContainsKey("pom.yaml"))
        },
        {
          TemplateIds.NodeJs,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.Pipelines.NodeJsExpressWebAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.Pipelines.NodeJsFunctionAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("host.json") && treeAnalysis.NodeDictionary.ContainsKey("function.json"))
        },
        {
          TemplateIds.Pipelines.PythonFunctionAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("host.json") && treeAnalysis.NodeDictionary.ContainsKey("function.json") && treeAnalysis.FileTypes.Contains(".py"))
        },
        {
          TemplateIds.Pipelines.NodeJsReactWebAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.Pipelines.NodeJsWithVue,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.NodeJsWithWebpack,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.Pipelines.NodeJsWithReact,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("package.json"))
        },
        {
          TemplateIds.NodeJsWithAngular,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("angular.json"))
        },
        {
          TemplateIds.NodeJsWithGrunt,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Gruntfile.js"))
        },
        {
          TemplateIds.NodeJsWithGulp,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("gulpfile.js"))
        },
        {
          TemplateIds.Workflow.Jekyll,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("_config.yml"))
        },
        {
          TemplateIds.Pipelines.JekyllContainer,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("_config.yml"))
        },
        {
          TemplateIds.Php,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("composer.json") || treeAnalysis.NodeDictionary.ContainsKey("phpunit.xml") || treeAnalysis.NodeDictionary.ContainsKey("index.php"))
        },
        {
          TemplateIds.Pipelines.PhpWebAppToLinuxOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("composer.json") || treeAnalysis.NodeDictionary.ContainsKey("phpunit.xml") || treeAnalysis.NodeDictionary.ContainsKey("index.php"))
        },
        {
          TemplateIds.PythonPackage,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".py"))
        },
        {
          TemplateIds.PythonDjango,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("manage.py"))
        },
        {
          TemplateIds.Pipelines.PythonToLinuxWebAppOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".py"))
        },
        {
          TemplateIds.Ruby,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.ContainsKey("Gemfile") || treeAnalysis.NodeDictionary.ContainsKey("Gemfile.lock") || treeAnalysis.NodeDictionary.ContainsKey("Rakefile"))
        },
        {
          TemplateIds.Xcode,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.DirectoryExtensions.Contains(".xcworkspace") || treeAnalysis.DirectoryExtensions.Contains(".xcodeproj"))
        },
        {
          TemplateIds.Html,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.NodeDictionary.TryGetValue("index.html", out source) && source.Any<TreeNode>((Func<TreeNode, bool>) (x => string.Equals(x.Path, "/index.html", StringComparison.OrdinalIgnoreCase))) || treeAnalysis.NodeDictionary.TryGetValue("index.htm", out source) && source.Any<TreeNode>((Func<TreeNode, bool>) (x => string.Equals(x.Path, "/index.htm", StringComparison.OrdinalIgnoreCase))) || treeAnalysis.NodeDictionary.TryGetValue("default.html", out source) && source.Any<TreeNode>((Func<TreeNode, bool>) (x => string.Equals(x.Path, "/default.html", StringComparison.OrdinalIgnoreCase))) || treeAnalysis.NodeDictionary.TryGetValue("default.htm", out source) && source.Any<TreeNode>((Func<TreeNode, bool>) (x => string.Equals(x.Path, "/default.htm", StringComparison.OrdinalIgnoreCase))) || treeAnalysis.NodeDictionary.TryGetValue("default.asp", out source) && source.Any<TreeNode>((Func<TreeNode, bool>) (x => string.Equals(x.Path, "/default.asp", StringComparison.OrdinalIgnoreCase))) || treeAnalysis.NodeDictionary.ContainsKey("fileindex.html"))
        },
        {
          TemplateIds.Aspnet,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.AspnetCore,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln") || treeAnalysis.FileTypes.Contains(".csproj"))
        },
        {
          TemplateIds.Pipelines.AspnetCoreFunctionAppToWindowsOnAzure,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".cs") && treeAnalysis.NodeDictionary.ContainsKey("host.json"))
        },
        {
          TemplateIds.AspnetCoreNetFramework,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.NetDesktop,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.UniversalWindowsPlatform,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.Workflow.Xamarin,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.Pipelines.XamarinAndroid,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        },
        {
          TemplateIds.Pipelines.XamarinIos,
          (Func<TreeAnalysis, bool>) (treeAnalysis => treeAnalysis.FileTypes.Contains(".sln"))
        }
      };
    }
  }
}
