// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.DepthAndObjectIds
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public struct DepthAndObjectIds
  {
    public readonly int CommitDepth;
    public readonly ICollection<Sha1Id> ObjectIds;

    [JsonConstructor]
    public DepthAndObjectIds(ICollection<Sha1Id> objectIds, int? commitDepth = 1)
    {
      this.CommitDepth = commitDepth ?? 1;
      ArgumentUtility.CheckForOutOfRange(this.CommitDepth, nameof (commitDepth), 0, 2, GitServerUtils.TraceArea);
      this.ObjectIds = objectIds;
    }
  }
}
