// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskOrchestrationJobBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class TaskOrchestrationJobBinder : ObjectBinder<TaskOrchestrationJob>
  {
    private SqlColumnBinder m_jobData = new SqlColumnBinder("JobData");

    protected override TaskOrchestrationJob Bind() => JsonUtility.Deserialize<TaskOrchestrationJob>(this.m_jobData.GetBytes((IDataReader) this.Reader, false));
  }
}
