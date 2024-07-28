// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent12
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
  internal class IdentityManagementComponent12 : IdentityManagementComponent11
  {
    public override IList<Guid> FetchIdentityIdsBatch(
      int batchSize,
      int maxSequenceId = -1,
      bool incudeOnlyClaimsAndBindPendingTypes = false)
    {
      try
      {
        this.TraceEnter(4701900, nameof (FetchIdentityIdsBatch));
        this.PrepareStoredProcedure("prc_FetchIdentityIdsBatch");
        this.BindInt("@batchSize", batchSize);
        this.BindInt("@maxSequenceId", maxSequenceId);
        this.BindBoolean("@incudeOnlyClaimsAndBindPendingTypes", incudeOnlyClaimsAndBindPendingTypes);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new IdentityManagementComponent.IdentityIdColumns());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().ToList<Guid>();
        }
      }
      finally
      {
        this.TraceLeave(4701909, nameof (FetchIdentityIdsBatch));
      }
    }

    public override ResultCollection ReadIdentities(
      IEnumerable<IdentityDescriptor> descriptors,
      IEnumerable<Guid> ids)
    {
      Guid? nullable1 = new Guid?();
      IdentityDescriptor identityDescriptor;
      if (descriptors != null && descriptors.Count<IdentityDescriptor>() == 1 && (identityDescriptor = descriptors.FirstOrDefault<IdentityDescriptor>()) != (IdentityDescriptor) null)
      {
        this.PrepareStoredProcedure("prc_ReadIdentityByIdentifier");
        this.BindString("@sid", identityDescriptor.Identifier, 256, true, SqlDbType.VarChar);
        this.BindString("@type", identityDescriptor.IdentityType, 64, true, SqlDbType.VarChar);
      }
      else
      {
        if (ids != null && ids.Count<Guid>() == 1)
        {
          nullable1 = new Guid?(ids.FirstOrDefault<Guid>());
          Guid? nullable2 = nullable1;
          if (nullable2.HasValue && nullable1.HasValue)
          {
            nullable2 = nullable1;
            Guid empty = Guid.Empty;
            if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            {
              this.PrepareStoredProcedure("prc_ReadIdentityByIdentifier");
              this.BindGuid("@id", nullable1.Value);
              goto label_7;
            }
          }
        }
        this.PrepareStoredProcedure("prc_ReadIdentities");
        this.BindOrderedDescriptorTable("@descriptors", descriptors, true);
        this.BindOrderedGuidTable("@ids", ids, true);
      }
label_7:
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<IdentityManagementComponent.IdentityData>((ObjectBinder<IdentityManagementComponent.IdentityData>) new IdentityManagementComponent10.IdentitiesColumns6());
      return resultCollection;
    }
  }
}
