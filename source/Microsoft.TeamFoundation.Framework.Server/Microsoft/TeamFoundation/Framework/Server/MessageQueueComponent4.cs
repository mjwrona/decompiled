// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageQueueComponent4
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class MessageQueueComponent4 : MessageQueueComponent3
  {
    public override void EmptyMessageQueue(string queueName)
    {
      this.TraceEnter(0, nameof (EmptyMessageQueue));
      this.PrepareStoredProcedure("prc_EmptyMessageQueue");
      this.BindString("@queueName", queueName, 128, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      this.TraceLeave(0, nameof (EmptyMessageQueue));
    }
  }
}
