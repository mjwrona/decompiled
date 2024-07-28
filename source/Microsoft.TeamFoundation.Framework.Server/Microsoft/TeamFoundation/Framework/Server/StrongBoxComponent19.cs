// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent19
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent19 : StrongBoxComponent18
  {
    public override List<StrongBoxItemInfo> ReadContentsWithPartialLookupKey(
      Guid drawerId,
      string partialLookupKey)
    {
      this.PrepareStoredProcedure("prc_ReadStrongBoxContentsWithPartialLookupKey");
      this.BindGuid("@drawerId", drawerId);
      this.BindString("@partialLookupKey", partialLookupKey, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        List<StrongBoxItemInfo> items = resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
        return items != null && items.Count > 0 ? items : new List<StrongBoxItemInfo>();
      }
    }
  }
}
