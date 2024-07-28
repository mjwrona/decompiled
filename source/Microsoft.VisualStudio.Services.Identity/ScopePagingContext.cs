// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.ScopePagingContext
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class ScopePagingContext
  {
    public Guid ScopeId { get; private set; }

    public int PageSize { get; private set; }

    public bool IncludeGroups { get; private set; }

    public bool IncludeNonGroups { get; private set; }

    public Guid? PagenationToken { get; private set; }

    public ScopePagingContext(
      Guid scopeId,
      int pageSize,
      bool includeGroups,
      bool includeNonGroups)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      ArgumentUtility.CheckForOutOfRange(pageSize, nameof (PageSize), 1);
      if (!includeGroups && !includeNonGroups)
        throw new ArgumentException(HostingResources.MustSpecifyPagingResults());
      this.ScopeId = scopeId;
      this.PageSize = pageSize;
      this.IncludeGroups = includeGroups;
      this.IncludeNonGroups = includeNonGroups;
      this.PagenationToken = new Guid?();
    }

    [JsonConstructor]
    internal ScopePagingContext(
      Guid scopeId,
      int pageSize,
      bool includeGroups,
      bool includeNonGroups,
      Guid? pagenationToken)
    {
      ArgumentUtility.CheckForEmptyGuid(scopeId, nameof (scopeId));
      ArgumentUtility.CheckForOutOfRange(pageSize, nameof (PageSize), 1);
      if (!includeGroups && !includeNonGroups)
        throw new ArgumentException(HostingResources.MustSpecifyPagingResults());
      this.ScopeId = scopeId;
      this.PageSize = pageSize;
      this.IncludeGroups = includeGroups;
      this.IncludeNonGroups = includeNonGroups;
      this.PagenationToken = pagenationToken;
    }

    public static ScopePagingContext FromContinuationToken(string continuationToken)
    {
      try
      {
        return JsonUtilities.Deserialize<ScopePagingContext>(Encoding.UTF8.GetString(Convert.FromBase64String(continuationToken)));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(HostingResources.InvalidContinuationToken((object) continuationToken));
      }
    }

    internal string ToContinuationToken(Guid? pagenationToken) => !pagenationToken.HasValue ? (string) null : Convert.ToBase64String(Encoding.UTF8.GetBytes(new ScopePagingContext(this.ScopeId, this.PageSize, this.IncludeGroups, this.IncludeNonGroups, pagenationToken).Serialize<ScopePagingContext>()));
  }
}
