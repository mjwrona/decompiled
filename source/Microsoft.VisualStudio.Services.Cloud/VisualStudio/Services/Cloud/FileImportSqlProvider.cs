// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FileImportSqlProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class FileImportSqlProvider : IFileImportSqlProvider
  {
    protected readonly ITFLogger Logger;
    protected readonly IComponentCreator ComponentCreator;
    private readonly int m_sourcePartitionId;
    private readonly int m_targetPartitionId;

    public ISqlConnectionInfo SourceSqlConnectionInfo { get; private set; }

    public ISqlConnectionInfo TargetSqlConnectionInfo { get; private set; }

    public FileImportSqlProvider(
      ISqlConnectionInfo sourceSqlConnectionInfo,
      ISqlConnectionInfo targetSqlConnectionInfo,
      ITFLogger logger)
    {
      this.SourceSqlConnectionInfo = sourceSqlConnectionInfo;
      this.TargetSqlConnectionInfo = targetSqlConnectionInfo;
      this.m_sourcePartitionId = FileImportSqlProvider.GetPartitionId(sourceSqlConnectionInfo);
      this.m_targetPartitionId = FileImportSqlProvider.GetPartitionId(targetSqlConnectionInfo);
      int fileVersion = FileImportSqlProvider.GetFileVersion(sourceSqlConnectionInfo, this.m_sourcePartitionId);
      this.ComponentCreator = FileComponent.ComponentFactory.GetComponentCreator(fileVersion, fileVersion);
      this.Logger = logger;
    }

    public long GetContentSize()
    {
      using (DiagnosticComponent componentRaw = this.SourceSqlConnectionInfo.CreateComponentRaw<DiagnosticComponent>(logger: this.Logger))
      {
        if (componentRaw is DiagnosticComponent3 diagnosticComponent3)
        {
          TableSpaceUsage tableSpaceUsage = diagnosticComponent3.QueryTableSpaceUsage().Where<TableSpaceUsage>((Func<TableSpaceUsage, bool>) (t => string.Equals("tbl_Content", t.TableName, StringComparison.OrdinalIgnoreCase))).SingleOrDefault<TableSpaceUsage>();
          if (tableSpaceUsage != null)
            return tableSpaceUsage.ReservedPageCount * 8192L;
        }
      }
      return -1;
    }

    public virtual (int, int) GetMinMaxFileId()
    {
      using (FileComponent fileComponent = this.CreateFileComponent(this.TargetSqlConnectionInfo, this.m_targetPartitionId))
      {
        FileIdUsage fileIdUsage = fileComponent.GetFileIdUsage();
        return ((int) fileIdUsage.MaxNegativeId, (int) fileIdUsage.MaxPositiveId);
      }
    }

    public virtual List<FileStatistics> GetFiles(Guid lastProcessedResourceId, int batchSize)
    {
      using (FileComponent fileComponent = this.CreateFileComponent(this.TargetSqlConnectionInfo, this.m_targetPartitionId))
      {
        using (ResultCollection resultCollection = fileComponent.QueryAllFiles((ObjectBinder<FileStatistics>) new FileStatisticsRawBinder(), RemoteStoreId.SqlServer, lastProcessedResourceId, batchSize))
          return resultCollection.GetCurrent<FileStatistics>().Items;
      }
    }

    public IFileImportContentProvider CreateContentProvider(Guid resourceId) => (IFileImportContentProvider) new FileImportSqlProvider.ContentProvider(this.CreateFileComponent(this.SourceSqlConnectionInfo, this.m_sourcePartitionId), resourceId);

    private FileComponent CreateFileComponent(ISqlConnectionInfo sqlConnectionInfo, int partitionId)
    {
      FileComponent fileComponent = this.ComponentCreator.Create(sqlConnectionInfo, 3600, 0, 200, this.Logger, (CircuitBreakerDatabaseProperties) null) as FileComponent;
      fileComponent.PartitionId = partitionId;
      return fileComponent;
    }

    protected static int GetPartitionId(ISqlConnectionInfo connectionInfo)
    {
      using (DatabasePartitionComponent componentRaw = connectionInfo.CreateComponentRaw<DatabasePartitionComponent>())
        return componentRaw.QueryOnlyPartition().PartitionId;
    }

    private static int GetFileVersion(ISqlConnectionInfo connectionInfo, int partitionId)
    {
      using (FileComponent componentRaw = connectionInfo.CreateComponentRaw<FileComponent>())
      {
        componentRaw.PartitionId = partitionId;
        return componentRaw.Version;
      }
    }

    private class ContentProvider : IFileImportContentProvider, IDisposable
    {
      private readonly FileComponent m_fileComponent;
      private readonly TeamFoundationFileSet m_fileSet;

      public Stream Content => this.m_fileSet?.FullVersion?.Content;

      public ContentProvider(FileComponent fileComponent, Guid resourceId)
      {
        this.m_fileComponent = fileComponent;
        this.m_fileSet = fileComponent.RetrieveFile(resourceId, failOnDelete: false);
      }

      public void Dispose()
      {
        try
        {
          this.m_fileSet?.Dispose();
        }
        finally
        {
          this.m_fileComponent?.Dispose();
        }
      }
    }
  }
}
