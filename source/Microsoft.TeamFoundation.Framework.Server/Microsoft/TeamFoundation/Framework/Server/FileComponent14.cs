// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent14
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent14 : FileComponent13
  {
    public override ResultCollection QueryAllFiles(
      RemoteStoreId remoteStoreId,
      Guid lastResourceId,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryAllFiles", 3600);
      this.BindInt("@remoteStoreId", (int) remoteStoreId);
      this.BindGuid("@lastResourceId", lastResourceId);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<FileStatistics>((ObjectBinder<FileStatistics>) this.CreateFileStatisticsColumns());
      return resultCollection;
    }

    internal override ResultCollection QueryAllFiles(
      ObjectBinder<FileStatistics> binder,
      RemoteStoreId remoteStoreId,
      Guid lastResourceId,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_QueryAllFiles", 3600);
      this.BindInt("@remoteStoreId", (int) remoteStoreId);
      this.BindGuid("@lastResourceId", lastResourceId);
      this.BindInt("@batchSize", batchSize);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<FileStatistics>(binder);
      return resultCollection;
    }

    internal override TeamFoundationFileSet RetrieveFile(
      Guid resourceId,
      bool includeContent = true,
      bool failOnDelete = true)
    {
      ArgumentUtility.CheckForEmptyGuid(resourceId, nameof (resourceId));
      this.PrepareStoredProcedure("prc_RetrieveFileFromResourceId", 300);
      this.BindGuid("@resourceId", resourceId);
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) new DataspaceAgnosticFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) new DataspaceAgnosticFileBinder());
      return new TeamFoundationFileSet(result);
    }
  }
}
