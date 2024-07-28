// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent7
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent7 : IdentityManagementComponent6
  {
    public override ResultCollection GetChanges(
      int sequenceId,
      ref int lastSequenceId,
      long firstAuditSequenceId,
      bool useIdentityAudit,
      IEnumerable<Guid> scopedIdentityIds = null,
      bool includeDescriptorChanges = false)
    {
      try
      {
        this.TraceEnter(4701500, nameof (GetChanges));
        this.PrepareStoredProcedure("prc_GetIdentityChangesById");
        this.BindInt("@sequenceId", sequenceId);
        if (lastSequenceId > 0)
          this.BindInt("@lastSequenceId", lastSequenceId);
        if (scopedIdentityIds == null)
          scopedIdentityIds = (IEnumerable<Guid>) new List<Guid>();
        this.BindGuidTable("@scopedIdentityIds", scopedIdentityIds);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        lastSequenceId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        ResultCollection changes = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        changes.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
        return changes;
      }
      finally
      {
        this.TraceLeave(4701509, nameof (GetChanges));
      }
    }
  }
}
