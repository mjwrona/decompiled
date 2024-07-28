// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildDefinitionBranch
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class BuildDefinitionBranch
  {
    public Dictionary<string, string> Properties = new Dictionary<string, string>();

    public string BranchName { get; set; }

    public long SourceId { get; set; }

    public Guid PendingSourceOwner { get; set; }

    public string PendingSourceVersion { get; set; }
  }
}
