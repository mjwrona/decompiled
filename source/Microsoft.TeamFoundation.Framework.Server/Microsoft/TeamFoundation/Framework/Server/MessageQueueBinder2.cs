// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageQueueBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class MessageQueueBinder2 : ObjectBinder<TeamFoundationMessageQueue>
  {
    private SqlColumnBinder QueueIdColumn = new SqlColumnBinder("QueueId");
    private SqlColumnBinder QueueNameColumn = new SqlColumnBinder("QueueName");
    private SqlColumnBinder QueueStatusColumn = new SqlColumnBinder("QueueStatus");
    private SqlColumnBinder SequenceIdColumn = new SqlColumnBinder("SequenceId");
    private SqlColumnBinder DateLastConnectedColumn = new SqlColumnBinder("DateLastConnected");

    protected override TeamFoundationMessageQueue Bind() => new TeamFoundationMessageQueue()
    {
      Id = this.QueueIdColumn.GetInt32((IDataReader) this.Reader),
      Name = this.QueueNameColumn.GetString((IDataReader) this.Reader, false),
      Status = (MessageQueueStatus) this.QueueStatusColumn.GetByte((IDataReader) this.Reader),
      SequenceId = this.SequenceIdColumn.GetInt32((IDataReader) this.Reader),
      DateLastConnected = this.DateLastConnectedColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
