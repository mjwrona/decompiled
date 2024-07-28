// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent29
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
  internal class IdentityManagementComponent29 : IdentityManagementComponent28
  {
    public override ResultCollection GetScopedPagedIdentityChanges(
      long sequenceId,
      Guid scopeId,
      bool useIdentityAudit,
      int pageSize,
      out long lastSequenceId)
    {
      try
      {
        this.TraceEnter(4704500, nameof (GetScopedPagedIdentityChanges));
        this.PrepareStoredProcedure("prc_GetScopedPagedIdentityChanges");
        this.BindLong("@identitySequenceId", sequenceId);
        this.BindGuid("@scopeId", scopeId);
        this.BindBoolean("@useIdentityAudit", useIdentityAudit);
        this.BindInt("@pageSize", pageSize);
        SqlDataReader reader = this.ExecuteReader();
        lastSequenceId = reader.Read() ? reader.GetInt64(0) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
        if (!reader.NextResult())
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        ResultCollection pagedIdentityChanges = new ResultCollection((IDataReader) reader, this.ProcedureName, this.RequestContext);
        pagedIdentityChanges.AddBinder<IdentityManagementComponent.ReferencedIdentity>((ObjectBinder<IdentityManagementComponent.ReferencedIdentity>) new IdentityManagementComponent.ReferencedIdentityColumns2());
        return pagedIdentityChanges;
      }
      finally
      {
        this.TraceLeave(4704509, nameof (GetScopedPagedIdentityChanges));
      }
    }

    public override List<IdentityManagementComponent.GdprClaimsIdentity> FetchGdprClaimsIdentities()
    {
      this.PrepareStoredProcedure("prc_FetchClaimsIdentities");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityManagementComponent.GdprClaimsIdentity>((ObjectBinder<IdentityManagementComponent.GdprClaimsIdentity>) new IdentityManagementComponent.GdprClaimsIdentityColumns());
        return resultCollection.GetCurrent<IdentityManagementComponent.GdprClaimsIdentity>().ToList<IdentityManagementComponent.GdprClaimsIdentity>();
      }
    }
  }
}
