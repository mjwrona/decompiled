// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DmvBlockedSessionInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DmvBlockedSessionInfo
  {
    public int SessionId { get; set; }

    public int BlockingSessionId { get; set; }

    public string SessionStatus { get; set; }

    public string RequestStatus { get; set; }

    public DateTime StartTime { get; set; }

    public int ElapsedMilliseconds { get; set; }

    public string Command { get; set; }

    public string WaitType { get; set; }

    public int WaitTime { get; set; }

    public string WaitResource { get; set; }

    public string Content { get; set; }

    public override string ToString() => string.Format("\nSession Id:             {0}\nBlocking Session Id:    {1}\nSession Status:         {2}\nRequest Status:         {3}\nStart Time:             {4}\nElapsed Milliseconds:   {5}\nCommand:                {6}\nWait Type:              {7}\nWait Time:              {8}\nWait Resource:          {9}\nContent:                {10}", (object) this.SessionId, (object) this.BlockingSessionId, (object) this.SessionStatus, (object) this.RequestStatus, (object) this.StartTime, (object) this.ElapsedMilliseconds, (object) this.Command, (object) this.WaitType, (object) this.WaitTime, (object) this.WaitResource, (object) this.Content);
  }
}
