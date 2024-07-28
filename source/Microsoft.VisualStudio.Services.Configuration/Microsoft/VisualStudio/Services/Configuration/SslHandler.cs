// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.SslHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class SslHandler
  {
    private readonly ITFLogger m_logger;
    private const string c_sslBindingInfoKeyFormat = "SYSTEM\\CurrentControlSet\\Services\\HTTP\\Parameters\\SslBindingInfo\\{0}:{1}";
    private const string c_sslSniBindingInfoKey = "SYSTEM\\CurrentControlSet\\Services\\HTTP\\Parameters\\SslSniBindingInfo";

    public SslHandler(ITFLogger logger) => this.m_logger = logger ?? (ITFLogger) new NullLogger();

    public void AddSslSniCertificateBinding(
      string host,
      int port,
      string certThumbprint,
      string certStoreName,
      Guid appId)
    {
      this.m_logger.Info("Creating an SSL server certificate binding");
      ArgumentUtility.CheckStringForNullOrEmpty(host, nameof (host));
      string args = string.Format("http add sslcert hostnameport={0}:{1} certhash={2} appid={3}", (object) host, (object) port, (object) certThumbprint, (object) appId.ToString("B"));
      if (!string.IsNullOrEmpty(certStoreName))
        args = args + " certstorename=" + certStoreName;
      if (this.ExecuteNetsh(args).ExitCode != 0)
        throw new SslServerCertificateException(ConfigurationResources.FailedToCreateSslSniServerCertificateBinding((object) host, (object) port));
    }

    public void AddSslCertificateBinding(
      string ipAddress,
      int port,
      string certThumbprint,
      string certStoreName,
      Guid appId)
    {
      this.m_logger.Info("Creating an SSL server certificate binding");
      ArgumentUtility.CheckStringForNullOrEmpty(ipAddress, nameof (ipAddress));
      SslBindingInfo sslBindingInfo = this.GetSslBindingInfo(ipAddress, port);
      if (sslBindingInfo != null)
      {
        if (!string.Equals(sslBindingInfo.SslCertHashHex, certThumbprint, StringComparison.OrdinalIgnoreCase))
          throw new SslServerCertificateException(ConfigurationResources.SslServerCertificateBindingIsAlreadyInUse((object) ipAddress, (object) port));
        this.m_logger.Info("SSL server certificate binding already exist.");
      }
      else
      {
        string args = string.Format("http add sslcert ipport={0}:{1} certhash={2} appid={3}", (object) ipAddress, (object) port, (object) certThumbprint, (object) appId.ToString("B"));
        if (!string.IsNullOrEmpty(certStoreName))
          args = args + " certstorename=" + certStoreName;
        if (this.ExecuteNetsh(args).ExitCode != 0)
          throw new SslServerCertificateException(ConfigurationResources.FailedToCreateSslServerCertificateBinding((object) ipAddress, (object) port));
      }
    }

    public void DeleteSslSniCertificateBinding(string hostName, int port)
    {
      this.m_logger.Info("Delete an SSL server certificate binding");
      ArgumentUtility.CheckStringForNullOrEmpty(hostName, nameof (hostName));
      if (this.GetSslSniBindingInfo(hostName, port) != null && this.ExecuteNetsh(string.Format("http delete sslcert hostnameport={0}:{1}", (object) hostName, (object) port)).ExitCode != 0)
        throw new SslServerCertificateException(ConfigurationResources.FailedToDeleteSslServerCertificateBinding((object) hostName, (object) port));
    }

    public void DeleteSslCertificateBinding(string ipAddress, int port)
    {
      this.m_logger.Info("Delete an SSL server certificate binding");
      ArgumentUtility.CheckStringForNullOrEmpty(ipAddress, nameof (ipAddress));
      if (this.GetSslBindingInfo(ipAddress, port) != null && this.ExecuteNetsh(string.Format("http delete sslcert ipport={0}:{1}", (object) ipAddress, (object) port)).ExitCode != 0)
        throw new SslServerCertificateException(ConfigurationResources.FailedToDeleteSslServerCertificateBinding((object) ipAddress, (object) port));
    }

    public SslBindingInfo GetSslBindingInfo(string ipAddress, int port)
    {
      this.m_logger.Info(string.Format("Getting Ssl binding info for {0}:{1}", (object) ipAddress, (object) port));
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SYSTEM\\CurrentControlSet\\Services\\HTTP\\Parameters\\SslBindingInfo\\{0}:{1}", (object) ipAddress, (object) port);
      using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, false))
      {
        if (registryKey == null)
          return (SslBindingInfo) null;
        SslBindingInfo sslBindingInfo = new SslBindingInfo();
        sslBindingInfo.IPAddress = ipAddress;
        sslBindingInfo.Port = port;
        sslBindingInfo.AppId = registryKey.GetValue("AppId") as byte[];
        sslBindingInfo.SslCertHash = registryKey.GetValue("SslCertHash") as byte[];
        sslBindingInfo.SslCertStoreName = registryKey.GetValue("SslCertStoreName") as string;
        return sslBindingInfo;
      }
    }

    public SslSniBindingInfo GetSslSniBindingInfo(string hostName, int port)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(hostName, nameof (hostName));
      string hostNamePort = string.Format("{0}:{1}", (object) hostName, (object) port);
      return ((IEnumerable<SslSniBindingInfo>) this.GetSslSniBindingInfos()).FirstOrDefault<SslSniBindingInfo>((Func<SslSniBindingInfo, bool>) (b => string.Equals(hostNamePort, b.HostnamePort, StringComparison.OrdinalIgnoreCase)));
    }

    public SslSniBindingInfo[] GetSslSniBindingInfos()
    {
      List<SslSniBindingInfo> sslSniBindingInfoList = new List<SslSniBindingInfo>();
      using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\HTTP\\Parameters\\SslSniBindingInfo", false))
      {
        if (registryKey1 == null)
          return Array.Empty<SslSniBindingInfo>();
        foreach (string subKeyName in registryKey1.GetSubKeyNames())
        {
          using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyName, false))
          {
            if (registryKey2.GetValue("HostnamePort") is string str)
            {
              SslSniBindingInfo sslSniBindingInfo1 = new SslSniBindingInfo();
              sslSniBindingInfo1.HostnamePort = str;
              sslSniBindingInfo1.AppId = registryKey2.GetValue("AppId") as byte[];
              sslSniBindingInfo1.SslCertHash = registryKey2.GetValue("SslCertHash") as byte[];
              sslSniBindingInfo1.SslCertStoreName = registryKey2.GetValue("SslCertStoreName") as string;
              SslSniBindingInfo sslSniBindingInfo2 = sslSniBindingInfo1;
              sslSniBindingInfoList.Add(sslSniBindingInfo2);
            }
          }
        }
      }
      return sslSniBindingInfoList.ToArray();
    }

    private ProcessOutput ExecuteNetsh(string args) => ProcessHandler.RunExe(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "netsh.exe"), args, this.m_logger);
  }
}
