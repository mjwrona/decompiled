// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.NewDomainUrlMigrationJobManager`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class NewDomainUrlMigrationJobManager<T> : ServicingOrchestrationJobManager<T> where T : ServicingOrchestrationRequest
  {
    public static readonly Guid NewDomainUrlMigrationJobId = new Guid("2999C635-4B7F-4082-A052-D52CF3353FFF");

    public override string Area => "NewDomainUrlMigration";

    protected override JobPriorityClass PriorityClass => JobPriorityClass.High;

    protected override JobPriorityLevel PriorityLevel => JobPriorityLevel.Highest;

    protected override TimeSpan RetryInterval => TimeSpan.FromMinutes(1.0);

    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      T request,
      TeamFoundationJobDefinition jobDefinition)
    {
      if (request.RequestId != NewDomainUrlMigrationJobManager<T>.NewDomainUrlMigrationJobId || request.ServicingJobId != NewDomainUrlMigrationJobManager<T>.NewDomainUrlMigrationJobId)
        throw new ArgumentException("Unexpected job request id").AsFatalServicingOrchestrationException<ArgumentException>();
    }
  }
}
