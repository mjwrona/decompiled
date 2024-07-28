// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent5
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent5 : GroupComponent4
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
        this.TraceEnter(4703500, nameof (GetChanges));
        this.PrepareStoredProcedure("prc_GetGroupChanges");
        this.BindInt("@sequenceId", GroupComponent.ReduceMaxLongToMaxInt(sequenceId));
        this.BindString("@everyoneSid", everyoneSid, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindBoolean("@includeIdentities", includeIdentities);
        SqlDataReader reader = this.ExecuteReader();
        if (reader.Read())
        {
          GroupComponent.GroupChangesHeaderColumns changesHeaderColumns = new GroupComponent.GroupChangesHeaderColumns();
          everyoneId = changesHeaderColumns.Id.GetGuid((IDataReader) reader);
          lastSequenceId = (long) changesHeaderColumns.SequenceId.GetInt32((IDataReader) reader);
        }
        else
        {
          everyoneId = Guid.Empty;
          lastSequenceId = 0L;
        }
        reader.NextResult();
        ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        changes.AddBinder<MembershipChangeInfo>((ObjectBinder<MembershipChangeInfo>) new GroupComponent.GroupChangesColumns());
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new GroupComponent.IdentityIdColumns());
        if (includeIdentities)
          changes.AddBinder<GroupComponent.ReferencedIdentity>((ObjectBinder<GroupComponent.ReferencedIdentity>) new GroupComponent5.ReferencedIdentityColumns());
        return changes;
      }
      finally
      {
        this.TraceLeave(4703509, nameof (GetChanges));
      }
    }

    protected class ReferencedIdentityColumns : ObjectBinder<GroupComponent.ReferencedIdentity>
    {
      private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));

      protected override GroupComponent.ReferencedIdentity Bind() => new GroupComponent.ReferencedIdentity()
      {
        IdentityId = this.Id.GetGuid((IDataReader) this.Reader),
        Location = GroupComponent.ReferencedIdentityLocation.Unknown
      };
    }
  }
}
