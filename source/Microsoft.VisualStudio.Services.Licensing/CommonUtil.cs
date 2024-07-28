// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.CommonUtil
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public static class CommonUtil
  {
    private const string s_fallbackCertificateThumbprint = "E6F5EA4EBB3D3E4EF245808B36AAAF69B8E8EF80";
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "BusinessLogic";

    public static string GetCurrentMethodName([CallerMemberName] string caller = null) => caller;

    public static Dictionary<Guid, License> ToLicenseDictionary(
      this IList<AccountEntitlement> accountEntitlements)
    {
      return accountEntitlements.GroupBy<AccountEntitlement, Guid>((Func<AccountEntitlement, Guid>) (g => g.UserId)).ToDictionary<IGrouping<Guid, AccountEntitlement>, Guid, License>((Func<IGrouping<Guid, AccountEntitlement>, Guid>) (g => g.Key), (Func<IGrouping<Guid, AccountEntitlement>, License>) (g => g.FirstOrDefault<AccountEntitlement>()?.License));
    }

    public static long ToUnixTime(this DateTime date)
    {
      DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return Convert.ToInt64((date.ToUniversalTime() - dateTime).TotalSeconds);
    }

    public static X509Certificate2 InstantiateCertificate(
      IVssRequestContext requestContext,
      string certificateThumbprint)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(certificateThumbprint, nameof (certificateThumbprint));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, "LicensingCertificates", false);
      if (!(drawerId == Guid.Empty))
        return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, certificateThumbprint);
      requestContext.Trace(1033000, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", LicensingResources.CertificateDrawerNotFound((object) "LicensingCertificates"));
      return CommonUtil.InstantiateCertificateFromCertStore(requestContext, certificateThumbprint);
    }

    public static X509Certificate2 GetCertificateFromConfigurationSecretsDrawerInStrongBox(
      IVssRequestContext requestContext,
      string lookupKey)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      Guid drawerId = service.UnlockDrawer(vssRequestContext, FrameworkServerConstants.ConfigurationSecretsDrawerName, false);
      if (drawerId == Guid.Empty)
      {
        requestContext.TraceAlways(1033001, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", LicensingResources.CertificateDrawerNotFound((object) FrameworkServerConstants.ConfigurationSecretsDrawerName));
        return (X509Certificate2) null;
      }
      if (service.GetItemInfo(vssRequestContext, drawerId, lookupKey, false) != null)
        return service.RetrieveFileAsCertificate(vssRequestContext, drawerId, lookupKey);
      requestContext.TraceAlways(1033002, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Certificate with lookupkey: " + lookupKey + " not found in drawer: " + FrameworkServerConstants.ConfigurationSecretsDrawerName);
      return (X509Certificate2) null;
    }

    private static X509Certificate2 InstantiateCertificateFromCertStore(
      IVssRequestContext requestContext,
      string certificateThumbprint)
    {
      CertHandler certHandler = new CertHandler();
      X509Certificate2 certificateByThumbprint1 = certHandler.FindCertificateByThumbprint(certificateThumbprint);
      if (certificateByThumbprint1 != null)
        return certificateByThumbprint1;
      requestContext.Trace(1033010, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", LicensingResources.CertificateNotFound((object) certificateThumbprint));
      X509Certificate2 certificateByThumbprint2 = certHandler.FindCertificateByThumbprint("E6F5EA4EBB3D3E4EF245808B36AAAF69B8E8EF80");
      if (certificateByThumbprint2 == null)
      {
        requestContext.Trace(1033011, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", LicensingResources.CertificateNotFound((object) "E6F5EA4EBB3D3E4EF245808B36AAAF69B8E8EF80"));
        throw new LicensingCertificateException(LicensingResources.CertificateNotFound((object) certificateThumbprint));
      }
      return certificateByThumbprint2;
    }

    public static void ValidateOfferSubscriptions(
      IVssRequestContext requestContext,
      IEnumerable<IOfferSubscription> offerSubscriptions)
    {
      if (offerSubscriptions == null || offerSubscriptions.Any<IOfferSubscription>((Func<IOfferSubscription, bool>) (o => o.OfferMeter == (OfferMeter) null)))
      {
        requestContext.TraceDataConditionally(1034286, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Offer subscription returned with null offer meter", methodName: nameof (ValidateOfferSubscriptions));
        throw new FailedToGetOfferSubscriptionsException();
      }
    }
  }
}
