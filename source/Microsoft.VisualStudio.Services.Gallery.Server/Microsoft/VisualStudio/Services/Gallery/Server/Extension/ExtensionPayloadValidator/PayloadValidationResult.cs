// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.PayloadValidationResult
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class PayloadValidationResult
  {
    public ExtensionDeploymentTechnology DeploymentTechnology { get; set; }

    public string FileName { get; set; }

    public bool IsValid { get; set; }

    public List<VsixValidationError> VsixValidationErrors { get; set; }

    public List<VsixValidationWarning> VsixValidationWarnings { get; set; }

    public int VsixPrimaryLanguage { get; set; }

    public ExtensionCategory RootCategory { get; set; }

    public string VsixId { get; set; }

    public IDictionary<string, string> VsixMetadata { get; set; }

    public List<InstallationTarget> InstallationTargets { get; set; }

    public bool IsSignedByMicrosoft { get; set; }

    public List<ProjectLocalizedStrings> VsixLocalizedStrings { get; set; }

    public int? VsixScreenShotFileId { get; set; }

    public int? VsixThumbnailFileId { get; set; }

    public VsExtensionType ExtensionType { get; set; }

    public List<KeyValuePair<string, string>> ValidationErrors { get; set; }

    public List<KeyValuePair<string, string>> ValidationWarnings { get; set; }

    public bool IsEditMode { get; set; }

    public bool IsExtensionSdk { get; set; }

    public bool IsExtensionPack { get; set; }

    public bool PayloadVerificationSkipped { get; set; }

    public bool IsPreview { get; set; }
  }
}
