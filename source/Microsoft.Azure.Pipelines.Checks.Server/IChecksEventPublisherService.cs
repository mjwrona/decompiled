// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.IChecksEventPublisherService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  [DefaultServiceImplementation(typeof (ChecksEventPublisherService))]
  public interface IChecksEventPublisherService : IVssFrameworkService
  {
    void NotifyCheckSuiteUpdatedEvent(
      IVssRequestContext requestContext,
      string eventType,
      Guid projectId,
      CheckSuite response);
  }
}
