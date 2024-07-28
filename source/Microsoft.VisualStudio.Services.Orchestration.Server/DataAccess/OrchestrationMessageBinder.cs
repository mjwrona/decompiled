// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess.OrchestrationMessageBinder
// Assembly: Microsoft.VisualStudio.Services.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 953225F5-5DFE-4840-B8F7-3B94A5257E43
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Orchestration.Server.DataAccess
{
  internal sealed class OrchestrationMessageBinder : ObjectBinder<OrchestrationMessage>
  {
    private SqlColumnBinder m_messageId = new SqlColumnBinder("MessageId");
    private SqlColumnBinder m_sessionId = new SqlColumnBinder("SessionId");
    private SqlColumnBinder m_queueTime = new SqlColumnBinder("QueueTime");
    private SqlColumnBinder m_scheduledDeliveryTime = new SqlColumnBinder("ScheduledDeliveryTime");
    private SqlColumnBinder m_compressionType = new SqlColumnBinder("CompressionType");
    private SqlColumnBinder m_content = new SqlColumnBinder("Content");

    protected override OrchestrationMessage Bind() => new OrchestrationMessage()
    {
      MessageId = this.m_messageId.GetInt64((IDataReader) this.Reader),
      SessionId = this.m_sessionId.GetString((IDataReader) this.Reader, true),
      QueueTime = this.m_queueTime.GetDateTime((IDataReader) this.Reader),
      ScheduledDeliveryTime = this.m_scheduledDeliveryTime.IsNull((IDataReader) this.Reader) ? new DateTime?() : new DateTime?(this.m_scheduledDeliveryTime.GetDateTime((IDataReader) this.Reader)),
      CompressionType = (CompressionType) this.m_compressionType.GetByte((IDataReader) this.Reader),
      Content = this.m_content.GetBytes((IDataReader) this.Reader, false)
    };
  }
}
