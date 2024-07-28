// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandDestroy
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandDestroy : VersionControlCommand
  {
    public CommandDestroy(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    internal Item[] Execute(
      ItemSpec item,
      VersionSpec versionSpec,
      VersionSpec stopAtSpec,
      int flags,
      out Failure[] failures,
      out StreamingCollection<PendingSet> pendingChanges,
      out StreamingCollection<PendingSet> shelvedChanges)
    {
      string str = item.Item;
      this.m_versionControlRequestContext.Validation.checkVersionSpec(versionSpec, nameof (versionSpec), false);
      this.m_versionControlRequestContext.Validation.checkVersionSpec(stopAtSpec, nameof (stopAtSpec), true);
      PathLength serverPathLength = this.m_versionControlRequestContext.MaxSupportedServerPathLength;
      this.m_versionControlRequestContext.Validation.checkItem(ref str, nameof (item), false, false, true, false, serverPathLength);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("path", (object) str);
      ctData.Add("stopAt", (object) stopAtSpec?.ToString());
      ctData.Add(nameof (flags), (object) flags);
      ClientTrace.Publish(this.RequestContext, "Destroy", ctData);
      return new DestroyRequest(this.m_versionControlRequestContext).DestroyItems(item, versionSpec, stopAtSpec, flags, out failures, out pendingChanges, out shelvedChanges);
    }

    protected override void Dispose(bool disposing)
    {
    }
  }
}
