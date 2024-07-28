// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CreateItemsTraceData
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class CreateItemsTraceData
  {
    public CreateItemsTraceData(Microsoft.VisualStudio.Services.FileContainer.FileContainer container, IList<FileContainerItem> items)
    {
      Microsoft.VisualStudio.Services.FileContainer.FileContainer fileContainer = container;
      this.ContainerId = fileContainer != null ? fileContainer.Id : 0L;
      this.ContainerItems = items.Select<FileContainerItem, FileContainerItemTraceData>((Func<FileContainerItem, FileContainerItemTraceData>) (item => new FileContainerItemTraceData(item, container)));
    }

    public long ContainerId { get; private set; }

    public IEnumerable<FileContainerItemTraceData> ContainerItems { get; private set; }
  }
}
