// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent12
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent12 : FileContainerComponent11
  {
    private static readonly SqlMetaData[] typ_Int64Table = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.BigInt)
    };

    public override ContainerItemBlobReference GetBlobReference(
      long artifactId,
      Guid? dataspaceIdentifier)
    {
      return this.GetBlobReferences((IEnumerable<long>) new long[1]
      {
        artifactId
      }, dataspaceIdentifier).FirstOrDefault<ContainerItemBlobReference>();
    }

    public override List<ContainerItemBlobReference> GetBlobReferences(
      IEnumerable<long> artifactIds,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (GetBlobReferences));
      this.PrepareStoredProcedure("prc_GetBlobReferences");
      this.BindDataspace(dataspaceIdentifier);
      this.BindTable("@artifactIds", "typ_Int64Table", artifactIds.Select<long, SqlDataRecord>((System.Func<long, SqlDataRecord>) (artifactId =>
      {
        SqlDataRecord blobReferences = new SqlDataRecord(FileContainerComponent12.typ_Int64Table);
        blobReferences.SetInt64(0, artifactId);
        return blobReferences;
      })));
      List<ContainerItemBlobReference> items;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        items = resultCollection.GetCurrent<ContainerItemBlobReference>().Items;
      }
      this.TraceLeave(0, nameof (GetBlobReferences));
      return items;
    }
  }
}
