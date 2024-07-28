// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendChangeSecurity
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
  internal class PendChangeSecurity
  {
    internal ItemPathPair TargetItemPathPair;
    internal ItemPathPair TargetSourceItemPathPair;
    internal ItemPathPair SourceItemToPathPair;
    public int SourceItemId;
    public int TargetItemId;
    public int InputIndex;
    public bool IsBranchObject;
    public ChangeType ChangeType;
    private bool m_failedSecurity;
    public bool PermissionFailureNotReported;
    public LockLevel LockLevel;
    public bool FailedPatternMatch;
    public bool FailedRestrictions;
    public int SourceItemDataspaceId;
    public int TargetItemDataspaceId;

    public string TargetServerItem => this.TargetItemPathPair.ProjectNamePath;

    public string TargetSourceServerItem => this.TargetSourceItemPathPair.ProjectNamePath;

    public string SourceServerItemTo => this.SourceItemToPathPair.ProjectNamePath;

    public bool FailedSecurity
    {
      get => this.m_failedSecurity;
      set
      {
        this.PermissionFailureNotReported = value;
        this.m_failedSecurity = value;
      }
    }

    internal static void UpdateLockLevel(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      PendChangeSecurity op,
      LockLevel lockLevel)
    {
      op.LockLevel = ChangeRequest.EnforceSingleCheckout(versionControlRequestContext, workspace, op.TargetServerItem, lockLevel);
    }

    internal static void UpdatePermissions(
      VersionControlRequestContext versionControlRequestContext,
      PendChangeSecurity op,
      List<Failure> failures,
      bool branch)
    {
      VersionedItemPermissions permissionRequired1 = VersionedItemPermissions.Read;
      VersionedItemPermissions permissionRequired2 = VersionedItemPermissions.Read | VersionedItemPermissions.PendChange;
      if ((op.ChangeType & ChangeType.Merge) == ChangeType.Merge || (op.ChangeType & ChangeType.Branch) == ChangeType.Branch)
        permissionRequired2 |= VersionedItemPermissions.Merge;
      VersionedItemPermissions permissionRequired3 = permissionRequired2;
      SecurityManager securityWrapper = versionControlRequestContext.VersionControlService.SecurityWrapper;
      if (op.LockLevel != LockLevel.Unchanged && op.LockLevel != LockLevel.None)
      {
        permissionRequired2 |= VersionedItemPermissions.Lock;
        permissionRequired3 |= VersionedItemPermissions.Lock;
      }
      if (op.SourceServerItemTo != null && !securityWrapper.HasItemPermission(versionControlRequestContext, permissionRequired1, op.SourceItemToPathPair))
        op.FailedSecurity = true;
      else if (op.TargetSourceServerItem != null && !VersionControlPath.Equals(op.TargetServerItem, op.TargetSourceServerItem) && !securityWrapper.HasItemPermission(versionControlRequestContext, permissionRequired3, op.TargetSourceItemPathPair))
      {
        op.FailedSecurity = true;
        try
        {
          if (!securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, op.TargetSourceItemPathPair))
            return;
          securityWrapper.CheckItemPermission(versionControlRequestContext, permissionRequired3 & ~VersionedItemPermissions.Read, op.TargetSourceItemPathPair);
        }
        catch (RequestCanceledException ex)
        {
          throw;
        }
        catch (ApplicationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700097, TraceLevel.Info, TraceArea.Permissions, TraceLayer.BusinessLogic, (Exception) ex);
          failures.Add(new Failure((Exception) ex));
          op.PermissionFailureNotReported = false;
        }
      }
      else
      {
        if (op.IsBranchObject && (op.ChangeType & ChangeType.Branch) == ChangeType.Branch)
        {
          securityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.ManageBranch, op.SourceItemToPathPair);
          securityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.ManageBranch, op.TargetItemPathPair);
        }
        if (!securityWrapper.HasItemPermission(versionControlRequestContext, permissionRequired2, op.TargetItemPathPair))
        {
          op.FailedSecurity = true;
          try
          {
            if (branch)
              throw new ResourceAccessException(versionControlRequestContext.RequestContext.GetUserId().ToString(), permissionRequired2.ToString(), op.TargetServerItem);
            if (!securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, op.TargetItemPathPair))
              return;
            securityWrapper.CheckItemPermission(versionControlRequestContext, permissionRequired2 & ~VersionedItemPermissions.Read, op.TargetItemPathPair);
          }
          catch (RequestCanceledException ex)
          {
            throw;
          }
          catch (ApplicationException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700098, TraceLevel.Info, TraceArea.Permissions, TraceLayer.BusinessLogic, (Exception) ex);
            failures.Add(new Failure((Exception) ex));
            op.PermissionFailureNotReported = false;
          }
        }
        else
        {
          if (op.TargetSourceServerItem == null || op.TargetServerItem == null)
            return;
          if (VersionControlPath.Equals(op.TargetServerItem, op.TargetSourceServerItem))
            return;
          try
          {
            securityWrapper.CheckItemPermissionForAllChildren(versionControlRequestContext, VersionedItemPermissions.Read, op.TargetSourceItemPathPair);
          }
          catch (ResourceAccessException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700099, TraceLevel.Info, TraceArea.Permissions, TraceLayer.BusinessLogic, (Exception) ex);
            op.FailedSecurity = true;
            op.PermissionFailureNotReported = false;
            failures.Add(new Failure((Exception) ex));
          }
        }
      }
    }
  }
}
