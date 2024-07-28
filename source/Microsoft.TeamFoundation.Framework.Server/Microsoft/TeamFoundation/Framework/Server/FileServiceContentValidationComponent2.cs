// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileServiceContentValidationComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileServiceContentValidationComponent2 : FileServiceContentValidationComponent
  {
    public override List<FileServiceContentValidationMetadata> QueryUnvalidatedFiles(
      int dataspaceId,
      int fileIdWatermark,
      int batchSize = 1000)
    {
      ArgumentUtility.CheckForOutOfRange(batchSize, nameof (batchSize), 1);
      this.PrepareStoredProcedure("prc_QueryUnvalidatedFiles");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (fileIdWatermark), fileIdWatermark);
      this.BindInt(nameof (batchSize), batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<FileServiceContentValidationMetadata>((ObjectBinder<FileServiceContentValidationMetadata>) this.GetContentValidationBinder());
        return resultCollection.GetCurrent<FileServiceContentValidationMetadata>().Items;
      }
    }

    public override void SaveMetadata(FileServiceContentValidationMetadata entry)
    {
      ArgumentUtility.CheckForNull<FileServiceContentValidationMetadata>(entry, nameof (entry));
      this.PrepareStoredProcedure("prc_SaveContentValidationMetadata");
      this.BindInt("fileId", entry.FileId);
      this.BindInt("dataspaceId", entry.DataspaceId);
      this.BindGuid("uploader", entry.Uploader);
      this.BindString("ipAddress", entry.IPAddress, 45, true, SqlDbType.NVarChar);
      ContentValidationScanType? scanType = entry.ScanType;
      this.BindNullableByte("scanType", scanType.HasValue ? new byte?((byte) scanType.GetValueOrDefault()) : new byte?());
      this.ExecuteNonQuery();
    }

    public override void CleanupMetadata(int retentionPeriodInDays = 14)
    {
      ArgumentUtility.CheckForOutOfRange(retentionPeriodInDays, nameof (retentionPeriodInDays), 0, int.MaxValue);
      this.PrepareStoredProcedure("prc_CleanupContentValidationMetadata");
      this.BindInt(nameof (retentionPeriodInDays), retentionPeriodInDays);
      this.ExecuteNonQuery();
    }

    public override int GetWatermark(int dataspaceId)
    {
      this.PrepareStoredProcedure("prc_QueryContentValidationWatermark");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      return (int) this.ExecuteScalar();
    }

    public override void SetWatermark(int dataspaceId, int fileIdWatermark)
    {
      this.PrepareStoredProcedure("prc_UpdateContentValidationWatermark");
      this.BindInt(nameof (dataspaceId), dataspaceId);
      this.BindInt(nameof (fileIdWatermark), fileIdWatermark);
      this.ExecuteNonQuery();
    }

    protected ContentValidationBinder GetContentValidationBinder() => new ContentValidationBinder(this.RequestContext);
  }
}
