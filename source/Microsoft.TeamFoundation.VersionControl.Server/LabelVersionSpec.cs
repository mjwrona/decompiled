// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.LabelVersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class LabelVersionSpec : VersionSpec
  {
    private string m_label;
    private string m_scope;

    public LabelVersionSpec()
    {
    }

    internal LabelVersionSpec(string label, string scope)
    {
      this.Label = label;
      this.Scope = scope;
    }

    [XmlAttribute("label")]
    public string Label
    {
      get => this.m_label;
      set => this.m_label = value;
    }

    [XmlAttribute("scope")]
    public string Scope
    {
      get => this.m_scope;
      set => this.m_scope = value;
    }

    internal override void QueryItems(
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

    internal override void QueryItems(
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
      if (this.m_useMappings)
        this.m_rc = db.QueryMappedItems(localWorkspace, itemSpec.ItemPathPair, itemSpec.RecursionType, options);
      else if (itemSpec.isServerItem)
        this.m_rc = db.QueryLabelItems(this.Label, ItemPathPair.FromServerItem(this.Scope), itemSpec.ItemPathPair, itemSpec.RecursionType, itemType, deletedState, options);
      else
        this.m_rc = db.QueryLabelItemsLocal(localWorkspace, this.Label, this.Scope, itemSpec.Item, itemSpec.RecursionType, itemType, deletedState, options, versionControlRequestContext.MaxSupportedServerPathLength);
      DetermineItemTypeColumns current = (DetermineItemTypeColumns) this.m_rc.GetCurrent<DeterminedItem>();
      current.MoveNext();
      queryPath = current.Current.QueryPath;
      filePattern = current.Current.FilePattern;
      this.m_rc.NextResult();
    }

    public override int GetHashCode() => this.Label.GetHashCode() + this.Scope.GetHashCode();

    public override int CompareTo(VersionSpec other)
    {
      int num = base.CompareTo(other);
      if (num == 0)
      {
        LabelVersionSpec labelVersionSpec = (LabelVersionSpec) other;
        num = TFStringComparer.LabelName.Compare(this.Label, labelVersionSpec.Label);
        if (num == 0)
          num = TFStringComparer.VersionControlPath.Compare(this.Scope, labelVersionSpec.Scope);
      }
      return num;
    }

    public override string ToString() => LabelSpec.Combine(this.Label, this.Scope);

    public override string ToDBString(IVssRequestContext requestContext) => "L" + LabelSpec.Combine(this.Label, DBPath.ServerToDatabasePath(this.Scope));

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkLabelName(this.m_label, "Label", false, false);
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      versionControlRequestContext.Validation.checkServerItem(ref this.m_scope, "Scope", true, false, true, false, serverPathLength);
    }
  }
}
