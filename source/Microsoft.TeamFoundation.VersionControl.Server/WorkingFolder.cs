// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkingFolder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class WorkingFolder : Mapping
  {
    internal int workspaceId;
    private string m_localItem;

    public WorkingFolder()
    {
    }

    public WorkingFolder(string serverItem, string localItem, WorkingFolderType type, int depth)
      : base(serverItem, type, depth)
    {
      this.LocalItem = localItem;
    }

    internal WorkingFolder(
      ItemPathPair itemPathPair,
      string localItem,
      WorkingFolderType type,
      int depth)
      : base(itemPathPair, type, depth)
    {
      this.LocalItem = localItem;
    }

    internal override void validate(
      VersionControlRequestContext versionControlRequestContext)
    {
      base.validate(versionControlRequestContext);
      if (this.Type == WorkingFolderType.Cloak)
      {
        this.LocalItem = (string) null;
      }
      else
      {
        versionControlRequestContext.Validation.checkLocalItem(this.m_localItem, "LocalItem", true, false, true, false);
        if (FileSpec.IsWildcard(this.LocalItem))
          throw new WildcardNotAllowedException("WorkingFolderWildcard", Array.Empty<object>());
      }
    }

    [XmlAttribute("local")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LocalItem
    {
      get => this.m_localItem;
      set => this.m_localItem = value;
    }
  }
}
