// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controls.IContributionControl
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Server.WebAccess.Contributions.HtmlContent;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controls
{
  public interface IContributionControl
  {
    IHtmlString ToHtml(
      ContributionHtmlProviderContext htmlProviderContext,
      ContributionNode contributionNode,
      bool internalContentHost = true);
  }
}
