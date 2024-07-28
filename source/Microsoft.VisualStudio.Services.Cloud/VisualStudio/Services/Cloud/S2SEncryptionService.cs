// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.S2SEncryptionService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class S2SEncryptionService : IS2SEncryptionService, IVssFrameworkService
  {
    private const string c_certLookupName = "S2SEncryptionCertificate";

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.CheckRequestContext(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string Encrypt(IVssRequestContext requestContext, SecureString unencryptedData)
    {
      this.CheckRequestContext(requestContext);
      ArgumentUtility.CheckForNull<SecureString>(unencryptedData, nameof (unencryptedData));
      RSACryptoServiceProvider key = (RSACryptoServiceProvider) this.GetCertificate(requestContext, false).PublicKey.Key;
      byte[] rgb = (byte[]) null;
      try
      {
        rgb = unencryptedData.ToByteArray();
        return Convert.ToBase64String(key.Encrypt(rgb, true));
      }
      finally
      {
        if (rgb != null)
          Array.Clear((Array) rgb, 0, rgb.Length);
      }
    }

    public SecureString Decrypt(IVssRequestContext requestContext, string encryptedData)
    {
      this.CheckRequestContext(requestContext);
      ArgumentUtility.CheckStringForNullOrEmpty(encryptedData, nameof (encryptedData));
      RSACryptoServiceProvider privateKey = (RSACryptoServiceProvider) this.GetCertificate(requestContext, true).PrivateKey;
      byte[] bytes = (byte[]) null;
      try
      {
        byte[] rgb = Convert.FromBase64String(encryptedData);
        bytes = privateKey.Decrypt(rgb, true);
        return bytes.ToSecureString();
      }
      finally
      {
        if (bytes != null)
          Array.Clear((Array) bytes, 0, bytes.Length);
      }
    }

    private X509Certificate2 GetCertificate(
      IVssRequestContext requestContext,
      bool expectPrivateKey)
    {
      TeamFoundationStrongBoxService service = requestContext.GetService<TeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(requestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, true);
      StrongBoxItemInfo itemInfo = service.GetItemInfo(requestContext, drawerId, "S2SEncryptionCertificate");
      return service.RetrieveFileAsCertificate(requestContext, itemInfo, expectPrivateKey: expectPrivateKey);
    }

    private void CheckRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckSystemRequestContext();
      requestContext.CheckHostedDeployment();
    }
  }
}
