// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionedItemUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.Services.Common.Internal;
using System.Text;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class VersionedItemUri : VersionControlIntegrationUri
  {
    private string m_serverItem;
    private int m_changeset;
    private int m_deletionId;
    private UriType m_type;

    public VersionedItemUri(
      string teamFoundationServerUrl,
      string serverItem,
      int changeset,
      int deletionId)
      : this(teamFoundationServerUrl, serverItem, changeset, deletionId, UriType.Normal)
    {
    }

    public VersionedItemUri(
      string teamFoundationServerUrl,
      string serverItem,
      int changeset,
      int deletionId,
      UriType type)
      : base(teamFoundationServerUrl, ArtifactType.VersionedItem, VersionedItemUri.UrlString(serverItem, changeset, deletionId, type))
    {
      this.m_serverItem = serverItem;
      this.m_changeset = changeset;
      this.m_deletionId = deletionId;
      this.m_type = type;
    }

    public VersionedItemUri(
      TfsTeamProjectCollection teamProjectCollection,
      string serverItem,
      int changeset,
      int deletionId)
      : this(teamProjectCollection, serverItem, changeset, deletionId, UriType.Normal)
    {
    }

    public VersionedItemUri(
      TfsTeamProjectCollection teamProjectCollection,
      string serverItem,
      int changeset,
      int deletionId,
      UriType type)
      : base(teamProjectCollection, ArtifactType.VersionedItem, VersionedItemUri.UrlString(serverItem, changeset, deletionId, type))
    {
      this.m_serverItem = serverItem;
      this.m_changeset = changeset;
      this.m_deletionId = deletionId;
    }

    public VersionedItemUri(string serverItem, int changeset, int deletionId, UriType type)
      : this((TfsTeamProjectCollection) null, serverItem, changeset, deletionId, type)
    {
    }

    public static void Decode(string versionedItemArtifactName, out string serverItem) => VersionedItemUri.Decode(versionedItemArtifactName, out serverItem, out int _, out int _, out UriType _);

    public static void Decode(
      string versionedItemArtifactName,
      out string serverItem,
      out int changeset,
      out int deletionId)
    {
      VersionedItemUri.Decode(versionedItemArtifactName, out serverItem, out changeset, out deletionId, out UriType _);
    }

    public static void Decode(
      string versionedItemArtifactName,
      out string serverItem,
      out int changeset,
      out int deletionId,
      out UriType type)
    {
      serverItem = string.Empty;
      changeset = 0;
      deletionId = 0;
      type = UriType.Normal;
      if (versionedItemArtifactName.IndexOf('&') != -1)
      {
        string[] strArray = versionedItemArtifactName.Split(VersionControlIntegrationUri.s_parameterDelimiters);
        if (!string.IsNullOrEmpty(strArray[0]))
          serverItem = UriUtility.UrlDecode(strArray[0]);
        if (strArray.Length > 1)
        {
          if (!int.TryParse(strArray[1].Split('=')[1], out changeset))
            throw new InvalidUriException(versionedItemArtifactName);
          if (strArray.Length > 2)
          {
            if (!int.TryParse(strArray[2].Split('=')[1], out deletionId))
              throw new InvalidUriException(versionedItemArtifactName);
            if (strArray.Length > 3)
            {
              bool result;
              if (!bool.TryParse(strArray[3].Split('=')[1], out result))
                throw new InvalidUriException(versionedItemArtifactName);
              if (result)
                type = UriType.Extended;
            }
          }
        }
      }
      else
        serverItem = versionedItemArtifactName;
      serverItem = VersionControlPath.PrependRootIfNeeded(serverItem);
    }

    public override string ArtifactTitle => VersionControlIntegrationResources.Format("VersionedItem", (object) this.m_changeset, (object) this.m_serverItem);

    private static string UrlString(
      string serverItem,
      int changeset,
      int deletionId,
      UriType type)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(UriUtility.UrlEncode(serverItem.Substring(2)));
      stringBuilder.Append("&changesetVersion=");
      stringBuilder.Append(changeset);
      stringBuilder.Append("&deletionId=");
      stringBuilder.Append(deletionId);
      if (type == UriType.Extended)
        stringBuilder.Append("&webView=true");
      return UriUtility.UrlEncode(stringBuilder.ToString());
    }
  }
}
