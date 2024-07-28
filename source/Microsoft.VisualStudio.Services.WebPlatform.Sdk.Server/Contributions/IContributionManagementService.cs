// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.IContributionManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DefaultServiceImplementation(typeof (ContributionManagementService))]
  public interface IContributionManagementService : IVssFrameworkService
  {
    void ComputeRequestContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback);

    T GetContributedObject<T>(IVssRequestContext requestContext, string contributionId);

    ContributionNode GetContribution(IVssRequestContext requestContext, string contributionId);

    IEnumerable<ContributionNode> GetContributions(IVssRequestContext requestContext);

    IEnumerable<ContributionNode> GetContributionsByType(
      IVssRequestContext requestContext,
      string contributionType);

    Dictionary<string, List<ContributionDiagnostics>> GetDiagnostics(
      IVssRequestContext requestContext);

    ContributedSite GetSite(IVssRequestContext requestContext);

    void ResetRequestData(IVssRequestContext requestContext);
  }
}
