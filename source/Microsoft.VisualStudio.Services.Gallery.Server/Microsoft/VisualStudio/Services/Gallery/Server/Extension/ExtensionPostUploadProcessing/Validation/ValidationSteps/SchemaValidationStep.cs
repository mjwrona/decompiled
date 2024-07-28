// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.SchemaValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Telemetry;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class SchemaValidationStep : PackageValidationStepBase
  {
    private const PackageValidationStepBase.ValidationStepType s_stepType = PackageValidationStepBase.ValidationStepType.SchemaValidation;
    private bool m_updateExtension;

    public SchemaValidationStep()
      : base(PackageValidationStepBase.ValidationStepType.SchemaValidation)
    {
    }

    public override void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.m_result = ValidationStatus.InProgress;
      bool updateExtension = false;
      try
      {
        this.InternalExecuteVersionValidation(requestContext, extension, packageStream, this.m_validationId, out updateExtension);
        this.m_result = ValidationStatus.Success;
        this.m_updateExtension = updateExtension;
      }
      catch (Exception ex)
      {
        this.m_result = ValidationStatus.Failure;
        this.ResultMessage = ex.Message;
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          string action = "PublishTimeError";
          requestContext.GetService<IGalleryTelemetryHelperService>().PublishAppInsightsPerExtensionTelemetryHelper(requestContext, extension, action);
        }
        requestContext.TraceException(12061094, "Gallery", "ProcessExtension", ex);
      }
    }

    public override void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (this.m_result != ValidationStatus.Success)
        return;
      requestContext.GetService<IPublishedExtensionService>().UpdateExtensionAfterValidation(requestContext, extension, this.m_updateExtension, extension.IsVsExtension());
    }

    private void InternalExecuteVersionValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream,
      Guid validationId,
      out bool updateExtension)
    {
      PackageDetails packageDetails = packageStream != null ? VSIXPackage.Parse(packageStream) : throw new VersionValidationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unable to find vsix manifest extension: {0}.{1} version {2} validation id:{3}", (object) extension.Publisher.PublisherName, (object) extension.ExtensionName, (object) extension.Versions[0].Version, (object) validationId));
      if (packageDetails.Manifest.Installation.Count == 0)
        throw new VersionValidationException("No installation targets found in package");
      updateExtension = this.FetchExtensionDetails(extension, packageDetails);
      using (IDisposableReadOnlyList<IExtensionPublishHandler> extensions = requestContext.GetExtensions<IExtensionPublishHandler>())
      {
        foreach (IExtensionPublishHandler extensionPublishHandler in (IEnumerable<IExtensionPublishHandler>) extensions)
        {
          if (extensionPublishHandler.UpdateExtension(requestContext, extension, packageDetails, packageStream, extension.Versions[0].Version, validationId))
            updateExtension = true;
        }
        foreach (IExtensionPublishHandler extensionPublishHandler in (IEnumerable<IExtensionPublishHandler>) extensions)
          extensionPublishHandler.ValidateExtension(requestContext, extension, packageDetails, packageStream, extension.Versions[0].Version, validationId);
      }
      this.ResultMessage = string.Empty;
    }

    private bool FetchExtensionDetails(PublishedExtension extension, PackageDetails packageDetails)
    {
      if (extension.Versions[0].Properties == null)
      {
        extension.Versions[0].Properties = new List<KeyValuePair<string, string>>();
      }
      else
      {
        for (int index = extension.Versions[0].Properties.Count - 1; index >= 0; --index)
        {
          if (extension.Versions[0].Properties[index].Key != "RegistrationId")
            extension.Versions[0].Properties.RemoveAt(index);
        }
      }
      foreach (Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty property1 in packageDetails.Manifest.Metadata.Properties)
      {
        Microsoft.VisualStudio.Services.Gallery.WebApi.ExtensionProperty property = property1;
        if (-1 == extension.Versions[0].Properties.FindIndex((Predicate<KeyValuePair<string, string>>) (x => x.Key == property.Id)))
          extension.Versions[0].Properties.Add(new KeyValuePair<string, string>(property.Id, property.Value));
      }
      extension.InstallationTargets = packageDetails.Manifest.Installation;
      if (extension.InstallationTargets != null)
        GalleryServerUtil.ParseInstallationTargetVersion((IEnumerable<InstallationTarget>) extension.InstallationTargets);
      return true;
    }
  }
}
