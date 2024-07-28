// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.IContributionNavigationService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DefaultServiceImplementation(typeof (ContributionManagementService))]
  public interface IContributionNavigationService : IVssFrameworkService
  {
    List<ContributedNavigation> GetSelectedElements(IVssRequestContext requestContext);

    ContributedNavigation GetSelectedElementByType(
      IVssRequestContext requestContext,
      string contributionType);

    List<ContributedNavigation> GetSelectedElementsByType(
      IVssRequestContext requestContext,
      string contributionType);

    IEnumerable<Contribution> GetPrimaryContributions(IVssRequestContext requestContext);

    string GetPageTitle(IVssRequestContext requestContext);

    void SetPageTitle(IVssRequestContext requestContext, string title);

    bool SupportsMobile(IVssRequestContext requestContext);
  }
}
