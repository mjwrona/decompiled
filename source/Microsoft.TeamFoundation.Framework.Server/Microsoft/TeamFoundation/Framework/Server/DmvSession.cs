// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DmvSession
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DmvSession
  {
    public int SessionId { get; set; }

    public string SessionStatus { get; set; }

    public DateTime LoginTime { get; set; }

    public string LoginName { get; set; }

    public string HostName { get; set; }

    public string ProgramName { get; set; }

    public int HostProcessId { get; set; }

    public override string ToString() => string.Format("\nSession Id:             {0}\nSession Status:         {1}\nLog In Time:            {2}\nLog In Name:            {3}\nHost Name:              {4}\nProgram Name:           {5}\nHost Process Id:        {6}", (object) this.SessionId, (object) this.SessionStatus, (object) this.LoginTime, (object) this.LoginName, (object) this.HostName, (object) this.ProgramName, (object) this.HostProcessId);
  }
}
