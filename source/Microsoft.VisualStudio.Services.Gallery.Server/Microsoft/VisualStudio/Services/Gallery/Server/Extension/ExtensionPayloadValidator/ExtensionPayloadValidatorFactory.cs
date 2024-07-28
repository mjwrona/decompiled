// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.ExtensionPayloadValidatorFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal class ExtensionPayloadValidatorFactory
  {
    private readonly Dictionary<string, ExtensionDeploymentTechnology> _allowedFileExtensionDeploymentTechnologyMap = new Dictionary<string, ExtensionDeploymentTechnology>()
    {
      {
        ExtensionPayloadValidatorFactory.GetDotPrefixedExtension("VSIX"),
        ExtensionDeploymentTechnology.Vsix
      },
      {
        ExtensionPayloadValidatorFactory.GetDotPrefixedExtension("EXE"),
        ExtensionDeploymentTechnology.Exe
      },
      {
        ExtensionPayloadValidatorFactory.GetDotPrefixedExtension("MSI"),
        ExtensionDeploymentTechnology.Msi
      }
    };
    private readonly Dictionary<ExtensionDeploymentTechnology, IExtensionPayloadValidator> _vsPayloadValidators = new Dictionary<ExtensionDeploymentTechnology, IExtensionPayloadValidator>()
    {
      {
        ExtensionDeploymentTechnology.Vsix,
        (IExtensionPayloadValidator) new VSixPayloadValidator(ExtensionDeploymentTechnology.Vsix)
      },
      {
        ExtensionDeploymentTechnology.Msi,
        (IExtensionPayloadValidator) new MsiExePayloadValidator(ExtensionDeploymentTechnology.Msi)
      },
      {
        ExtensionDeploymentTechnology.Exe,
        (IExtensionPayloadValidator) new MsiExePayloadValidator(ExtensionDeploymentTechnology.Exe)
      },
      {
        ExtensionDeploymentTechnology.ReferralLink,
        (IExtensionPayloadValidator) new ReferralLinkPayloadValidator()
      }
    };

    public virtual IExtensionPayloadValidator GetValidatorForDeploymentTechnology(
      string productType,
      ExtensionDeploymentTechnology deploymentTechnology)
    {
      IExtensionPayloadValidator deploymentTechnology1;
      if (string.Equals(productType, "vs", StringComparison.OrdinalIgnoreCase) && this._vsPayloadValidators.TryGetValue(deploymentTechnology, out deploymentTechnology1))
        return deploymentTechnology1;
      throw new ArgumentException("Invalid payload and product type.");
    }

    public ExtensionDeploymentTechnology GetFileDeploymentTechnology(
      string fileName,
      Stream payloadStream)
    {
      if (fileName.IsNullOrEmpty<char>())
      {
        if (payloadStream.Length == 0L)
          return ExtensionDeploymentTechnology.ReferralLink;
        throw new ArgumentNullException(nameof (fileName));
      }
      string upperInvariant = Path.GetExtension(fileName).ToUpperInvariant();
      if (payloadStream.Length == 0L)
        throw new ArgumentNullException(nameof (payloadStream));
      foreach (KeyValuePair<string, ExtensionDeploymentTechnology> deploymentTechnology in this._allowedFileExtensionDeploymentTechnologyMap)
      {
        if (string.Equals(upperInvariant, deploymentTechnology.Key))
          return deploymentTechnology.Value;
      }
      throw new ArgumentException("Filetype is not supported as deployment type");
    }

    public ExtensionDeploymentTechnology GetDeploymentTechnology(PublishedExtension extension)
    {
      if (extension.Metadata != null)
      {
        foreach (ExtensionMetadata extensionMetadata in extension.Metadata)
        {
          ExtensionDeploymentTechnology result;
          if (string.Equals(extensionMetadata.Key, "DeploymentTechnology") && GalleryConstants.MetaDataDeploymentTechnologyTypes.TryParseExtensionDeploymentTechnology(extensionMetadata.Value, out result))
            return result;
        }
      }
      throw new ArgumentException("Cannot determing the extension deployment technology");
    }

    private static string GetDotPrefixedExtension(string fileExtension) => "." + fileExtension.ToUpperInvariant();
  }
}
