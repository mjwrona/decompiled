// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentitySearchResult
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TeamFoundationIdentitySearchResult
  {
    public TeamFoundationIdentity[] Identities;
    public string PagingContext;

    public TeamFoundationIdentitySearchResult(
      TeamFoundationIdentity[] identities,
      string pagingContext)
    {
      this.Identities = identities;
      this.PagingContext = pagingContext;
    }

    public TeamFoundationIdentitySearchResult(IdentitySearchResult searchResult)
      : this(IdentityUtil.Convert(searchResult.Identities), searchResult.PagingContext)
    {
    }
  }
}
