// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent13 : FileContainerComponent12
  {
    private static readonly SqlMetaData[] typ_Int64Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.BigInt)
    };

    public override List<ContainerItemBlobReference> GetUnusedBlobReferences(
      int maxNumberOfFiles,
      int retentionPeriodInDays)
    {
      this.TraceEnter(0, nameof (GetUnusedBlobReferences));
      this.PrepareStoredProcedure("prc_GetUnusedBlobReferences");
      this.BindInt("@chunkSize", maxNumberOfFiles);
      this.BindInt("@retentionPeriodInDays", retentionPeriodInDays);
      List<ContainerItemBlobReference> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        items = resultCollection.GetCurrent<ContainerItemBlobReference>().Items;
      }
      this.TraceLeave(0, nameof (GetUnusedBlobReferences));
      return items;
    }

    public override void HardDeleteBlobReferences(IEnumerable<long> artifactIds)
    {
      this.TraceEnter(0, nameof (HardDeleteBlobReferences));
      this.PrepareStoredProcedure("prc_HardDeleteBlobReferences");
      this.BindTable("@artifactIds", "typ_Int64Table", artifactIds.Select<long, SqlDataRecord>((System.Func<long, SqlDataRecord>) (artifactId =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FileContainerComponent13.typ_Int64Table);
        sqlDataRecord.SetInt64(0, artifactId);
        return sqlDataRecord;
      })));
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (HardDeleteBlobReferences));
    }

    internal override ContainerItemBlobReferenceBinder GetContainerItemBlobReferenceBinder() => (ContainerItemBlobReferenceBinder) new ContainerItemBlobReferenceBinder2((TeamFoundationSqlResourceComponent) this);
  }
}
