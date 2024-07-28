// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent16
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent16 : FileContainerComponent15
  {
    internal override sealed FileContainerCleanupStats CleanupDeletedFileContentByShares(
      bool useSecondaryFileIdRange,
      Decimal startingPoint,
      Decimal endingPoint,
      bool hashJoin = false,
      int batchSelectSize = 0,
      int batchDeleteSize = 0)
    {
      this.PrepareStoredProcedure("prc_CleanupDeletedFileContentSegmentedByShares", 43200);
      this.BindBoolean("@useSecondaryFileIdRange", useSecondaryFileIdRange);
      this.BindDecimal("@startingpoint", startingPoint);
      this.BindDecimal("@endingpoint", endingPoint);
      this.BindInt("@batchSelectSize", batchSelectSize);
      this.BindInt("@batchDeleteSize", batchDeleteSize);
      this.BindBoolean("@hashJoin", hashJoin);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileContainerCleanupStats>((ObjectBinder<FileContainerCleanupStats>) this.GetFileContainerCleanupStatsBinder());
        return resultCollection.GetCurrent<FileContainerCleanupStats>().Items[0];
      }
    }
  }
}
