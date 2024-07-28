// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Mapping
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class Mapping
  {
    internal ItemPathPair m_itemPathPair;
    protected WorkingFolderType m_type;
    protected int m_depth = 120;

    public Mapping()
    {
    }

    public Mapping(string serverItem, WorkingFolderType type, int depth)
      : this(ItemPathPair.FromServerItem(serverItem), type, depth)
    {
    }

    internal Mapping(ItemPathPair itemPathPair, WorkingFolderType type, int depth)
    {
      this.ItemPathPair = itemPathPair;
      this.Type = type;
      this.Depth = depth;
    }

    [XmlAttribute("item")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServerItem
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set => this.m_itemPathPair = value;
    }

    [XmlAttribute("type")]
    [DefaultValue(WorkingFolderType.Map)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public WorkingFolderType Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    [XmlAttribute("depth")]
    [DefaultValue(120)]
    [ClientProperty(ClientVisibility.Private)]
    public int Depth
    {
      get => this.m_depth;
      set => this.m_depth = value;
    }

    internal virtual void validate(
      VersionControlRequestContext versionControlRequestContext)
    {
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string serverItem = this.ServerItem;
      versionControlRequestContext.Validation.checkServerItem(ref serverItem, "ServerItem", false, false, true, false, serverPathLength);
      if (Wildcard.IsWildcard(this.ServerItem))
        throw new WildcardNotAllowedException("WorkingFolderWildcard", Array.Empty<object>());
      if (this.m_depth != 120 && this.m_depth != 1)
        throw new WorkingFolderException("WorkingFolderDepthNotSupported", this);
      if (this.m_depth == 1 && this.Type == WorkingFolderType.Cloak)
        throw new WorkingFolderException("WorkingFolderDepthCloakNotSupported", this);
      this.m_itemPathPair = new ItemPathPair(serverItem, this.ItemPathPair.ProjectGuidPath);
    }

    internal bool DoesMatchPattern(string serverItem)
    {
      int folderDepth1 = VersionControlPath.GetFolderDepth(serverItem);
      int folderDepth2 = VersionControlPath.GetFolderDepth(this.ServerItem);
      return folderDepth1 >= folderDepth2 && folderDepth1 <= folderDepth2 + this.m_depth;
    }

    internal static int CompareDepth(Mapping folder1, Mapping folder2)
    {
      string serverItem1 = folder1.ServerItem;
      string serverItem2 = folder2.ServerItem;
      int folderDepth1 = VersionControlPath.GetFolderDepth(serverItem1);
      int folderDepth2 = VersionControlPath.GetFolderDepth(serverItem2);
      return folderDepth1 == folderDepth2 ? VersionControlPath.Compare(serverItem1, serverItem2) : folderDepth1 - folderDepth2;
    }

    internal static void OptimizeSingleRootMappings(List<Mapping> mappings)
    {
      if (mappings == null)
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mappings.Sort(Mapping.\u003C\u003EO.\u003C0\u003E__CompareDepth ?? (Mapping.\u003C\u003EO.\u003C0\u003E__CompareDepth = new Comparison<Mapping>(Mapping.CompareDepth)));
      for (int index1 = 0; index1 < mappings.Count; ++index1)
      {
        Mapping mapping = mappings[index1];
        Mapping parent = (Mapping) null;
        for (int index2 = index1 - 1; index2 >= 0; --index2)
        {
          if (VersionControlPath.IsSubItem(mapping.ServerItem, mappings[index2].ServerItem))
          {
            parent = mappings[index2];
            break;
          }
        }
        int num = 120;
        WorkingFolderType workingFolderType = WorkingFolderType.Map;
        if (parent != null)
        {
          if (Mapping.RedundantMapping(mapping, parent))
          {
            mappings.RemoveAt(index1--);
            continue;
          }
          num = parent.Depth;
          workingFolderType = parent.Type;
        }
        if (num == 120 && mapping.Depth == 120 && workingFolderType == mapping.Type)
          mappings.RemoveAt(index1--);
      }
    }

    internal static void ValidateSingleRootMappings(
      List<Mapping> mappings,
      string rootServerItem,
      VersionControlRequestContext context)
    {
      if (mappings == null)
        return;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      mappings.Sort(Mapping.\u003C\u003EO.\u003C0\u003E__CompareDepth ?? (Mapping.\u003C\u003EO.\u003C0\u003E__CompareDepth = new Comparison<Mapping>(Mapping.CompareDepth)));
      Mapping mapping1 = (Mapping) null;
      foreach (Mapping mapping2 in mappings)
      {
        mapping2.validate(context);
        if (mapping1 != null && VersionControlPath.Equals(mapping1.ServerItem, mapping2.ServerItem))
          throw new WorkingFolderException("FolderMappedTwice", mapping2);
        if (VersionControlPath.Equals(mapping2.ServerItem, rootServerItem) && mapping2.Type == WorkingFolderType.Cloak)
          throw new WorkingFolderException("CannotCloakRoot", mapping2);
        if (!VersionControlPath.IsSubItem(mapping2.ServerItem, rootServerItem))
          throw new WorkingFolderException("BranchMappingNotUnderRoot", mapping2);
        mapping1 = mapping2;
      }
    }

    internal static bool RedundantMapping(Mapping folder, Mapping parent) => parent.Depth == 1 && folder.Type == WorkingFolderType.Cloak && !parent.DoesMatchPattern(folder.ServerItem);
  }
}
