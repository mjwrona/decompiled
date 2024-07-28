// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DelegatedAuthorizationCertificateManager
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Configuration;
using Microsoft.VisualStudio.Services.DelegatedAuthorization.Settings;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class DelegatedAuthorizationCertificateManager
  {
    public X509Certificate2 GetCertificate(
      IVssRequestContext requestContext,
      DelegatedAuthorizationSettings settings)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, settings.SigningCertificateDrawerName, false);
      if (!(drawerId == Guid.Empty))
        return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, settings.SigningCertificateThumbprint);
      return this.InstantiateCertificateFromStore(settings.SigningCertificateThumbprint) ?? throw new ArgumentException("Thumbprint not found.");
    }

    private X509Certificate2 InstantiateCertificateFromStore(string certificateThumbprint) => new CertHandler().FindCertificateByThumbprint(certificateThumbprint);
  }
}
