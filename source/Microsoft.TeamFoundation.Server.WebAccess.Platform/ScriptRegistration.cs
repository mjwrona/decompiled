// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ScriptRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ScriptRegistration
  {
    private static List<ScriptArea> s_registeredResourceAreas = new List<ScriptArea>();
    private static List<ContributionPath> s_registeredContributedPaths = new List<ContributionPath>();
    private static List<ModuleShimConfiguration> s_registeredModuleShimConfiguration = new List<ModuleShimConfiguration>();
    private static DateTime lastModified = DateTime.UtcNow;
    private static Dictionary<string, ResourceModuleInfo> s_resourceModulesById;

    public static DateTime GetLastModified() => ScriptRegistration.lastModified;

    public static IEnumerable<ScriptArea> RegisteredAreas => (IEnumerable<ScriptArea>) ScriptRegistration.s_registeredResourceAreas;

    public static IEnumerable<ContributionPath> ContributedPaths => (IEnumerable<ContributionPath>) ScriptRegistration.s_registeredContributedPaths;

    public static IEnumerable<ModuleShimConfiguration> ModuleShimConfigurations => (IEnumerable<ModuleShimConfiguration>) ScriptRegistration.s_registeredModuleShimConfiguration;

    public static ResourceModuleInfo GetResourceManagerForScriptModule(string moduleId)
    {
      if (ScriptRegistration.s_resourceModulesById == null)
      {
        Dictionary<string, ResourceModuleInfo> dictionary = new Dictionary<string, ResourceModuleInfo>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ScriptArea registeredArea in ScriptRegistration.RegisteredAreas)
        {
          foreach (KeyValuePair<string, Func<ResourceManager>> registeredResource in registeredArea.RegisteredResources)
          {
            string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}.Resources.{2}", (object) registeredArea.ResourcesModulePath, (object) registeredArea.Prefix, (object) registeredResource.Key);
            dictionary[key] = new ResourceModuleInfo()
            {
              Name = registeredResource.Key,
              ModuleId = moduleId,
              GetResourceManager = registeredResource.Value
            };
          }
        }
        ScriptRegistration.s_resourceModulesById = dictionary;
      }
      ResourceModuleInfo managerForScriptModule;
      ScriptRegistration.s_resourceModulesById.TryGetValue(moduleId, out managerForScriptModule);
      return managerForScriptModule;
    }

    public static ScriptArea RegisterArea(string name, Func<ResourceManager> resourceGenerator) => ScriptRegistration.RegisterBundledArea(name, resourceGenerator, (string) null);

    public static ScriptArea RegisterBundledArea(
      string name,
      Func<ResourceManager> resourceGenerator,
      string bundlePrefix)
    {
      ScriptArea scriptArea = ScriptRegistration.RegisterArea(name + "/Scripts", name + "/Scripts/Resources");
      if (!string.IsNullOrEmpty(bundlePrefix))
        scriptArea.Prefix = bundlePrefix;
      scriptArea.RegisterResource(name, resourceGenerator);
      return scriptArea;
    }

    public static ScriptArea RegisterArea(string baseModulePath, string resourceModulesPath) => ScriptRegistration.RegisterBundledArea(baseModulePath, resourceModulesPath, (string) null);

    public static ScriptArea RegisterBundledArea(
      string baseModulePath,
      string resourceModulesPath,
      string bundlePrefix)
    {
      ScriptArea scriptArea = new ScriptArea(baseModulePath, resourceModulesPath);
      if (!string.IsNullOrEmpty(bundlePrefix))
        scriptArea.Prefix = bundlePrefix;
      ScriptRegistration.s_registeredResourceAreas.Add(scriptArea);
      return scriptArea;
    }

    public static ContributionPath RegisterContributedPath(string path, ContributionPathType type = ContributionPathType.Default)
    {
      ContributionPath contributionPath = new ContributionPath()
      {
        PathType = type,
        Value = path
      };
      ScriptRegistration.s_registeredContributedPaths.Add(contributionPath);
      return contributionPath;
    }

    public static ModuleShimConfiguration RegisterShimConfiguration(
      string module,
      string[] dependencies,
      string exports)
    {
      ModuleShimConfiguration shimConfiguration = new ModuleShimConfiguration()
      {
        ModuleId = module,
        Dependencies = dependencies,
        Exports = exports
      };
      ScriptRegistration.s_registeredModuleShimConfiguration.Add(shimConfiguration);
      return shimConfiguration;
    }

    public static ResourceManager GetResourceManager(string resourceName)
    {
      foreach (ScriptArea registeredResourceArea in ScriptRegistration.s_registeredResourceAreas)
      {
        Func<ResourceManager> func;
        if (registeredResourceArea.RegisteredResources.TryGetValue(resourceName, out func) && func != null)
          return func();
      }
      return (ResourceManager) null;
    }

    public static IEnumerable<ResourceManager> GetResourceManagers()
    {
      foreach (ScriptArea registeredResourceArea in ScriptRegistration.s_registeredResourceAreas)
      {
        foreach (string key in registeredResourceArea.RegisteredResources.Keys)
          yield return ScriptRegistration.GetResourceManager(key);
      }
    }
  }
}
