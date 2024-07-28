// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ItemSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class ItemSpec : IValidatable
  {
    private ItemPathPair m_itemPathPair;
    private string m_folder;
    private string m_fileNamePattern;
    private bool m_serverItem;
    private int m_deletionId;
    private RecursionType m_recursionType;
    private bool m_allow8Dot3Path = true;
    private bool m_checkReservedCharacters;
    private bool m_allowWildCard = true;

    public ItemSpec()
    {
    }

    public ItemSpec(string item, RecursionType recursionType)
      : this(item, recursionType, 0)
    {
    }

    public ItemSpec(string item, RecursionType recursionType, int deletionId)
      : this(ItemPathPair.FromServerItem(item), recursionType, 0)
    {
    }

    internal ItemSpec(ItemPathPair itemPathPair, RecursionType recursionType)
      : this(itemPathPair, recursionType, 0)
    {
    }

    internal ItemSpec(ItemPathPair itemPathPair, RecursionType recursionType, int deletionId)
    {
      this.ItemPathPair = itemPathPair;
      this.RecursionType = recursionType;
      this.DeletionId = deletionId;
    }

    [XmlAttribute("item")]
    public string Item
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set
      {
        this.m_itemPathPair = value;
        this.parseItem();
      }
    }

    [XmlAttribute("recurse")]
    [DefaultValue(RecursionType.None)]
    public RecursionType RecursionType
    {
      get => this.m_recursionType;
      set => this.m_recursionType = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(this.Item, this.Item.Length + 64);
      if (this.RecursionType != RecursionType.None || this.DeletionId != 0)
      {
        bool flag = false;
        stringBuilder.Append(" (");
        if (this.RecursionType != RecursionType.None)
        {
          stringBuilder.Append(this.RecursionType.ToString());
          flag = true;
        }
        if (this.DeletionId != 0)
        {
          if (flag)
            stringBuilder.Append(", ");
          stringBuilder.Append(this.DeletionId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        }
        stringBuilder.Append(")");
      }
      return stringBuilder.ToString();
    }

    internal bool isServerItem => this.m_serverItem;

    internal bool isWildcard => this.m_fileNamePattern != null;

    internal bool postMatch(string path)
    {
      if (!this.isWildcard)
        return true;
      return !VersionControlPath.IsServerItem(path) ? FileSpec.MatchFileName(path, this.m_fileNamePattern) : VersionControlPath.MatchFileName(path, this.m_fileNamePattern);
    }

    internal void requireLocalItem()
    {
      if (this.m_serverItem)
        throw new LocalItemRequiredException(this.Item);
    }

    internal void requireServerItem()
    {
      if (!this.m_serverItem)
        throw new ServerItemRequiredException(this.Item);
    }

    internal static string toLocalItem(
      IVssRequestContext requestContext,
      string item,
      Workspace localWorkspace,
      bool honorCloaks)
    {
      if (string.IsNullOrEmpty(item))
        return (string) null;
      if (!VersionControlPath.IsServerItem(item))
        return item;
      if (localWorkspace == null)
        throw new LocalItemRequiredException(item);
      return localWorkspace.ServerToLocalItem(requestContext, item, honorCloaks);
    }

    internal string toLocalItem(IVssRequestContext requestContext, Workspace localWorkspace) => ItemSpec.toLocalItem(requestContext, this.Item, localWorkspace, true);

    internal static ItemPathPair toServerItem(
      IVssRequestContext requestContext,
      ItemPathPair itemPathPair,
      Workspace localWorkspace,
      bool honorCloaks)
    {
      if (string.IsNullOrEmpty(itemPathPair.ProjectNamePath))
        return new ItemPathPair();
      if (VersionControlPath.IsServerItem(itemPathPair.ProjectNamePath))
        return itemPathPair;
      if (localWorkspace == null)
        throw new ServerItemRequiredException(itemPathPair.ProjectNamePath);
      return localWorkspace.LocalToServerItemPathPair(requestContext, itemPathPair.ProjectNamePath, honorCloaks);
    }

    internal static ItemPathPair toServerItem(
      IVssRequestContext requestContext,
      string item,
      Workspace localWorkspace,
      bool honorCloaks)
    {
      return ItemSpec.toServerItem(requestContext, ItemPathPair.FromServerItem(item), localWorkspace, honorCloaks);
    }

    internal ItemPathPair toServerItem(
      IVssRequestContext requestContext,
      Workspace localWorkspace,
      bool honorCloaks)
    {
      return ItemSpec.toServerItem(requestContext, this.ItemPathPair, localWorkspace, honorCloaks);
    }

    internal ItemPathPair toServerItem(IVssRequestContext requestContext, Workspace localWorkspace) => ItemSpec.toServerItem(requestContext, this.ItemPathPair, localWorkspace, true);

    internal ItemPathPair toServerItemWithoutMappingRenames(
      VersionControlRequestContext versionControlRequestContext,
      Workspace localWorkspace,
      bool honorCloaks)
    {
      if (string.IsNullOrEmpty(this.Item))
        return new ItemPathPair();
      if (VersionControlPath.IsServerItem(this.Item))
        return this.ItemPathPair;
      if (localWorkspace == null)
        throw new ServerItemRequiredException(this.Item);
      return localWorkspace.LocalToCommittedServerItem(versionControlRequestContext, this.Item, honorCloaks);
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      if (this.isWildcard && this.RecursionType == RecursionType.None)
        this.RecursionType = RecursionType.OneLevel;
      string str1 = this.Item != null && this.Item.Length != 0 ? this.Item : throw new ItemRequiredException();
      string projectNamePath = this.Item;
      if (this.m_serverItem)
      {
        PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
        versionControlRequestContext.Validation.checkServerItem(ref projectNamePath, "Item", false, this.m_allowWildCard, this.m_allow8Dot3Path, this.m_checkReservedCharacters, serverPathLength);
      }
      else
        versionControlRequestContext.Validation.checkLocalItem(projectNamePath, "Item", false, this.m_allowWildCard, this.m_allow8Dot3Path, this.m_checkReservedCharacters);
      string str2 = projectNamePath;
      if ((object) str1 == (object) str2)
        return;
      this.m_itemPathPair = new ItemPathPair(projectNamePath, this.ItemPathPair.ProjectGuidPath);
      this.parseItem();
    }

    private void parseItem()
    {
      if (this.Item != null)
      {
        this.m_serverItem = VersionControlPath.IsServerItem(this.Item);
        if ((this.m_serverItem ? (VersionControlPath.IsWildcard(this.Item) ? 1 : 0) : (FileSpec.IsWildcard(this.Item) ? 1 : 0)) != 0)
        {
          if (this.m_serverItem)
            VersionControlPath.Parse(this.Item, out this.m_folder, out this.m_fileNamePattern);
          else
            FileSpec.Parse(this.Item, out this.m_folder, out this.m_fileNamePattern);
        }
        else
        {
          this.m_fileNamePattern = (string) null;
          this.m_folder = (string) null;
        }
      }
      else
      {
        this.m_serverItem = false;
        this.m_fileNamePattern = (string) null;
        this.m_folder = (string) null;
      }
    }

    internal void SetValidationOptions(
      bool allow8Dot3Path,
      bool checkReservedCharacters,
      bool allowWildCards)
    {
      this.m_allow8Dot3Path = allow8Dot3Path;
      this.m_checkReservedCharacters = checkReservedCharacters;
      this.m_allowWildCard = allowWildCards;
    }
  }
}
