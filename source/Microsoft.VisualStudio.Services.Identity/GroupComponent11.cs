// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent11
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
  internal class GroupComponent11 : GroupComponent10
  {
    public override IList<Guid> GetIdentityIdsVisibleInScope(Guid scopeId)
    {
      try
      {
        this.TraceEnter(4704550, nameof (GetIdentityIdsVisibleInScope));
        this.PrepareStoredProcedure("prc_GetIdentityIdsVisibleInScope");
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4704559, nameof (GetIdentityIdsVisibleInScope));
      }
    }
  }
}
