// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Bundling.CSSPluginModuleNameParser
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Bundling
{
  internal class CSSPluginModuleNameParser
  {
    private const string c_cssLoaderPluginModule = "VSS/LoaderPlugins/Css";
    private HashSet<string> m_cssModulePrefixes;
    private bool m_ignoreModulePrefix;

    internal CSSPluginModuleNameParser(bool ignoreModulePrefix, HashSet<string> cssModulePrefixes)
    {
      this.m_ignoreModulePrefix = ignoreModulePrefix;
      this.m_cssModulePrefixes = cssModulePrefixes;
    }

    public string GetCSSPluginFile(VssScriptsModuleInfo module)
    {
      string cssPluginFile = (string) null;
      if ("VSS/LoaderPlugins/Css".Equals(module.Id, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(module.PluginName))
      {
        cssPluginFile = module.PluginName;
        string[] strArray = cssPluginFile.Split(new char[1]
        {
          ':'
        }, 2);
        if (strArray.Length > 1)
        {
          if (!this.m_ignoreModulePrefix && !this.m_cssModulePrefixes.Contains(strArray[0]))
            return (string) null;
          cssPluginFile = strArray[1];
        }
      }
      return cssPluginFile;
    }
  }
}
