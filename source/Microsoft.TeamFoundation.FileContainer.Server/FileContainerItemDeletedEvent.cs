// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerItemDeletedEvent
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class FileContainerItemDeletedEvent
  {
    public FileContainerItemDeletedEvent(long containerId, IList<string> paths)
    {
      this.ContainerId = containerId;
      this.Paths = paths;
    }

    public long ContainerId { get; private set; }

    public IList<string> Paths { get; private set; }
  }
}
