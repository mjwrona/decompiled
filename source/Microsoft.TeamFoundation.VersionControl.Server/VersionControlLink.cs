// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionControlLink
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class VersionControlLink : IValidatable, IComparable<VersionControlLink>
  {
    internal int ShelvesetId;
    private int m_linkType;
    private string m_url;

    [XmlAttribute("type")]
    public int LinkType
    {
      get => this.m_linkType;
      set => this.m_linkType = value;
    }

    [XmlAttribute("url")]
    public string Url
    {
      get => this.m_url;
      set => this.m_url = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkUrl(this.m_url, "url", false);
    }

    public int CompareTo(VersionControlLink other)
    {
      if (other == null)
        return 1;
      return this.LinkType != other.LinkType ? this.LinkType - other.LinkType : VssStringComparer.Url.Compare(this.Url, other.Url);
    }

    internal static bool AreEqual(VersionControlLink[] array1, VersionControlLink[] array2)
    {
      if (array1 == null && array2 == null)
        return true;
      if (array1 == null || array2 == null || array1.Length != array2.Length)
        return false;
      Array.Sort<VersionControlLink>(array1);
      Array.Sort<VersionControlLink>(array2);
      for (int index = 0; index < array1.Length; ++index)
      {
        VersionControlLink versionControlLink = array1[index];
        VersionControlLink other = array2[index];
        if (versionControlLink == null && other != null || versionControlLink.CompareTo(other) != 0)
          return false;
      }
      return true;
    }
  }
}
