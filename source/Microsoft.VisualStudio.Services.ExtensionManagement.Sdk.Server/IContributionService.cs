// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.IContributionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [DefaultServiceImplementation(typeof (ContributionService))]
  public interface IContributionService : IVssFrameworkService
  {
    Task<Stream> QueryAsset(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType);

    string QueryAssetLocation(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType,
      string prefferedLocation = "Local");

    Dictionary<string, string> QueryAssetLocations(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType);

    Contribution QueryContribution(IVssRequestContext requestContext, string contributionId);

    IEnumerable<Contribution> QueryContributionsForChild(
      IVssRequestContext requestContext,
      string contributionId);

    IEnumerable<Contribution> QueryContributionsForTarget(
      IVssRequestContext requestContext,
      string contributionId);

    ContributionProviderDetails QueryContributionProviderDetails(
      IVssRequestContext requestContext,
      string contributionId);

    IEnumerable<Contribution> QueryContributionsForType(
      IVssRequestContext requestContext,
      string contributionType);

    IEnumerable<Contribution> QueryContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds = null,
      HashSet<string> contributionTypes = null,
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeSelf,
      ContributionQueryCallback queryCallback = null,
      ContributionDiagnostics diagnostics = null);

    IDictionary<string, ContributionNode> QueryContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics = null);
  }
}
