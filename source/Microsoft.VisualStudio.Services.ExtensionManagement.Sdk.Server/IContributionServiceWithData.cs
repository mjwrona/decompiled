// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.IContributionServiceWithData
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  [DefaultServiceImplementation(typeof (ContributionService))]
  public interface IContributionServiceWithData : IVssFrameworkService, IContributionService
  {
    bool QueryContribution<T>(
      IVssRequestContext requestContext,
      string contributionId,
      string associatedDataName,
      out Contribution contribution,
      out T associatedData);

    bool QueryContributionsForTarget<T>(
      IVssRequestContext requestContext,
      string contributionId,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData);

    bool QueryContributionsForType<T>(
      IVssRequestContext requestContext,
      string contributionType,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData);

    bool QueryContributions<T>(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      HashSet<string> contributionTypes,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData);

    bool QueryContributions<T>(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics,
      string associatedDataName,
      out IDictionary<string, ContributionNode> contributionNodes,
      out T associatedData);

    void Set(
      IVssRequestContext requestContext,
      string associatedDataName,
      IEnumerable<Contribution> contributions,
      object associatedData);

    void Set(
      IVssRequestContext requestContext,
      string associatedDataName,
      IEnumerable<ContributionNode> contributionNodes,
      object associatedData);
  }
}
