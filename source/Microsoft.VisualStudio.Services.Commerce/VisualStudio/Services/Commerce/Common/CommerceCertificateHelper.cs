// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.CommerceCertificateHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Commerce.Common
{
  public class CommerceCertificateHelper
  {
    private const string Area = "Commerce";
    private const string Layer = "CommerceCertificateHelper";

    public X509Certificate2 InstantiateCertificate(
      IVssRequestContext requestContext,
      string certificateThumbprint)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(certificateThumbprint, nameof (certificateThumbprint));
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, "AzureBillingService", false);
      if (!(drawerId == Guid.Empty))
        return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, certificateThumbprint);
      requestContext.Trace(5106755, TraceLevel.Error, "Commerce", nameof (CommerceCertificateHelper), "Null certificate drawer");
      return this.InstantiateCertificateFromCertStore(requestContext, certificateThumbprint);
    }

    private X509Certificate2 InstantiateCertificateFromCertStore(
      IVssRequestContext requestContext,
      string certificateThumbprint)
    {
      X509Certificate2 certificateByThumbprint = new CertHandler().FindCertificateByThumbprint(certificateThumbprint);
      if (certificateByThumbprint != null)
        return certificateByThumbprint;
      requestContext.Trace(1033010, TraceLevel.Error, "Commerce", nameof (CommerceCertificateHelper), "Certificate not present in local store");
      return certificateByThumbprint;
    }
  }
}
