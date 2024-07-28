// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks.WebHook
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts.WebHooks
{
  public class WebHook
  {
    public WebHook() => this.Subscriptions = (IList<IWebHookSubscription>) new List<IWebHookSubscription>();

    public string ArtifactType { get; set; }

    public string UniqueArtifactIdentifier { get; set; }

    public Guid WebHookId { get; set; }

    public Guid ConnectionId { get; set; }

    public IList<IWebHookSubscription> Subscriptions { get; set; }
  }
}
