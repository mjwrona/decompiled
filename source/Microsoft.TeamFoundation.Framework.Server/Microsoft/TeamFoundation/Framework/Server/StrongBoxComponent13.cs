// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent13 : StrongBoxComponent12
  {
    internal override IList<Guid> QueryStrongBoxSigningKeysExcludingKeyType(
      SigningKeyType signingKeyType)
    {
      this.PrepareStoredProcedure("prc_QueryStrongBoxSigningKeysExcludingKeyType");
      this.BindByte("@keyType", Convert.ToByte((object) signingKeyType));
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder keyIdColumn = new SqlColumnBinder("SigningKeyId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
        return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }
  }
}
