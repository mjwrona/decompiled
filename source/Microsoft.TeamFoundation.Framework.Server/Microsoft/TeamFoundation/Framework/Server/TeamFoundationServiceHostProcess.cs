// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostProcess
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TeamFoundationServiceHostProcess
  {
    public Guid MachineId { get; set; }

    public string MachineName { get; set; }

    public Guid ProcessId { get; set; }

    public string ProcessName { get; set; }

    public int OSProcessId { get; set; }

    public string ProcessIdentity { get; set; }

    public DateTime StartTime { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "[MachineId={0}, MachineName={1}, ProcessId={2}, ProcessName={3}, OSProcessId={4}, ProcessIdentity={5}, StartTime={6}]", (object) this.MachineId, (object) this.MachineName, (object) this.ProcessId, (object) this.ProcessName, (object) this.OSProcessId, (object) this.ProcessIdentity, (object) this.StartTime);
  }
}
