// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent34
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent34 : IdentityManagementComponent33
  {
    public override ResultCollection GetScopedIdentityChanges(
      long sequenceId,
      Guid scopeId,
      bool useIdentityAudit,
      bool includeDescriptorChanges,
      out long lastSequenceId)
    {
      try
      {
        this.TraceEnter(4704500, nameof (GetScopedIdentityChanges));
        this.PrepareStoredProcedure("prc_GetScopedIdentityChanges");
        this.BindLong("@identitySequenceId", sequenceId);
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@useIdentityAudit", useIdentityAudit);
        this.BindBoolean("@includeDescriptorChanges", includeDescriptorChanges);
        SqlDataReader reader = this.ExecuteReader();
        lastSequenceId = reader.Read() ? reader.GetInt64(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        ResultCollection scopedIdentityChanges = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        scopedIdentityChanges.AddBinder<IdentityManagementComponent.ReferencedIdentity>((ObjectBinder<IdentityManagementComponent.ReferencedIdentity>) new IdentityManagementComponent.ReferencedIdentityColumns2());
        return scopedIdentityChanges;
      }
      finally
      {
        this.TraceLeave(4704509, nameof (GetScopedIdentityChanges));
      }
    }
  }
}
