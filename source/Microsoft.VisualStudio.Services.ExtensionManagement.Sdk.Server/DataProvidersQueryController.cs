// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DataProvidersQueryController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "Contribution", ResourceName = "dataProvidersQuery")]
  public class DataProvidersQueryController : TfsApiController
  {
    private const string c_includePerformanceTimingsKey = "includePerformanceTimings";
    private const string c_performanceTimingsDataKey = "PerformanceTimings";
    private const string c_dataProviderScopeDataKey = "DataProviderQuery.Scope";

    [ApplyRequestLanguage]
    [HttpPost]
    [ClientLocationId("738368DB-35EE-4B85-9F94-77ED34AF2B0D")]
    [PublicDataProviderRequestRestrictions]
    public DataProviderResult QueryDataProviders(
      DataProviderQuery query,
      string scopeName = null,
      string scopeValue = null)
    {
      ArgumentUtility.CheckForNull<DataProviderQuery>(query, nameof (query), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.ContributionIds, "query.contributionIds", this.TfsRequestContext.ServiceName);
      IExtensionDataProviderService service = this.TfsRequestContext.GetService<IExtensionDataProviderService>();
      service.SetRequestDataProviderContext(this.TfsRequestContext, (IDictionary<string, object>) query.Context?.Properties);
      string b1;
      this.RequestContext.RouteData.Values.TryGetValue<string>(nameof (scopeName), out b1);
      if (!string.Equals(scopeName, b1, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Invalid scopeName specified.");
      string b2;
      this.RequestContext.RouteData.Values.TryGetValue<string>(nameof (scopeValue), out b2);
      if (!string.Equals(scopeValue, b2, StringComparison.OrdinalIgnoreCase))
        throw new ArgumentException("Invalid scopeValue specified.");
      IDataProviderScope scope = (IDataProviderScope) null;
      if (!string.IsNullOrEmpty(scopeName) && !string.IsNullOrEmpty(scopeValue))
      {
        scope = ExtensionDataProviderScopeExtensions.GetScope(this.TfsRequestContext, scopeName, scopeValue);
        this.TfsRequestContext.RootContext.Items["DataProviderQuery.Scope"] = (object) scope;
      }
      Dictionary<string, object> properties = query.Context?.Properties;
      bool userFriendlySerialization = true;
      VssJsonMediaTypeFormatter mediaTypeFormatter = new VssJsonMediaTypeFormatter(this.Request);
      if (mediaTypeFormatter.EnumsAsNumbers && mediaTypeFormatter.UseMsDateFormat)
        userFriendlySerialization = false;
      DataProviderResult dataProviderData = service.GetDataProviderData(this.TfsRequestContext, query, true, userFriendlySerialization, scope);
      object obj;
      if (properties != null && query.Context.Properties.TryGetValue("includePerformanceTimings", out obj) && obj is bool flag && flag)
        dataProviderData.Data["PerformanceTimings"] = (object) PerformanceTimer.GetAllTimings(this.TfsRequestContext, (ISecuredObject) dataProviderData);
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return dataProviderData;
    }

    public override string TraceArea => "DataProviderData";

    public override string ActivityLogArea => "Extensions";
  }
}
