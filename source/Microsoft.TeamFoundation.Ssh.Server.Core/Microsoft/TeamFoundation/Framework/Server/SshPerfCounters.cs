// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SshPerfCounters
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class SshPerfCounters
  {
    private const string UriBase = "Microsoft.TeamFoundation.Ssh.";
    public const string BytesWrittenToClientPerSec = "Microsoft.TeamFoundation.Ssh.BytesWrittenToClientPerSec";
    public const string BytesReadFromClientPerSec = "Microsoft.TeamFoundation.Ssh.BytesReadFromClientPerSec";
    public const string ActiveConnections = "Microsoft.TeamFoundation.Ssh.ActiveConnections";
    public const string PrivateBytesPercent = "Microsoft.TeamFoundation.Ssh.PrivateBytesPercent";
    public const string PrivateBytesPercentBase = "Microsoft.TeamFoundation.Ssh.PrivateBytesPercentBase";
  }
}
