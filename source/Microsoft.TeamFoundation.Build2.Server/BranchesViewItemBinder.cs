// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BranchesViewItemBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class BranchesViewItemBinder : BuildObjectBinder<BranchesViewItem>
  {
    private SqlColumnBinder m_buildId = new SqlColumnBinder("BuildId");
    private SqlColumnBinder m_buildNumber = new SqlColumnBinder("BuildNumber");
    private SqlColumnBinder m_sourceBranch = new SqlColumnBinder("SourceBranch");
    private SqlColumnBinder m_repositoryType = new SqlColumnBinder("RepositoryType");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_sourceVersionInfo = new SqlColumnBinder("SourceVersionInfo");
    private SqlColumnBinder m_sourceBranchId = new SqlColumnBinder("SourceBranchId");
    private SqlColumnBinder m_repositoryId = new SqlColumnBinder("RepositoryId");
    private SqlColumnBinder m_appendCommitMessageToRunName = new SqlColumnBinder("AppendCommitMessageToRunName");

    protected override BranchesViewItem Bind()
    {
      string str = string.Empty;
      if (this.m_sourceVersionInfo.ColumnExists((IDataReader) this.Reader))
      {
        string toDeserialize = this.m_sourceVersionInfo.GetString((IDataReader) this.Reader, true);
        if (!string.IsNullOrEmpty(toDeserialize))
          str = JsonUtility.FromString<SourceVersionInfo>(toDeserialize)?.Message;
      }
      int int32_1 = this.m_buildId.GetInt32((IDataReader) this.Reader);
      string serverPath = DBHelper.DBPathToServerPath(this.m_buildNumber.GetString((IDataReader) this.Reader, false));
      int status = (int) this.m_status.GetByte((IDataReader) this.Reader);
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      BuildResult? result = nullableByte.HasValue ? new BuildResult?((BuildResult) nullableByte.GetValueOrDefault()) : new BuildResult?();
      string repositoryType = this.m_repositoryType.GetString((IDataReader) this.Reader, true);
      int int32_2 = this.m_repositoryId.GetInt32((IDataReader) this.Reader, -1, -1);
      string sourceBranch = this.m_sourceBranch.GetString((IDataReader) this.Reader, false);
      int int32_3 = this.m_sourceBranchId.GetInt32((IDataReader) this.Reader, -1, -1);
      DateTime dateTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader);
      DateTime? nullableDateTime1 = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      DateTime? nullableDateTime2 = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      string sourceVersionMessage = str;
      int num = this.m_appendCommitMessageToRunName.GetBoolean((IDataReader) this.Reader, true, true) ? 1 : 0;
      return new BranchesViewItem(int32_1, serverPath, (BuildStatus) status, result, repositoryType, int32_2, sourceBranch, int32_3, dateTime, nullableDateTime1, nullableDateTime2, sourceVersionMessage, num != 0);
    }
  }
}
