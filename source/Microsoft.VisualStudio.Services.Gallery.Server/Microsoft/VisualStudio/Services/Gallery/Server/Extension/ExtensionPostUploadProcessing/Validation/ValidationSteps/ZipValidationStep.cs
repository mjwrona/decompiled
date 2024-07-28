// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.ZipValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class ZipValidationStep : PackageValidationStepBase
  {
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.ZipValidation;

    public ZipValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.ZipValidation)
    {
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.InProgress;
      if (!extension.IsVsixTypeExtension() || packageStream == null)
      {
        this.m_result = ValidationStatus.Success;
        this.ResultMessage = string.Empty;
      }
      else
      {
        string str1 = string.Empty;
        try
        {
          using (ZipArchive zipArchive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
          {
            IReadOnlyCollection<string> relativeFilePaths = ZipValidationStep.GetRelativeFilePaths(zipArchive);
            string normalizedDestination = ZipValidationStep.NormalizeDirectoryPath(Environment.CurrentDirectory);
            foreach (string str2 in (IEnumerable<string>) relativeFilePaths)
            {
              str1 = str2;
              string normalizedFilePath = Uri.UnescapeDataString(str2.Replace('/', Path.DirectorySeparatorChar));
              if (!ZipValidationStep.ValidPackageEntry(normalizedDestination, normalizedFilePath))
              {
                this.m_result = ValidationStatus.Failure;
                this.ResultMessage = string.Format("The extension contains an entry {0} which is unsafe for extraction.", (object) str2);
                this.PublishTelemetryForZipSlipValidation(requestContext, extension, "ZipSlipValidationFailure", this.ResultMessage);
                return;
              }
            }
            this.m_result = ValidationStatus.Success;
            this.ResultMessage = string.Empty;
            this.PublishTelemetryForZipSlipValidation(requestContext, extension, "ZipSlipValidationSuccess", this.ResultMessage);
          }
        }
        catch (Exception ex)
        {
          this.m_result = ValidationStatus.Success;
          this.ResultMessage = string.Empty;
          this.PublishTelemetryForZipSlipValidation(requestContext, extension, "ZipSlipValidationPlatformSpecificError", ex.Message + str1);
        }
      }
    }

    private static bool ValidPackageEntry(string normalizedDestination, string normalizedFilePath)
    {
      string fullPath = Path.GetFullPath(Path.Combine(normalizedDestination, normalizedFilePath));
      return fullPath.StartsWith(normalizedDestination, StringComparison.OrdinalIgnoreCase) && fullPath.Length != normalizedDestination.Length;
    }

    private static IReadOnlyCollection<string> GetRelativeFilePaths(ZipArchive zipArchive) => (IReadOnlyCollection<string>) zipArchive.Entries.Select<ZipArchiveEntry, string>((Func<ZipArchiveEntry, string>) (entry => ZipValidationStep.UnescapePath(entry.FullName))).ToList<string>();

    private static string UnescapePath(string path) => path != null && path.IndexOf('%') > -1 ? Uri.UnescapeDataString(path) : path;

    private static string NormalizeDirectoryPath(string path)
    {
      if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
        path += Path.DirectorySeparatorChar.ToString();
      return Path.GetFullPath(path);
    }

    private void PublishTelemetryForZipSlipValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      string feature,
      string message)
    {
      CustomerIntelligenceService service = requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ExtensionName", extension.ExtensionName);
      List<ExtensionVersion> versions = extension.Versions;
      // ISSUE: explicit non-virtual call
      if ((versions != null ? (__nonvirtual (versions.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        intelligenceData.Add("Version", extension.Versions[0].Version);
        intelligenceData.Add("TargetPlatform", extension.Versions[0].TargetPlatform);
      }
      intelligenceData.Add("PublisherName", extension.Publisher?.PublisherName);
      intelligenceData.Add("Message", message);
      IVssRequestContext requestContext1 = requestContext;
      string feature1 = feature;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext1, "Microsoft.VisualStudio.Services.Gallery", feature1, properties);
    }
  }
}
