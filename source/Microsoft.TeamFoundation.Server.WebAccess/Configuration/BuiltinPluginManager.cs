// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Configuration.BuiltinPluginManager
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Configuration
{
  public static class BuiltinPluginManager
  {
    private static List<WebAccessPluginModule> sm_plugins = new List<WebAccessPluginModule>();
    private static List<WebAccessPluginBase> sm_bases = new List<WebAccessPluginBase>();
    private static string sm_bootstrap = (string) null;

    internal static IEnumerable<WebAccessPluginModule> Plugins => (IEnumerable<WebAccessPluginModule>) BuiltinPluginManager.sm_plugins;

    public static void RegisterPlugin(string moduleNamespace, string loadAfter)
    {
      BuiltinPluginManager.sm_bootstrap = (string) null;
      BuiltinPluginManager.sm_plugins.Add(new WebAccessPluginModule(moduleNamespace, loadAfter, (Func<TfsWebContext, bool>) null));
      BundlingHelper.RegisterPlugin(moduleNamespace, loadAfter);
    }

    public static void RegisterPluginBase(string moduleNamespace, string path) => BuiltinPluginManager.sm_bases.Add(new WebAccessPluginBase()
    {
      @namespace = moduleNamespace,
      @base = path
    });

    internal static MvcHtmlString GetBootstrap(TfsWebContext tfsWebContext)
    {
      string pluginsScript = BuiltinPluginManager.GetPluginsScript(tfsWebContext);
      if (string.IsNullOrEmpty(pluginsScript))
        return MvcHtmlString.Empty;
      string str = pluginsScript;
      string disabledPluginsScript = BuiltinPluginManager.GetDisabledPluginsScript(tfsWebContext);
      if (!string.IsNullOrEmpty(disabledPluginsScript))
        str += disabledPluginsScript;
      TagBuilder tagBuilder = new TagBuilder("script");
      tagBuilder.MergeAttribute("type", "text/javascript");
      tagBuilder.AddNonceAttribute(tfsWebContext.TfsRequestContext, tfsWebContext.RequestContext.HttpContext);
      tagBuilder.InnerHtml = str;
      return tagBuilder.ToHtmlString();
    }

    public static List<WebAccessPluginModule> GetPlugins() => BuiltinPluginManager.sm_plugins;

    public static List<WebAccessPluginBase> GetBases() => BuiltinPluginManager.sm_bases;

    private static string GetPluginsScript(TfsWebContext tfsWebContext)
    {
      if (BuiltinPluginManager.sm_bootstrap == null)
      {
        if (BuiltinPluginManager.sm_plugins.Any<WebAccessPluginModule>() || BuiltinPluginManager.sm_bases.Any<WebAccessPluginBase>())
        {
          StringBuilder stringBuilder = new StringBuilder();
          stringBuilder.Append("var _builtinPlugins = ");
          int num = tfsWebContext.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? 1 : 0;
          if (num != 0)
            stringBuilder.Append(JsonConvert.SerializeObject((object) BuiltinPluginManager.sm_plugins));
          else
            stringBuilder.Append(new JavaScriptSerializer().Serialize((object) BuiltinPluginManager.sm_plugins));
          stringBuilder.Append(";");
          stringBuilder.Append("var _builtInBases = ");
          if (num != 0)
            stringBuilder.Append(JsonConvert.SerializeObject((object) BuiltinPluginManager.sm_bases));
          else
            stringBuilder.Append(new JavaScriptSerializer().Serialize((object) BuiltinPluginManager.sm_bases));
          stringBuilder.Append(";");
          BuiltinPluginManager.sm_bootstrap = stringBuilder.ToString();
        }
        else
          BuiltinPluginManager.sm_bootstrap = string.Empty;
      }
      return BuiltinPluginManager.sm_bootstrap;
    }

    private static string GetDisabledPluginsScript(TfsWebContext tfsWebContext)
    {
      IEnumerable<string> source = BuiltinPluginManager.sm_plugins.Where<WebAccessPluginModule>((Func<WebAccessPluginModule, bool>) (p => !p.IsEnabled(tfsWebContext))).Select<WebAccessPluginModule, string>((Func<WebAccessPluginModule, string>) (p => p.@namespace));
      if (!source.Any<string>())
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("var _disabledPlugins = ");
      if (tfsWebContext.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer"))
        stringBuilder.Append(JsonConvert.SerializeObject((object) source));
      else
        stringBuilder.Append(new JavaScriptSerializer().Serialize((object) source));
      stringBuilder.Append(";");
      return stringBuilder.ToString();
    }
  }
}
