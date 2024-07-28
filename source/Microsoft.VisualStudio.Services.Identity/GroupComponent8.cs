// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.GroupComponent8
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class GroupComponent8 : GroupComponent7
  {
    public override void RetrieveFirstAuditSequenceIds(out long firstIdentityAuditSequenceId)
    {
      try
      {
        this.TraceEnter(4703210, nameof (RetrieveFirstAuditSequenceIds));
        this.PrepareStoredProcedure("prc_GetFirstAuditSequenceIds");
        SqlDataReader reader = this.ExecuteReader();
        firstIdentityAuditSequenceId = reader.Read() ? new SqlColumnBinder("FirstIdentityAuditSequenceId").GetInt64((IDataReader) reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
      }
      finally
      {
        this.TraceLeave(4703219, nameof (RetrieveFirstAuditSequenceIds));
      }
    }

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
        this.BindBoolean("@useGroupAudit", this.RequestContext.ExecutionEnvironment.IsHostedDeployment);
        this.BindLong("@firstAuditSequenceId", 1L);
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
  }
}
