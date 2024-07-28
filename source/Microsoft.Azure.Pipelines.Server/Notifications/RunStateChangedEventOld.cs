// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.Notifications.RunStateChangedEventOld
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.Notifications
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-pipelines.run-state-changed-event")]
  public sealed class RunStateChangedEventOld
  {
    public Guid ProjectId { get; set; }

    public Run Run { get; set; }

    public PipelineReference Pipeline { get; set; }

    public int RunId { get; set; }

    public string RunUrl { get; set; }

    public List<RepositoryWebhook> Repositories { get; set; }
  }
}
