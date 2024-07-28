// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers.ApiDiagnosticsController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.Controllers
{
  [SupportedRouteArea("Api", NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiDiagnosticsController : DiagnosticsAreaController
  {
    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetHosts(string accountName)
    {
      List<JsObject> data = new List<JsObject>();
      this.TfsRequestContext.Elevate().GetService<TeamFoundationHostManagementService>();
      if (this.TfsWebContext.IsHosted)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(accountName, nameof (accountName));
        using (IDisposableReadOnlyList<ITeamFoundationApplicationHostProperties> extensions = this.TfsRequestContext.GetExtensions<ITeamFoundationApplicationHostProperties>())
        {
          foreach (ITeamFoundationApplicationHostProperties applicationHostProperties in (IEnumerable<ITeamFoundationApplicationHostProperties>) extensions)
          {
            foreach (TeamProjectCollectionProperties collection in applicationHostProperties.GetCollections(this.TfsRequestContext, accountName))
              data.Add(collection.ToJson());
          }
        }
      }
      else
      {
        foreach (TfsServiceHostDescriptor allCollection in this.TfsWebContext.GetAllCollections())
          data.Add(allCollection.ToJson());
      }
      data.Add(this.TfsRequestContext.To(TeamFoundationHostType.Deployment).ToServiceHostJson());
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult Export(Guid instanceId, string userName) => (ActionResult) new DiagnosticExportResult(this.TfsRequestContext, instanceId, userName);

    [HttpGet]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    public ActionResult GetActivityLog(
      Guid instanceId,
      string userName,
      IEnumerable<string> sortFields)
    {
      TeamFoundationHostManagementService service1 = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>();
      List<KeyValuePair<ActivityLogColumns, SortOrder>> sortColumns = this.ParseSortColumns(sortFields);
      IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
      Guid instanceId1 = instanceId;
      using (IVssRequestContext vssRequestContext = service1.BeginRequest(requestContext, instanceId1, RequestContextType.UserContext, true, true))
      {
        TeamFoundationActivityLogService service2 = vssRequestContext.GetService<TeamFoundationActivityLogService>();
        IEnumerable<int> source1 = service2.QueryActivityLogIds(vssRequestContext, userName, 1000000, sortColumns);
        IEnumerable<ActivityLogEntry> source2 = service2.QueryActivityLogEntries(vssRequestContext, source1.Take<int>(200).ToArray<int>());
        ActivityLogQueryResultModel queryResultModel = new ActivityLogQueryResultModel((IEnumerable<int>) source1.ToArray<int>(), (IEnumerable<ActivityLogEntry>) source2.ToArray<ActivityLogEntry>());
        SecureJsonResult activityLog = new SecureJsonResult();
        activityLog.Data = (object) queryResultModel.ToJson();
        activityLog.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
        activityLog.MaxJsonLength = new int?(62914560);
        return (ActionResult) activityLog;
      }
    }

    [HttpGet]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult GetActivityLogEntry(Guid instanceId, int commandId)
    {
      using (IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>().BeginRequest(this.TfsRequestContext.Elevate(), instanceId, RequestContextType.UserContext, true, true))
        return (ActionResult) this.Json((object) vssRequestContext.GetService<TeamFoundationActivityLogService>().GetActivitylogEntry(vssRequestContext, commandId).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment | NavigationContextLevels.Application)]
    public ActionResult PageActivityLogEntries(Guid instanceId, string entryIds)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(entryIds, nameof (entryIds));
      int[] array = ((IEnumerable<string>) entryIds.Split(new char[1]
      {
        ','
      }, StringSplitOptions.None)).Select<string, int>((Func<string, int>) (idString =>
      {
        int result;
        if (int.TryParse(idString, out result))
          return result;
        throw new InvalidArgumentValueException(nameof (entryIds));
      })).ToArray<int>();
      using (IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate().To(TeamFoundationHostType.Deployment).GetService<TeamFoundationHostManagementService>().BeginRequest(this.TfsRequestContext.Elevate(), instanceId, RequestContextType.UserContext, true, true))
        return (ActionResult) this.Json((object) vssRequestContext.GetService<TeamFoundationActivityLogService>().QueryActivityLogEntries(vssRequestContext, array).Select<ActivityLogEntry, JsObject>((Func<ActivityLogEntry, JsObject>) (entry => entry.ToJson())), JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult SaveTrace(
      Guid traceId,
      Guid? traceServiceHost,
      int? tracePoint,
      string processName,
      string userLogin,
      string service,
      string method,
      string area,
      int? level,
      string userAgent,
      string layer,
      string uri,
      string path,
      string userDefined)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationTracingService service1 = vssRequestContext.GetService<TeamFoundationTracingService>();
      service1.StopTrace(vssRequestContext, traceId);
      Microsoft.VisualStudio.Services.WebApi.TraceFilter traceFilter1 = new Microsoft.VisualStudio.Services.WebApi.TraceFilter();
      traceFilter1.TraceId = traceId;
      traceFilter1.IsEnabled = true;
      traceFilter1.ServiceHost = traceServiceHost.HasValue ? traceServiceHost.Value : Guid.Empty;
      traceFilter1.Tracepoint = tracePoint.HasValue ? tracePoint.Value : 0;
      traceFilter1.ProcessName = processName;
      traceFilter1.UserLogin = userLogin;
      traceFilter1.Service = service;
      traceFilter1.Method = method;
      traceFilter1.Area = area;
      traceFilter1.Level = level.HasValue ? (TraceLevel) level.Value : TraceLevel.Off;
      traceFilter1.UserAgent = userAgent;
      traceFilter1.Layer = layer;
      traceFilter1.Uri = uri;
      traceFilter1.Path = path;
      Microsoft.VisualStudio.Services.WebApi.TraceFilter traceFilter2 = traceFilter1;
      string[] strArray;
      if (userDefined == null)
        strArray = (string[]) null;
      else
        strArray = userDefined.Split(':');
      traceFilter2.Tags = strArray;
      Microsoft.VisualStudio.Services.WebApi.TraceFilter traceFilter3 = traceFilter1;
      service1.StartTrace(vssRequestContext, traceFilter3);
      return (ActionResult) this.Json((object) traceFilter3.ToJson());
    }

    [AcceptVerbs(HttpVerbs.Post)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult DeleteTrace(Guid traceId)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<TeamFoundationTracingService>().StopTrace(vssRequestContext, traceId);
      return (ActionResult) this.Json((object) new
      {
        success = true
      });
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult QueryForTraces()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationTracingService service = context.GetService<TeamFoundationTracingService>();
      List<JsObject> jsObjectList = new List<JsObject>();
      IVssRequestContext requestContext = context;
      Guid? ownerId = new Guid?();
      foreach (Microsoft.VisualStudio.Services.WebApi.TraceFilter queryTrace in service.QueryTraces(requestContext, ownerId))
        jsObjectList.Add(queryTrace.ToJson());
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) jsObjectList;
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      secureJsonResult.MaxJsonLength = new int?(62914560);
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    [AcceptNavigationLevels(NavigationContextLevels.Deployment)]
    [Microsoft.TeamFoundation.Server.WebAccess.RequireDeploymentAdmin(true)]
    public ActionResult QueryTraceData(Guid? traceId, DateTime? since, int pageSize)
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      TeamFoundationTraceReadingService service = context.GetService<TeamFoundationTraceReadingService>();
      List<JsObject> jsObjectList = new List<JsObject>();
      IVssRequestContext requestContext = context;
      Guid traceId1 = traceId.HasValue ? traceId.Value : Guid.Empty;
      DateTime since1 = since.Value;
      int pageSize1 = pageSize;
      foreach (TraceEvent traceEvent in service.QueryTraceData(requestContext, traceId1, since1, pageSize1))
        jsObjectList.Add(traceEvent.ToJson());
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) jsObjectList;
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      secureJsonResult.MaxJsonLength = new int?(62914560);
      return (ActionResult) secureJsonResult;
    }

    private List<KeyValuePair<ActivityLogColumns, SortOrder>> ParseSortColumns(
      IEnumerable<string> fields)
    {
      List<KeyValuePair<ActivityLogColumns, SortOrder>> sortColumns = new List<KeyValuePair<ActivityLogColumns, SortOrder>>();
      bool flag = false;
      if (fields != null)
      {
        foreach (string field in fields)
        {
          if (!string.IsNullOrWhiteSpace(field))
          {
            string[] strArray = field.Split(';');
            ActivityLogColumns key = (ActivityLogColumns) int.Parse(strArray[0].Trim(), (IFormatProvider) NumberFormatInfo.InvariantInfo);
            if (!flag)
              flag = key == ActivityLogColumns.CommandId;
            if (strArray.Length > 1 && !string.IsNullOrWhiteSpace(strArray[1]))
            {
              string str = strArray[1].Trim();
              sortColumns.Add(new KeyValuePair<ActivityLogColumns, SortOrder>(key, str.Equals("asc", StringComparison.InvariantCultureIgnoreCase) ? SortOrder.Ascending : SortOrder.Descending));
            }
          }
        }
      }
      if (!flag)
        sortColumns.Add(new KeyValuePair<ActivityLogColumns, SortOrder>(ActivityLogColumns.CommandId, SortOrder.Descending));
      return sortColumns;
    }
  }
}
