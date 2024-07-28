// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DequeueMessageDataBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class DequeueMessageDataBinder : ObjectBinder<TfsmqDequeueData>
  {
    private SqlColumnBinder QueueIdColumn = new SqlColumnBinder("QueueId");
    private SqlColumnBinder QueueNameColumn = new SqlColumnBinder("QueueName");
    private SqlColumnBinder MessageIdColumn = new SqlColumnBinder("MessageId");
    private SqlColumnBinder MessageColumn = new SqlColumnBinder("Message");

    protected override TfsmqDequeueData Bind() => new TfsmqDequeueData()
    {
      QueueId = this.QueueIdColumn.GetInt32((IDataReader) this.Reader),
      QueueName = this.QueueNameColumn.GetString((IDataReader) this.Reader, false),
      MessageId = this.MessageIdColumn.GetInt64((IDataReader) this.Reader),
      MessageString = this.MessageColumn.GetString((IDataReader) this.Reader, false)
    };
  }
}
