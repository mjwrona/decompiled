// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandInspectBigFiles
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandInspectBigFiles : VersionControlCommand
  {
    private List<InspectBigFilesInfo> m_bigFiles;

    public CommandInspectBigFiles(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute(ItemSpec itemSpec, int limit)
    {
      if (!itemSpec.isServerItem)
        throw new ArgumentException(Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("ServerItemRequiredException", (object) itemSpec.Item), nameof (itemSpec));
      this.m_bigFiles = new List<InspectBigFilesInfo>();
      ItemPathPair scopePath = ItemPathPair.FromServerItem(itemSpec.Item);
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.InspectBigFiles(scopePath, limit);
        ObjectBinder<InspectBigFilesInfo> current = resultCollection.GetCurrent<InspectBigFilesInfo>();
        while (current.MoveNext())
        {
          this.m_bigFiles.Add(current.Current);
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

    public List<InspectBigFilesInfo> BigFiles => this.m_bigFiles;
  }
}
