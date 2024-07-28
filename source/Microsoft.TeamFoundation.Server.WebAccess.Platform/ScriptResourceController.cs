// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ScriptResourceController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Resources;
using System.Text;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [SupportedRouteArea("Api", NavigationContextLevels.All)]
  public class ScriptResourceController : WebPlatformController
  {
    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "JsResourceStrings")]
    public ActionResult Module()
    {
      DateTime result;
      if (DateTime.TryParse(this.HttpContext.Request.Headers["If-Modified-Since"], (IFormatProvider) null, DateTimeStyles.AdjustToUniversal, out result) && ScriptRegistration.GetLastModified() < result)
        return (ActionResult) new HttpStatusCodeResult(HttpStatusCode.NotModified);
      ScriptResourceController.ParseResourceParametersResult resourceParameters = this.ParseResourceParameters();
      return this.GenerateResources(resourceParameters.ResourceName, resourceParameters.Locale, true);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "JsResourceStrings")]
    public ActionResult Script()
    {
      ScriptResourceController.ParseResourceParametersResult resourceParameters = this.ParseResourceParameters();
      return this.GenerateResources(resourceParameters.ResourceName, resourceParameters.Locale, false);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [OutputCache(CacheProfile = "JsResourceStrings")]
    public ActionResult Index(string resourceName, string __loc, bool? emitModule) => this.GenerateResources(resourceName, __loc, !emitModule.HasValue || emitModule.Value);

    private ScriptResourceController.ParseResourceParametersResult ParseResourceParameters()
    {
      ScriptResourceController.ParseResourceParametersResult resourceParameters = new ScriptResourceController.ParseResourceParametersResult();
      resourceParameters.ResourceName = this.RouteData.GetRouteValue<string>("parameters", (string) null);
      if (!string.IsNullOrEmpty(resourceParameters.ResourceName))
      {
        resourceParameters.ResourceName = resourceParameters.ResourceName.Trim('/');
        string[] strArray = resourceParameters.ResourceName.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 2)
        {
          resourceParameters.ResourceName = strArray[strArray.Length - 1];
          resourceParameters.Locale = strArray[strArray.Length - 2];
        }
        int num = resourceParameters.ResourceName.IndexOf(".Resources.", StringComparison.OrdinalIgnoreCase);
        if (num >= 0)
          resourceParameters.ResourceName = resourceParameters.ResourceName.Substring(num + ".Resources.".Length);
        if (resourceParameters.ResourceName.EndsWith(".js", StringComparison.OrdinalIgnoreCase))
          resourceParameters.ResourceName = resourceParameters.ResourceName.Substring(0, resourceParameters.ResourceName.Length - ".js".Length);
      }
      return resourceParameters;
    }

    private ActionResult GenerateResources(string resourceName, string locale, bool emitModule)
    {
      ResourceManager resourceManager = ScriptRegistration.GetResourceManager(resourceName);
      if (resourceManager == null)
      {
        this.Response.StatusCode = 404;
        return (ActionResult) new EmptyResult();
      }
      CultureInfo culture = (CultureInfo) null;
      if (!string.IsNullOrEmpty(locale))
      {
        if (!locale.Equals("auto", StringComparison.OrdinalIgnoreCase))
        {
          try
          {
            culture = new CultureInfo(locale);
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
          }
        }
      }
      return (ActionResult) this.JavaScript(ScriptResourceController.GenerateResourceScript(resourceManager, resourceName, culture, emitModule));
    }

    internal static string GenerateResourceScript(
      ResourceManager resourceManager,
      string resourceName,
      CultureInfo culture,
      bool emitModule,
      bool useResourceName = false)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      ResourceSet resourceSet1 = resourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, true);
      ResourceSet resourceSet2 = resourceManager.GetResourceSet(culture, true, true);
      IDictionaryEnumerator enumerator = resourceSet1.GetEnumerator();
      while (enumerator.MoveNext())
      {
        string str = enumerator.Key.ToString();
        object obj = resourceSet2.GetObject(str) ?? enumerator.Value;
        dictionary[str] = obj;
      }
      StringBuilder stringBuilder = new StringBuilder();
      if (!emitModule)
      {
        stringBuilder.AppendFormat("var {0} = ", (object) resourceName);
        stringBuilder.Append(new JavaScriptSerializer().Serialize((object) dictionary));
        stringBuilder.Append(";");
      }
      else
      {
        if (useResourceName)
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "define(\"{0}\",[\"require\",\"exports\"],function(require,exports){{", (object) resourceName);
        else
          stringBuilder.AppendLine("define([\"require\",\"exports\"],function(require,exports){");
        stringBuilder.Append("var e=exports;");
        JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
        foreach (KeyValuePair<string, object> keyValuePair in dictionary)
        {
          stringBuilder.Append("e.");
          stringBuilder.Append(keyValuePair.Key);
          stringBuilder.Append("=");
          stringBuilder.Append(scriptSerializer.Serialize(keyValuePair.Value));
          stringBuilder.Append(";");
        }
        stringBuilder.Append("});");
        stringBuilder.AppendLine();
      }
      return stringBuilder.ToString();
    }

    private class ParseResourceParametersResult
    {
      public string ResourceName { get; set; }

      public string Locale { get; set; }
    }
  }
}
