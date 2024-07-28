// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent26
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent26 : IdentityManagementComponent25
  {
    public override IList<Guid> ReadIdentityIdsWithNoDirectoryAliasByPage(
      Guid tenantId,
      Guid? pageIndex,
      int pageSize)
    {
      try
      {
        this.TraceEnter(47011101, nameof (ReadIdentityIdsWithNoDirectoryAliasByPage));
        this.PrepareStoredProcedure("prc_ReadPagedIdsForIdentitiesWithNoDirectoryAlias");
        this.BindGuid("@tenantId", tenantId);
        this.BindNullableGuid("@pageIndex", pageIndex);
        this.BindInt("@pageSize", pageSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(47011102, nameof (ReadIdentityIdsWithNoDirectoryAliasByPage));
      }
    }

    public override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesByDomainAndOid(
      string domain,
      Guid externalId)
    {
      try
      {
        this.TraceEnter(47011112, nameof (ReadIdentitiesByDomainAndOid));
        this.PrepareStoredProcedure("prc_ReadIdentitiesByDomainAndOid");
        this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
        this.BindGuid("@externalId", externalId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentityColumns());
        return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
      }
      finally
      {
        this.TraceLeave(47011114, nameof (ReadIdentitiesByDomainAndOid));
      }
    }

    public override IList<Guid> ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage(
      Guid? pageIndex,
      int pageSize,
      string identityType,
      Guid tenantId)
    {
      ArgumentUtility.CheckForEmptyGuid(tenantId, nameof (tenantId));
      try
      {
        this.TraceEnter(47011111, nameof (ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage));
        this.PrepareStoredProcedure("prc_ReadPagedIdsForIdentitiesWithNoStorageKeyCuidMapping_ByTypeIdAndSidPattern");
        this.BindNullableGuid("@pageIndex", pageIndex);
        this.BindInt("@pageSize", pageSize);
        this.BindByte("@typeId", IdentityTypeMapper.Instance.GetTypeIdFromName(identityType));
        this.BindString("@sidPattern", this.GetSidPattern(identityType, tenantId), 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(47011119, nameof (ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage));
      }
    }

    private string GetSidPattern(string identityType, Guid tenantId)
    {
      string sidPattern = string.Format("{0}\\{1}", (object) tenantId, (object) "%");
      switch (identityType)
      {
        case "Microsoft.IdentityModel.Claims.ClaimsIdentity":
          return sidPattern;
        case "Microsoft.TeamFoundation.BindPendingIdentity":
          return "upn:" + sidPattern;
        default:
          throw new ArgumentException("Cannot generate SID pattern for identity type '" + identityType + "'");
      }
    }

    public override IList<Guid> ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage(
      Guid? pageIndex,
      int pageSize,
      string identityType)
    {
      try
      {
        this.TraceEnter(47011121, nameof (ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage));
        this.PrepareStoredProcedure("prc_ReadPagedIdsForIdentitiesWithNoStorageKeyCuidMapping_ByTypeId");
        this.BindNullableGuid("@pageIndex", pageIndex);
        this.BindInt("@pageSize", pageSize);
        this.BindByte("@typeId", IdentityTypeMapper.Instance.GetTypeIdFromName(identityType));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(47011129, nameof (ReadIdsForIdentitiesWithNoStorageKeyCuidMapByPage));
      }
    }

    public override void InsertIdentityRepairChanges(
      IReadOnlyList<IdentityRepairChange> identityRepairChanges)
    {
      this.TraceEnter(47011141, nameof (InsertIdentityRepairChanges));
      try
      {
        this.PrepareStoredProcedure("prc_InsertIdentityRepairChanges");
        this.BindIdentityRepairChangeTable("@identityRepairChanges", (IEnumerable<IdentityRepairChange>) identityRepairChanges);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011149, nameof (InsertIdentityRepairChanges));
      }
    }
  }
}
