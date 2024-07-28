// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.TeamFoundationSshKeyService
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using SBSSHKeyStorage;
using SBX509;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public class TeamFoundationSshKeyService : IVssFrameworkService
  {
    private TElSSHMemoryKeyStorage m_keys;
    private static readonly string s_Area = "ssh";
    private static readonly string s_Layer = nameof (TeamFoundationSshKeyService);

    static TeamFoundationSshKeyService() => EldosInitializer.Init();

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      using (this.m_keys)
        this.m_keys = (TElSSHMemoryKeyStorage) null;
    }

    public string GetRsaKeyFingerprint(IVssRequestContext requestContext) => this.GetSshServerHostKey(requestContext).get_Keys(0).FingerprintMD5String;

    public virtual string GetSha1KeyFingerprint(IVssRequestContext requestContext) => this.GetSshServerHostKey(requestContext).get_Keys(0).FingerprintSHA1String;

    public TElSSHMemoryKeyStorage GetSshServerHostKey(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      if (this.m_keys == null)
      {
        TElSSHMemoryKeyStorage memoryKeyStorage = new TElSSHMemoryKeyStorage();
        TElSSHKey Key = this.RetrieveSshServerHostKey(requestContext);
        memoryKeyStorage.Add(Key);
        if (Interlocked.CompareExchange<TElSSHMemoryKeyStorage>(ref this.m_keys, memoryKeyStorage, (TElSSHMemoryKeyStorage) null) != null)
          memoryKeyStorage.Dispose();
      }
      return this.m_keys;
    }

    public void SetSshServerHostKey(
      IVssRequestContext deploymentRequestContext,
      bool replaceExistingkey,
      ITFLogger logger)
    {
      bool flag = true;
      if (!replaceExistingkey && this.RetrieveSshServerHostKey(deploymentRequestContext) != null)
        flag = false;
      if (!flag)
        return;
      TElSSHKey serverHostPrivateKey = this.GenerateSshServerHostPrivateKey(deploymentRequestContext, logger);
      this.UploadKeyToStrongboxOnly(deploymentRequestContext, serverHostPrivateKey);
    }

    public void UploadKeyToStrongboxOnly(
      IVssRequestContext deploymentRequestContext,
      TElSSHKey sshKey)
    {
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext, "ConfigurationSecrets", false);
      if (drawerId.Equals(Guid.Empty))
        drawerId = service.CreateDrawer(deploymentRequestContext, "ConfigurationSecrets");
      service.AddString(deploymentRequestContext, drawerId, "SshHostPrivateKey", this.KeyToString(sshKey));
    }

    public string KeyToString(TElSSHKey keyToSet)
    {
      byte[] Buffer = (byte[]) null;
      try
      {
        int Size = 0;
        keyToSet.SavePrivateKey(ref Buffer, ref Size);
        Buffer = new byte[Size];
        keyToSet.SavePrivateKey(ref Buffer, ref Size);
        return Convert.ToBase64String(Buffer);
      }
      finally
      {
        if (Buffer != null)
          Array.Clear((Array) Buffer, 0, Buffer.Length);
      }
    }

    public TElSSHKey RetrieveSshServerHostKey(IVssRequestContext deploymentRequestContext)
    {
      byte[] Buffer = (byte[]) null;
      try
      {
        TElSSHKey telSshKey = new TElSSHKey();
        ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(deploymentRequestContext, "ConfigurationSecrets", "SshHostPrivateKey", false);
        if (itemInfo == null)
          return (TElSSHKey) null;
        string s = service.GetString(deploymentRequestContext, itemInfo);
        if (string.IsNullOrEmpty(s))
          return (TElSSHKey) null;
        Buffer = Convert.FromBase64String(s);
        int loadKeyResult = telSshKey.LoadPrivateKey(Buffer, Buffer.Length, string.Empty);
        return loadKeyResult == 0 || telSshKey.IsKeyValid() ? telSshKey : throw new SshKeyInvalidException(loadKeyResult);
      }
      finally
      {
        if (Buffer != null)
          Array.Clear((Array) Buffer, 0, Buffer.Length);
      }
    }

    public TElSSHKey RetrieveLegacySshServerHostKey(IVssRequestContext deploymentRequestContext)
    {
      ITeamFoundationStrongBoxService service = deploymentRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(deploymentRequestContext, "SshSecrets", false);
      if (drawerId == Guid.Empty)
        return (TElSSHKey) null;
      byte[] numArray = (byte[]) null;
      TElSSHKey telSshKey = new TElSSHKey();
      try
      {
        long streamLength;
        using (Stream stream = service.RetrieveFile(deploymentRequestContext, drawerId, "SshHostPrivateKey", out streamLength))
        {
          numArray = streamLength != 0L && streamLength <= 50000L ? new byte[streamLength] : throw new SshKeyInvalidException(string.Format("Specified key is not a valid SSH key. Length: {0}", (object) streamLength));
          stream.Read(numArray, 0, numArray.Length);
        }
        int loadKeyResult = telSshKey.LoadPrivateKey(numArray, numArray.Length, string.Empty);
        if (loadKeyResult != 0)
          throw new SshKeyInvalidException(loadKeyResult);
      }
      finally
      {
        if (numArray != null)
          Array.Clear((Array) numArray, 0, numArray.Length);
      }
      return telSshKey;
    }

    public TElSSHKey GenerateSshServerHostPrivateKey(
      IVssRequestContext requestContext,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      CertHandler certHandler = new CertHandler(logger);
      string environmentVariable = Environment.GetEnvironmentVariable("TFS_SSH_CERT_THUMBPRINT");
      X509Certificate2 x509Cert = (X509Certificate2) null;
      if (!string.IsNullOrEmpty(environmentVariable))
      {
        logger.Info("{0} : {1}", (object) "TFS_SSH_CERT_THUMBPRINT", (object) environmentVariable);
        x509Cert = certHandler.FindCertificateByThumbprint(environmentVariable);
        if (x509Cert == null)
          logger.Warning("Certificate with thumbprint {0} not found.", (object) environmentVariable);
      }
      if (x509Cert == null)
      {
        logger.Info("Creating new Cert for Ssh service.");
        X509CertGenerator x509CertGenerator = new X509CertGenerator(logger);
        try
        {
          x509Cert = x509CertGenerator.GenerateOAuthSigningCertificate();
        }
        catch (Exception ex)
        {
          logger.Warning("certGenerator.GenerateOAuthSigningCertificate failed. The following error was reported: " + ex.Message + ".");
          x509Cert = certHandler.MakeOAuthSigningCertificate();
        }
      }
      return this.SshKeyFromCert(requestContext, x509Cert) ?? throw new VssServiceException("Couldn't generate an SSH key.");
    }

    private TElSSHKey SshKeyFromCert(IVssRequestContext requestContext, X509Certificate2 x509Cert)
    {
      TElSSHKey telSshKey = new TElSSHKey();
      using (TElX509Certificate telX509Certificate = new TElX509Certificate())
      {
        telX509Certificate.FromX509Certificate2(x509Cert);
        byte[] Buffer = (byte[]) null;
        try
        {
          if (!telX509Certificate.SaveKeyToBufferPEM(out Buffer, string.Empty))
          {
            requestContext.Trace(13001001, TraceLevel.Error, TeamFoundationSshKeyService.s_Area, TeamFoundationSshKeyService.s_Layer, "SaveKeyToBufferPEM call failed.");
            return (TElSSHKey) null;
          }
          int num = telSshKey.LoadPrivateKey(Buffer, Buffer.Length);
          if (num != 0)
          {
            requestContext.Trace(13001002, TraceLevel.Error, TeamFoundationSshKeyService.s_Area, TeamFoundationSshKeyService.s_Layer, string.Format("LoadPrivateKey call failed. loadKeyResult: {0}.", (object) num));
            return (TElSSHKey) null;
          }
        }
        finally
        {
          if (Buffer != null)
            Array.Clear((Array) Buffer, 0, Buffer.Length);
        }
      }
      return telSshKey;
    }

    private string GetServerPublicKey(TElSSHKey sshHostKey, ITFLogger logger)
    {
      byte[] Buffer1 = (byte[]) null;
      int Size = 0;
      sshHostKey.SavePublicKey(ref Buffer1, ref Size);
      byte[] Buffer2 = new byte[Size];
      sshHostKey.SavePublicKey(ref Buffer2, ref Size);
      string[] strArray = Encoding.ASCII.GetString(Buffer2).Split(' ');
      if (strArray.Length >= 2)
        return strArray[1];
      logger.Error("couldn't retrieve the public key to compute fingerprint");
      return (string) null;
    }

    private string GetSha256Fingerprint(TElSSHKey sshHostKey, ITFLogger logger)
    {
      using (SHA256 shA256 = SHA256.Create())
      {
        byte[] buffer = Convert.FromBase64String(this.GetServerPublicKey(sshHostKey, logger));
        return Convert.ToBase64String(shA256.ComputeHash(buffer)).TrimEnd('=');
      }
    }

    public string GetSha256Fingerprint(IVssRequestContext requestContext)
    {
      using (TElSSHKey sshHostKey = this.RetrieveSshServerHostKey(requestContext))
        return sshHostKey == null ? (string) null : this.GetSha256Fingerprint(sshHostKey, (ITFLogger) new NullLogger());
    }

    public string GetMD5Fingerprint(IVssRequestContext requestContext)
    {
      using (TElSSHKey telSshKey = this.RetrieveSshServerHostKey(requestContext))
        return telSshKey?.FingerprintMD5String;
    }
  }
}
