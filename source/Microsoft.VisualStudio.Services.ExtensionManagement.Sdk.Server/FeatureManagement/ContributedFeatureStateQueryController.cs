// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ContributedFeatureStateQueryController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  [VersionedApiControllerCustomName(Area = "FeatureManagement", ResourceName = "FeatureStatesQuery")]
  public class ContributedFeatureStateQueryController : TfsApiController
  {
    public override string TraceArea => "FeatureManagement";

    public override string ActivityLogArea => "Extensions";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ContributedFeatureNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ContributedFeatureInvalidScopeException>(HttpStatusCode.BadRequest);
    }

    [HttpPost]
    [ClientLocationId("2B4486AD-122B-400C-AE65-17B6672C1F9D")]
    public ContributedFeatureStateQuery QueryFeatureStates(ContributedFeatureStateQuery query)
    {
      ArgumentUtility.CheckForNull<ContributedFeatureStateQuery>(query, nameof (query));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.FeatureIds, "query.FeatureIds");
      IContributedFeatureService service = this.TfsRequestContext.GetService<IContributedFeatureService>();
      query.FeatureStates = service.GetFeatureStates(this.TfsRequestContext, (IEnumerable<string>) query.FeatureIds, query.ScopeValues);
      return query;
    }

    [HttpPost]
    [ClientLocationId("3F810F28-03E2-4239-B0BC-788ADD3005E5")]
    public ContributedFeatureStateQuery QueryFeatureStatesForDefaultScope(
      string userScope,
      ContributedFeatureStateQuery query)
    {
      return this.QueryFeatureStatesForNamedScope(userScope, (string) null, (string) null, query);
    }

    [HttpPost]
    [ClientLocationId("F29E997B-C2DA-4D15-8380-765788A1A74C")]
    public ContributedFeatureStateQuery QueryFeatureStatesForNamedScope(
      string userScope,
      string scopeName,
      string scopeValue,
      ContributedFeatureStateQuery query)
    {
      ArgumentUtility.CheckForNull<ContributedFeatureStateQuery>(query, nameof (query));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.FeatureIds, "query.FeatureIds");
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, "query.UserScope");
      IContributedFeatureService service = this.TfsRequestContext.GetService<IContributedFeatureService>();
      query.FeatureStates = service.GetEffectiveFeatureStates(this.TfsRequestContext, (IEnumerable<string>) query.FeatureIds, SettingsUserScope.Parse(userScope), scopeName, scopeValue, query.ScopeValues);
      return query;
    }
  }
}
