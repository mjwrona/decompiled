// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceFlagStatus
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TraceFlagStatus
  {
    public TraceFlagStatus(short traceFlag, short status, short global, short session)
    {
      this.TraceFlag = traceFlag;
      this.Status = status;
      this.Global = global;
      this.Session = session;
    }

    public short TraceFlag { get; }

    public short Status { get; }

    public short Global { get; }

    public short Session { get; }
  }
}
