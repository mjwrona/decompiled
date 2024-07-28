// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningComponent7
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SigningComponent7 : SigningComponent6
  {
    public override List<SigningComponent.SigningServiceKey> GetPrivateKeysByIds(
      IEnumerable<Guid> identifiers)
    {
      this.PrepareStoredProcedure("prc_GetSigningKeysByIds");
      this.BindGuidTable("@signingKeyIds", identifiers);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SigningComponent.SigningServiceKey>((ObjectBinder<SigningComponent.SigningServiceKey>) new SigningKeyColumns());
        return resultCollection.GetCurrent<SigningComponent.SigningServiceKey>().Items;
      }
    }
  }
}
