// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ConvertRetentionResultBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class ConvertRetentionResultBinder : ObjectBinder<ConvertRetentionResult>
  {
    private SqlColumnBinder m_convertedCount = new SqlColumnBinder("ConvertedCount");
    private SqlColumnBinder m_lastFinishTime = new SqlColumnBinder("LastFinishTime");

    protected override ConvertRetentionResult Bind() => new ConvertRetentionResult()
    {
      ConvertedCount = this.m_convertedCount.GetInt32((IDataReader) this.Reader),
      LastFinishTime = this.m_lastFinishTime.GetNullableDateTime((IDataReader) this.Reader) ?? DateTime.MaxValue
    };
  }
}
