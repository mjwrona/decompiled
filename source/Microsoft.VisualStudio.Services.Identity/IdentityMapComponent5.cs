// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapComponent5
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMapComponent5 : IdentityMapComponent4
  {
    internal List<IdentityIdMapping> QueryIdentityMappings(ICollection<Guid> localIds)
    {
      ArgumentUtility.CheckForNull<ICollection<Guid>>(localIds, nameof (localIds));
      this.PrepareStoredProcedure("prc_QueryIdentityMappings");
      this.BindGuidTable("@localIds", (IEnumerable<Guid>) localIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityIdMapping>((ObjectBinder<IdentityIdMapping>) new IdentityIdMappingColumns(true));
        List<IdentityIdMapping> items = resultCollection.GetCurrent<IdentityIdMapping>().Items;
        if (localIds.Count != items.Count)
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        return items;
      }
    }

    internal List<IdentityIdMapping> MapIdentities(ICollection<Guid> masterIds)
    {
      ArgumentUtility.CheckForNull<ICollection<Guid>>(masterIds, nameof (masterIds));
      this.PrepareStoredProcedure("prc_MapIdentities");
      this.BindGuidTable("@masterIds", (IEnumerable<Guid>) masterIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityIdMapping>((ObjectBinder<IdentityIdMapping>) new IdentityIdMappingColumns());
        List<IdentityIdMapping> items = resultCollection.GetCurrent<IdentityIdMapping>().Items;
        if (masterIds.Count != items.Count)
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        return items;
      }
    }
  }
}
