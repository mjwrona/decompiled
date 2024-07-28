// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent27
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent27 : IdentityManagementComponent26
  {
    public override Microsoft.VisualStudio.Services.Identity.Identity ReadIdentityByDomainAndOidWithLargestSequenceId(
      string domain,
      Guid externalId)
    {
      try
      {
        this.TraceEnter(47011112, nameof (ReadIdentityByDomainAndOidWithLargestSequenceId));
        this.PrepareStoredProcedure("prc_ReadIdentityByDomainAndOidWithLargestSequenceId");
        this.BindString("@domain", domain, 256, true, SqlDbType.NVarChar);
        this.BindGuid("@externalId", externalId);
        ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
        resultCollection.AddBinder<Microsoft.VisualStudio.Services.Identity.Identity>((ObjectBinder<Microsoft.VisualStudio.Services.Identity.Identity>) this.GetIdentityColumns());
        return resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      finally
      {
        this.TraceLeave(47011114, nameof (ReadIdentityByDomainAndOidWithLargestSequenceId));
      }
    }

    public override void UpdateIdentityId(Guid identityId, Guid newIdentityId)
    {
      try
      {
        this.TraceEnter(47011113, nameof (UpdateIdentityId));
        this.PrepareStoredProcedure("prc_UpdateIdentityId");
        this.BindGuid("@identityId", identityId);
        this.BindGuid("@newIdentityId", newIdentityId);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011115, nameof (UpdateIdentityId));
      }
    }
  }
}
