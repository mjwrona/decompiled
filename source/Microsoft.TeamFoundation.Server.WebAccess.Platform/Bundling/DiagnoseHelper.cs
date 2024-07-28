// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.DiagnoseHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  public static class DiagnoseHelper
  {
    private const string c_bundleVariable = "__vssBundles";

    public static string GetBundleInfo(string id, IEnumerable<FileInfo> dependencies)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "if (typeof {0} ==='undefined') {{ var {0} = []; }}", (object) "__vssBundles"));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{0}.length] = {1};", (object) "__vssBundles", (object) JsonConvert.SerializeObject((object) new BundleInfo()
      {
        Id = id,
        Modules = Array.Empty<string>(),
        ImmediateModules = dependencies.Select<FileInfo, string>((Func<FileInfo, string>) (fi => VssScriptsModuleInfo.GetVssScriptsModuleInfo(fi).Id)).ToArray<string>()
      })));
      return stringBuilder.ToString();
    }

    public static string GetBundleModuleInfo(
      string id,
      long size,
      string directDependenciesString,
      string expandedDependenciesString)
    {
      StringBuilder stringBuilder = new StringBuilder();
      BundleModuleInfo bundleModuleInfo = new BundleModuleInfo()
      {
        Id = id,
        Size = size,
        Dependencies = Array.Empty<string>()
      };
      if (!string.IsNullOrEmpty(directDependenciesString))
        bundleModuleInfo.Dependencies = directDependenciesString.Substring(VssScriptsModuleInfo.s_directDependenciesStart.Length).Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
      else if (!string.IsNullOrEmpty(expandedDependenciesString))
        bundleModuleInfo.Dependencies = expandedDependenciesString.Substring(VssScriptsModuleInfo.s_dependenciesStart.Length).Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries);
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{0}.length-1].Modules.push({1});", (object) "__vssBundles", (object) JsonConvert.SerializeObject((object) bundleModuleInfo)));
      return stringBuilder.ToString();
    }
  }
}
