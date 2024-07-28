// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssLockManager
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssLockManager : LockManager
  {
    private const string c_area = "TeamFoundation";

    protected override string Area => "TeamFoundation";

    protected override bool IsNamedLockHierarchyViolation(
      LockLevel lockLevel,
      LockLevel lastLockLevel,
      LockManager.NamedLockObject lockObject,
      LockManager.NamedLockObject lastLockObject)
    {
      if (lockLevel < lastLockLevel)
        return true;
      return lockLevel == lastLockLevel && lockLevel != LockLevel.Resource;
    }

    protected override void CheckForNamedLockReentryViolation(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      LockManager.LockHeldEntry lockHeld,
      long requestId)
    {
      if (lockType == LockManager.LockType.ResourceConnection)
      {
        string lockName = lockObject.LockName.ToString();
        TeamFoundationTracingService.TraceRaw(39908, TraceLevel.Error, this.Area, this.Layer, FrameworkResources.InvalidNamedLockReentry((object) lockName, (object) requestId));
        VssPerformanceEventSource.Log.DetectedLockReentryViolation(lockName);
        throw new InvalidOperationException("lock reentry violation");
      }
    }

    protected override void CheckForNamedLockUsageViolation(
      LockManager.NamedLockObject lockObject,
      LockManager.LockType lockType,
      IList<LockManager.LockHeldEntry> locksHeld,
      long requestId)
    {
      if (lockType != LockManager.LockType.ResourceConnection && !VssLockManager.IsGlobalLockName(lockObject.LockName))
        return;
      IList<string> values = (IList<string>) null;
      for (int index = 0; index < locksHeld.Count; ++index)
      {
        LockManager.LockHeldEntry lockHeldEntry = locksHeld[index];
        if (lockHeldEntry.LockObject != lockObject)
        {
          ILockName lockName = ((LockManager.NamedLockObject) lockHeldEntry.LockObject).LockName;
          if (lockHeldEntry.LockType == LockManager.LockType.ResourceConnection || VssLockManager.IsGlobalLockName(lockName))
          {
            if (values == null)
              values = (IList<string>) new List<string>();
            values.Add(lockName.ToString());
          }
        }
      }
      if (values != null)
      {
        string lockName = lockObject.LockName.ToString();
        string locksHeld1 = string.Join(", ", (IEnumerable<string>) values);
        string message = FrameworkResources.InvalidNamedLockUsage((object) lockName, (object) requestId, (object) locksHeld1);
        TeamFoundationTracingService.TraceRaw(39907, TraceLevel.Error, this.Area, this.Layer, message);
        VssPerformanceEventSource.Log.DetectedLockUsageViolation(lockName, locksHeld1);
        throw new InvalidOperationException("lock usage violation: " + message);
      }
    }

    private static bool IsGlobalLockName(ILockName lockName) => ((uint) ((LockName<short, Guid, string>) lockName).NameValue1 & 1U) > 0U;

    [Conditional("DEBUG")]
    private void LogStackTraces(string lockName, LockManager.LockHeldEntry lockHeld)
    {
    }

    [Conditional("DEBUG")]
    private void LogStackTraces(string lockName, IList<LockManager.LockHeldEntry> locksHeld)
    {
      if (!TeamFoundationTracingService.IsRawTracingEnabled(39909, TraceLevel.Verbose, this.Area, this.Layer, (string[]) null))
        return;
      string str = "StackTraces " + lockName + Environment.NewLine + Environment.NewLine;
      foreach (LockManager.LockHeldEntry lockHeldEntry in (IEnumerable<LockManager.LockHeldEntry>) locksHeld)
      {
        str = str + ((LockManager.NamedLockObject) lockHeldEntry.LockObject).LockName?.ToString() + Environment.NewLine;
        if (lockHeldEntry.StackTraces == null)
        {
          str += Environment.NewLine;
        }
        else
        {
          foreach (KeyValuePair<int, StackTrace> stackTrace in lockHeldEntry.StackTraces)
            str = str + stackTrace.Key.ToString() + Environment.NewLine + stackTrace.Value?.ToString() + Environment.NewLine + Environment.NewLine;
        }
      }
      TeamFoundationTracingService.TraceRaw(39909, TraceLevel.Verbose, this.Area, this.Layer, str + lockName + Environment.NewLine + new StackTrace(5, true)?.ToString());
    }
  }
}
