// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.LanguageMetadataRecordBinder
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class LanguageMetadataRecordBinder : ObjectBinder<LanguageMetadataRecord>
  {
    private SqlColumnBinder m_projectId = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder m_repositoryType = new SqlColumnBinder("RepositoryType");
    private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
    private SqlColumnBinder m_fileCount = new SqlColumnBinder("FileCount");
    private SqlColumnBinder m_updatedTime = new SqlColumnBinder("UpdatedTime");
    private SqlColumnBinder m_changeSetId = new SqlColumnBinder("ChangeSetId");
    private SqlColumnBinder m_commitId = new SqlColumnBinder("CommitId");
    private SqlColumnBinder m_branch = new SqlColumnBinder("Branch");
    private SqlColumnBinder m_languageBreakdown = new SqlColumnBinder("LanguageBreakdown");
    private SqlColumnBinder m_resultPhase = new SqlColumnBinder("ResultPhase");
    private SqlColumnBinder m_version = new SqlColumnBinder("RecordVersion");

    protected override LanguageMetadataRecord Bind()
    {
      Guid guid1 = this.m_projectId.GetGuid((IDataReader) this.Reader, false);
      byte num1 = this.m_repositoryType.GetByte((IDataReader) this.Reader);
      Guid guid2 = this.m_repositoryId.GetGuid((IDataReader) this.Reader, false);
      int int32_1 = this.m_fileCount.GetInt32((IDataReader) this.Reader);
      DateTime dateTime = this.m_updatedTime.GetDateTime((IDataReader) this.Reader);
      int? nullableInt32 = this.m_changeSetId.GetNullableInt32((IDataReader) this.Reader);
      Sha1Id? nullableSha1Id = this.m_commitId.GetNullableSha1Id((IDataReader) this.Reader);
      string str1 = this.m_branch.GetString((IDataReader) this.Reader, true);
      string str2 = this.m_languageBreakdown.GetString((IDataReader) this.Reader, false);
      byte num2 = this.m_resultPhase.GetByte((IDataReader) this.Reader);
      int int32_2 = this.m_version.GetInt32((IDataReader) this.Reader, 0, 0);
      int repositoryType = (int) num1;
      Guid repositoryId = guid2;
      int fileCount = int32_1;
      DateTime updatedTime = dateTime;
      int? changesetId = nullableInt32;
      Sha1Id? commitId = nullableSha1Id;
      string branch = str1;
      string languageBreakdown = str2;
      int resultPhase = (int) num2;
      int recordVersion = int32_2;
      return new LanguageMetadataRecord(guid1, (byte) repositoryType, repositoryId, fileCount, updatedTime, changesetId, commitId, branch, languageBreakdown, (byte) resultPhase, recordVersion);
    }
  }
}
