// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.WebApi.CheckSuiteUpdatedEvent
// Assembly: Microsoft.Azure.Pipelines.Checks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 381241F9-9196-42AF-BB4C-5187E3EFE32E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;

namespace Microsoft.Azure.Pipelines.Checks.WebApi
{
  [ServiceEventObject]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class CheckSuiteUpdatedEvent
  {
    public CheckSuiteUpdatedEvent(string eventType, Guid projectId, CheckSuite response)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<CheckSuite>(response, nameof (response));
      this.EventType = eventType;
      this.ProjectId = projectId;
      this.Response = response;
    }

    public string EventType { get; set; }

    public Guid ProjectId { get; set; }

    public CheckSuite Response { get; set; }
  }
}
