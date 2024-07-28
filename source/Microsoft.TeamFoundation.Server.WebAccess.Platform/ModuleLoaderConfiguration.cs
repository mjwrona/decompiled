// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ModuleLoaderConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ModuleLoaderConfiguration : WebSdkMetadata
  {
    private const int c_defaultModuleLoadTimeoutSeconds = 30;
    private const int c_debugModuleLoadTimeoutSeconds = 60;
    private string m_resourceBaseUrl;
    private string m_thirdPartyBaseUrl;

    public ModuleLoaderConfiguration(WebContext webContext, bool setDefaultConfiguration)
    {
      this.Initialize(webContext);
      if (!setDefaultConfiguration)
        return;
      this.SetDefaultConfiguration(webContext);
    }

    public ModuleLoaderConfiguration()
    {
    }

    [DataMember]
    public string BaseUrl { get; set; }

    public string FallbackBaseUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, string> Paths { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, Dictionary<string, string>> Map { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ContributionPath> ContributionPaths { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<string, ModuleLoaderShimConfiguration> Shim { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? WaitSeconds { get; set; }

    public void AddShimConfig(string moduleId) => this.AddShimConfig(moduleId, (string[]) null, (string) null);

    public void AddShimConfig(string moduleId, string[] dependencies) => this.AddShimConfig(moduleId, dependencies, (string) null);

    public void AddShimConfig(string moduleId, string[] dependencies, string exports) => this.Shim[moduleId] = new ModuleLoaderShimConfiguration(dependencies, exports);

    public void AddResourceModulePaths(string resourceBaseUrl)
    {
      this.m_resourceBaseUrl = resourceBaseUrl;
      foreach (ScriptArea scriptArea in ScriptRegistration.RegisteredAreas.Where<ScriptArea>((Func<ScriptArea, bool>) (ra => !string.IsNullOrEmpty(ra.ResourcesModulePath))))
      {
        string[] strArray = scriptArea.ResourcesModulePath.Split(new char[1]
        {
          ':'
        }, 2);
        if (strArray.Length == 1)
          this.Paths[scriptArea.ResourcesModulePath] = resourceBaseUrl;
        else
          this.Paths[strArray[0]] = Path.Combine(resourceBaseUrl, strArray[1]).Replace('\\', '/');
      }
      foreach (ContributionPath contributionPath in this.ContributionPaths.Values.Where<ContributionPath>((Func<ContributionPath, bool>) (c => c.PathType == ContributionPathType.Resource)))
        contributionPath.Value = resourceBaseUrl;
    }

    public void AddBaseModulePaths(string baseModulePrefix = null)
    {
      foreach (ScriptArea scriptArea in ScriptRegistration.RegisteredAreas.Where<ScriptArea>((Func<ScriptArea, bool>) (ra => !string.IsNullOrEmpty(ra.BaseModulePath))))
      {
        string str = scriptArea.BaseModulePath;
        if (!string.IsNullOrEmpty(baseModulePrefix))
          str = baseModulePrefix.TrimEnd('/') + "/" + scriptArea.BaseModulePath.TrimStart('/');
        this.Paths[scriptArea.BaseModulePath] = str;
      }
    }

    public void AddContributionPath(string path, ContributionPathType type = ContributionPathType.Default, string pathValue = null)
    {
      ContributionPath contributionPath = (ContributionPath) null;
      switch (type)
      {
        case ContributionPathType.Default:
          contributionPath = new ContributionPath()
          {
            Value = this.FallbackBaseUrl + (pathValue ?? path),
            PathType = type
          };
          break;
        case ContributionPathType.Resource:
          contributionPath = new ContributionPath()
          {
            Value = this.m_resourceBaseUrl,
            PathType = type
          };
          break;
        case ContributionPathType.ThirdParty:
          contributionPath = new ContributionPath()
          {
            Value = this.m_thirdPartyBaseUrl + pathValue,
            PathType = type
          };
          break;
      }
      if (contributionPath == null)
        return;
      this.ContributionPaths[path] = contributionPath;
    }

    private void Initialize(WebContext webContext)
    {
      this.BaseUrl = webContext.Url.TfsScriptContent(string.Empty, webContext.Diagnostics.DebugMode);
      this.FallbackBaseUrl = StaticResources.Versioned.Scripts.TFS.GetLocalLocation(webContext.Diagnostics.DebugMode ? "debug/" : "min/", webContext.TfsRequestContext);
      this.m_thirdPartyBaseUrl = StaticResources.ThirdParty.Scripts.GetLocation(string.Empty);
      this.Paths = new Dictionary<string, string>();
      this.Map = new Dictionary<string, Dictionary<string, string>>();
      this.Shim = new Dictionary<string, ModuleLoaderShimConfiguration>();
      this.ContributionPaths = new Dictionary<string, ContributionPath>();
    }

    private void SetDefaultConfiguration(WebContext webContext)
    {
      this.WaitSeconds = new int?(webContext.Diagnostics.DebugMode ? 60 : 30);
      this.AddShimConfig("jquery", (string[]) null, "jQuery");
      string resourceBaseUrl = Uri.EscapeDataString(CultureInfo.CurrentUICulture.Name);
      if (!string.IsNullOrEmpty(StaticResources.GetCdnRootUrl(webContext.TfsRequestContext)))
        resourceBaseUrl = this.FallbackBaseUrl + resourceBaseUrl;
      this.AddResourceModulePaths(resourceBaseUrl);
      this.AddContributionPath("VSS");
      this.AddContributionPath("VSS/Resources", ContributionPathType.Resource);
      this.AddContributionPath("q");
      this.AddContributionPath("knockout");
      this.AddContributionPath("mousetrap");
      this.AddContributionPath("mustache");
      this.AddContributionPath("react", pathValue: "react.15.3");
      this.AddContributionPath("react-dom", pathValue: "react-dom.15.3");
      this.AddContributionPath("react-transition-group", pathValue: "react-transition-group.15.3");
      this.AddContributionPath("jQueryUI");
      this.AddContributionPath("jquery");
      this.AddContributionPath("OfficeFabric");
      this.AddContributionPath("tslib");
      this.AddContributionPath("@uifabric");
      this.AddContributionPath("VSSUI");
      foreach (ContributionPath contributedPath in ScriptRegistration.ContributedPaths)
        this.AddContributionPath(contributedPath.Value, contributedPath.PathType);
      foreach (ModuleShimConfiguration shimConfiguration in ScriptRegistration.ModuleShimConfigurations)
        this.AddShimConfig(shimConfiguration.ModuleId, shimConfiguration.Dependencies, shimConfiguration.Exports);
    }
  }
}
