// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SigningComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SigningComponent6 : SigningComponent5
  {
    public override SigningComponent.SigningServiceKey SetPrivateKey(
      Guid identifier,
      SigningKeyType keyType,
      byte[] storedKeyData,
      bool overwriteExisting)
    {
      this.PrepareStoredProcedure("prc_SetSigningKey");
      this.BindGuid("@id", identifier);
      this.BindBinary("@privateKey", storedKeyData, SqlDbType.VarBinary);
      this.BindByte("@keyType", (byte) keyType);
      this.BindBoolean("@overwriteExisting", overwriteExisting);
      this.BindGuid("@authorId", this.Author);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<SigningComponent.SigningServiceKey>((ObjectBinder<SigningComponent.SigningServiceKey>) new SigningKeyColumns());
        return resultCollection.GetCurrent<SigningComponent.SigningServiceKey>().Items.Single<SigningComponent.SigningServiceKey>();
      }
    }
  }
}
