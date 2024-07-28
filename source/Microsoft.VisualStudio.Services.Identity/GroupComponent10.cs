// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent10
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent10 : GroupComponent9
  {
    public override IList<Guid> GetAncestorScopeIds(Guid scopeId, bool includeInactiveScopes = false)
    {
      try
      {
        this.TraceEnter(4704540, nameof (GetAncestorScopeIds));
        this.PrepareStoredProcedure("prc_GetScopeAncestorIds");
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.ScopeIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4704549, nameof (GetAncestorScopeIds));
      }
    }
  }
}
