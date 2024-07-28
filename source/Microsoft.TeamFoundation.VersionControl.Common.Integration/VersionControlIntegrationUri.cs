// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionControlIntegrationUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class VersionControlIntegrationUri
  {
    protected const char ParameterDelimiterCharacter = '&';
    protected const char ValueDelimiterCharacter = '=';
    protected static char[] s_parameterDelimiters = new char[1]
    {
      '&'
    };
    private string m_teamFoundationServerUrl;
    private ILinking m_linkingProvider;
    protected ArtifactType m_artifactType;
    protected string m_artifactName;

    public VersionControlIntegrationUri(
      string teamFoundationServerUrl,
      ArtifactType artifactType,
      string artifactName)
    {
      this.m_artifactName = artifactName;
      this.m_artifactType = artifactType;
      if (string.IsNullOrEmpty(teamFoundationServerUrl))
        return;
      this.m_teamFoundationServerUrl = teamFoundationServerUrl;
      this.m_linkingProvider = (ILinking) this.GetTeamProjectCollection(teamFoundationServerUrl).GetService(typeof (ILinking));
    }

    public VersionControlIntegrationUri(
      TfsTeamProjectCollection teamProjectCollection,
      ArtifactType artifactType,
      string artifactName)
    {
      this.m_artifactName = artifactName;
      this.m_artifactType = artifactType;
      if (teamProjectCollection == null)
        return;
      this.m_linkingProvider = (ILinking) teamProjectCollection.GetService(typeof (ILinking));
      this.m_teamFoundationServerUrl = teamProjectCollection.Uri.AbsoluteUri;
    }

    public VersionControlIntegrationUri(ILinking linkingProvider, string artifactUri) => this.InitializeWithLinkingProvider(linkingProvider, artifactUri);

    private void InitializeWithLinkingProvider(ILinking linkingProvider, string artifactUri)
    {
      ArtifactId artifactId;
      try
      {
        artifactId = LinkingUtilities.DecodeUri(artifactUri);
      }
      catch (InvalidUriException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new InvalidUriException(artifactUri, ex);
      }
      this.m_artifactType = this.string2artifactType(artifactId.ArtifactType);
      this.m_artifactName = artifactId.ToolSpecificId;
      this.m_teamFoundationServerUrl = artifactId.VisualStudioServerNamespace;
      this.m_linkingProvider = linkingProvider;
    }

    public VersionControlIntegrationUri(string teamFoundationServerUrl, string artifactUri)
    {
      ILinking linkingProvider = (ILinking) null;
      if (!string.IsNullOrEmpty(teamFoundationServerUrl))
        linkingProvider = (ILinking) this.GetTeamProjectCollection(teamFoundationServerUrl).GetService(typeof (ILinking));
      this.InitializeWithLinkingProvider(linkingProvider, artifactUri);
    }

    public VersionControlIntegrationUri(
      TfsTeamProjectCollection teamProjectCollection,
      string bisUri)
    {
      ILinking linkingProvider = (ILinking) null;
      if (teamProjectCollection != null)
        linkingProvider = (ILinking) teamProjectCollection.GetService(typeof (ILinking));
      this.InitializeWithLinkingProvider(linkingProvider, bisUri);
    }

    public VersionControlIntegrationUri(string artifactUri)
      : this((TfsTeamProjectCollection) null, artifactUri)
    {
    }

    public ArtifactType ArtifactType => this.m_artifactType;

    public string TeamFoundationServerUrl => this.m_teamFoundationServerUrl;

    public string ArtifactName => this.m_artifactName;

    public virtual string ArtifactTitle => this.m_artifactType.ToString() + " " + this.m_artifactName;

    public override string ToString() => this.ArtifactUri;

    public Uri Uri => new Uri(this.ArtifactUri);

    public string ToUrl() => this.ArtifactUrl;

    private ArtifactType string2artifactType(string artifactType)
    {
      if (VssStringComparer.ArtifactType.Equals(artifactType, "VersionedItem"))
        return ArtifactType.VersionedItem;
      if (VssStringComparer.ArtifactType.Equals(artifactType, "Changeset"))
        return ArtifactType.Changeset;
      if (VssStringComparer.ArtifactType.Equals(artifactType, "Label"))
        return ArtifactType.Label;
      if (VssStringComparer.ArtifactType.Equals(artifactType, "LatestItemVersion"))
        return ArtifactType.LatestItemVersion;
      if (VssStringComparer.ArtifactType.Equals(artifactType, "Shelveset"))
        return ArtifactType.Shelveset;
      if (VssStringComparer.ArtifactType.Equals(artifactType, "ShelvedItem"))
        return ArtifactType.ShelvedItem;
      throw new InvalidArtifactTypeException(artifactType);
    }

    private TfsTeamProjectCollection GetTeamProjectCollection(string teamProjectCollectionUrl) => TfsTeamProjectCollectionFactory.GetTeamProjectCollection(TfsTeamProjectCollection.GetFullyQualifiedUriForName(teamProjectCollectionUrl));

    private string ArtifactUrl => this.m_linkingProvider != null ? this.m_linkingProvider.GetArtifactUrlExternal(this.ArtifactUri) : (string) null;

    private string ArtifactUri => LinkingUtilities.EncodeUri(this.ArtifactId);

    public ArtifactId ArtifactId => new ArtifactId()
    {
      ArtifactType = this.m_artifactType.ToString(),
      VisualStudioServerNamespace = this.m_teamFoundationServerUrl,
      ToolSpecificId = this.m_artifactName,
      Tool = "VersionControl"
    };
  }
}
