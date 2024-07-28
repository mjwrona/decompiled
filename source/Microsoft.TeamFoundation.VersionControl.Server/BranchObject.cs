// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchObject
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class BranchObject
  {
    private BranchProperties m_properties;
    private List<ItemIdentifier> m_childBranches;
    private List<ItemIdentifier> m_relatedBranches;
    private DateTime m_dateCreated;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public BranchProperties Properties
    {
      get => this.m_properties;
      set => this.m_properties = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated
    {
      get => this.m_dateCreated;
      set => this.m_dateCreated = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public List<ItemIdentifier> ChildBranches
    {
      get => this.m_childBranches;
      set => this.m_childBranches = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public List<ItemIdentifier> RelatedBranches
    {
      get => this.m_relatedBranches;
      set => this.m_relatedBranches = value;
    }

    internal static void Delete(VersionControlRequestContext context, ItemIdentifier rootItem)
    {
      rootItem?.SetValidationOptions(BranchObject.ValidItemOptions, BranchObject.ValidVersionSpecOptions);
      context.Validation.check((IValidatable) rootItem, nameof (rootItem), false);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add(nameof (rootItem), (object) rootItem);
      ClientTrace.Publish(context.RequestContext, "DeleteBranchObject", ctData);
      context.VersionControlService.SecurityWrapper.CheckItemPermission(context, VersionedItemPermissions.ManageBranch, rootItem.ItemPathPair);
      using (VersionedItemComponent versionedItemComponent = context.VersionControlService.GetVersionedItemComponent(context))
        versionedItemComponent.DeleteBranchObject(rootItem);
    }

    internal static void Update(
      VersionControlRequestContext context,
      BranchProperties branchProperties,
      bool updateExisting)
    {
      context.Validation.check((IValidatable) branchProperties, nameof (branchProperties), false);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = context.VersionControlService.SecurityWrapper.FindIdentity(context.RequestContext);
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (branchProperties.Owner != null)
        identity2 = TfvcIdentityHelper.FindIdentity(context.RequestContext, branchProperties.Owner);
      if (identity2 == null)
      {
        identity2 = identity1;
        string identityName;
        string displayName;
        context.VersionControlService.SecurityWrapper.FindIdentityNames(context.RequestContext, identity1.Id, out identityName, out displayName);
        branchProperties.Owner = identityName;
        branchProperties.OwnerDisplayName = displayName;
      }
      context.VersionControlService.SecurityWrapper.CheckItemPermission(context, VersionedItemPermissions.ManageBranch, branchProperties.RootItem.ItemPathPair);
      if (branchProperties.ParentBranch != null)
      {
        bool flag = false;
        if (updateExisting)
        {
          using (VersionedItemComponent versionedItemComponent = context.VersionControlService.GetVersionedItemComponent(context))
          {
            using (ResultCollection resultCollection = versionedItemComponent.QueryBranchObjects(branchProperties.RootItem, RecursionType.None))
            {
              ObjectBinder<BranchObject> current1 = resultCollection.GetCurrent<BranchObject>();
              if (current1.MoveNext())
              {
                BranchObject current2 = current1.Current;
                if (current2.Properties.ParentBranch != null)
                {
                  if (VersionControlPath.Equals(current2.Properties.ParentBranch.Item, branchProperties.ParentBranch.Item))
                    goto label_19;
                }
                flag = true;
              }
            }
          }
        }
        else
          flag = true;
label_19:
        if (flag)
          context.VersionControlService.SecurityWrapper.CheckItemPermission(context, VersionedItemPermissions.ManageBranch, branchProperties.ParentBranch.ItemPathPair);
      }
      branchProperties.OwnerId = identity2.Id;
      Mapping.OptimizeSingleRootMappings(branchProperties.BranchMappings);
      using (VersionedItemComponent versionedItemComponent = context.VersionControlService.GetVersionedItemComponent(context))
        versionedItemComponent.UpdateBranchObject(branchProperties, updateExisting, context.MaxSupportedServerPathLength);
    }

    internal static ItemValidationOptions ValidItemOptions => ItemValidationOptions.Allow8Dot3Paths | ItemValidationOptions.DisallowLocalItem | ItemValidationOptions.DisallowRoot;

    internal static VersionSpecValidationOptions ValidVersionSpecOptions => VersionSpecValidationOptions.Date | VersionSpecValidationOptions.Changeset | VersionSpecValidationOptions.Latest;
  }
}
