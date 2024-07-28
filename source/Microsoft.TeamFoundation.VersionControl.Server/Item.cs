// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Item
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [ClassNotSealed]
  [CallOnDeserialization("AfterDeserialize")]
  public class Item : ISignable, ICacheable, IPropertyMergerItem
  {
    internal int fileId;
    private int m_changesetId;
    private DateTime m_checkinDate;
    private int m_deletionId;
    private int m_encoding = -2;
    private ItemType m_itemType;
    private int m_itemId;
    private ItemPathPair m_itemPathPair;
    private string m_timeZone;
    private string m_timeZoneOffset;
    private byte[] m_hashValue;
    private long m_fileLength;
    private string m_downloadUrl;
    private bool m_isBranch;

    public Item()
    {
    }

    internal Item(WorkspaceItem item)
    {
      this.ChangesetId = item.ChangesetId;
      this.CheckinDate = item.CheckinDate;
      this.FileLength = item.FileLength;
      this.DeletionId = item.DeletionId;
      this.DownloadUrl = item.DownloadUrl;
      this.fileId = item.fileId;
      this.Encoding = item.Encoding;
      this.HashValue = item.HashValue;
      this.ItemId = item.ItemId;
      this.ItemType = item.ItemType;
      this.ItemPathPair = item.ItemPathPair;
      this.PropertyId = item.PropertyId;
      this.ItemDataspaceId = item.ItemDataspaceId;
    }

    [XmlAttribute("cs")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int ChangesetId
    {
      get => this.m_changesetId;
      set => this.m_changesetId = value;
    }

    [XmlAttribute("date")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime CheckinDate
    {
      get => this.m_checkinDate;
      set => this.m_checkinDate = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    [XmlAttribute("enc")]
    [DefaultValue(-2)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int Encoding
    {
      get => this.m_encoding;
      set => this.m_encoding = value;
    }

    [XmlAttribute("type")]
    [DefaultValue(ItemType.Any)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ItemType ItemType
    {
      get => this.m_itemType;
      set => this.m_itemType = value;
    }

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
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

    [XmlAttribute("tz")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string TimeZone
    {
      get => this.m_timeZone;
      set => this.m_timeZone = value;
    }

    [XmlAttribute("tzo")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string TimeZoneOffset
    {
      get => this.m_timeZoneOffset;
      set => this.m_timeZoneOffset = value;
    }

    [XmlAttribute("hash")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public byte[] HashValue
    {
      get => this.m_hashValue;
      set => this.m_hashValue = value;
    }

    [XmlAttribute("len")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, PropertyName = "ContentLength")]
    public long FileLength
    {
      get => this.m_fileLength;
      set => this.m_fileLength = value;
    }

    [XmlAttribute("durl")]
    [ClientProperty(ClientVisibility.Private)]
    public string DownloadUrl
    {
      get => this.m_downloadUrl;
      set => this.m_downloadUrl = value;
    }

    [XmlIgnore]
    public bool IsFolder => this.ItemType == ItemType.Folder;

    [DefaultValue(false)]
    [XmlAttribute("isbranch")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsBranch
    {
      get => this.m_isBranch;
      set => this.m_isBranch = value;
    }

    public int GetCachedSize() => 700;

    [XmlIgnore]
    public int SequenceId { get; set; }

    [XmlIgnore]
    internal int PropertyId { get; set; }

    [XmlIgnore]
    internal Guid ItemDataspaceId { get; set; }

    [Obsolete("Please use the Attributes property instead", false)]
    [XmlIgnore]
    public StreamingCollection<PropertyValue> Properties
    {
      get => this.Attributes;
      set => this.Attributes = value;
    }

    [XmlArray("Properties")]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalAttributes")]
    public StreamingCollection<PropertyValue> Attributes { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalPropertyValues")]
    public StreamingCollection<PropertyValue> PropertyValues { get; set; }

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => object.Equals((object) artifactKind, (object) VersionControlPropertyKinds.VersionedItem) || object.Equals((object) artifactKind, (object) VersionControlPropertyKinds.Annotation) ? (this.ChangesetId == 0 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.ItemId, this.QueryUnversionedProperties ? 0 : this.ChangesetId, this.ItemDataspaceId)) : (this.PropertyId == -1 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.PropertyId, 0, this.ItemDataspaceId));

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => artifactKind.Equals(VersionControlPropertyKinds.VersionedItem) || artifactKind.Equals(VersionControlPropertyKinds.Annotation) ? this.Attributes : this.PropertyValues;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      if (artifactKind.Equals(VersionControlPropertyKinds.VersionedItem) || artifactKind.Equals(VersionControlPropertyKinds.Annotation))
        this.Attributes = properties;
      else
        this.PropertyValues = properties;
    }

    internal bool QueryUnversionedProperties { get; set; }

    [XmlIgnore]
    public bool IsContentDestroyed => this.fileId == 1023;

    internal static Item QueryItem(
      VersionControlRequestContext versionControlRequestContext,
      int itemId,
      ChangesetVersionSpec versionTo,
      bool generateDownloadUrl)
    {
      Item[] objArray = Item.QueryItems(versionControlRequestContext, new int[1]
      {
        itemId
      }, versionTo, (generateDownloadUrl ? 1 : 0) != 0, 0);
      return objArray[0] != null ? objArray[0] : throw new ItemNotFoundException(itemId.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    }

    internal static Item[] QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      int[] itemIds,
      ChangesetVersionSpec versionTo,
      bool generateDownloadUrl,
      int options)
    {
      Item[] objArray = new Item[itemIds.Length];
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        using (versionControlRequestContext.RequestContext.AcquireExemptionLock())
        {
          ResultCollection resultCollection = versionedItemComponent.QueryServerItemByItemIds(itemIds, versionTo.ChangesetId, options);
          ObjectBinder<Item> current1 = resultCollection.GetCurrent<Item>();
          bool flag1 = versionControlRequestContext.GetPrivilegeSecurity().HasPermission(versionControlRequestContext.RequestContext, SecurityConstants.GlobalSecurityResource, 32, false);
          int index = 0;
          using (UrlSigner urlSigner = new UrlSigner(versionControlRequestContext.RequestContext))
          {
            while (current1.MoveNext())
            {
              if (index < itemIds.Length)
              {
                Item current2 = current1.Current;
                Item obj;
                try
                {
                  bool flag2 = versionControlRequestContext.VersionControlService.SecurityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, current2.ItemPathPair);
                  if (!flag2 && !flag1)
                    throw new ItemNotFoundException(current2.ServerItem);
                  while (current2.ItemId != itemIds[index] && index < itemIds.Length)
                    ++index;
                  if (flag2 & generateDownloadUrl)
                    urlSigner.SignObject((ISignable) current2);
                  if (index < itemIds.Length)
                  {
                    objArray[index] = current2;
                    ++index;
                    resultCollection.IncrementRowCounter();
                  }
                }
                catch (ItemNotFoundException ex)
                {
                  versionControlRequestContext.RequestContext.TraceException(700092, TraceLevel.Info, TraceArea.QueryItems, TraceLayer.BusinessLogic, (Exception) ex);
                  obj = (Item) null;
                }
                catch (ResourceAccessException ex)
                {
                  versionControlRequestContext.RequestContext.TraceException(700093, TraceLevel.Info, TraceArea.QueryItems, TraceLayer.BusinessLogic, (Exception) ex);
                  obj = (Item) null;
                }
              }
              else
                break;
            }
          }
        }
      }
      return objArray;
    }

    public int GetDownloadUrlCount() => 1;

    public int GetFileId(int index) => this.fileId;

    public byte[] GetHashValue(int index) => this.m_hashValue;

    public void SetDownloadUrl(int index, string downloadUrl) => this.DownloadUrl = downloadUrl;

    internal virtual bool HasPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions required)
    {
      return this.HasPermission(versionControlRequestContext, required, false);
    }

    internal virtual bool HasPermission(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemPermissions required,
      bool alwaysAllowAdmins)
    {
      return this.ServerItem == null || versionControlRequestContext.VersionControlService.SecurityWrapper.HasItemPermission(versionControlRequestContext, required, this.ItemPathPair, alwaysAllowAdmins);
    }

    internal bool MatchDeletedState(DeletedState deletedState)
    {
      if (deletedState == DeletedState.NonDeleted)
        return this.DeletionId == 0;
      return deletedState != DeletedState.Deleted || this.DeletionId != 0;
    }
  }
}
