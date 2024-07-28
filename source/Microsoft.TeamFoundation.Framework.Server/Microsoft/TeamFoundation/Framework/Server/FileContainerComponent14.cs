// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerComponent14
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileContainerComponent14 : FileContainerComponent13
  {
    public override ContainerItemBlobReference AddPendingBlobReference(
      Guid sessionId,
      BlobCompressionType compressionType,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (AddPendingBlobReference));
      this.PrepareStoredProcedure("prc_AddPendingBlobReference");
      this.BindDataspace(dataspaceIdentifier);
      this.BindGuid("@sessionId", sessionId);
      this.BindByte("@compressionType", (byte) compressionType);
      ContainerItemBlobReference itemBlobReference;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        itemBlobReference = resultCollection.GetCurrent<ContainerItemBlobReference>().FirstOrDefault<ContainerItemBlobReference>();
      }
      this.TraceLeave(0, nameof (AddPendingBlobReference));
      return itemBlobReference;
    }

    public override ContainerItemBlobReference CompletePendingBlobReference(
      long artifactId,
      string artifactHash,
      Guid? dataspaceIdentifier)
    {
      this.TraceEnter(0, nameof (CompletePendingBlobReference));
      this.PrepareStoredProcedure("prc_CompletePendingBlobReference");
      this.BindLong("@artifactId", artifactId);
      this.BindDataspace(dataspaceIdentifier);
      this.BindString("@artifactHash", artifactHash, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ContainerItemBlobReference itemBlobReference;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ContainerItemBlobReference>((ObjectBinder<ContainerItemBlobReference>) this.GetContainerItemBlobReferenceBinder());
        itemBlobReference = resultCollection.GetCurrent<ContainerItemBlobReference>().FirstOrDefault<ContainerItemBlobReference>();
      }
      this.TraceLeave(0, nameof (CompletePendingBlobReference));
      return itemBlobReference;
    }

    internal override ContainerItemBlobReferenceBinder GetContainerItemBlobReferenceBinder() => (ContainerItemBlobReferenceBinder) new ContainerItemBlobReferenceBinder3((TeamFoundationSqlResourceComponent) this);
  }
}
