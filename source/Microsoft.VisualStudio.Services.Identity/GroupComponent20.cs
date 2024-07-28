// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent20
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent20 : GroupComponent19
  {
    public override ResultCollection GetChanges(
      long sequenceId,
      string everyoneSid,
      Guid scopeId,
      out Guid everyoneId,
      out long lastSequenceId,
      bool includeIdentities = false,
      bool getScopedGroupChanges = false)
    {
      try
      {
        this.TraceEnter(4704500, nameof (GetChanges));
        if (getScopedGroupChanges)
        {
          this.PrepareStoredProcedure("prc_GetScopedGroupChanges");
          this.BindGuid("@scopeId", scopeId);
        }
        else
          this.PrepareStoredProcedure("prc_GetGroupChanges");
        this.BindLong("@sequenceId", sequenceId);
        this.BindString("@everyoneSid", everyoneSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@includeIdentities", includeIdentities);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          GroupComponent.GroupChangesHeaderColumns changesHeaderColumns = new GroupComponent.GroupChangesHeaderColumns();
          everyoneId = changesHeaderColumns.Id.GetGuid((IDataReader) reader);
          lastSequenceId = changesHeaderColumns.SequenceId.GetInt64((IDataReader) reader);
        }
        else
        {
          everyoneId = Guid.Empty;
          lastSequenceId = 0L;
        }
        reader.NextResult();
        ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        changes.AddBinder<MembershipChangeInfo>((ObjectBinder<MembershipChangeInfo>) this.GetGroupChangesColumnBinder());
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        if (includeIdentities)
          changes.AddBinder<GroupComponent.ReferencedIdentity>((ObjectBinder<GroupComponent.ReferencedIdentity>) new GroupComponent15.ReferencedIdentityColumns2());
        return changes;
      }
      finally
      {
        this.TraceLeave(4704509, nameof (GetChanges));
      }
    }

    internal override IList<Guid> GetIdentityIdsInScope(Guid scopeId)
    {
      try
      {
        this.TraceEnter(4704570, nameof (GetIdentityIdsInScope));
        this.PrepareStoredProcedure("prc_GetIdentityIdsInScope");
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4704579, nameof (GetIdentityIdsInScope));
      }
    }
  }
}
