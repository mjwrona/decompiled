// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileComponent6
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class FileComponent6 : FileComponent5
  {
    protected override void CommitFile(
      long fileId,
      Guid dataspaceIdentifier,
      byte[] hashValue,
      long fileLength)
    {
      this.PrepareStoredProcedure("prc_CommitFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      this.BindBinary("@hashValue", hashValue, 16, SqlDbType.Binary);
      this.BindLong("@fileLength", fileLength);
      this.BindInt("@relatedFileId", 0);
      this.BindLong("@maxPatchableSize", 4194304L);
      this.BindInt("@maxRetryCount", 5);
      this.ExecuteNonQuery();
    }

    internal override TeamFoundationFileSet RetrievePendingFile(long fileId)
    {
      this.PrepareStoredProcedure("prc_RetrieveFile", 3600);
      this.BindInt("@fileId", (int) fileId);
      this.BindBoolean("@includeContent", true);
      this.BindBoolean("@failOnDelete", true);
      this.BindBoolean("@readIncomplete", true);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }

    internal override TeamFoundationFileSet RetrieveFile(
      FileIdentifier fileIdentifier,
      bool includeContent,
      bool failOnDelete)
    {
      int fileId = (int) fileIdentifier.FileId;
      if (fileId == 0)
        throw new FileIdNotFoundException((long) fileId);
      this.PrepareStoredProcedure("prc_RetrieveFile", 3600);
      this.BindInt("@fileId", fileId);
      this.BindBoolean("@includeContent", includeContent);
      this.BindBoolean("@failOnDelete", failOnDelete);
      this.BindBoolean("@readIncomplete", false);
      ResultCollection result = new ResultCollection((IDataReader) this.ExecuteReader(CommandBehavior.SequentialAccess), this.ProcedureName, this.RequestContext);
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      result.AddBinder<TeamFoundationFile>((ObjectBinder<TeamFoundationFile>) this.CreateFileBinder());
      return new TeamFoundationFileSet(result);
    }
  }
}
