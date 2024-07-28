// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandInspectLabels
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandInspectLabels : VersionControlCommand
  {
    private List<InspectLabelsInfo> m_labels;

    public CommandInspectLabels(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute()
    {
      this.m_labels = new List<InspectLabelsInfo>();
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.InspectLabels();
        ObjectBinder<InspectLabelsInfo> current = resultCollection.GetCurrent<InspectLabelsInfo>();
        while (current.MoveNext())
        {
          this.m_labels.Add(current.Current);
          resultCollection.IncrementRowCounter();
        }
      }
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    protected override void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
    }

    public List<InspectLabelsInfo> Labels => this.m_labels;
  }
}
