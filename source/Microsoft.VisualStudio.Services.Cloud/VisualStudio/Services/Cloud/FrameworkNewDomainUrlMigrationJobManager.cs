// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.FrameworkNewDomainUrlMigrationJobManager
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class FrameworkNewDomainUrlMigrationJobManager : 
    NewDomainUrlMigrationJobManager<FrameworkNewDomainUrlMigrationRequest>
  {
    protected override void ValidateRequest(
      IVssRequestContext requestContext,
      FrameworkNewDomainUrlMigrationRequest request,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.ValidateRequest(requestContext, request, jobDefinition);
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      if (request.HostId != requestContext.ServiceHost.InstanceId)
        throw new ArgumentException("Unexpected host id").AsFatalServicingOrchestrationException<ArgumentException>();
      if (!(request is ResetRoutingDataRequest))
        throw new ArgumentException("Unexpected request type").AsFatalServicingOrchestrationException<ArgumentException>();
    }
  }
}
