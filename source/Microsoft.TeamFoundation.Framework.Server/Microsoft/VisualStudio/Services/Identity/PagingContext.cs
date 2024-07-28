// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.PagingContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class PagingContext
  {
    public int LastRecordIndex;
    public int PageSize;
    public string Query;
    public IdentitySearchType IdentitySearchType;
    public IdentitySearchKind SearchKind;
    public Guid ScopeId;

    public PagingContext(
      int index,
      int pageSize,
      string query,
      IdentitySearchType identitySearchType,
      IdentitySearchKind searchKind,
      Guid scopeId = default (Guid))
    {
      this.LastRecordIndex = index;
      this.PageSize = pageSize;
      this.Query = query;
      this.IdentitySearchType = identitySearchType;
      this.SearchKind = searchKind;
      this.ScopeId = scopeId;
    }

    public string ToBase64() => Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Serialize<PagingContext>()));

    public static PagingContext FromBase64(string base64String)
    {
      try
      {
        return JsonUtilities.Deserialize<PagingContext>(Encoding.UTF8.GetString(Convert.FromBase64String(base64String)));
      }
      catch (Exception ex)
      {
        throw new ArgumentException(string.Format("wrong paging context format: {0}", (object) base64String), ex);
      }
    }
  }
}
