// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingState
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class PendingState : IValidatable
  {
    private int m_itemId;
    private int m_revertToVersion;
    private ConflictInformation m_conflictInfo;

    [XmlAttribute("id")]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    [XmlAttribute("rtv")]
    [DefaultValue(0)]
    public int RevertToVersion
    {
      get => this.m_revertToVersion;
      set => this.m_revertToVersion = value;
    }

    public ConflictInformation ConflictInfo
    {
      get => this.m_conflictInfo;
      set => this.m_conflictInfo = value;
    }

    internal static void Update(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      PendingState[] updates)
    {
      versionControlRequestContext.Validation.check((IValidatable[]) updates, nameof (updates), false);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      List<Tuple<int, int>> revertToList = new List<Tuple<int, int>>();
      List<PendingState> localConflicts = new List<PendingState>();
      foreach (PendingState update in updates)
      {
        int num = -3;
        if (update.RevertToVersion != 0 && update.RevertToVersion != -3 || update.ConflictInfo != null && update.RevertToVersion == 0)
        {
          num = update.RevertToVersion;
          revertToList.Add(new Tuple<int, int>(update.ItemId, update.RevertToVersion));
        }
        if (update.ConflictInfo != null)
        {
          update.RevertToVersion = num;
          localConflicts.Add(update);
        }
      }
      if (revertToList.Count > 0)
      {
        using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
          versionedItemComponent.UpdateRevertToBatch(workspace, (IEnumerable<Tuple<int, int>>) revertToList);
      }
      if (localConflicts.Count <= 0)
        return;
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        versionedItemComponent.AddLocalConflictsBatch(workspace, (IEnumerable<PendingState>) localConflicts);
    }

    public override string ToString()
    {
      string str = "ItemId: " + (object) this.ItemId + "; " + (this.RevertToVersion != 0 ? (object) ("RevertToVersion: " + this.RevertToVersion.ToString() + "; ") : (object) (string) null);
      if (this.ConflictInfo != null)
        str = str + "ConflictInfo(type=" + (object) this.ConflictInfo.ConflictType + ",VersionFrom=" + (object) this.ConflictInfo.VersionFrom + ",PendingChangeId=" + (object) this.ConflictInfo.PendingChangeId + ",SourceLocalItem=" + this.ConflictInfo.SourceLocalItem + ",TargetLocalItem=" + this.ConflictInfo.TargetLocalItem + ",Reason=" + (object) this.ConflictInfo.Reason + ")";
      return str;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
    }
  }
}
