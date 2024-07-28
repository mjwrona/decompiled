// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServiceVersionInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DebuggerDisplay("Version: {Version}, MinVersion: {MinVersion}")]
  public struct ServiceVersionInfo
  {
    private readonly int m_version;
    private readonly int m_minVersion;

    internal ServiceVersionInfo(int version, int minVersion)
    {
      this.m_version = version;
      this.m_minVersion = minVersion;
    }

    public int Version => this.m_version;

    public int MinVersion => this.m_minVersion;
  }
}
