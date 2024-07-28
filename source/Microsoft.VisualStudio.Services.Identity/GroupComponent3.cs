// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent3
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent3 : GroupComponent2
  {
    public override ResultCollection QueryGroups(
      IEnumerable<Guid> scopeIds,
      bool recurse,
      bool deleted)
    {
      try
      {
        this.TraceEnter(4703300, nameof (QueryGroups));
        this.PrepareStoredProcedure("prc_QueryGroups");
        this.BindGuidTable("@scopeIds", scopeIds);
        this.BindBoolean("@recurse", recurse);
        this.BindBoolean("@deleted", deleted);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) new GroupComponent.GroupIdentityColumns());
        return resultCollection;
      }
      finally
      {
        this.TraceLeave(4703309, nameof (QueryGroups));
      }
    }
  }
}
