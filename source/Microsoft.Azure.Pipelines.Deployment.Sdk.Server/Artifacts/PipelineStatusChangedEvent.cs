// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts.PipelineStatusChangedEvent
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.Azure.Pipelines.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Artifacts
{
  public sealed class PipelineStatusChangedEvent
  {
    public string EventType { get; set; }

    public Guid ProjectId { get; set; }

    public Run Run { get; set; }

    public PipelineReference Pipeline { get; set; }

    public List<string> Tags { get; set; }

    public List<Stage> Stages { get; set; }
  }
}
