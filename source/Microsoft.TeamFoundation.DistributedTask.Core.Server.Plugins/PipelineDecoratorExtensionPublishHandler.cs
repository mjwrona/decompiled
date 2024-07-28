// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.PipelineDecoratorExtensionPublishHandler
// Assembly: Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6486B3F7-B3D2-46E4-8024-05D53FB42B10
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins
{
  public class PipelineDecoratorExtensionPublishHandler : IExtensionPublishHandler
  {
    private readonly string m_galleryFlagsElementName = "GalleryFlags";
    private readonly string m_galleryFlagsPublic = "Public";
    private readonly string m_pipelineDecoratorContributionType = "ms.azure-pipelines.pipeline-decorator";

    public bool UpdateExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PackageDetails packageDetails,
      Stream packageStream,
      string version,
      Guid validationId)
    {
      return false;
    }

    public void ValidateExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      PackageDetails packageDetails,
      Stream packageStream,
      string version,
      Guid validationId)
    {
      bool extensionContainsPipelineDecorators = false;
      bool extensionIsPublic = false;
      VSIXPackage.Parse(packageStream, (Func<ManifestFile, Stream, bool>) ((manifestFile, fileStream) =>
      {
        if (manifestFile.AssetType == "Microsoft.VisualStudio.Services.Manifest")
        {
          ExtensionManifest extensionManifest = JsonUtility.Deserialize<ExtensionManifest>(fileStream);
          if (extensionManifest != null && extensionManifest.Contributions != null)
          {
            foreach (Contribution contribution in extensionManifest.Contributions)
            {
              if (string.Equals(contribution.Type, this.m_pipelineDecoratorContributionType, StringComparison.OrdinalIgnoreCase))
              {
                extensionContainsPipelineDecorators = true;
                return false;
              }
            }
          }
        }
        if (manifestFile.AssetType == "Microsoft.VisualStudio.Services.VsixManifest")
        {
          using (StreamReader streamReader = new StreamReader(fileStream))
          {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(streamReader.ReadToEnd());
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(this.m_galleryFlagsElementName);
            if (elementsByTagName != null)
            {
              if (elementsByTagName.Count > 0)
              {
                XmlNode xmlNode = elementsByTagName[0];
                if (!string.IsNullOrEmpty(xmlNode.InnerText))
                {
                  if (((IEnumerable<string>) xmlNode.InnerText.Split(' ')).Any<string>((Func<string, bool>) (flag => flag.Equals(this.m_galleryFlagsPublic, StringComparison.OrdinalIgnoreCase))))
                  {
                    extensionIsPublic = true;
                    return false;
                  }
                }
              }
            }
          }
        }
        return false;
      }));
      if (extensionContainsPipelineDecorators & extensionIsPublic)
        throw new ExtensionIsPublicAndHasPipelineDecoratorsException(Resources.ExtensionIsPublicAndHasPipelineDecorators((object) extension.ExtensionId));
    }
  }
}
