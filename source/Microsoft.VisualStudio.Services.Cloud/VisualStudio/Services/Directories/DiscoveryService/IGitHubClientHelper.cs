// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.IGitHubClientHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal interface IGitHubClientHelper
  {
    GitHubData.V3.User[] SearchUsers(
      IVssRequestContext requestContext,
      string searchQuery,
      int maxResults);

    GitHubData.V3.User GetUserByLogin(IVssRequestContext requestContext, string userLogin);

    GitHubData.V3.User GetUserById(IVssRequestContext requestContext, string userId);
  }
}
