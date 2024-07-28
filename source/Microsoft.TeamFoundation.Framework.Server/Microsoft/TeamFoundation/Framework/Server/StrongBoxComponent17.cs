// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent17
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent17 : StrongBoxComponent16
  {
    public IList<DrawerStats> GetDrawerStats(int numberOfDrawersRetrieve)
    {
      if (numberOfDrawersRetrieve < 0)
        throw new ArgumentOutOfRangeException(nameof (numberOfDrawersRetrieve), "numberOfDrawersRetrieve can't be negative");
      if (numberOfDrawersRetrieve == 0)
        return (IList<DrawerStats>) new List<DrawerStats>();
      this.PrepareStoredProcedure("prc_CountStrongBoxDrawerItems");
      this.BindInt("@count", numberOfDrawersRetrieve);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<DrawerStats>((ObjectBinder<DrawerStats>) new DrawerStatsBinder());
        return (IList<DrawerStats>) resultCollection.GetCurrent<DrawerStats>().Items;
      }
    }
  }
}
