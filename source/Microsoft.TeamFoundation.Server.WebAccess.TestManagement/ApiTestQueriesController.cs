// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ApiTestQueriesController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [SupportedRouteArea("Api", NavigationContextLevels.Project | NavigationContextLevels.Team)]
  [DemandFeature("2DD84BB6-7821-4FDE-85BA-A6CC4AB1B7E9", true)]
  public class ApiTestQueriesController : TestManagementAreaController
  {
    private QueriesStore m_queriesStore;
    private QueryAdapterFactory m_queryAdapters;

    internal QueriesStore QueryStore
    {
      get
      {
        if (this.m_queriesStore == null)
          this.m_queriesStore = new QueriesStore(this.TestContext, this.QueryAdapters);
        return this.m_queriesStore;
      }
    }

    internal QueryAdapterFactory QueryAdapters
    {
      get
      {
        if (this.m_queryAdapters == null)
          this.m_queryAdapters = new QueryAdapterFactory(this.TestContext);
        return this.m_queryAdapters;
      }
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetQueryHierarchy(string[] itemTypes)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) itemTypes, nameof (itemTypes), "Test Results");
      DataContractJsonResult queryHierarchy = new DataContractJsonResult((object) this.QueryStore.GetQueryHierarchy(((IEnumerable<string>) itemTypes).Where<string>((Func<string, bool>) (itemType => !string.IsNullOrEmpty(itemType))), this.TestContext));
      queryHierarchy.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryHierarchy;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    [ValidateInput(false)]
    public ActionResult ExecuteQuery(string queryJson)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) queryJson, nameof (queryJson), "Test Results");
      QueryModel query = this.DeserializeQueryJson(this.TfsRequestContext, queryJson);
      SecureJsonResult secureJsonResult = new SecureJsonResult();
      secureJsonResult.Data = (object) this.QueryAdapters.GetAdapter(query.ItemType).ExecuteQuery(query).ToJson();
      secureJsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) secureJsonResult;
    }

    [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Put)]
    public ActionResult GetQueryData(
      string queryType,
      string[] itemIds,
      [ModelBinder(typeof (JsonModelBinder))] QueryDisplayColumn[] displayColumns)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      ArgumentUtility.CheckForNull<string[]>(itemIds, nameof (itemIds), "Test Results");
      ArgumentUtility.CheckForNull<QueryDisplayColumn[]>(displayColumns, nameof (displayColumns), "Test Results");
      SecureJsonResult queryData = new SecureJsonResult();
      queryData.Data = (object) this.QueryAdapters.GetAdapter(queryType).FetchDataRowsFromIds((IEnumerable<string>) itemIds, (IEnumerable<QueryDisplayColumn>) displayColumns).Select<QueryResultDataRowModel, JsObject>((Func<QueryResultDataRowModel, JsObject>) (row => row.ToJson()));
      queryData.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryData;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetQueryableFieldNames(string queryType)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      DataContractJsonResult queryableFieldNames = new DataContractJsonResult((object) this.QueryAdapters.GetAdapter(queryType).GetQueryableFields().Select<QueryField, string>((Func<QueryField, string>) (f => f.DisplayName)).OrderBy<string, string>((Func<string, string>) (n => n), (IComparer<string>) StringComparer.InvariantCultureIgnoreCase).ToList<string>());
      queryableFieldNames.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryableFieldNames;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public ActionResult GetQueryField(string queryType, string fieldName)
    {
      this.TfsRequestContext.ServiceName = "Test Results";
      ArgumentUtility.CheckForNull<string>(fieldName, nameof (fieldName), "Test Results");
      JsObject jsObject = new JsObject();
      if (!string.IsNullOrWhiteSpace(fieldName))
      {
        QueryField fieldByDisplayName = this.QueryAdapters.GetAdapter(queryType).TryGetFieldByDisplayName(fieldName);
        if (fieldByDisplayName != null)
          jsObject["field"] = (object) fieldByDisplayName.ToJson(true);
      }
      SecureJsonResult queryField = new SecureJsonResult();
      queryField.Data = (object) jsObject;
      queryField.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
      return (ActionResult) queryField;
    }
  }
}
