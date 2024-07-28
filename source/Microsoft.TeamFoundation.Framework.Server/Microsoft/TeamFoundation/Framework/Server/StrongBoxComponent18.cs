// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent18
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent18 : StrongBoxComponent17
  {
    internal override ICollection<StrongBoxItemInfo> QueryItemsToReencryptMultisets(
      int batchSize,
      StrongBoxItemInfo lastAttemptedItem)
    {
      batchSize = batchSize < 1 ? 1 : batchSize;
      Guid parameterValue = lastAttemptedItem != null ? lastAttemptedItem.DrawerId : Guid.Empty;
      this.PrepareStoredProcedure("prc_QueryStrongBoxItemsBatchToReencrypt");
      this.BindInt("@batchSize", batchSize);
      this.BindGuid("@drawerIdWatermark", parameterValue);
      this.BindString("@lookupKeyWatermark", lastAttemptedItem?.LookupKey, 512, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        List<StrongBoxItemInfo> items = resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
        if (resultCollection.TryNextResult())
          items.AddRange((IEnumerable<StrongBoxItemInfo>) resultCollection.GetCurrent<StrongBoxItemInfo>().Items);
        return (ICollection<StrongBoxItemInfo>) items;
      }
    }
  }
}
