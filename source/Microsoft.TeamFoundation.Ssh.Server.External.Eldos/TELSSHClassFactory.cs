// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.TELSSHClassFactory
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Ssh.Server.Core;
using SBSSHCommon;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal static class TELSSHClassFactory
  {
    public static void SetKexInitOptions(
      KexInitOptions kexInitOptions,
      TElSSHClass sshClass,
      Action<int, TraceLevel, string, string> traceAction)
    {
      TELSSHClassFactory.DoApplyTo(kexInitOptions.kex_algorithms, SBSSHConstants.__Global.SSH2KexStrings, new Action<short, bool>(sshClass.set_KexAlgorithms), new Action<short, int>(sshClass.set_KexAlgorithmPriorities), "kex_algorithms", traceAction);
      TELSSHClassFactory.DoApplyTo(kexInitOptions.server_host_key_algorithms, SBSSHConstants.__Global.SSH2PublicStrings, new Action<short, bool>(sshClass.set_PublicKeyAlgorithms), new Action<short, int>(sshClass.set_PublicKeyAlgorithmPriorities), "server_host_key_algorithms", traceAction);
      TELSSHClassFactory.DoApplyTo(kexInitOptions.encryption_algorithms, SBSSHConstants.__Global.SSH2CipherStrings, new Action<short, bool>(sshClass.set_EncryptionAlgorithms), new Action<short, int>(sshClass.set_EncryptionAlgorithmPriorities), "encryption_algorithms", traceAction);
      TELSSHClassFactory.DoApplyTo(kexInitOptions.mac_algorithms, SBSSHConstants.__Global.SSH2MacStrings, new Action<short, bool>(sshClass.set_MacAlgorithms), new Action<short, int>(sshClass.set_MacAlgorithmPriorities), "mac_algorithms", traceAction);
      TELSSHClassFactory.DoApplyTo(kexInitOptions.compression_algorithms, SBSSHConstants.__Global.SSH2CompStrings, new Action<short, bool>(sshClass.set_CompressionAlgorithms), new Action<short, int>(sshClass.set_CompressionAlgorithmPriorities), "compression_algorithms", traceAction);
    }

    private static void DoApplyTo(
      string names,
      string[] indexesToNames,
      Action<short, bool> setEnabled,
      Action<short, int> setPriority,
      string algorithmsScope,
      Action<int, TraceLevel, string, string> traceAction)
    {
      for (short index = 0; (int) index < indexesToNames.Length; ++index)
        setEnabled(index, false);
      string[] strArray = names.Split(',');
      int length = strArray.Length;
      foreach (string str in strArray)
      {
        bool flag = false;
        for (short index = 0; (int) index < indexesToNames.Length; ++index)
        {
          if (indexesToNames[(int) index] == str)
          {
            setEnabled(index, true);
            setPriority(index, length);
            flag = true;
            traceAction(13001162, TraceLevel.Info, "Ssh", string.Format("KexInit: {0} is choosed for {1} with {2} priority", (object) str, (object) algorithmsScope, (object) length));
            break;
          }
        }
        if (!flag)
          throw new InvalidOperationException(str + " not found in indexesToNames");
        --length;
      }
    }
  }
}
