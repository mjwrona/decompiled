// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.RetentionLeaseBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class RetentionLeaseBinder : BuildObjectBinder<RetentionLease>
  {
    private SqlColumnBinder m_id = new SqlColumnBinder("LeaseId");
    private SqlColumnBinder m_ownerId = new SqlColumnBinder("OwnerId");
    private SqlColumnBinder m_runId = new SqlColumnBinder("RunId");
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder m_validUntil = new SqlColumnBinder("ValidUntil");
    private SqlColumnBinder m_highPriority = new SqlColumnBinder("HighPriority");

    public RetentionLeaseBinder(
      IVssRequestContext requestContext,
      BuildSqlComponentBase resourceComponent)
      : base(requestContext, resourceComponent)
    {
    }

    protected override RetentionLease Bind()
    {
      int int32_1 = this.m_id.GetInt32((IDataReader) this.Reader);
      string str = this.m_ownerId.GetString((IDataReader) this.Reader, false);
      int int32_2 = this.m_runId.GetInt32((IDataReader) this.Reader);
      int int32_3 = this.m_definitionId.GetInt32((IDataReader) this.Reader);
      DateTime dateTime1 = this.m_createdOn.GetDateTime((IDataReader) this.Reader);
      DateTime dateTime2 = this.m_validUntil.GetDateTime((IDataReader) this.Reader);
      bool boolean = this.m_highPriority.GetBoolean((IDataReader) this.Reader);
      string ownerId = str;
      int runId = int32_2;
      int definitionId = int32_3;
      DateTime createdOn = dateTime1;
      DateTime validUntil = dateTime2;
      int num = boolean ? 1 : 0;
      return new RetentionLease(int32_1, ownerId, runId, definitionId, createdOn, validUntil, num != 0);
    }
  }
}
