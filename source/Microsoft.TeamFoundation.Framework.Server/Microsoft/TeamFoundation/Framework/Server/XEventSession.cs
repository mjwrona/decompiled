// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.XEventSession
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class XEventSession
  {
    public XEventSession(
      int sessionId,
      string sessionName,
      bool isEventFileTruncated,
      int buffersLogged,
      int buffersDropped,
      string eventFileName)
    {
      this.SessionId = sessionId;
      this.SessionName = sessionName;
      this.IsEventFileTruncated = isEventFileTruncated;
      this.BuffersLogged = buffersLogged;
      this.BuffersDropped = buffersDropped;
      this.EventFileName = eventFileName;
    }

    public int SessionId { get; }

    public string SessionName { get; }

    public bool IsEventFileTruncated { get; }

    public int BuffersLogged { get; }

    public int BuffersDropped { get; }

    public string EventFileName { get; }
  }
}
