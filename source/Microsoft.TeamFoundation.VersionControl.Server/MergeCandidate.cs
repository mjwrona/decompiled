// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.MergeCandidate
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class MergeCandidate
  {
    private Changeset m_changeset;
    private bool m_partial;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Changeset Changeset
    {
      get => this.m_changeset;
      set => this.m_changeset = value;
    }

    [XmlAttribute("part")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool Partial
    {
      get => this.m_partial;
      set => this.m_partial = value;
    }

    internal static List<MergeCandidate> QueryMergeCandidates(
      VersionControlRequestContext versionControlRequestContext,
      Workspace localWorkspace,
      ItemSpec source,
      ItemSpec target,
      MergeOptionsEx options)
    {
      List<MergeCandidate> ret = new List<MergeCandidate>();
      Dictionary<int, Changeset> changesets = new Dictionary<int, Changeset>();
      ItemPathPair serverItem1 = source.toServerItem(versionControlRequestContext.RequestContext, localWorkspace);
      ItemPathPair serverItem2 = target.toServerItem(versionControlRequestContext.RequestContext, localWorkspace);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, serverItem1);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, serverItem2);
      bool useRecompileVersion = versionControlRequestContext.RequestContext.IsFeatureEnabled("Tfvc.UsePendMergeWithRecompile");
      List<ItemMerge> items1;
      List<ItemMerge> items2;
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        using (ResultCollection resultCollection = versionedItemComponent.QueryMergeCandidates(localWorkspace, serverItem1, serverItem2, source.RecursionType, source.DeletionId, options, versionControlRequestContext.MaxSupportedServerPathLength, useRecompileVersion))
        {
          while (resultCollection.GetCurrent<Changeset>().MoveNext())
            changesets.Add(resultCollection.GetCurrent<Changeset>().Current.ChangesetId, resultCollection.GetCurrent<Changeset>().Current);
          resultCollection.NextResult();
          items1 = resultCollection.GetCurrent<ItemMerge>().Items;
          resultCollection.NextResult();
          items2 = resultCollection.GetCurrent<ItemMerge>().Items;
        }
      }
      MergeCandidate.AggregateItems(versionControlRequestContext, ret, items1, changesets);
      MergeCandidate.MarkPartialMerges(versionControlRequestContext, ret, items2);
      return ret;
    }

    private static void AggregateItems(
      VersionControlRequestContext versionControlRequestContext,
      List<MergeCandidate> ret,
      List<ItemMerge> unmergedList,
      Dictionary<int, Changeset> changesets)
    {
      int key = 0;
      foreach (ItemMerge unmerged in unmergedList)
      {
        if (unmerged.SourceVersionFrom != key && unmerged.HasPermission(versionControlRequestContext))
        {
          key = unmerged.SourceVersionFrom;
          MergeCandidate mergeCandidate = new MergeCandidate();
          mergeCandidate.Changeset = changesets[key];
          string identityName;
          string displayName;
          versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, mergeCandidate.Changeset.ownerId, out identityName, out displayName);
          mergeCandidate.Changeset.Owner = identityName;
          mergeCandidate.Changeset.OwnerDisplayName = displayName;
          mergeCandidate.Partial = false;
          ret.Add(mergeCandidate);
        }
      }
    }

    private static void MarkPartialMerges(
      VersionControlRequestContext versionControlRequestContext,
      List<MergeCandidate> ret,
      List<ItemMerge> mergedList)
    {
      int num = 0;
      foreach (ItemMerge merged in mergedList)
      {
        if (merged.SourceVersionFrom != num && merged.HasPermission(versionControlRequestContext))
        {
          num = merged.SourceVersionFrom;
          for (int index = 0; index < ret.Count; ++index)
          {
            if (ret[index].Changeset.ChangesetId == num)
            {
              ret[index].Partial = true;
              break;
            }
          }
        }
      }
    }
  }
}
