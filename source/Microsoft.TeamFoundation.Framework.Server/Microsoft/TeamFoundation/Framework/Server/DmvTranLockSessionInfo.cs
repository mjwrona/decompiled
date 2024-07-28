// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DmvTranLockSessionInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DmvTranLockSessionInfo
  {
    public int SessionId { get; set; }

    public string LockStatus { get; set; }

    public string LockType { get; set; }

    public string LockMode { get; set; }

    public string LockDesc { get; set; }

    public string LockOwner { get; set; }

    public int DatabaseId { get; set; }

    public string RequestStatus { get; set; }

    public int BlockingSessionId { get; set; }

    public DateTime StartTime { get; set; }

    public int ElapsedMilliseconds { get; set; }

    public string Content { get; set; }

    public string Stmt { get; set; }

    public override string ToString() => string.Format("@SessionId={0}\n@LockStatus={1}\n@LockType={2}\n@LockMode={3}\n@LockDesc={4}\n@LockOwner={5}\n@DatabaseId={6}\n@requestStatus={7}\n@BlockingSession Id={8}\n@StartTime={9}\n@ElapsedMilliseconds={10}\n@Content={11}\n@Statement={12}", (object) this.SessionId, (object) this.LockStatus, (object) this.LockType, (object) this.LockMode, (object) this.LockDesc, (object) this.LockOwner, (object) this.DatabaseId, (object) this.RequestStatus, (object) this.BlockingSessionId, (object) this.StartTime, (object) this.ElapsedMilliseconds, (object) this.Content, (object) this.Stmt);
  }
}
