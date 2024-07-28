// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ESRPConfigHelper
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using MS.Ess.EsrpClient.Contracts.ExeShell.V1;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ESRPConfigHelper
  {
    private string m_esrpServerUrl;
    private string m_esrpClientId;
    private string m_authCertThumbprint;
    private GalleryFileSystemHelper m_galleryFSHelper;
    private const string s_layer = "ESRPHelper";

    public ESRPConfigHelper(
      string esrpServerUrl,
      string clientId,
      GalleryFileSystemHelper galleryFSHelper)
    {
      this.m_esrpServerUrl = esrpServerUrl;
      this.m_esrpClientId = clientId;
      this.m_galleryFSHelper = galleryFSHelper != null ? galleryFSHelper : new GalleryFileSystemHelper();
    }

    ~ESRPConfigHelper() => this.Reset();

    public string AuthFilePath { get; set; }

    public string SigningAuthFilePath { get; set; }

    public string ConfigFilePath { get; set; }

    public string PolicyFilePath { get; set; }

    public void Reset()
    {
      this.m_galleryFSHelper.DeleteFiles(new List<string>()
      {
        this.AuthFilePath,
        this.ConfigFilePath,
        this.PolicyFilePath,
        this.SigningAuthFilePath
      });
      this.AuthFilePath = (string) null;
      this.ConfigFilePath = (string) null;
      this.PolicyFilePath = (string) null;
      this.SigningAuthFilePath = (string) null;
    }

    public void ValidateAndCreateESRPConfigFiles(
      IVssRequestContext requestContext,
      string authCertThumbprint,
      string esrpClientId,
      string tenantId,
      string esrpAPIEndpoint,
      string certificateSubjectName,
      bool isSigning = false)
    {
      requestContext.TraceEnter(12061119, "gallery", "ESRPHelper", "validateAndCreateESRPConfigFiles");
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.UseOfOAuthForESRPEnabled"))
      {
        if (isSigning)
          this.GeneraAuthFileUsingOuth(requestContext, esrpClientId, tenantId, this.SigningAuthFilePath, true);
        else
          this.GeneraAuthFileUsingOuth(requestContext, esrpClientId, tenantId, this.AuthFilePath);
      }
      else if (isSigning)
        this.GeneraAuthFileUsingCertificates(requestContext, authCertThumbprint, certificateSubjectName, esrpClientId, tenantId, this.SigningAuthFilePath, true);
      else
        this.GeneraAuthFileUsingCertificates(requestContext, authCertThumbprint, certificateSubjectName, esrpClientId, tenantId, this.AuthFilePath);
      if (string.IsNullOrWhiteSpace(this.ConfigFilePath) || !this.m_galleryFSHelper.Exists(this.ConfigFilePath))
      {
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", "validateAndCreateESRPConfigFiles", "Generating Config File");
        if (!string.IsNullOrWhiteSpace(this.ConfigFilePath))
          this.FileChangedTelemetry(requestContext, GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType.ConfigFile, "Temp File deletion");
        this.CreateConfigFile();
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", "validateAndCreateESRPConfigFiles", "Generated Config File: " + this.ConfigFilePath);
      }
      if (string.IsNullOrWhiteSpace(this.PolicyFilePath) || !this.m_galleryFSHelper.Exists(this.PolicyFilePath))
      {
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", "validateAndCreateESRPConfigFiles", "Generating Policy File");
        if (!string.IsNullOrWhiteSpace(this.PolicyFilePath))
          this.FileChangedTelemetry(requestContext, GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType.PolicyFile, "Temp File deletion");
        this.CreatePolicyFile();
        requestContext.Trace(12061119, TraceLevel.Info, "gallery", "validateAndCreateESRPConfigFiles", "Generated Policy File: " + this.PolicyFilePath);
      }
      requestContext.TraceLeave(12061119, "gallery", "ESRPHelper", "validateAndCreateESRPConfigFiles");
    }

    private void GeneraAuthFileUsingOuth(
      IVssRequestContext requestContext,
      string esrpClientId,
      string tenantId,
      string authFilePath,
      bool isSigning = false)
    {
      if (!string.IsNullOrWhiteSpace(authFilePath) && this.m_galleryFSHelper.Exists(authFilePath))
        return;
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (GeneraAuthFileUsingOuth), "Generating authentication File with Signing : " + isSigning.ToString());
      if (!string.IsNullOrWhiteSpace(authFilePath))
        this.FileChangedTelemetry(requestContext, GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType.AuthFile, "File not found");
      this.CreateAuthFileForOAuth(esrpClientId, tenantId, isSigning);
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (GeneraAuthFileUsingOuth), "Generated authentication File : " + authFilePath + "with signing: " + isSigning.ToString());
    }

    private void GeneraAuthFileUsingCertificates(
      IVssRequestContext requestContext,
      string authCertThumbprint,
      string certificateSubjectName,
      string esrpClientId,
      string tenantId,
      string authFilePath,
      bool isSigning = false)
    {
      if (!string.IsNullOrWhiteSpace(authFilePath) && this.m_galleryFSHelper.Exists(authFilePath) && this.m_authCertThumbprint.Equals(authCertThumbprint, StringComparison.OrdinalIgnoreCase))
        return;
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (GeneraAuthFileUsingCertificates), "Generating authentication File");
      if (!string.IsNullOrWhiteSpace(this.m_authCertThumbprint) && !this.m_authCertThumbprint.Equals(authCertThumbprint, StringComparison.OrdinalIgnoreCase))
        this.FileChangedTelemetry(requestContext, GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType.AuthFile, "Certificate Changed");
      else if (!string.IsNullOrWhiteSpace(authFilePath))
        this.FileChangedTelemetry(requestContext, GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType.AuthFile, "File not found");
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableUseOfNewESRPCertificate"))
        this.CreateAuthFile(esrpClientId, tenantId, certificateSubjectName, isSigning);
      else
        this.CreateAuthFileWithRTSPCertificate(authCertThumbprint, isSigning);
      requestContext.Trace(12061119, TraceLevel.Info, "gallery", nameof (GeneraAuthFileUsingCertificates), "Generated authentication File: " + this.AuthFilePath);
    }

    private string CreateConfigFile()
    {
      ConfigInfo objectToSerialize = new ConfigInfo()
      {
        EsrpApiBaseUri = this.m_esrpServerUrl,
        EsrpSessionTimeoutInSec = 2400,
        MaxDegreeOfParallelism = -1,
        ExponentialFirstFastRetry = new bool?(true),
        ExponentialRetryCount = 3,
        ExponentialRetryMinBackOff = TimeSpan.FromSeconds(3.0),
        ExponentialRetryMaxBackOff = TimeSpan.FromSeconds(30.0),
        ExponentialRetryDeltaBackOff = TimeSpan.FromSeconds(5.0)
      };
      this.ConfigFilePath = FileSpec.GetTempFileNameWithExtension("json");
      this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.ConfigFilePath);
      return this.ConfigFilePath;
    }

    private string CreatePolicyFile()
    {
      PolicyInfo objectToSerialize = new PolicyInfo()
      {
        Intent = "Product Release",
        ContentType = "App",
        ContentOrigin = "3rd Party",
        Audience = "External Broad"
      };
      this.PolicyFilePath = FileSpec.GetTempFileNameWithExtension("json");
      this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.PolicyFilePath);
      return this.PolicyFilePath;
    }

    private string CreateAuthFileWithRTSPCertificate(string authCertThumbprint, bool isSigning = false)
    {
      this.m_authCertThumbprint = authCertThumbprint;
      string str = "CN=" + this.m_esrpClientId;
      AuthInfo objectToSerialize = new AuthInfo()
      {
        AuthenticationType = "AAD_CERT",
        ClientId = this.m_esrpClientId,
        AuthCert = new CertInfo()
        {
          StoreLocation = "LocalMachine",
          StoreName = "My",
          SubjectName = GalleryServerUtil.GetSubjectNameFromThumbprint(this.m_authCertThumbprint)
        }
      };
      if (isSigning)
      {
        objectToSerialize.RequestSigningCert = new CertInfo()
        {
          StoreLocation = "LocalMachine",
          StoreName = "My",
          SubjectName = str
        };
        this.SigningAuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
        this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.SigningAuthFilePath);
        return this.SigningAuthFilePath;
      }
      this.AuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
      this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.AuthFilePath);
      return this.AuthFilePath;
    }

    private string CreateAuthFile(
      string clientId,
      string tenantId,
      string certificateSubjectName,
      bool isSigning = false)
    {
      string str = "CN=" + clientId;
      AuthInfo objectToSerialize = new AuthInfo()
      {
        AuthenticationType = "AAD_CERT",
        ClientId = clientId,
        TenantId = tenantId,
        AuthCert = new CertInfo()
        {
          StoreLocation = "LocalMachine",
          StoreName = "My",
          SubjectName = certificateSubjectName,
          SendX5c = true
        }
      };
      if (isSigning)
      {
        objectToSerialize.RequestSigningCert = new CertInfo()
        {
          StoreLocation = "LocalMachine",
          StoreName = "My",
          SubjectName = str
        };
        this.SigningAuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
        this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.SigningAuthFilePath);
        return this.SigningAuthFilePath;
      }
      this.AuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
      this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.AuthFilePath);
      return this.AuthFilePath;
    }

    private string CreateAuthFileForOAuth(string clientId, string tenantId, bool isSigning = false)
    {
      string str = "CN=" + clientId;
      AuthInfo objectToSerialize = new AuthInfo()
      {
        AuthenticationType = "AAD_OAUTH_TOKEN",
        ClientId = clientId,
        TenantId = tenantId,
        OAuthToken = new OAuthTokenInfo()
        {
          EnvironmentVariable = "ESRPOAuthToken",
          EnvironmentVariableTarget = EnvironmentVariableTarget.Process
        }
      };
      if (isSigning)
      {
        objectToSerialize.RequestSigningCert = new CertInfo()
        {
          StoreLocation = "LocalMachine",
          StoreName = "My",
          SubjectName = str
        };
        this.SigningAuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
        this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.SigningAuthFilePath);
        return this.SigningAuthFilePath;
      }
      this.AuthFilePath = FileSpec.GetTempFileNameWithExtension("json");
      this.m_galleryFSHelper.SerializeAndWriteObjectToFile((object) objectToSerialize, this.AuthFilePath);
      return this.AuthFilePath;
    }

    private void FileChangedTelemetry(
      IVssRequestContext requestContext,
      GalleryServiceConstants.PackageVerification.ESRP.EsrpRequestFileType fileType,
      string reason)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("FileType", (object) fileType);
      intelligenceData.Add("Reason", reason);
      IVssRequestContext requestContext1 = requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", "ESRPConfigFilesRegeneration", properties);
    }
  }
}
