// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConnectionNotificationBinder2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ConnectionNotificationBinder2 : ObjectBinder<TfsmqConnectionNotification>
  {
    private SqlColumnBinder QueueIdColumn = new SqlColumnBinder("QueueId");
    private SqlColumnBinder QueueNameColumn = new SqlColumnBinder("QueueName");
    private SqlColumnBinder SequenceIdColumn = new SqlColumnBinder("SequenceId");
    private SqlColumnBinder SessionIdColumn = new SqlColumnBinder("SessionId");
    private SqlColumnBinder DateLastConnectedColumn = new SqlColumnBinder("DateLastConnected");
    private SqlColumnBinder OldConnectionStatusColumn = new SqlColumnBinder("OldConnectionStatus");
    private SqlColumnBinder NewConnectionStatusColumn = new SqlColumnBinder("NewConnectionStatus");

    protected override TfsmqConnectionNotification Bind()
    {
      int int32_1 = this.QueueIdColumn.GetInt32((IDataReader) this.Reader);
      string str = this.QueueNameColumn.GetString((IDataReader) this.Reader, false);
      int int32_2 = this.SequenceIdColumn.GetInt32((IDataReader) this.Reader);
      Guid guid = this.SessionIdColumn.GetGuid((IDataReader) this.Reader, true);
      DateTime dateTime = this.DateLastConnectedColumn.GetDateTime((IDataReader) this.Reader);
      MessageQueueStatus messageQueueStatus1 = (MessageQueueStatus) this.OldConnectionStatusColumn.GetByte((IDataReader) this.Reader);
      MessageQueueStatus messageQueueStatus2 = (MessageQueueStatus) this.NewConnectionStatusColumn.GetByte((IDataReader) this.Reader);
      string queueName = str;
      int sequenceId = int32_2;
      Guid sessionId = guid;
      DateTime dateLastConnected = dateTime;
      int oldConnectionStatus = (int) messageQueueStatus1;
      int newConnectionStatus = (int) messageQueueStatus2;
      return new TfsmqConnectionNotification(int32_1, queueName, sequenceId, sessionId, dateLastConnected, (MessageQueueStatus) oldConnectionStatus, (MessageQueueStatus) newConnectionStatus);
    }
  }
}
