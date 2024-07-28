// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.AgentReservationWebService4
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "AgentReservationService4", CollectionServiceIdentifier = "EE81D94D-9E12-4547-A878-F790748946AE")]
  [WebService(Name = "AgentReservationService", Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class AgentReservationWebService4 : BuildWebServiceBase
  {
    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public AgentReservation ReserveAgent(
      [ClientType(typeof (Uri))] string buildUri,
      string name,
      List<string> requiredTags,
      TagComparison tagComparison,
      Guid buildProjectId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReserveAgent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddArrayParameter<string>(nameof (requiredTags), (IList<string>) requiredTags);
        methodInformation.AddParameter(nameof (tagComparison), (object) tagComparison);
        methodInformation.AddParameter(nameof (buildProjectId), (object) buildProjectId);
        this.EnterMethod(methodInformation);
        return this.BuildResourceService.ReserveBuildAgent(this.RequestContext, buildProjectId, buildUri, name, (IList<string>) requiredTags, tagComparison);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [ClientServiceMethod(AsyncPattern = true, SyncPattern = true)]
    public void ReleaseAgent(int reservationId)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReleaseAgent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter("requestId", (object) reservationId);
        this.EnterMethod(methodInformation);
        this.BuildResourceService.ReleaseBuildAgent(this.RequestContext, reservationId);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "BuildAdministration", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
