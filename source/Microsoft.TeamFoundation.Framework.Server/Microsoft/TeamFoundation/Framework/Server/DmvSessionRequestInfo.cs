// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DmvSessionRequestInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DmvSessionRequestInfo
  {
    public int SessionId { get; set; }

    public string SessionStatus { get; set; }

    public string RequestStatus { get; set; }

    public DateTime ConnectionTime { get; set; }

    public Guid ConnectionId { get; set; }

    public Guid ParentConnectionId { get; set; }

    public string HostName { get; set; }

    public DateTime LoginTime { get; set; }

    public string LoginName { get; set; }

    public DateTime RequestStartTime { get; set; }

    public int RequestCpuTime { get; set; }

    public int RequestTotalTime { get; set; }

    public string Content { get; set; }

    public string BlockingStmt { get; set; }

    public override string ToString() => string.Format("\nSession Id:             {0}\nSession Status:         {1}\nRequest Status:         {2}\nConnection Time:        {3}\nConnection Id:          {4}\nParent Connection Id:   {5}\nHost Name:              {6}\nLog In Time:            {7}\nLog In Name:            {8}\nRequest start time:     {9}\nRequest CPU Time(ms):   {10}\nRequest Total Time(ms): {11}\nBlocking Statement:     \n{12}\nContent:                \n{13}", (object) this.SessionId, (object) this.SessionStatus, (object) this.RequestStatus, (object) this.ConnectionTime, (object) this.ConnectionId, (object) this.ParentConnectionId, (object) this.HostName, (object) this.LoginTime, (object) this.LoginName, (object) this.RequestStartTime, (object) this.RequestCpuTime, (object) this.RequestTotalTime, (object) this.BlockingStmt, (object) this.Content);
  }
}
