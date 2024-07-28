// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributionHierarchyQueryController
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [VersionedApiControllerCustomName(Area = "Contribution", ResourceName = "HierarchyQuery")]
  public class ContributionHierarchyQueryController : ContributionHierarchyControllerBase
  {
    [ApplyRequestLanguage]
    [HttpPost]
    [PublicDataProviderRequestRestrictions]
    [ClientLocationId("3353E165-A11E-43AA-9D88-14F2BB09B6D9")]
    public ContributionHierarchyControllerBase.ContributedHierarchy QueryContributedHierarchy(
      ContributionHierarchyControllerBase.ContributedHierarchyQuery query,
      string scopeName = null,
      string scopeValue = null)
    {
      ArgumentUtility.CheckForNull<ContributionHierarchyControllerBase.ContributedHierarchyQuery>(query, nameof (query), this.TfsRequestContext.ServiceName);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) query.ContributionIds, "query.contributionIds", this.TfsRequestContext.ServiceName);
      this.TfsRequestContext.TraceConditionally(10013591, TraceLevel.Info, this.TraceArea, WebPlatformTraceLayers.Controller, (Func<string>) (() => "'" + this.TfsRequestContext.Title() + "' for following contributions: '" + string.Join(",", (IEnumerable<string>) query.ContributionIds) + "'."));
      return this.GetContributedHierarchy((IEnumerable<string>) query.ContributionIds, query.DataProviderContext, scopeName, scopeValue);
    }
  }
}
