// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandCheckInShelveset
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandCheckInShelveset : CommandCheckIn
  {
    private bool m_deleteShelveset;
    private string m_ownerName;
    private string m_shelvesetName;

    public CommandCheckInShelveset(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public CheckinResult Execute(
      string shelvesetName,
      string ownerName,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions)
    {
      return this.Execute(shelvesetName, ownerName, 0, changesetOwner, checkinNotificationInfo, checkinOptions);
    }

    internal CheckinResult Execute(
      string shelvesetName,
      string ownerName,
      int shelvesetVersion,
      string changesetOwner,
      CheckinNotificationInfo checkinNotificationInfo,
      CheckInOptions2 checkinOptions)
    {
      this.m_versionControlRequestContext.Validation.checkShelvesetName(shelvesetName, nameof (shelvesetName), false);
      this.m_versionControlRequestContext.Validation.checkIdentity(ref ownerName, nameof (ownerName), false);
      this.m_versionControlRequestContext.Validation.checkIdentity(ref changesetOwner, nameof (changesetOwner), true);
      ClientTrace.Publish(this.RequestContext, "CheckInShelveset");
      List<Shelveset> shelvesetList = Shelveset.Query(this.m_versionControlRequestContext, ownerName, shelvesetName, shelvesetVersion);
      if (shelvesetList.Count == 0)
        throw new ShelvesetNotFoundException(shelvesetName, ownerName);
      this.m_ownerName = ownerName;
      this.m_shelvesetName = shelvesetName;
      this.m_deleteShelveset = (checkinOptions & CheckInOptions2.DeleteShelveset) == CheckInOptions2.DeleteShelveset;
      Changeset info = new Changeset();
      info.CheckinNote = shelvesetList[0].CheckinNote;
      info.Comment = shelvesetList[0].Comment;
      if (!string.IsNullOrEmpty(changesetOwner))
        info.Owner = changesetOwner;
      info.PolicyOverride = new PolicyOverrideInfo();
      info.PolicyOverride.Comment = shelvesetList[0].PolicyOverrideComment;
      CheckinResult checkinResult = this.ExecuteInternal(TfvcIdentityHelper.FindIdentity(this.RequestContext, shelvesetList[0].ownerId), shelvesetName, shelvesetVersion, PendingSetType.Shelveset, (string) null, (Workspace) null, (string[]) null, info, checkinNotificationInfo, checkinOptions, false, 0);
      this.PublishCustomerIntelligence(checkinResult);
      return checkinResult;
    }

    protected override void CompleteCheckIn(bool successful)
    {
      base.CompleteCheckIn(successful);
      if (!successful)
        return;
      if (!this.m_deleteShelveset)
        return;
      try
      {
        Shelveset.Delete(this.m_versionControlRequestContext, this.m_shelvesetName, this.m_ownerName, -1);
      }
      catch (RequestCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(700050, TraceLevel.Info, TraceArea.CheckIn, TraceLayer.Command, ex);
        string uniqueName = IdentityHelper.GetUniqueName(TfvcIdentityHelper.FindIdentity(this.m_versionControlRequestContext.RequestContext, this.m_ownerName));
        this.Failures.Enqueue(new Failure(ex)
        {
          Severity = SeverityType.Warning,
          Message = Resources.Format("DeleteShelvesetOnCheckinFailed", (object) WorkspaceSpec.Combine(this.m_shelvesetName, uniqueName), (object) ex.Message)
        });
      }
    }
  }
}
