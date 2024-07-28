// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.TElSSHServerFactory
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using SBCryptoProv;
using SBCryptoProvWin32;
using SBSSHCommon;
using SBSSHServer;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public static class TElSSHServerFactory
  {
    private static string s_SoftwareName = "SSHBlackbox.10";

    public static TElCustomCryptoProviderManager Win32DefaultCryptoProviderManager { get; set; }

    public static TElSSHServer Create(SshOptions options, ITraceRequest rawTraceRequest)
    {
      TElSSHServer sshClass = new TElSSHServer();
      sshClass.SoftwareName = TElSSHServerFactory.s_SoftwareName;
      sshClass.AuthenticationTypes = 6;
      if (TElSSHServerFactory.Win32DefaultCryptoProviderManager == null)
      {
        TElSSHServerFactory.Win32DefaultCryptoProviderManager = (TElCustomCryptoProviderManager) SBCryptoProvManager.__Global.DefaultCryptoProviderManager();
        TElWin32CryptoProviderOptions options1 = (TElWin32CryptoProviderOptions) SBCryptoProvWin32.__Global.Win32CryptoProvider().Options;
        options1.UseForSymmetricKeyOperations = true;
        options1.UseForHashingOperations = true;
        TElSSHServerFactory.Win32DefaultCryptoProviderManager.SetDefaultCryptoProvider(SBCryptoProvWin32.__Global.Win32CryptoProvider());
      }
      sshClass.CryptoProviderManager = TElSSHServerFactory.Win32DefaultCryptoProviderManager;
      sshClass.ForceCompression = false;
      sshClass.CloseIfNoActiveTunnels = true;
      sshClass.UseUTF8 = true;
      TELSSHClassFactory.SetKexInitOptions(options.KexInitOptions, (TElSSHClass) sshClass, (Action<int, TraceLevel, string, string>) ((tracepoint, traceLevel, area, message) => rawTraceRequest.Trace(tracepoint, traceLevel, area, nameof (TElSSHServerFactory), message)));
      return sshClass;
    }
  }
}
