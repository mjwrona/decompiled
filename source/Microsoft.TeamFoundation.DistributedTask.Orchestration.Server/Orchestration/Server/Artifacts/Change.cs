// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.Change
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class Change
  {
    public string Id { get; set; }

    public string Message { get; set; }

    public string ChangeType { get; set; }

    public IdentityRef Author { get; set; }

    public DateTime? Timestamp { get; set; }

    public Uri Location { get; set; }

    public Uri DisplayUri { get; set; }

    [Obsolete("Use PushedBy instead")]
    public string Pusher { get; set; }

    public IdentityRef PushedBy { get; set; }
  }
}
