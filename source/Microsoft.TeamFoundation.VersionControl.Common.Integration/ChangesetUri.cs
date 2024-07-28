// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ChangesetUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class ChangesetUri : VersionControlIntegrationUri
  {
    public ChangesetUri(string teamFoundationServerUrl, int changeset)
      : this(teamFoundationServerUrl, changeset, UriType.Normal)
    {
    }

    public ChangesetUri(TfsTeamProjectCollection teamProjectCollection, int changeset)
      : this(teamProjectCollection, changeset, UriType.Normal)
    {
    }

    public ChangesetUri(
      TfsTeamProjectCollection teamProjectCollection,
      int changeset,
      UriType type)
      : base(teamProjectCollection, ArtifactType.Changeset, type == UriType.Extended ? changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "&webView=true" : changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
    }

    public ChangesetUri(string teamFoundationServerUrl, int changeset, UriType type)
      : base(teamFoundationServerUrl, ArtifactType.Changeset, type == UriType.Extended ? changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "&webView=true" : changeset.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
    }

    public ChangesetUri(int changeset, UriType type)
      : this((TfsTeamProjectCollection) null, changeset, type)
    {
    }

    public static void Decode(string changesetArtifactName, out int changesetId)
    {
      changesetId = 0;
      string s = changesetArtifactName.IndexOf('&') <= 0 ? changesetArtifactName : changesetArtifactName.Split(VersionControlIntegrationUri.s_parameterDelimiters)[0];
      if (!string.IsNullOrEmpty(s) && !int.TryParse(s, out changesetId))
        throw new InvalidUriException(changesetArtifactName);
    }

    public override string ArtifactTitle => VersionControlIntegrationResources.Format("ChangesetFormat", (object) this.m_artifactName);
  }
}
