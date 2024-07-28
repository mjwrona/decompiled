// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributionHierarchyController
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [VersionedApiControllerCustomName(Area = "Contribution", ResourceName = "Hierarchy")]
  public class ContributionHierarchyController : ContributionHierarchyControllerBase
  {
    [ApplyRequestLanguage]
    [PublicDataProviderRequestRestrictions]
    [ClientLocationId("8EC9F10C-AB9F-4618-8817-48F3125DDE6A")]
    public ContributionHierarchyControllerBase.ContributedHierarchy GetContributedHierarchy(
      string contributionId,
      string scopeName = null,
      string scopeValue = null)
    {
      return this.GetContributedHierarchy((IEnumerable<string>) new string[1]
      {
        contributionId
      }, (DataProviderContext) null, scopeName, scopeValue);
    }
  }
}
