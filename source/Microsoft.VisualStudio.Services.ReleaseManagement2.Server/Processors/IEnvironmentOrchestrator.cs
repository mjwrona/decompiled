// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.IEnvironmentOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  public interface IEnvironmentOrchestrator
  {
    IEnumerable<ReleaseEnvironmentStep> RejectStep(
      ReleaseEnvironmentStep releaseEnvironmentStep,
      Guid rejectedBy,
      string comment,
      Release release);

    IEnumerable<ReleaseEnvironmentStep> AcceptDeployStep(
      int releaseEnvironmentStepId,
      ReleaseEnvironmentStepStatus releaseEnvironmentStepStatus,
      Release release);

    void RejectGreenlightingStep(Release release, int releaseEnvironmentId, int releaseStepId);
  }
}
