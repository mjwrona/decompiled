// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class StrongBoxExtensions
  {
    public static void AddCertificate(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      StrongBoxItemInfo item,
      X509Certificate2 cert)
    {
      byte[] buffer = cert.Export(X509ContentType.Pfx);
      try
      {
        MemoryStream content = new MemoryStream(buffer);
        item.ExpirationDate = new DateTime?(cert.NotAfter);
        strongBoxService.UploadFile(requestContext, item, (Stream) content);
      }
      finally
      {
        Array.Clear((Array) buffer, 0, buffer.Length);
      }
    }

    internal static IList<X509Certificate2> RetrieveFilesAsCertificate(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      string drawerName,
      IList<string> lookupKeys,
      bool throwOnFailure = true,
      bool exportable = false)
    {
      Guid drawerId = strongBoxService.UnlockDrawer(requestContext, drawerName, throwOnFailure);
      if (drawerId == Guid.Empty)
        return (IList<X509Certificate2>) new List<X509Certificate2>();
      List<X509Certificate2> x509Certificate2List = new List<X509Certificate2>();
      foreach (string lookupKey in (IEnumerable<string>) lookupKeys)
        x509Certificate2List.Add(strongBoxService.RetrieveFileAsCertificate(requestContext, drawerId, lookupKey, exportable, throwOnFailure));
      return (IList<X509Certificate2>) x509Certificate2List;
    }

    internal static X509Certificate2 EnsureCertificate(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      string certificateThumbprint,
      string drawerName,
      ITFLogger logger = null)
    {
      return strongBoxService.EnsureCertificate(requestContext, certificateThumbprint, drawerName, (Func<ITFLogger, string, X509Certificate2>) ((l, t) => StrongBoxExtensions.LoadCertificateFromStore(l, t, StoreLocation.LocalMachine)), logger);
    }

    internal static X509Certificate2 EnsureCertificate(
      this ITeamFoundationStrongBoxService strongBoxService,
      IVssRequestContext requestContext,
      string certificateThumbprint,
      string drawerName,
      Func<ITFLogger, string, X509Certificate2> certificateProvider,
      ITFLogger logger = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(certificateThumbprint, nameof (certificateThumbprint));
      StrongBoxExtensions.LogInfo(logger, "Ensuring Certificate with thumbprint '" + certificateThumbprint + "' is in the StrongBox.");
      StrongBoxExtensions.LogInfo(logger, "Attempting to unlock StrongBox drawer '{0}'.", (object) drawerName);
      Guid drawerId = strongBoxService.UnlockDrawer(requestContext, drawerName, false);
      if (drawerId != Guid.Empty)
      {
        StrongBoxExtensions.LogInfo(logger, "Unlocked StrongBox drawer with id = {0}", (object) drawerId);
        IList<StrongBoxItemInfo> drawerContents = (IList<StrongBoxItemInfo>) strongBoxService.GetDrawerContents(requestContext, drawerId);
        StrongBoxExtensions.LogInfo(logger, "Strong box drawer contains {0} items.", (object) drawerContents.Count);
        foreach (StrongBoxItemInfo strongBoxItemInfo in (IEnumerable<StrongBoxItemInfo>) drawerContents)
        {
          if (string.Equals(strongBoxItemInfo.LookupKey, certificateThumbprint, StringComparison.OrdinalIgnoreCase))
          {
            long streamLength;
            using (BinaryReader binaryReader = new BinaryReader(strongBoxService.RetrieveFile(requestContext, drawerId, strongBoxItemInfo.LookupKey, out streamLength)))
            {
              StrongBoxExtensions.LogInfo(logger, "Found certificate with thumbprint '" + certificateThumbprint + "' in strongBox, retrieving it");
              byte[] rawData = binaryReader.ReadBytes((int) streamLength);
              X509Certificate2 x509Certificate2 = new X509Certificate2();
              x509Certificate2.Import(rawData);
              return x509Certificate2;
            }
          }
        }
        StrongBoxExtensions.LogInfo(logger, "StrongBox did not contain certificate with thumbprint '" + certificateThumbprint + "'");
      }
      else
      {
        StrongBoxExtensions.LogInfo(logger, "StrongBox drawer did not exist, creating drawer '{0}'", (object) drawerName);
        drawerId = strongBoxService.CreateDrawer(requestContext, drawerName);
        StrongBoxExtensions.LogInfo(logger, "StrongBox drawer created, id = {0}.", (object) drawerId);
      }
      X509Certificate2 cert = certificateProvider(logger, certificateThumbprint);
      if (cert != null)
      {
        StrongBoxExtensions.LogInfo(logger, FormattableString.Invariant(FormattableStringFactory.Create("Uploading certificate with thumbprint '{0}' to StrongBox drawer with id = {1}, Expiration date = '{2}'", (object) cert.Thumbprint, (object) drawerId, (object) cert.NotAfter)));
        StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo()
        {
          DrawerId = drawerId,
          LookupKey = cert.Thumbprint,
          ExpirationDate = new DateTime?(cert.NotAfter)
        };
        strongBoxService.AddCertificate(requestContext, strongBoxItemInfo, cert);
      }
      return cert;
    }

    private static X509Certificate2 LoadCertificateFromStore(
      ITFLogger servicingContext,
      string certificateThumbprint,
      StoreLocation storeLocation)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(certificateThumbprint, nameof (certificateThumbprint));
      X509Store x509Store = new X509Store(StoreName.My, storeLocation);
      try
      {
        x509Store.Open(OpenFlags.ReadOnly);
        StrongBoxExtensions.LogInfo(servicingContext, "Looking for certificate with thumbprint '" + certificateThumbprint + "' in local cert store");
        X509Certificate2 x509Certificate2 = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) certificateThumbprint, false).Cast<X509Certificate2>().FirstOrDefault<X509Certificate2>();
        if (x509Certificate2 != null)
          return x509Certificate2;
        servicingContext?.Error("Did not find certificate with thumbprint '" + certificateThumbprint + "' in local cert store. This may cause further errors.");
        return (X509Certificate2) null;
      }
      finally
      {
        x509Store.Close();
      }
    }

    private static void LogInfo(ITFLogger logger, string messageFormat, params object[] args) => logger?.Info(messageFormat, args);
  }
}
