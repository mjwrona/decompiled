// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.CommandInspectShelvesets
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class CommandInspectShelvesets : VersionControlCommand
  {
    private List<InspectShelvesetInfo> m_shelvesets;

    public CommandInspectShelvesets(VersionControlRequestContext context)
      : base(context)
    {
    }

    public void Execute()
    {
      this.m_shelvesets = new List<InspectShelvesetInfo>();
      using (VersionedItemComponent versionedItemComponent = this.m_versionControlRequestContext.VersionControlService.GetVersionedItemComponent(this.m_versionControlRequestContext))
      {
        ResultCollection resultCollection = versionedItemComponent.InspectShelvesets();
        ObjectBinder<InspectShelvesetInfo> current = resultCollection.GetCurrent<InspectShelvesetInfo>();
        while (current.MoveNext())
        {
          this.m_shelvesets.Add(current.Current);
          resultCollection.IncrementRowCounter();
        }
      }
      StreamingCollection<InspectShelvesetInfo> streamingCollection = new StreamingCollection<InspectShelvesetInfo>((Command) this)
      {
        HandleExceptions = false
      };
      foreach (InspectShelvesetInfo shelveset in this.m_shelvesets)
      {
        string identityName;
        string displayName;
        this.m_versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(this.m_versionControlRequestContext.RequestContext, shelveset.OwnerId, out identityName, out displayName);
        shelveset.Owner = identityName;
        shelveset.OwnerDisplayName = displayName;
        streamingCollection.Enqueue(shelveset);
      }
      streamingCollection.IsComplete = true;
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    protected override void Dispose(bool disposing)
    {
      int num = disposing ? 1 : 0;
    }

    public List<InspectShelvesetInfo> Shelvesets => this.m_shelvesets;
  }
}
