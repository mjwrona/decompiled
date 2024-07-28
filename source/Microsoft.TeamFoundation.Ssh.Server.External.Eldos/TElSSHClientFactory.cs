// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.TElSSHClientFactory
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using SBSSHClient;
using SBSSHCommon;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public static class TElSSHClientFactory
  {
    public static TElSSHClient Create(SshOptions options, ITraceRequest requestTracer)
    {
      TElSSHClient telSshClient = new TElSSHClient();
      telSshClient.ForceCompression = false;
      telSshClient.UseUTF8 = true;
      telSshClient.CloseIfNoActiveTunnels = true;
      telSshClient.Versions = (short) 2;
      telSshClient.SoftwareName = "VSTS_FORWARDED_SSH_GIT_COMMAND_CLIENT";
      telSshClient.DefaultWindowSize = 2097152;
      telSshClient.MinWindowSize = 1048576;
      TElSSHClient sshClass = telSshClient;
      TELSSHClassFactory.SetKexInitOptions(options.KexInitOptions, (TElSSHClass) sshClass, (Action<int, TraceLevel, string, string>) ((tracepoint, traceLevel, area, message) => requestTracer.Trace(tracepoint, traceLevel, area, nameof (TElSSHClientFactory), message)));
      return sshClass;
    }
  }
}
