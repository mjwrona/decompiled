// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models.WorkItem
// Assembly: Microsoft.Azure.Pipelines.Deployment.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2CF55160-AB9F-45A3-BD33-54D24F269988
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Sdk.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Sdk.Server.Traceability.Models
{
  public class WorkItem
  {
    public string Id { get; set; }

    public string Url { get; set; }

    public string Title { get; set; }

    public DateTime CreatedOn { get; set; }

    public string Type { get; set; }

    public string CurrentState { get; set; }

    public IdentityRef AssignedTo { get; set; }
  }
}
