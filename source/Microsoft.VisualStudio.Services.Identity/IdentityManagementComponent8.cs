// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent8
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent8 : IdentityManagementComponent7
  {
    public override int InvalidateIdentities(bool updateIdentityAudit, IList<Guid> identityIds = null)
    {
      try
      {
        this.TraceEnter(4701600, nameof (InvalidateIdentities));
        if (identityIds == null || identityIds.Count == 0)
          return -1;
        this.PrepareStoredProcedure("prc_InvalidateIdentities");
        this.BindGuid("@eventAuthor", this.Author);
        this.BindGuidTable("@ids", (IEnumerable<Guid>) identityIds);
        SqlDataReader sqlDataReader = this.ExecuteReader();
        return sqlDataReader.Read() ? sqlDataReader.GetInt32(0) : -1;
      }
      finally
      {
        this.TraceLeave(4701609, nameof (InvalidateIdentities));
      }
    }
  }
}
