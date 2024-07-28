// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent2
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent2 : GroupComponent
  {
    protected override SqlParameter BindVersionedGroupTable(
      string parameterName,
      IEnumerable<GroupDescription> descriptions)
    {
      return this.BindGroupTable2(parameterName, descriptions);
    }

    public override long CreateGroups(
      Guid scopeId,
      bool errorOnDuplicate,
      GroupDescription[] groups,
      bool addActiveScopeMembership = true)
    {
      try
      {
        this.TraceEnter(4703200, nameof (CreateGroups));
        this.PrepareStoredProcedure("prc_CreateGroups");
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@errorOnDuplicate", errorOnDuplicate);
        this.BindVersionedGroupTable("@groups", (IEnumerable<GroupDescription>) groups);
        this.BindGuid("@eventAuthor", this.Author);
        return (long) this.ReadIntSequenceIdResult(this.ExecuteReader());
      }
      finally
      {
        this.TraceLeave(4703209, nameof (CreateGroups));
      }
    }
  }
}
