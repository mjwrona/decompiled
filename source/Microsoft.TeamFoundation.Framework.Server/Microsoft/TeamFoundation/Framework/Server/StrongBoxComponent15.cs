// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent15
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent15 : StrongBoxComponent14
  {
    internal override ICollection<StrongBoxItemInfo> QueryItemsBatchToReencrypt(
      int batchSize = -1,
      int errorOffset = 0)
    {
      this.PrepareStoredProcedure("prc_QueryStrongBoxItemsBatchToReencrypt");
      this.BindInt("@batchSize", batchSize);
      this.BindInt("@errorOffset", errorOffset);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        return (ICollection<StrongBoxItemInfo>) resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
      }
    }

    public override void ReencryptMultipleItems(
      IEnumerable<TeamFoundationStrongBoxServiceBase.ReencryptionData> items)
    {
      this.PrepareStoredProcedure("prc_UpdateStrongBoxItemsEncryption");
      this.BindStrongBoxItemInfoTableForReencryption("@items", items);
      this.ExecuteNonQuery();
    }
  }
}
