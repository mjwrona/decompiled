// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.MyWork.MyWorkController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.MyWork, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8442996D-DF5E-4B6F-9622-CCF23EF07ED1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.MyWork.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.MyWork.ViewModels;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.MyWork
{
  [TfsHandleFeatureFlag("WebAccess.Home.MyWorkHub", null)]
  [SupportedRouteArea(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
  [OutputCache(CacheProfile = "NoCache")]
  public class MyWorkController : MyWorkAreaController
  {
    private const string CacheNameSpaceId = "FA047A2F-30DA-4354-AC1E-4EB6DB6E857E";
    private const string WidgetSettingsTraceLayer = "WidgetSettings";
    private const int MyWorkControllerStart = 10023000;

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult Index() => (ActionResult) this.View();

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult SetWidgetSettings([ModelBinder(typeof (JsonModelBinder))] WidgetSettingsModel widgetSettingsModel)
    {
      ArgumentUtility.CheckForNull<WidgetSettingsModel>(widgetSettingsModel, "widgetSettings");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(widgetSettingsModel.WidgetName, "widgetName");
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(widgetSettingsModel.Settings, "widgetSettings");
      WidgetSettingsViewModel data = new WidgetSettingsViewModel()
      {
        Success = false,
        Settings = (Dictionary<string, string>) null
      };
      try
      {
        IRedisCacheService service = this.TfsRequestContext.GetService<IRedisCacheService>();
        string settingsKey = this.GetSettingsKey(widgetSettingsModel.WidgetName, this.TfsRequestContext);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        if (service.IsEnabled(tfsRequestContext))
        {
          this.GetRedisContainer(this.TfsRequestContext).Set(this.TfsRequestContext, (IDictionary<string, Dictionary<string, string>>) new Dictionary<string, Dictionary<string, string>>()
          {
            {
              settingsKey,
              widgetSettingsModel.Settings
            }
          });
          data.Success = true;
          data.Settings = widgetSettingsModel.Settings;
        }
        else
        {
          this.TfsRequestContext.Trace(10023000, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "RedisCacheService Disabled: couldn't set cache value for key: {0}", (object) settingsKey);
          data.Success = false;
        }
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.Trace(10023001, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "Error saving settings in cache {0}.", (object) ex);
        data.Success = false;
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetWidgetSettings(string widgetName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(widgetName, nameof (widgetName));
      WidgetSettingsViewModel data = new WidgetSettingsViewModel()
      {
        Success = false,
        Settings = (Dictionary<string, string>) null
      };
      try
      {
        IRedisCacheService service = this.TfsRequestContext.GetService<IRedisCacheService>();
        string settingsKey = this.GetSettingsKey(widgetName, this.TfsRequestContext);
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        if (service.IsEnabled(tfsRequestContext))
        {
          Dictionary<string, string> dictionary;
          if (this.GetRedisContainer(this.TfsRequestContext).TryGet<string, Dictionary<string, string>>(this.TfsRequestContext, settingsKey, out dictionary))
          {
            this.TfsRequestContext.Trace(10023002, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "Cache hit for the key: {0}", (object) settingsKey);
            data.Success = true;
            data.Settings = dictionary;
          }
          else
          {
            this.TfsRequestContext.Trace(10023003, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "Cache miss for the key: {0}", (object) settingsKey);
            data.Success = true;
          }
        }
        else
          this.TfsRequestContext.Trace(10023004, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "RedisCacheService Disabled: couldn't get cache value for key: {0}", (object) settingsKey);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.Trace(10023005, TraceLevel.Verbose, this.TraceArea, "WidgetSettings", "Error getting from cache {0}", (object) ex);
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    private string GetSettingsKey(string widgetName, IVssRequestContext requestContext) => string.Format("{0}_{1}", (object) widgetName, (object) requestContext.UserContext.Identifier);

    private IMutableDictionaryCacheContainer<string, Dictionary<string, string>> GetRedisContainer(
      IVssRequestContext requestContext)
    {
      ContainerSettings settings = new ContainerSettings()
      {
        KeyExpiry = new TimeSpan?(TimeSpan.MaxValue - TimeSpan.FromSeconds(1.0)),
        CiAreaName = this.AreaName
      };
      return requestContext.GetService<IRedisCacheService>().GetVolatileDictionaryContainer<string, Dictionary<string, string>, MyWorkController.MyWorkCacheSecurityToken>(requestContext, new Guid("FA047A2F-30DA-4354-AC1E-4EB6DB6E857E"), settings);
    }

    internal sealed class MyWorkCacheSecurityToken
    {
    }
  }
}
