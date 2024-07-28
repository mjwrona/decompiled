// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionManifestContributionProvider
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ExtensionManifestContributionProvider : IContributionProvider
  {
    private string m_providerDisplayName;
    private string m_providerVersion;
    private ExtensionManifest m_manifest;
    private ContributionData m_contributionData;
    private string m_providerName;
    private string m_registrationId;
    private ContributionProviderDetails m_details;

    public ExtensionManifestContributionProvider(
      string publisherName,
      string publisherDisplayName,
      string publisherVersion,
      string extensionName,
      string registrationId,
      ExtensionManifest manifest)
    {
      this.m_providerDisplayName = publisherDisplayName;
      this.m_providerVersion = publisherVersion;
      this.m_manifest = manifest;
      this.m_registrationId = registrationId;
      this.m_providerName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      this.m_contributionData = new ContributionData()
      {
        Contributions = manifest.Contributions,
        ContributionTypes = manifest.ContributionTypes,
        Constraints = manifest.Constraints
      };
      this.m_details = new ContributionProviderDetails()
      {
        Name = this.m_providerName,
        DisplayName = this.m_providerDisplayName,
        Version = this.m_providerVersion
      };
      this.m_details.Properties.Add("::BaseUri", this.m_manifest.BaseUri);
      this.m_details.Properties.Add("::FallbackBaseUri", this.m_manifest.FallbackBaseUri);
      this.m_details.Properties.Add("::Version", this.m_providerVersion);
      this.m_details.Properties.Add("::RegistrationId", this.m_registrationId);
    }

    public string ProviderName => this.m_providerName;

    public string ProviderDisplayName => this.m_providerDisplayName;

    public IEnumerable<Contribution> Contributions => this.m_manifest.Contributions;

    public IEnumerable<ContributionType> ContributionTypes => this.m_manifest.ContributionTypes;

    public IEnumerable<ContributionConstraint> Constraints => this.m_manifest.Constraints;

    public ContributionData QueryContributionData(IVssRequestContext requestContext) => this.m_contributionData;

    public ContributionProviderDetails QueryProviderDetails(IVssRequestContext requestContext) => this.m_details;
  }
}
