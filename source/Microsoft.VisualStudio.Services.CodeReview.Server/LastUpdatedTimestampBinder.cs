// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.LastUpdatedTimestampBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal sealed class LastUpdatedTimestampBinder : ObjectBinder<DateTime>
  {
    private SqlColumnBinder m_lastUpdatedDate = new SqlColumnBinder("LastUpdatedTimestamp");

    protected override DateTime Bind() => this.m_lastUpdatedDate.GetDateTime((IDataReader) this.Reader);
  }
}
