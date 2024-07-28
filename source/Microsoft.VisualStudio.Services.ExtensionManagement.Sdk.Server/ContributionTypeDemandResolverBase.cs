// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionTypeDemandResolverBase
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal abstract class ContributionTypeDemandResolverBase : DemandResolverBase
  {
    private const string c_contributionTypeDemandType = "contributionType";
    private static readonly string[] s_supportedDemandTypes = new string[1]
    {
      "contributionType"
    };

    public override IEnumerable<string> DemandTypes => (IEnumerable<string>) ContributionTypeDemandResolverBase.s_supportedDemandTypes;

    protected static bool TryParseContributionTypeIdentifier(
      string contributionTypeId,
      out ContributionIdentifier contributionTypeIdentifier)
    {
      try
      {
        contributionTypeIdentifier = new ContributionIdentifier(contributionTypeId);
        GalleryUtil.CheckPublisherName(contributionTypeIdentifier.PublisherName);
        GalleryUtil.CheckExtensionName(contributionTypeIdentifier.ExtensionName);
        ArgumentUtility.CheckStringForNullOrEmpty(contributionTypeIdentifier.RelativeId, "relativeId");
        return true;
      }
      catch (ArgumentException ex)
      {
        contributionTypeIdentifier = (ContributionIdentifier) null;
        return false;
      }
    }
  }
}
