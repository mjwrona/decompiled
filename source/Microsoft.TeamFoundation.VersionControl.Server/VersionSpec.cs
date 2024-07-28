// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.VersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [XmlInclude(typeof (ChangesetVersionSpec))]
  [XmlInclude(typeof (DateVersionSpec))]
  [XmlInclude(typeof (LabelVersionSpec))]
  [XmlInclude(typeof (LatestVersionSpec))]
  [XmlInclude(typeof (WorkspaceVersionSpec))]
  public abstract class VersionSpec : IValidatable, IComparable<VersionSpec>
  {
    internal ResultCollection m_rc;
    internal static readonly ServerVersionSpecFactory ServerVersionSpecFactory = new ServerVersionSpecFactory();
    protected bool m_useMappings;
    public static readonly int UnknownChangeset = -1;
    public static readonly string Separator = ";";
    internal static readonly char DeletionModifier = 'X';

    internal virtual void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      VersionedItemComponent db,
      DeletedState deletedState,
      ItemType itemType,
      IList items,
      out string queryPath,
      out string filePattern,
      int options)
    {
      this.QueryItems(versionControlRequestContext, itemSpec, localWorkspace, db, deletedState, itemType, out queryPath, out filePattern, options);
      Item obj;
      while (this.TryGetNextItem(out obj))
        items.Add((object) obj);
    }

    internal virtual void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      VersionedItemComponent db,
      DeletedState deletedState,
      ItemType itemType,
      out string queryPath,
      out string filePattern,
      int options)
    {
      int changeset = this.ToChangeset(versionControlRequestContext.RequestContext);
      if (changeset == VersionSpec.UnknownChangeset)
        throw new NotSupportedException();
      this.m_rc = !this.m_useMappings ? (!itemSpec.isServerItem ? db.QueryItemsLocal(localWorkspace, itemSpec.Item, changeset, itemSpec.RecursionType, deletedState, itemType, options) : db.QueryItems(itemSpec.ItemPathPair, changeset, itemSpec.RecursionType, deletedState, itemType, options)) : db.QueryMappedItems(localWorkspace, itemSpec.ItemPathPair, itemSpec.RecursionType, options);
      DetermineItemTypeColumns current = (DetermineItemTypeColumns) this.m_rc.GetCurrent<DeterminedItem>();
      current.MoveNext();
      queryPath = current.Current.QueryPath;
      filePattern = current.Current.FilePattern;
      this.m_rc.NextResult();
    }

    internal virtual void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      DeletedState deletedState,
      ItemType itemType,
      IList items,
      out string queryPath,
      out string filePattern,
      int options)
    {
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        this.QueryItems(versionControlRequestContext, itemSpec, localWorkspace, versionedItemComponent, deletedState, itemType, out queryPath, out filePattern, options);
        Item obj;
        while (this.TryGetNextItem(out obj))
          items.Add((object) obj);
      }
    }

    internal virtual void QueryItems(
      VersionControlRequestContext versionControlRequestContext,
      ItemSpec itemSpec,
      Workspace localWorkspace,
      DeletedState deletedState,
      ItemType itemType,
      out string queryPath,
      out string filePattern,
      int options)
    {
      int changeset = this.ToChangeset(versionControlRequestContext.RequestContext);
      if (changeset == VersionSpec.UnknownChangeset)
        throw new NotSupportedException();
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        this.m_rc = !this.m_useMappings ? (!itemSpec.isServerItem ? versionedItemComponent.QueryItemsLocal(localWorkspace, itemSpec.Item, changeset, itemSpec.RecursionType, deletedState, itemType, options) : versionedItemComponent.QueryItems(itemSpec.ItemPathPair, changeset, itemSpec.RecursionType, deletedState, itemType, options)) : versionedItemComponent.QueryMappedItems(localWorkspace, itemSpec.ItemPathPair, itemSpec.RecursionType, options);
        DetermineItemTypeColumns current = (DetermineItemTypeColumns) this.m_rc.GetCurrent<DeterminedItem>();
        current.MoveNext();
        queryPath = current.Current.QueryPath;
        filePattern = current.Current.FilePattern;
        this.m_rc.NextResult();
      }
    }

    internal virtual bool TryGetNextItem(out Item item)
    {
      bool nextItem = false;
      item = (Item) null;
      if (this.m_rc != null)
      {
        if (this.m_useMappings)
        {
          nextItem = this.m_rc.GetCurrent<MappedItem>().MoveNext();
          if (nextItem)
            item = (Item) this.m_rc.GetCurrent<MappedItem>().Current;
        }
        else
        {
          nextItem = this.m_rc.GetCurrent<Item>().MoveNext();
          if (nextItem)
            item = this.m_rc.GetCurrent<Item>().Current;
        }
        if (!nextItem)
        {
          this.m_rc.Dispose();
          this.m_rc = (ResultCollection) null;
        }
      }
      return nextItem;
    }

    public static VersionSpec ParseSingleSpec(string versionSpec, string user) => VersionSpec.ParseSingleSpec(versionSpec, user, user);

    public static VersionSpec ParseSingleSpec(string versionSpec, string user, string userDisplay) => (VersionSpec) VersionSpecCommon.ParseSingleSpec((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, versionSpec, user, userDisplay);

    public static VersionSpec[] Parse(string versionSpec, string user) => VersionSpec.Parse(versionSpec, user, user);

    public static VersionSpec[] Parse(string versionSpec, string userDisplay, string userUnique)
    {
      object[] sourceArray = VersionSpecCommon.Parse((VersionSpecFactory) VersionSpec.ServerVersionSpecFactory, versionSpec, userDisplay, userUnique);
      VersionSpec[] destinationArray = new VersionSpec[sourceArray.Length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
      return destinationArray;
    }

    public override bool Equals(object other) => other is VersionSpec other1 && this.CompareTo(other1) == 0;

    public override int GetHashCode() => this.GetType().Name.GetHashCode();

    public virtual int CompareTo(VersionSpec other) => string.CompareOrdinal(this.GetType().Name, other.GetType().Name);

    public virtual int ToChangeset(IVssRequestContext requestContext) => VersionSpec.UnknownChangeset;

    public abstract string ToDBString(IVssRequestContext requestContext);

    internal abstract void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName);

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      this.Validate(versionControlRequestContext, parameterName);
    }

    internal static string AddDeletionModifierIfNecessary(string path, int deletionId)
    {
      if (deletionId == 0)
        return path;
      return path + VersionSpec.Separator + (object) VersionSpec.DeletionModifier + (object) deletionId;
    }

    internal bool UseMappings
    {
      set => this.m_useMappings = value;
    }
  }
}
