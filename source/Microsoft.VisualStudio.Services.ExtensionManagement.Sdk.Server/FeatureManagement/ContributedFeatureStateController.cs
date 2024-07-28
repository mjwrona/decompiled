// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ContributedFeatureStateController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using Microsoft.VisualStudio.Services.Settings;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  [VersionedApiControllerCustomName(Area = "FeatureManagement", ResourceName = "FeatureStates")]
  public class ContributedFeatureStateController : TfsApiController
  {
    public override string TraceArea => "FeatureManagement";

    public override string ActivityLogArea => "Extensions";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ContributedFeatureNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<ContributedFeatureInvalidScopeException>(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    [ClientLocationId("98911314-3F9B-4EAF-80E8-83900D8E85D9")]
    public ContributedFeatureState GetFeatureState(string featureId, string userScope) => this.GetFeatureStateForScope(featureId, userScope, (string) null, (string) null);

    [HttpGet]
    [ClientLocationId("DD291E43-AA9F-4CEE-8465-A93C78E414A4")]
    public ContributedFeatureState GetFeatureStateForScope(
      string featureId,
      string userScope,
      string scopeName,
      string scopeValue)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureId, nameof (featureId));
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      return this.TfsRequestContext.GetService<IContributedFeatureService>().GetFeatureState(this.TfsRequestContext, featureId, SettingsUserScope.Parse(userScope), scopeName, scopeValue) ?? throw new ContributedFeatureNotFoundException(ExtMgmtResources.ContributedFeatureNotFoundMessage((object) featureId));
    }

    [HttpPatch]
    [ClientLocationId("98911314-3F9B-4EAF-80E8-83900D8E85D9")]
    public ContributedFeatureState SetFeatureState(
      ContributedFeatureState feature,
      string featureId,
      string userScope,
      string reason = null,
      string reasonCode = null)
    {
      return this.SetFeatureStateForScope(feature, featureId, userScope, (string) null, (string) null, reason, reasonCode);
    }

    [HttpPatch]
    [ClientLocationId("DD291E43-AA9F-4CEE-8465-A93C78E414A4")]
    public ContributedFeatureState SetFeatureStateForScope(
      ContributedFeatureState feature,
      string featureId,
      string userScope,
      string scopeName,
      string scopeValue,
      string reason = null,
      string reasonCode = null)
    {
      ArgumentUtility.CheckForNull<ContributedFeatureState>(feature, nameof (feature));
      ArgumentUtility.CheckStringForNullOrEmpty(featureId, nameof (featureId));
      ArgumentUtility.CheckStringForNullOrEmpty(userScope, nameof (userScope));
      SettingsUserScope userScope1 = SettingsUserScope.Parse(userScope);
      this.TfsRequestContext.GetService<IContributedFeatureService>().SetFeatureState(this.TfsRequestContext, featureId, feature.State, userScope1, scopeName, scopeValue);
      CustomerIntelligenceService service = this.TfsRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("FeatureId", featureId);
      intelligenceData.Add("State", (object) feature.State);
      intelligenceData.Add("UserScope", userScope1.ToString());
      intelligenceData.Add("SettingScope", scopeName ?? string.Empty);
      intelligenceData.Add("SettingScopeValue", scopeValue ?? string.Empty);
      intelligenceData.Add("Reason", reason ?? string.Empty);
      intelligenceData.Add("ReasonCode", reasonCode ?? string.Empty);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      string survey = CustomerIntelligenceArea.Survey;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(tfsRequestContext, survey, "FeatureManagement", properties);
      feature.FeatureId = featureId;
      feature.Scope = new ContributedFeatureSettingScope()
      {
        SettingScope = scopeName,
        UserScoped = userScope1.IsUserScoped
      };
      return feature;
    }
  }
}
