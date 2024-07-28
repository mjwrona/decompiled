// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionPackageUtil
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class ExtensionPackageUtil
  {
    private const string c_manifestLanguageFileIdRegistryPathFormat = "/Configuration/Extensions/{0}/{1}/{2}/FileId";
    private const string c_manifestFileIdRegistryPathformat = "/Configuration/Extensions/{0}/{1}/FileId";
    private const int c_maxNumberOfValidationChecks = 30;
    private const int c_timeToSleepBetweenValidationChecks = 250;

    public static void SaveManifestsFallback(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      byte[] packageBytes)
    {
      IVssRequestContext deploymentContext = requestContext.To(TeamFoundationHostType.Deployment);
      using (MemoryStream packageStream = new MemoryStream(packageBytes))
      {
        ITeamFoundationFileService fileService = deploymentContext.GetService<ITeamFoundationFileService>();
        IVssRegistryService registryService = deploymentContext.GetService<IVssRegistryService>();
        VSIXPackage.Parse((Stream) packageStream, (IImageResizeUtility) new DefaultImageResizeUtility((Func<Exception, bool>) (ex => true), deploymentContext.ExecutionEnvironment.IsHostedDeployment), (Func<ManifestFile, Stream, bool>) ((manifestFile, fileStream) =>
        {
          if (manifestFile.AssetType.Equals("Microsoft.VisualStudio.Services.Manifest", StringComparison.OrdinalIgnoreCase))
          {
            int num = fileService.UploadFile(deploymentContext, fileStream, OwnerId.Generic, Guid.Empty);
            registryService.SetValue<int>(deploymentContext, string.IsNullOrEmpty(manifestFile.Language) ? string.Format("/Configuration/Extensions/{0}/{1}/FileId", (object) publisherName, (object) extensionName) : string.Format("/Configuration/Extensions/{0}/{1}/{2}/FileId", (object) publisherName, (object) extensionName, (object) manifestFile.Language), num);
          }
          return false;
        }));
      }
    }

    public static bool TryCheckIfExtensionIsValid(
      IVssRequestContext deploymentContext,
      ITFLogger logger,
      string publisherName,
      string extensionName,
      string versionPublished,
      out bool isValid)
    {
      isValid = false;
      int num = 1;
      IGalleryService service = deploymentContext.GetService<IGalleryService>();
      do
      {
        logger.Info("Attempt '{0}' to check if version is valid", (object) num);
        PublishedExtension extension = service.GetExtension(deploymentContext, publisherName, extensionName, versionPublished, ExtensionQueryFlags.IncludeVersions);
        ExtensionVersion extensionVersion = extension.Versions != null ? extension.Versions.FirstOrDefault<ExtensionVersion>() : (ExtensionVersion) null;
        if (extensionVersion == null)
        {
          logger.Error("Couldn't find version '{0}' of extension '{1}'", (object) versionPublished, (object) GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName));
          return false;
        }
        if (extensionVersion.Flags.HasFlag((Enum) ExtensionVersionFlags.Validated))
        {
          logger.Info("Version '{0}' is validated. ", (object) versionPublished);
          isValid = true;
          return true;
        }
        if (!string.IsNullOrEmpty(extensionVersion.ValidationResultMessage))
        {
          logger.Error("Version '{0}' just published is not valid: {1}", (object) versionPublished, (object) extensionVersion.ValidationResultMessage);
          isValid = false;
          return true;
        }
        Thread.Sleep(250);
      }
      while (num++ <= 30);
      return false;
    }
  }
}
