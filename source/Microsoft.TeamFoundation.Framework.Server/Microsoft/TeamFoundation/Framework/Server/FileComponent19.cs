// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent19
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent19 : FileComponent18
  {
    public override IDictionary<long, long> QueryFileSizes(
      IEnumerable<long> fileIds,
      Guid dataspaceIdentifier,
      OwnerId ownerId)
    {
      this.PrepareStoredProcedure("prc_QueryFileSizes");
      int dataspaceId = this.GetDataspaceId(dataspaceIdentifier, TeamFoundationFileService.GetCategoryFromOwnerId(ownerId));
      this.BindInt32Table("@fileIds", fileIds.Select<long, int>((System.Func<long, int>) (f => (int) f)));
      this.BindInt("@dataspaceId", dataspaceId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<int, long>>((ObjectBinder<Tuple<int, long>>) new FileComponent19.FileSizesBinder());
      return (IDictionary<long, long>) resultCollection.GetCurrent<Tuple<int, long>>().Items.ToDictionary<Tuple<int, long>, long, long>((System.Func<Tuple<int, long>, long>) (t => (long) t.Item1), (System.Func<Tuple<int, long>, long>) (t => t.Item2));
    }

    public int GetDataspaceId(FileIdentifier f, OwnerId ownerId) => !f.DataspaceIdentifier.HasValue ? 0 : this.GetDataspaceId(f.DataspaceIdentifier.Value, TeamFoundationFileService.GetCategoryFromOwnerId(ownerId));

    internal class FileSizesBinder : ObjectBinder<Tuple<int, long>>
    {
      protected SqlColumnBinder FileIdColumn = new SqlColumnBinder("FileId");
      protected SqlColumnBinder FileLengthColumn = new SqlColumnBinder("FileLength");

      protected override Tuple<int, long> Bind() => new Tuple<int, long>(this.FileIdColumn.GetInt32((IDataReader) this.Reader), this.FileLengthColumn.GetInt64((IDataReader) this.Reader));
    }
  }
}
