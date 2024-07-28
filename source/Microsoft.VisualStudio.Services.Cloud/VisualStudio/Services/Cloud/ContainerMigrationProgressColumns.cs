// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContainerMigrationProgressColumns
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ContainerMigrationProgressColumns : ObjectBinder<ContainerMigrationProgress>
  {
    private SqlColumnBinder MigrationIdColumn = new SqlColumnBinder("MigrationId");
    private SqlColumnBinder ParallelBlobMigrationCoordinatorJobIdColumn = new SqlColumnBinder("ParallelBlobMigrationCoordinatorJobId");
    private SqlColumnBinder TfsBlobMigrationJobIdColumn = new SqlColumnBinder("TfsBlobMigrationJobId");
    private SqlColumnBinder JobNoColumn = new SqlColumnBinder("JobNo");
    private SqlColumnBinder TotalJobsColumn = new SqlColumnBinder("TotalJobs");
    private SqlColumnBinder UriColumn = new SqlColumnBinder("Uri");
    private SqlColumnBinder PrefixColumn = new SqlColumnBinder("Prefix");
    private SqlColumnBinder BlobsCopiedColumn = new SqlColumnBinder("BlobsCopied");
    private SqlColumnBinder BlobsSkippedColumn = new SqlColumnBinder("BlobsSkipped");
    private SqlColumnBinder BlobsFailedColumn = new SqlColumnBinder("BlobsFailed");
    private SqlColumnBinder CompletedColumn = new SqlColumnBinder("Completed");
    private SqlColumnBinder ChangedDateColumn = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder StatusMessageColumn = new SqlColumnBinder("StatusMessage");

    protected override ContainerMigrationProgress Bind() => new ContainerMigrationProgress()
    {
      MigrationId = this.MigrationIdColumn.GetGuid((IDataReader) this.Reader),
      ParallelBlobMigrationCoordinatorJobId = this.ParallelBlobMigrationCoordinatorJobIdColumn.GetGuid((IDataReader) this.Reader),
      TfsBlobMigrationJobId = this.TfsBlobMigrationJobIdColumn.GetGuid((IDataReader) this.Reader),
      JobNo = this.JobNoColumn.GetInt32((IDataReader) this.Reader, 0),
      TotalJobs = this.TotalJobsColumn.GetInt32((IDataReader) this.Reader, 0),
      Uri = this.UriColumn.GetString((IDataReader) this.Reader, false),
      Prefix = this.PrefixColumn.GetString((IDataReader) this.Reader, false),
      BlobsCopied = this.BlobsCopiedColumn.GetInt64((IDataReader) this.Reader, 0L),
      BlobsSkipped = this.BlobsSkippedColumn.GetInt64((IDataReader) this.Reader, 0L),
      BlobsFailed = this.BlobsFailedColumn.GetInt64((IDataReader) this.Reader, 0L),
      Completed = this.CompletedColumn.GetBoolean((IDataReader) this.Reader, false, false),
      ChangedDate = this.ChangedDateColumn.GetDateTime((IDataReader) this.Reader),
      StatusMessage = this.StatusMessageColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
