// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.IClientContributionProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [DefaultServiceImplementation(typeof (ClientContributionProviderService))]
  public interface IClientContributionProviderService : IVssFrameworkService
  {
    void AddContribution(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string contributionId);

    void AddContributions(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      IEnumerable<ContributionNode> contributions);

    void AddContributionsForType(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string contributionType);
  }
}
