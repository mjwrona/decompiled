// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandQueryCodeMetrics
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandQueryCodeMetrics : VersionControlCommand
  {
    public CommandQueryCodeMetrics(
      VersionControlRequestContext versionControlRequestContext)
      : base(versionControlRequestContext)
    {
    }

    public CodeMetrics Execute(Guid projectId, int startingTimeBucket, int endTimeBucket)
    {
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
        return versionedItemComponent.QueryCodeMetrics(projectId, startingTimeBucket, endTimeBucket);
    }

    protected override void Dispose(bool disposing)
    {
    }
  }
}
