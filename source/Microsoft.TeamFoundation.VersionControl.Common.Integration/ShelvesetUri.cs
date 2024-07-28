// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ShelvesetUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class ShelvesetUri : VersionControlIntegrationUri
  {
    private string m_shelvesetName;
    private string m_shelvesetOwner;

    public ShelvesetUri(
      string teamFoundationServerUrl,
      string shelvesetName,
      string shelvesetOwner)
      : this(teamFoundationServerUrl, shelvesetName, shelvesetOwner, UriType.Normal)
    {
    }

    public ShelvesetUri(
      TfsTeamProjectCollection teamProjectCollection,
      string shelvesetName,
      string shelvesetOwner)
      : this(teamProjectCollection, shelvesetName, shelvesetOwner, UriType.Normal)
    {
    }

    public ShelvesetUri(
      TfsTeamProjectCollection teamProjectCollection,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
      : base(teamProjectCollection, ArtifactType.Shelveset, ShelvesetUri.UrlString(shelvesetName, shelvesetOwner, type))
    {
      this.m_shelvesetName = shelvesetName;
      this.m_shelvesetOwner = shelvesetOwner;
    }

    public ShelvesetUri(
      string teamFoundationServerUrl,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
      : base(teamFoundationServerUrl, ArtifactType.Shelveset, ShelvesetUri.UrlString(shelvesetName, shelvesetOwner, type))
    {
      this.m_shelvesetName = shelvesetName;
      this.m_shelvesetOwner = shelvesetOwner;
    }

    public ShelvesetUri(string shelvesetName, string shelvesetOwner, UriType type)
      : this((TfsTeamProjectCollection) null, shelvesetName, shelvesetOwner, type)
    {
    }

    public static void Decode(
      string shelvesetArtifactName,
      out string shelvesetName,
      out string shelvesetOwner)
    {
      ShelvesetUri.Decode(shelvesetArtifactName, out shelvesetName, out shelvesetOwner, out UriType _);
    }

    public static void Decode(
      string shelvesetArtifactName,
      out string shelvesetName,
      out string shelvesetOwner,
      out UriType type)
    {
      shelvesetName = string.Empty;
      shelvesetOwner = string.Empty;
      type = UriType.Normal;
      string[] strArray1 = shelvesetArtifactName.IndexOf('&') != -1 ? shelvesetArtifactName.Split(VersionControlIntegrationUri.s_parameterDelimiters) : throw new InvalidUriException(shelvesetArtifactName);
      shelvesetName = UriUtility.UrlDecode(strArray1[0]);
      if (string.IsNullOrEmpty(shelvesetName))
        throw new InvalidUriException(shelvesetArtifactName);
      string[] strArray2 = strArray1[1].Split('=');
      shelvesetOwner = strArray2.Length == 2 ? UriUtility.UrlDecode(strArray2[1]) : throw new InvalidUriException(shelvesetArtifactName);
      if (string.IsNullOrEmpty(shelvesetOwner))
        throw new InvalidUriException(shelvesetArtifactName);
      if (strArray1.Length <= 2)
        return;
      string[] strArray3 = strArray1[2].Split('=');
      if (strArray3.Length != 2)
        throw new InvalidUriException(shelvesetArtifactName);
      bool result;
      if (!bool.TryParse(strArray3[1], out result))
        throw new InvalidUriException(shelvesetArtifactName);
      if (!result)
        return;
      type = UriType.Extended;
    }

    private static string UrlString(string shelvesetName, string shelvesetOwner, UriType type)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(UriUtility.UrlEncode(shelvesetName));
      stringBuilder.Append("&shelvesetOwner=");
      stringBuilder.Append(UriUtility.UrlEncode(shelvesetOwner));
      if (type == UriType.Extended)
        stringBuilder.Append("&webView=true");
      return UriUtility.UrlEncode(stringBuilder.ToString());
    }

    public override string ArtifactTitle => VersionControlIntegrationResources.Format("ShelvesetFormat", (object) this.m_shelvesetName, (object) this.m_shelvesetOwner);
  }
}
