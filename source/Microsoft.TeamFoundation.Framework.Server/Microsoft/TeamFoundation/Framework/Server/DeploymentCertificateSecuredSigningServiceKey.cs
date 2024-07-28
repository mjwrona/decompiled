// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeploymentCertificateSecuredSigningServiceKey
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DeploymentCertificateSecuredSigningServiceKey : StrongboxSecuredSigningServiceKey
  {
    internal DeploymentCertificateSecuredSigningServiceKey(Guid identifier)
      : base(SigningKeyType.DeploymentCertificateSecured, identifier)
    {
    }

    protected override IVssRequestContext GetKeyTargetRequestContext(
      IVssRequestContext requestContext)
    {
      return requestContext.To(TeamFoundationHostType.Deployment).Elevate();
    }

    protected string CertificateLookupKey(byte[] thumbprintBytes) => FrameworkServerConstants.MigrationCertificateItemName(Encoding.ASCII.GetString(thumbprintBytes));

    protected StrongBoxItemInfo GetCertificateItemByThumbprint(
      IVssRequestContext requestContext,
      byte[] thumbprintBytes)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      return vssRequestContext.GetService<ITeamFoundationStrongBoxService>().GetItemInfo(vssRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, this.CertificateLookupKey(thumbprintBytes), true);
    }

    internal X509Certificate2 LoadCertificate(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, FrameworkServerConstants.SigningServiceStrongBoxDrawerName, true);
      return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, this.CertificateLookupKey(this.KeyData), true);
    }

    protected override byte[] GetKeyData(IVssRequestContext requestContext) => this.LoadCertificate(requestContext).Export(X509ContentType.Pfx);

    protected override void StoreKeyData(
      IVssRequestContext requestContext,
      byte[] rawKeyData,
      bool overwriteExisting)
    {
      this.CheckKeyDataIsNull();
      this.GetCertificateItemByThumbprint(requestContext, rawKeyData);
      this.KeyData = this.StoreKeyDataToDatabase(requestContext, rawKeyData, overwriteExisting).KeyData;
    }

    protected override void DeleteKeyData(IVssRequestContext requestContext)
    {
    }

    protected override ISigner GetKeySigner(
      IVssRequestContext requestContext,
      SigningAlgorithm paddingAlgorithm)
    {
      return SigningManager.GetSigner(this.LoadCertificate(requestContext), paddingAlgorithm);
    }

    public string Thumbprint
    {
      get
      {
        this.CheckKeyDataIsNotNull();
        return Encoding.ASCII.GetString(this.KeyData);
      }
    }
  }
}
