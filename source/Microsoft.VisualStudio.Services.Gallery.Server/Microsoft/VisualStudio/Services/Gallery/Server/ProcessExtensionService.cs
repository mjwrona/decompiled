// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ProcessExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ProcessExtensionService : IProcessExtensionService, IVssFrameworkService
  {
    internal List<IPostUploadExtensionProcessorService> m_postUplaodExtensionHandler;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_postUplaodExtensionHandler = new List<IPostUploadExtensionProcessorService>()
    {
      (IPostUploadExtensionProcessorService) systemRequestContext.GetService<IExtensionAssetUploadService>(),
      (IPostUploadExtensionProcessorService) systemRequestContext.GetService<IExtensionVersionValidationService>()
    };

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch the exception and show it as part of result")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is comming for tracepoints so ignoring")]
    public bool ProcessExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      Guid vsid,
      out string resultMessage)
    {
      requestContext.TraceEnter(12061092, "Gallery", nameof (ProcessExtension), nameof (ProcessExtension));
      resultMessage = "";
      bool flag1 = true;
      PublishedExtension extension = (PublishedExtension) null;
      Stream packageStream = (Stream) null;
      try
      {
        requestContext.Trace(12061092, TraceLevel.Info, "Gallery", nameof (ProcessExtension), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Processing Extension: {0}.{1} version {2} targetPlatform {3} validation id:{4}", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform, (object) validationId));
        extension = this.GetExtension(requestContext, extensionId, version, targetPlatform, validationId);
        if (extension != null)
        {
          packageStream = this.GetPackageStream(requestContext, extension);
          foreach (IPostUploadExtensionProcessorService processorService in this.m_postUplaodExtensionHandler)
          {
            TimeSpan timeout = TimeSpan.FromMinutes(5.0);
            requestContext.Trace(12061092, TraceLevel.Info, "Gallery", nameof (ProcessExtension), "Calling handler: " + processorService.GetType().ToString());
            using (new TraceWatch(requestContext, 12061092, TraceLevel.Error, timeout, "Gallery", nameof (ProcessExtension), "Handler: " + processorService.GetType().ToString() + " took more than " + timeout.ToString() + " minutes to complete.", Array.Empty<object>()))
            {
              string resultMessage1;
              bool flag2 = processorService.ProcessExtension(requestContext, extension, packageStream, validationId, vsid, out resultMessage1);
              resultMessage += resultMessage1;
              flag1 &= flag2;
              if (!flag1)
                break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string str = "PublishTimeError";
          IGalleryTelemetryHelperService service = requestContext.GetService<IGalleryTelemetryHelperService>();
          if (extension == null)
            extension = new PublishedExtension()
            {
              ExtensionName = extensionName,
              Publisher = new PublisherFacts()
              {
                PublisherName = publisherName
              }
            };
          IVssRequestContext requestContext1 = requestContext;
          PublishedExtension publishedExtension = extension;
          string action = str;
          service.PublishAppInsightsPerExtensionTelemetryHelper(requestContext1, publishedExtension, action);
        }
        requestContext.TraceException(12061092, "Gallery", nameof (ProcessExtension), ex);
        flag1 = false;
        resultMessage = ex.ToString();
      }
      finally
      {
        packageStream?.Dispose();
        requestContext.TraceLeave(12061092, "Gallery", nameof (ProcessExtension), nameof (ProcessExtension));
      }
      return flag1;
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We want to catch the exception and show it as part of result")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is comming for tracepoints so ignoring")]
    public bool UpdateExtensionValidationStatus(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      Guid vsid,
      out string resultMessage)
    {
      requestContext.TraceEnter(12061092, "Gallery", "ProcessExtension", "ProcessExtension");
      resultMessage = string.Empty;
      bool flag = true;
      PublishedExtension extension = (PublishedExtension) null;
      try
      {
        requestContext.Trace(12061092, TraceLevel.Info, "Gallery", nameof (UpdateExtensionValidationStatus), string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Processing Extension: {0}.{1} version {2} targetPlatform {3} validation id:{4}", (object) publisherName, (object) extensionName, (object) version, (object) targetPlatform, (object) validationId));
        extension = this.GetExtension(requestContext, extensionId, version, targetPlatform, validationId);
        if (extension != null)
          flag = requestContext.GetService<IExtensionVersionValidationService>().UpdateValidationStatus(requestContext, extension, validationId, vsid, out resultMessage);
      }
      catch (Exception ex)
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string str = "PublishTimeError";
          IGalleryTelemetryHelperService service = requestContext.GetService<IGalleryTelemetryHelperService>();
          if (extension == null)
            extension = new PublishedExtension()
            {
              ExtensionName = extensionName,
              Publisher = new PublisherFacts()
              {
                PublisherName = publisherName
              }
            };
          IVssRequestContext requestContext1 = requestContext;
          PublishedExtension publishedExtension = extension;
          string action = str;
          service.PublishAppInsightsPerExtensionTelemetryHelper(requestContext1, publishedExtension, action);
        }
        requestContext.Trace(12061092, TraceLevel.Info, "Gallery", nameof (UpdateExtensionValidationStatus), ex.ToString());
        flag = false;
        resultMessage = ex.ToString();
      }
      finally
      {
        requestContext.TraceLeave(12061092, "Gallery", nameof (UpdateExtensionValidationStatus), nameof (UpdateExtensionValidationStatus));
      }
      return flag;
    }

    private PublishedExtension GetExtension(
      IVssRequestContext requestContext,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId)
    {
      PublishedExtension extension = requestContext.GetService<IPublishedExtensionService>().QueryExtensionById(requestContext, extensionId, version, ExtensionQueryFlags.AllAttributes | ExtensionQueryFlags.IncludeMetadata, validationId);
      if (extension != null && extension.Versions != null)
        extension.Versions.RemoveAll((Predicate<ExtensionVersion>) (extensionVersion => !string.Equals(targetPlatform, extensionVersion.TargetPlatform)));
      if (extension == null || extension.Versions == null || extension.Versions.Count != 1)
      {
        requestContext.Trace(12061092, TraceLevel.Info, "Gallery", "ProcessExtension", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found for extension: {0} version {1} targetPlatform {2} validation id:{3}", (object) extensionId, (object) version, (object) targetPlatform, (object) validationId));
        throw new ExtensionDoesNotExistException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Extension not found for extension: {0} version {1} targetPlatform {2} validation id:{3}", (object) extensionId, (object) version, (object) targetPlatform, (object) validationId));
      }
      return extension;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", Justification = "This is comming for tracepoints so ignoring")]
    private Stream GetPackageStream(IVssRequestContext requestContext, PublishedExtension extension)
    {
      Stream packageStream = (Stream) null;
      ExtensionFile extensionFile = (ExtensionFile) null;
      if (extension != null && extension.Versions[0] != null && extension.Versions[0].Files != null)
      {
        foreach (ExtensionFile file in extension.Versions[0].Files)
        {
          if (file.AssetType.Equals("Microsoft.VisualStudio.Services.VSIXPackage"))
          {
            extensionFile = file;
            break;
          }
        }
        if (extensionFile == null)
        {
          foreach (ExtensionFile file in extension.Versions[0].Files)
          {
            if (file.AssetType.Equals("Microsoft.VisualStudio.Ide.Payload", StringComparison.OrdinalIgnoreCase) || file.AssetType.EndsWith("vsix", StringComparison.OrdinalIgnoreCase))
            {
              extensionFile = file;
              break;
            }
          }
        }
      }
      if (extensionFile != null)
      {
        requestContext.Trace(12061092, TraceLevel.Info, "Gallery", "ProcessExtension", "Vsix found for extension");
        packageStream = requestContext.GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) extensionFile.FileId, false, out byte[] _, out long _, out CompressionType _);
      }
      return packageStream;
    }
  }
}
