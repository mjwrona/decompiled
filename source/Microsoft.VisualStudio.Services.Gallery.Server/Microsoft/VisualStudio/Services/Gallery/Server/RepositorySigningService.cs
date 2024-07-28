// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.RepositorySigningService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Configuration;
using Microsoft.VisualStudio.Services.Gallery.Server.Facade;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class RepositorySigningService : IRepositorySigningService, IVssFrameworkService
  {
    private string m_signingToolPath;
    private GalleryFileSystemHelper m_fileSystemHelper;
    private GalleryProcessHandlerHelper m_processHandlerHelper;
    private IGalleryFileDownloadHttpClient m_galleryFileDownloadHttpClient;
    private string m_additionalValidationPolicy;
    private const string s_layer = "RepositorySigningService";

    public RepositorySigningService()
    {
    }

    public RepositorySigningService(
      GalleryFileSystemHelper galleryFSHelper,
      GalleryProcessHandlerHelper galleryProcessHandlerHelper)
    {
      this.m_fileSystemHelper = galleryFSHelper;
      this.m_processHandlerHelper = galleryProcessHandlerHelper;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(12062097, "gallery", nameof (RepositorySigningService), nameof (ServiceStart));
      if (systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback), false, "/Configuration/Service/Gallery/PackageVerification/**");
        this.m_fileSystemHelper = this.m_fileSystemHelper != null ? this.m_fileSystemHelper : new GalleryFileSystemHelper();
        this.m_processHandlerHelper = this.m_processHandlerHelper != null ? this.m_processHandlerHelper : new GalleryProcessHandlerHelper();
        this.m_galleryFileDownloadHttpClient = (IGalleryFileDownloadHttpClient) new GalleryFileDownloadHttpClient(HttpClientFactory.New());
        this.LoadSigningToolConfiguration(systemRequestContext);
      }
      systemRequestContext.TraceLeave(12061119, "gallery", nameof (RepositorySigningService), nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.ConfigurationChangeCallback));
    }

    public Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode GenerateSigningManifestFile(
      IVssRequestContext requestContext,
      string packageFilePath,
      string manifestFilePath)
    {
      requestContext.TraceEnter(12062097, "gallery", nameof (RepositorySigningService), nameof (GenerateSigningManifestFile));
      Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode signingManifestFile = Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode.None;
      try
      {
        CommandLineBuilder generationCommand = this.GetManifestGenerationCommand("generatemanifest", packageFilePath, manifestFilePath);
        ProcessOutput vsceToolOutput = this.m_processHandlerHelper.RunExe(this.m_signingToolPath, generationCommand, (ITFLogger) new NullLogger());
        signingManifestFile = (Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode) vsceToolOutput.ExitCode;
        this.LogDetailedCILog(requestContext, generationCommand.ToString(), vsceToolOutput);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062097, "gallery", "generatemanifest", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12062097, "gallery", nameof (RepositorySigningService), nameof (GenerateSigningManifestFile));
      }
      return signingManifestFile;
    }

    public Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode GenerateSignatureArchive(
      IVssRequestContext requestContext,
      string manifestFilePath,
      string signatureFilePath,
      string signatureArchiveFilePath)
    {
      requestContext.TraceEnter(12062097, "gallery", nameof (RepositorySigningService), nameof (GenerateSignatureArchive));
      Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode signatureArchive = Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode.None;
      try
      {
        CommandLineBuilder signatureArchiveCommand = this.GetGenerateSignatureArchiveCommand("zip", manifestFilePath, signatureFilePath, signatureArchiveFilePath);
        ProcessOutput vsceToolOutput = this.m_processHandlerHelper.RunExe(this.m_signingToolPath, signatureArchiveCommand, (ITFLogger) new NullLogger());
        signatureArchive = (Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode) vsceToolOutput.ExitCode;
        this.LogDetailedCILog(requestContext, signatureArchiveCommand.ToString(), vsceToolOutput);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062097, "gallery", "zip", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12062097, "gallery", nameof (RepositorySigningService), nameof (GenerateSignatureArchive));
      }
      return signatureArchive;
    }

    public Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode VerifyPackageSignature(
      IVssRequestContext requestContext,
      string packageFilePath,
      string signatureArchiveFilePath)
    {
      requestContext.TraceEnter(12062097, "gallery", nameof (RepositorySigningService), nameof (VerifyPackageSignature));
      Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode returnCode = Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode.None;
      try
      {
        CommandLineBuilder signatureCommand = this.GetVerifyPackageSignatureCommand("verify", packageFilePath, signatureArchiveFilePath);
        ProcessOutput vsceToolOutput = this.m_processHandlerHelper.RunExe(this.m_signingToolPath, signatureCommand, (ITFLogger) new NullLogger());
        returnCode = (Microsoft.VisualStudio.Services.Gallery.Server.SigningTool.ReturnCode) vsceToolOutput.ExitCode;
        this.LogDetailedCILog(requestContext, signatureCommand.ToString(), vsceToolOutput);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12062097, "gallery", "verify", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12062097, "gallery", nameof (RepositorySigningService), nameof (VerifyPackageSignature));
      }
      return returnCode;
    }

    public bool DownloadFile(IVssRequestContext requestContext, string fileUrl, string filePath) => this.m_galleryFileDownloadHttpClient != null && !string.IsNullOrWhiteSpace(fileUrl) && !string.IsNullOrWhiteSpace(filePath) && this.m_galleryFileDownloadHttpClient.DownloadFileAsync(requestContext, fileUrl, filePath).SyncResult<bool>();

    private void ConfigurationChangeCallback(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSigningToolConfiguration(requestContext);
    }

    private void LoadSigningToolConfiguration(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(12062097, "gallery", nameof (RepositorySigningService), nameof (LoadSigningToolConfiguration));
      this.m_additionalValidationPolicy = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Configuration/Service/Gallery/RepositorySigning/**").GetValueFromPath<string>("/Configuration/Service/Gallery/RepositorySigning/AdditionalValidationPolicyRegistryPath", "VSMP");
      this.m_signingToolPath = Path.Combine(requestContext.ServiceHost.PhysicalDirectory, "bin\\signing\\vsce-sign.exe");
      requestContext.Trace(12062097, TraceLevel.Info, "gallery", nameof (LoadSigningToolConfiguration), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Signing Tool Client Path: {0}", (object) this.m_signingToolPath));
      requestContext.TraceLeave(12061119, "gallery", nameof (RepositorySigningService), nameof (LoadSigningToolConfiguration));
    }

    private CommandLineBuilder GetManifestGenerationCommand(
      string command,
      string packageFilePath,
      string signatureManifestFilePath)
    {
      CommandLineBuilder generationCommand = new CommandLineBuilder();
      generationCommand.Append(command);
      generationCommand.Append("-p");
      generationCommand.Append(packageFilePath);
      generationCommand.Append("-o");
      generationCommand.Append(signatureManifestFilePath);
      generationCommand.Append("--verbose");
      return generationCommand;
    }

    private CommandLineBuilder GetVerifyPackageSignatureCommand(
      string command,
      string packageFilePath,
      string signatureArchiveFilePath)
    {
      CommandLineBuilder signatureCommand = new CommandLineBuilder();
      signatureCommand.Append(command);
      signatureCommand.Append("-p");
      signatureCommand.Append(packageFilePath);
      signatureCommand.Append("-sa");
      signatureCommand.Append(signatureArchiveFilePath);
      signatureCommand.Append("-ap");
      signatureCommand.Append(this.m_additionalValidationPolicy);
      signatureCommand.Append("--verbose");
      return signatureCommand;
    }

    private CommandLineBuilder GetGenerateSignatureArchiveCommand(
      string command,
      string manifestFilePath,
      string signatureFilePath,
      string signatureArchiveFilePath)
    {
      CommandLineBuilder signatureArchiveCommand = new CommandLineBuilder();
      signatureArchiveCommand.Append(command);
      signatureArchiveCommand.Append("-m");
      signatureArchiveCommand.Append(manifestFilePath);
      signatureArchiveCommand.Append("-s");
      signatureArchiveCommand.Append(signatureFilePath);
      signatureArchiveCommand.Append("-o");
      signatureArchiveCommand.Append(signatureArchiveFilePath);
      signatureArchiveCommand.Append("--verbose");
      return signatureArchiveCommand;
    }

    private void LogDetailedCILog(
      IVssRequestContext requestContext,
      string commandLineArgs,
      ProcessOutput vsceToolOutput)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("ExitCode", (double) vsceToolOutput.ExitCode);
      properties.Add("ExecutionTime", vsceToolOutput.RunningTime.TotalMilliseconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      properties.Add("CommandLine", commandLineArgs);
      properties.Add("StdError", vsceToolOutput.StdErr);
      properties.Add("StdOut", vsceToolOutput.StdOut);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Microsoft.VisualStudio.Services.Gallery", "RepositorySigningFeature", properties);
    }

    public void DeleteRepositorySigningFiles(List<string> filesToBeDeleted) => this.m_fileSystemHelper.DeleteFiles(filesToBeDeleted);
  }
}
