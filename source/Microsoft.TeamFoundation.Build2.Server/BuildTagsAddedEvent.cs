// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildTagsAddedEvent
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildTagsAddedEvent : BuildEventBase
  {
    public BuildTagsAddedEvent(IReadOnlyBuildData build, IEnumerable<string> newTags)
      : base(build)
    {
      this.NewTags = new List<string>(newTags ?? Enumerable.Empty<string>());
    }

    public List<string> NewTags { get; private set; }
  }
}
