// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.ShelvedItemUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class ShelvedItemUri : VersionControlIntegrationUri
  {
    private string m_serverItem;
    private string m_shelvesetName;
    private string m_shelvesetOwner;
    private UriType m_type;

    public ShelvedItemUri(
      string teamFoundationServerUrl,
      string serverItem,
      string shelvesetName,
      string shelvesetOwner)
      : this(teamFoundationServerUrl, serverItem, shelvesetName, shelvesetOwner, UriType.Normal)
    {
    }

    public ShelvedItemUri(
      string teamFoundationServerUrl,
      string serverItem,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
      : base(teamFoundationServerUrl, ArtifactType.ShelvedItem, ShelvedItemUri.UrlString(serverItem, shelvesetName, shelvesetOwner, type))
    {
      this.m_serverItem = serverItem;
      this.m_shelvesetName = shelvesetName;
      this.m_shelvesetOwner = shelvesetOwner;
      this.m_type = type;
    }

    public ShelvedItemUri(
      TfsTeamProjectCollection teamProjectCollection,
      string serverItem,
      string shelvesetName,
      string shelvesetOwner)
      : this(teamProjectCollection, serverItem, shelvesetName, shelvesetOwner, UriType.Normal)
    {
    }

    public ShelvedItemUri(
      TfsTeamProjectCollection teamProjectCollection,
      string serverItem,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
      : base(teamProjectCollection, ArtifactType.ShelvedItem, ShelvedItemUri.UrlString(serverItem, shelvesetName, shelvesetOwner, type))
    {
      this.m_serverItem = serverItem;
      this.m_shelvesetName = shelvesetName;
      this.m_shelvesetOwner = shelvesetOwner;
    }

    public ShelvedItemUri(
      string serverItem,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
      : this((TfsTeamProjectCollection) null, serverItem, shelvesetName, shelvesetOwner, type)
    {
    }

    public static void Decode(
      string shelvedItemArtifactName,
      out string serverItem,
      out string shelvesetName,
      out string shelvesetOwner)
    {
      ShelvedItemUri.Decode(shelvedItemArtifactName, out serverItem, out shelvesetName, out shelvesetOwner, out UriType _);
    }

    public static void Decode(
      string shelvedItemArtifactName,
      out string serverItem,
      out string shelvesetName,
      out string shelvesetOwner,
      out UriType type)
    {
      serverItem = string.Empty;
      shelvesetName = string.Empty;
      shelvesetOwner = string.Empty;
      type = UriType.Normal;
      string[] strArray1 = shelvedItemArtifactName.IndexOf('&') != -1 ? shelvedItemArtifactName.Split(VersionControlIntegrationUri.s_parameterDelimiters) : throw new InvalidUriException(shelvedItemArtifactName);
      serverItem = UriUtility.UrlDecode(strArray1[0]);
      string[] strArray2 = !string.IsNullOrEmpty(strArray1[0]) ? strArray1[1].Split('=') : throw new InvalidUriException(shelvedItemArtifactName);
      shelvesetName = strArray2.Length == 2 ? UriUtility.UrlDecode(strArray2[1]) : throw new InvalidUriException(shelvedItemArtifactName);
      if (string.IsNullOrEmpty(shelvesetName))
        throw new InvalidUriException(shelvedItemArtifactName);
      string[] strArray3 = strArray1.Length > 2 ? strArray1[2].Split('=') : throw new InvalidUriException(shelvedItemArtifactName);
      shelvesetOwner = strArray3.Length == 2 ? UriUtility.UrlDecode(strArray3[1]) : throw new InvalidUriException(shelvedItemArtifactName);
      if (string.IsNullOrEmpty(shelvesetOwner))
        throw new InvalidUriException(shelvedItemArtifactName);
      if (strArray1.Length > 3)
      {
        string[] strArray4 = strArray1[3].Split('=');
        if (strArray4.Length != 2)
          throw new InvalidUriException(shelvedItemArtifactName);
        bool result;
        if (!bool.TryParse(strArray4[1], out result))
          throw new InvalidUriException(shelvedItemArtifactName);
        if (result)
          type = UriType.Extended;
      }
      serverItem = VersionControlPath.PrependRootIfNeeded(serverItem);
    }

    public override string ArtifactTitle => VersionControlIntegrationResources.Format("ShelvedItem", (object) this.m_shelvesetName, (object) this.m_shelvesetOwner, (object) this.m_serverItem);

    private static string UrlString(
      string serverItem,
      string shelvesetName,
      string shelvesetOwner,
      UriType type)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(UriUtility.UrlEncode(serverItem?.Substring(2)));
      stringBuilder.Append("&shelvesetName=");
      stringBuilder.Append(UriUtility.UrlEncode(shelvesetName));
      stringBuilder.Append("&shelvesetOwner=");
      stringBuilder.Append(UriUtility.UrlEncode(shelvesetOwner));
      if (type == UriType.Extended)
        stringBuilder.Append("&webView=true");
      return UriUtility.UrlEncode(stringBuilder.ToString());
    }
  }
}
