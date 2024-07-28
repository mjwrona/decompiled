// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Core.SshConstants
// Assembly: Microsoft.TeamFoundation.Ssh.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3DF8FBEE-AA1B-4659-8650-E7C7E1E085EB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.Core.dll

namespace Microsoft.TeamFoundation.Ssh.Server.Core
{
  public static class SshConstants
  {
    public const string SshLegacyStrongBoxDrawerName = "SshSecrets";
    public const string SshHostPrivateKeyStrongBoxItemName = "SshHostPrivateKey";
    public const int DefaultPort = 22;
    public const string OnPremSshServiceName = "TeamFoundationSshService";
    public const string HostedSshServiceName = "TeamFoundationSshService-Hosted";
    public const string ForwardedSshClientSoftwareName = "VSTS_FORWARDED_SSH_GIT_COMMAND_CLIENT";
    public const string TraceArea = "Ssh";

    public static class SshRsaDeprecation
    {
      public const int DefaultSshRsaDelayMilliseconds = 5000;
      public const string FailRequestScheduleDateFormat = "yyyyMMdd";
    }
  }
}
