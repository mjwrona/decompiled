// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.LockBasedLeaseInfo
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Threading;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  public class LockBasedLeaseInfo : ILeaseInfo, IDisposable
  {
    private IDisposable lockObject;

    public LockBasedLeaseInfo(IDisposable lockObject, string leaseName, DateTime leaseObtained)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(leaseName, nameof (leaseName));
      ArgumentUtility.CheckForNull<IDisposable>(lockObject, nameof (lockObject));
      this.LeaseName = leaseName;
      this.LeaseObtained = this.LeaseObtained;
      this.lockObject = lockObject;
    }

    public DateTime LeaseExpires => DateTime.MaxValue;

    public string LeaseName { get; private set; }

    public DateTime LeaseObtained { get; private set; }

    public Guid LeaseOwner { get; private set; }

    public TimeSpan LeaseTime { get; private set; } = Timeout.InfiniteTimeSpan;

    public Guid ProcessId { get; private set; }

    public void Dispose() => this.lockObject.Dispose();

    public void Renew()
    {
    }
  }
}
