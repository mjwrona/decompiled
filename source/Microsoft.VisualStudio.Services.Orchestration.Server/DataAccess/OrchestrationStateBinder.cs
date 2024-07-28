// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.OrchestrationStateBinder
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal sealed class OrchestrationStateBinder : ObjectBinder<OrchestrationState>
  {
    private SqlColumnBinder m_instanceId = new SqlColumnBinder("InstanceId");
    private SqlColumnBinder m_executionId = new SqlColumnBinder("ExecutionId");
    private SqlColumnBinder m_parentInstanceId = new SqlColumnBinder("ParentInstanceId");
    private SqlColumnBinder m_parentExecutionId = new SqlColumnBinder("ParentExecutionId");
    private SqlColumnBinder m_parentInstanceName = new SqlColumnBinder("ParentName");
    private SqlColumnBinder m_parentInstanceVersion = new SqlColumnBinder("ParentVersion");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_version = new SqlColumnBinder("Version");
    private SqlColumnBinder m_status = new SqlColumnBinder("Status");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_completedOn = new SqlColumnBinder("CompletedOn");
    private SqlColumnBinder m_lastUpdatedOn = new SqlColumnBinder("LastUpdatedOn");
    private SqlColumnBinder m_size = new SqlColumnBinder("Size");
    private SqlColumnBinder m_compressedSize = new SqlColumnBinder("CompressedSize");
    private SqlColumnBinder m_output = new SqlColumnBinder("Output");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");

    protected override OrchestrationState Bind()
    {
      OrchestrationState orchestrationState = new OrchestrationState();
      orchestrationState.OrchestrationInstance = new OrchestrationInstance()
      {
        InstanceId = this.m_instanceId.GetString((IDataReader) this.Reader, false),
        ExecutionId = this.m_executionId.GetString((IDataReader) this.Reader, false)
      };
      string str = this.m_parentInstanceId.GetString((IDataReader) this.Reader, true);
      if (!string.IsNullOrEmpty(str))
        orchestrationState.ParentInstance = new ParentInstance()
        {
          OrchestrationInstance = new OrchestrationInstance()
          {
            InstanceId = str,
            ExecutionId = this.m_parentExecutionId.GetString((IDataReader) this.Reader, false)
          },
          Name = this.m_parentInstanceName.GetString((IDataReader) this.Reader, false),
          Version = this.m_parentInstanceVersion.GetString((IDataReader) this.Reader, false)
        };
      orchestrationState.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      orchestrationState.Version = this.m_version.GetString((IDataReader) this.Reader, false);
      orchestrationState.OrchestrationStatus = (OrchestrationStatus) this.m_status.GetByte((IDataReader) this.Reader);
      orchestrationState.CreatedTime = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      orchestrationState.CompletedTime = this.m_completedOn.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.m_completedOn.GetDateTime((IDataReader) this.Reader));
      orchestrationState.LastUpdatedTime = this.m_lastUpdatedOn.GetDateTime((IDataReader) this.Reader);
      orchestrationState.Size = this.m_size.GetInt64((IDataReader) this.Reader);
      orchestrationState.CompressedSize = this.m_compressedSize.GetInt64((IDataReader) this.Reader);
      orchestrationState.Output = this.m_output.GetString((IDataReader) this.Reader, true);
      orchestrationState.Status = this.m_state.GetString((IDataReader) this.Reader, true);
      return orchestrationState;
    }
  }
}
