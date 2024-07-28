// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.CertHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class CertHandler : ICertHandler
  {
    private const string c_makeServiceCertArgs = "-r -pe -n \"CN={0}\" -eku 1.3.6.1.5.5.7.3.1,1.3.6.1.5.5.7.3.2 -m 24 -a sha512 -sky exchange -sp \"Microsoft Enhanced RSA and AES Cryptographic Provider\" -sy 24 -len 2048 -ss my";

    public CertHandler()
      : this((ITFLogger) null)
    {
    }

    public CertHandler(ITFLogger logger) => this.Logger = logger ?? (ITFLogger) new NullLogger();

    protected ITFLogger Logger { get; private set; }

    public void AddAccessRule(
      X509Certificate2 certificate,
      IdentityReference identity,
      CryptoKeyRights rights,
      AccessControlType accessType)
    {
      ArgumentUtility.CheckForNull<IdentityReference>(identity, nameof (identity));
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      if (!certificate.HasPrivateKey)
        throw new ArgumentException("Specified certificate does not have a private key.", nameof (certificate));
      if (!(certificate.PrivateKey is RSACryptoServiceProvider privateKey))
        throw new ArgumentException("Private key of the specified certificate is not an RSA.", nameof (certificate));
      foreach (AuthorizationRule authorizationRule in privateKey.CspKeyContainerInfo.CryptoKeySecurity.GetAccessRules(true, true, typeof (SecurityIdentifier)).Cast<AuthorizationRule>().ToArray<AuthorizationRule>())
      {
        if (authorizationRule is CryptoKeyAccessRule cryptoKeyAccessRule && identity.Equals((object) cryptoKeyAccessRule.IdentityReference) && cryptoKeyAccessRule.AccessControlType == accessType && (cryptoKeyAccessRule.CryptoKeyRights & rights) == rights)
          return;
      }
      this.Logger.Info("Adding access to rule. Identity: {0}, Thumbprint: {1}, Rights: {2}.", (object) identity, (object) certificate.Thumbprint, (object) rights);
      CspParameters parameters = new CspParameters(privateKey.CspKeyContainerInfo.ProviderType, privateKey.CspKeyContainerInfo.ProviderName, privateKey.CspKeyContainerInfo.KeyContainerName);
      parameters.Flags = CspProviderFlags.UseMachineKeyStore | CspProviderFlags.UseExistingKey;
      parameters.CryptoKeySecurity = privateKey.CspKeyContainerInfo.CryptoKeySecurity;
      parameters.CryptoKeySecurity.AddAccessRule(new CryptoKeyAccessRule(identity, rights, accessType));
      using (new RSACryptoServiceProvider(parameters))
        ;
      this.Logger.Info("CertHandler.AddAccessRule done.");
    }

    public void Add(StoreName storeName, StoreLocation storeLocation, X509Certificate2 cert)
    {
      using (X509Store x509Store = new X509Store(storeName, storeLocation))
      {
        try
        {
          x509Store.Open(OpenFlags.ReadWrite);
          if (x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) cert.Thumbprint, false).OfType<X509Certificate2>().FirstOrDefault<X509Certificate2>() != null)
          {
            this.Logger.Info("Certificate with Thumbprint " + cert.Thumbprint + " already exists in store");
          }
          else
          {
            this.Logger.Info("Adding certificate with SubjectName " + cert.SubjectName.Name + " and Thumbprint " + cert.Thumbprint + " to store");
            x509Store.Add(cert);
          }
        }
        catch (Exception ex)
        {
          this.Logger.Error("Excpetion adding certificate with SubjectName " + cert.SubjectName.Name + " and Thumbprint " + cert.Thumbprint + " to store: " + ex.Message);
          throw;
        }
        finally
        {
          x509Store.Close();
        }
      }
    }

    public void Remove(string storeName, StoreLocation storeLocation, string thumbprint)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(storeName, nameof (storeName));
      ArgumentUtility.CheckStringForNullOrEmpty(thumbprint, nameof (thumbprint));
      using (X509Store x509Store = new X509Store(storeName, storeLocation))
      {
        try
        {
          x509Store.Open(OpenFlags.ReadWrite | OpenFlags.OpenExistingOnly);
        }
        catch (CryptographicException ex)
        {
          return;
        }
        X509Certificate2Enumerator enumerator = x509Store.Certificates.Find(X509FindType.FindByThumbprint, (object) thumbprint, false).GetEnumerator();
        while (enumerator.MoveNext())
        {
          X509Certificate2 current = enumerator.Current;
          this.Logger.Info("Removing certifcate with Thumbprint: {0}, Store Name: {1}, Store Location: {2}", (object) current.Thumbprint, (object) storeName, (object) storeLocation.ToString());
          x509Store.Remove(current);
        }
      }
    }

    public void GrantReadAccessToNetworkService(X509Certificate2 certificate)
    {
      IdentityReference identity = (IdentityReference) new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, (SecurityIdentifier) null);
      this.AddAccessRule(certificate, identity, CryptoKeyRights.GenericRead, AccessControlType.Allow);
    }

    public string GetSubjectCommonName(X509Certificate2 certificate)
    {
      AdminTraceLogger.Default.Enter("CertHandler.GetSubjectName");
      string subjectCommonName = (string) null;
      if (!string.IsNullOrEmpty(certificate.Subject))
      {
        string message = ((IEnumerable<string>) ((IEnumerable<string>) certificate.Subject.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (x => x.TrimStart())).ToArray<string>()).FirstOrDefault<string>((Func<string, bool>) (p => p.StartsWith("CN=")));
        AdminTraceLogger.Default.Verbose(message);
        if (message != null)
          subjectCommonName = message.Substring(3);
      }
      return subjectCommonName;
    }

    public string[] GetSubjectAlternativeNames(X509Certificate2 certificate)
    {
      AdminTraceLogger.Default.Enter("CertHandler.GetSubjectAlternativeNames");
      AdminTraceLogger.Default.Verbose("Thumbprint: {0}", (object) certificate.Thumbprint);
      string subjectAlternativeNamesString = (string) null;
      foreach (X509Extension extension in certificate.Extensions)
      {
        if (string.Equals(extension.Oid.FriendlyName, "Subject Alternative Name", StringComparison.Ordinal))
        {
          subjectAlternativeNamesString = extension.Format(false);
          break;
        }
      }
      if (subjectAlternativeNamesString != null)
        return CertHandler.TryParseSanString(subjectAlternativeNamesString);
      AdminTraceLogger.Default.Verbose("Subject Alternative Name not found");
      return Array.Empty<string>();
    }

    public void LogCertificateInfo(X509Certificate2 cert)
    {
      this.Logger.Info("    Certifcate details          :");
      this.Logger.Info("    Thumbprint                  : " + cert.Thumbprint);
      this.Logger.Info("    Issuer                      : " + cert.Issuer);
      this.Logger.Info("    Subject                     : " + cert.Subject);
      string[] alternativeNames = this.GetSubjectAlternativeNames(cert);
      string str = "    Subject Alternative Names   ";
      if (alternativeNames.Length != 0)
      {
        for (int index = 0; index < alternativeNames.Length; ++index)
          this.Logger.Info((index == 0 ? str : "".PadLeft(str.Length, ' ')) + ": " + alternativeNames[index]);
      }
      else
        this.Logger.Info(str + ":");
      this.Logger.Info(string.Format("    Not before                  : {0}", (object) cert.NotBefore));
      this.Logger.Info(string.Format("    Not after                   : {0}", (object) cert.NotAfter));
    }

    public bool IsSelfSigned(X509Certificate2 certificate)
    {
      ArgumentUtility.CheckForNull<X509Certificate2>(certificate, nameof (certificate));
      this.Logger.Info("CertHandler.IsSelfSigned is called.");
      this.Logger.Info("Subject: '" + certificate.Subject + "', Issuer: '" + certificate.Issuer + "'.");
      bool flag = string.Equals(certificate.Subject, certificate.Issuer);
      this.Logger.Info("Self-signed: {0}", (object) flag);
      return flag;
    }

    public bool CheckSslCertDomain(X509Certificate2 certificate, string domain)
    {
      AdminTraceLogger.Default.Enter("CertHandler.CheckSslCertDomain");
      AdminTraceLogger.Default.Verbose("Thumbprint: {0}", (object) certificate.Thumbprint);
      List<string> source = new List<string>();
      string[] alternativeNames = this.GetSubjectAlternativeNames(certificate);
      if (alternativeNames.Length != 0)
      {
        source.AddRange((IEnumerable<string>) alternativeNames);
      }
      else
      {
        string subjectCommonName = this.GetSubjectCommonName(certificate);
        if (subjectCommonName != null)
          source.Add(subjectCommonName);
      }
      bool flag;
      if (domain.StartsWith("*.", StringComparison.Ordinal))
      {
        flag = source.FirstOrDefault<string>((Func<string, bool>) (n => n.Equals(domain, StringComparison.OrdinalIgnoreCase))) != null;
      }
      else
      {
        flag = source.FirstOrDefault<string>((Func<string, bool>) (n => n.Equals(domain, StringComparison.OrdinalIgnoreCase))) != null;
        if (!flag)
        {
          int num = domain.IndexOf('.');
          if (num >= 0)
          {
            domain = domain.Substring(num + 1);
            flag = source.FirstOrDefault<string>((Func<string, bool>) (n => n.StartsWith("*.", StringComparison.OrdinalIgnoreCase) && n.Substring(2).Equals(domain, StringComparison.OrdinalIgnoreCase))) != null;
          }
        }
      }
      return flag;
    }

    public X509Certificate2 MakeOAuthSigningCertificate()
    {
      int year = DateTime.Now.Year;
      return this.MakeServiceCert("-r -pe -n \"CN={0}\" -b 01/01/" + year.ToString() + " -e 01/01/" + (year + 20).ToString() + " -eku 1.3.6.1.5.5.7.3.1 -sky exchange -sp \"Microsoft Enhanced RSA and AES Cryptographic Provider\" -sy 24 -len 2048 -a sha256 -ss my");
    }

    public X509Certificate2 MakeServiceCert() => this.MakeServiceCert("-r -pe -n \"CN={0}\" -eku 1.3.6.1.5.5.7.3.1,1.3.6.1.5.5.7.3.2 -m 24 -a sha512 -sky exchange -sp \"Microsoft Enhanced RSA and AES Cryptographic Provider\" -sy 24 -len 2048 -ss my");

    public X509Certificate2 MakeServiceCertWithName(string certName) => this.MakeServiceCert("-r -pe -n \"CN={0}\" -eku 1.3.6.1.5.5.7.3.1,1.3.6.1.5.5.7.3.2 -m 24 -a sha512 -sky exchange -sp \"Microsoft Enhanced RSA and AES Cryptographic Provider\" -sy 24 -len 2048 -ss my", certName);

    public X509Certificate2 MakeServiceCert(string makeCertArgs) => this.MakeServiceCert(makeCertArgs, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TfsServiceCert({0}) from {1}", (object) DateTime.Now, (object) Environment.MachineName));

    public X509Certificate2 MakeServiceCert(string makeCertArgs, string certName)
    {
      AdminTraceLogger.Default.Enter("CertHandler.MakeServiceCert");
      string args = string.Format((IFormatProvider) CultureInfo.InvariantCulture, makeCertArgs, (object) certName);
      StoreLocation storeLocation = StoreLocation.CurrentUser;
      string errorMsg;
      if (!this.ExecuteMakeCert(args, out errorMsg))
      {
        storeLocation = StoreLocation.LocalMachine;
        if (!this.ExecuteMakeCert(args + " -sr LocalMachine", out errorMsg))
          throw new MakeCertException(ConfigurationResources.MakeCertFailed((object) errorMsg));
      }
      X509Store x509Store = new X509Store(StoreName.My, storeLocation);
      X509Certificate2 certificate;
      try
      {
        x509Store.Open(OpenFlags.ReadWrite);
        X509Certificate2Collection certificate2Collection = x509Store.Certificates.Find(X509FindType.FindBySubjectName, (object) certName, false);
        certificate = certificate2Collection.Count > 0 ? certificate2Collection[0] : throw new MakeCertException(ConfigurationResources.MakeCertFailedNoCert());
        x509Store.Remove(certificate);
      }
      catch (MakeCertException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new MakeCertException(ConfigurationResources.MakeCertFailed((object) ex.Message), ex);
      }
      finally
      {
        x509Store.Close();
      }
      AdminTraceLogger.Default.Exit("CertHandler.MakeServiceCert");
      return certificate;
    }

    public X509Certificate2 FindCertificateByThumbprint(
      StoreName storeName,
      StoreLocation storeLocation,
      string thumbprint,
      bool validOnly)
    {
      return this.FindCertificate(storeName, storeLocation, X509FindType.FindByThumbprint, (object) thumbprint, validOnly);
    }

    public X509Certificate2 FindCertificateByThumbprint(string thumbprint)
    {
      for (int index = 0; index < 5; ++index)
      {
        X509Certificate2 certificateByThumbprint = (X509Certificate2) null;
        if (!string.IsNullOrEmpty(thumbprint))
          certificateByThumbprint = this.FindCertificateByThumbprint(StoreName.My, StoreLocation.LocalMachine, thumbprint, false) ?? this.FindCertificateByThumbprint(StoreName.My, StoreLocation.CurrentUser, thumbprint, false);
        if (certificateByThumbprint != null)
          return certificateByThumbprint;
        this.Logger.Info(string.Format("Certificate not found on attempt #{0}.  Sleeping before trying again", (object) index));
        Thread.Sleep(TimeSpan.FromSeconds(1.0));
      }
      return (X509Certificate2) null;
    }

    public X509Certificate2 FindCertificate(
      StoreName storeName,
      StoreLocation storeLocation,
      X509FindType findType,
      object findValue,
      bool validOnly)
    {
      AdminTraceLogger.Default.Enter("CertHandler.FindCertificate");
      X509Store x509Store = new X509Store(storeName, storeLocation);
      X509Certificate2 certificate;
      try
      {
        try
        {
          x509Store.Open(OpenFlags.OpenExistingOnly);
        }
        catch (CryptographicException ex)
        {
          return (X509Certificate2) null;
        }
        certificate = x509Store.Certificates.Find(findType, findValue, validOnly).OfType<X509Certificate2>().FirstOrDefault<X509Certificate2>();
      }
      finally
      {
        x509Store.Close();
      }
      AdminTraceLogger.Default.Exit("CertHandler.FindCertificate");
      return certificate;
    }

    public X509Certificate2 FindCertificate(
      string storeName,
      StoreLocation storeLocation,
      X509FindType findType,
      object findValue,
      bool validOnly)
    {
      AdminTraceLogger.Default.Enter("CertHandler.FindCertificate");
      X509Store x509Store = new X509Store(storeName, storeLocation);
      X509Certificate2 certificate;
      try
      {
        try
        {
          x509Store.Open(OpenFlags.OpenExistingOnly);
        }
        catch (CryptographicException ex)
        {
          return (X509Certificate2) null;
        }
        certificate = x509Store.Certificates.Find(findType, findValue, validOnly).OfType<X509Certificate2>().FirstOrDefault<X509Certificate2>();
      }
      finally
      {
        x509Store.Close();
      }
      AdminTraceLogger.Default.Exit("CertHandler.FindCertificate");
      return certificate;
    }

    public bool ExecuteMakeCert(string args, out string errorMsg)
    {
      this.Logger.Info("--> CertHandler.ExecuteMakeCert");
      errorMsg = (string) null;
      using (BinaryReader binaryReader = new BinaryReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("makecert.exe")))
      {
        byte[] bytes = binaryReader.ReadBytes((int) binaryReader.BaseStream.Length);
        string nameWithExtension = FileSpec.GetTempFileNameWithExtension(".exe");
        try
        {
          File.WriteAllBytes(nameWithExtension, bytes);
          Process process1 = new Process();
          ProcessStartInfo startInfo = new ProcessStartInfo()
          {
            Arguments = args,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            LoadUserProfile = true,
            FileName = nameWithExtension
          };
          this.Logger.Info("Executing: \"{0}\" {1}", (object) nameWithExtension, (object) args);
          using (Process process2 = Process.Start(startInfo))
          {
            process2.WaitForExit();
            if (process2.ExitCode != 0)
            {
              string end1 = process2.StandardOutput.ReadToEnd();
              string end2 = process2.StandardError.ReadToEnd();
              errorMsg = "Makecert failed: " + end1 + end2;
            }
          }
          return errorMsg == null;
        }
        finally
        {
          if (File.Exists(nameWithExtension))
          {
            try
            {
              File.Delete(nameWithExtension);
            }
            catch (Exception ex)
            {
              this.Logger.Warning("Failed to delete makecert image. Error: {0}", (object) ex);
            }
          }
          this.Logger.Info("<-- CertHandler.ExecuteMakeCert");
        }
      }
    }

    public static bool IsValidThumbprint(string thumbprint)
    {
      if (string.IsNullOrEmpty(thumbprint))
        return false;
      foreach (char c in thumbprint)
      {
        if ((char.IsDigit(c) || c >= 'a' && c <= 'f' ? 1 : (c < 'A' ? 0 : (c <= 'F' ? 1 : 0))) == 0)
          return false;
      }
      return true;
    }

    private static string[] TryParseSanString(string subjectAlternativeNamesString)
    {
      string[] strArray = subjectAlternativeNamesString.Split(new string[1]
      {
        ", "
      }, StringSplitOptions.None);
      List<string> stringList = new List<string>();
      for (int index = 0; index < strArray.Length; ++index)
      {
        if (strArray[index].StartsWith("DNS Name=", StringComparison.Ordinal))
          stringList.Add(strArray[index].Substring("DNS Name=".Length));
        else
          AdminTraceLogger.Default.Error("'{0}' is not a well-formed subject alternative names string");
      }
      return stringList.ToArray();
    }
  }
}
