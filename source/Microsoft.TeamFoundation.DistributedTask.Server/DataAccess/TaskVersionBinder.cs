// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskVersionBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskVersionBinder : ObjectBinder<KeyValuePair<Guid, TaskVersion>>
  {
    private SqlColumnBinder m_taskId = new SqlColumnBinder("TaskId");
    private SqlColumnBinder m_majorVersion = new SqlColumnBinder("MajorVersion");
    private SqlColumnBinder m_minorVersion = new SqlColumnBinder("MinorVersion");
    private SqlColumnBinder m_patchVersion = new SqlColumnBinder("PatchVersion");
    private SqlColumnBinder m_isTest = new SqlColumnBinder("IsTest");

    protected override KeyValuePair<Guid, TaskVersion> Bind() => new KeyValuePair<Guid, TaskVersion>(this.m_taskId.GetGuid((IDataReader) this.Reader), new TaskVersion()
    {
      Major = this.m_majorVersion.GetInt32((IDataReader) this.Reader),
      Minor = this.m_minorVersion.GetInt32((IDataReader) this.Reader),
      Patch = this.m_patchVersion.GetInt32((IDataReader) this.Reader),
      IsTest = this.m_isTest.GetBoolean((IDataReader) this.Reader)
    });
  }
}
