// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningComponent5
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SigningComponent5 : SigningComponent4
  {
    public virtual List<SigningComponent.SigningServiceKey> GetPrivateKeys()
    {
      this.PrepareStoredProcedure("prc_GetSigningKeys");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SigningComponent.SigningServiceKey>((ObjectBinder<SigningComponent.SigningServiceKey>) new SigningKeyColumns());
        return resultCollection.GetCurrent<SigningComponent.SigningServiceKey>().Items;
      }
    }

    public override void RemoveKeys(IList<Guid> identifiers)
    {
      this.PrepareStoredProcedure("prc_DeleteSigningKeys");
      this.BindGuidTable("@signingKeyIds", (IEnumerable<Guid>) identifiers);
      this.ExecuteNonQuery();
    }
  }
}
