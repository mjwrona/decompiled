// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.MachineInfo
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Net;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class MachineInfo
  {
    static MachineInfo() => MachineInfo.Local = new MachineInfo(Dns.GetHostName());

    public MachineInfo(string machineName) => this.MachineName = machineName;

    public string MachineName { get; private set; }

    public static MachineInfo Local { get; private set; }
  }
}
