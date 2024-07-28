// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContributionNodeExtensions
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public static class ContributionNodeExtensions
  {
    public static IEnumerable<ContributionNode> GetChildrenOfType(
      this ContributionNode contribution,
      string contributionType)
    {
      return contribution != null && contribution.Children != null ? contribution.Children.Where<ContributionNode>((Func<ContributionNode, bool>) (c => c.Contribution.IsOfType(contributionType))) : Enumerable.Empty<ContributionNode>();
    }
  }
}
