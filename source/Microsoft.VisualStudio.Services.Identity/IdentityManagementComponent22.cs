// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityManagementComponent22
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityManagementComponent22 : IdentityManagementComponent21
  {
    public override void InsertIdentities(IList<Microsoft.VisualStudio.Services.Identity.Identity> identities, bool ignoreDuplicates)
    {
      this.TraceEnter(47011191, nameof (InsertIdentities));
      try
      {
        this.PrepareStoredProcedure("prc_InsertIdentities");
        this.BindIdentityTableForUpdateOrInsert("@identities", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities, (HashSet<string>) null);
        this.BindBoolean("@ignoreDuplicates", ignoreDuplicates);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(47011192, nameof (InsertIdentities));
      }
    }

    internal override Guid GenerateIdentityId(Microsoft.VisualStudio.Services.Identity.Identity identity) => IdentityCuidHelper.ComputeCuid(this.RequestContext, (IReadOnlyVssIdentity) identity, NameBasedGuidVersion.Six, true);
  }
}
