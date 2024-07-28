// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshRegistryConstants
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public static class SshRegistryConstants
  {
    public const string SshServerRoot = "/Configuration/SshServer/";
    public const string SshPort = "/Configuration/SshServer/Port";
    public const string SshEnabled = "/Configuration/SshServer/Enabled";

    public class SshRsaDeprecation
    {
      public const string SshRsaDeprecationRoot = "/Configuration/SshServer/SshRsaDeprecation/";
      public const string SshRsaDelayMilliseconds = "/Configuration/SshServer/SshRsaDeprecation/SshRsaDelayMilliseconds";
      public const string FailRequestScheduleStringFormat = "/Configuration/SshServer/SshRsaDeprecation/FailRequestSchedule/{0}";
    }
  }
}
