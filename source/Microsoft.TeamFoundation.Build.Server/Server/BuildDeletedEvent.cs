// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDeletedEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  [Serializable]
  public sealed class BuildDeletedEvent
  {
    public BuildDeletedEvent()
    {
    }

    public BuildDeletedEvent(
      BuildDetail build,
      DeleteOptions options,
      IdentityDescriptor deletedBy)
    {
      this.BuildDetail = build;
      this.DeletedBy = deletedBy;
      this.Options = options;
    }

    public BuildDetail BuildDetail { get; private set; }

    public IdentityDescriptor DeletedBy { get; private set; }

    public DeleteOptions Options { get; private set; }
  }
}
