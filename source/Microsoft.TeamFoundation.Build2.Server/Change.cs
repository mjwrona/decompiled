// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.Change
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class Change
  {
    public string Id { get; set; }

    public string Message { get; set; }

    public string Type { get; set; }

    public IdentityRef Author { get; set; }

    public IdentityRef Committer { get; set; }

    public DateTime? Timestamp { get; set; }

    public DateTime? CommitTime { get; set; }

    public Uri Location { get; set; }

    public bool MessageTruncated { get; set; }

    public Uri DisplayUri { get; set; }

    public string Pusher { get; set; }

    public bool? Distinct { get; set; }
  }
}
