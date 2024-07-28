// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ContributedFeatureController
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  [VersionedApiControllerCustomName(Area = "FeatureManagement", ResourceName = "Features")]
  public class ContributedFeatureController : TfsApiController
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
    public List<ContributedFeature> GetFeatures(string targetContributionId = null)
    {
      IContributedFeatureService service = this.TfsRequestContext.GetService<IContributedFeatureService>();
      return (!string.IsNullOrEmpty(targetContributionId) ? (IEnumerable<ContributedFeature>) service.GetFeaturesForTarget(this.TfsRequestContext, targetContributionId).ToList<ContributedFeature>() : service.GetFeatures(this.TfsRequestContext)).ToList<ContributedFeature>();
    }

    [HttpGet]
    public ContributedFeature GetFeature(string featureId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(featureId, nameof (featureId));
      return this.TfsRequestContext.GetService<IContributedFeatureService>().GetFeature(this.TfsRequestContext, featureId) ?? throw new ContributedFeatureNotFoundException(ExtMgmtResources.ContributedFeatureNotFoundMessage((object) featureId));
    }
  }
}
