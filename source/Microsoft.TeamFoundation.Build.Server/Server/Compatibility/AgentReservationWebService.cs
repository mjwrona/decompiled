// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.AgentReservationWebService
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [WebService]
  [ClientService(ComponentName = "TeamBuild", RegistrationName = "Build", ServiceName = "AgentReservationService", CollectionServiceIdentifier = "d64516b0-fc8d-41a5-9319-584474c267c4")]
  public sealed class AgentReservationWebService : BuildWebServiceBase
  {
    [WebMethod]
    public AgentReservation2010 ReserveAgent(
      [ClientType(typeof (Uri))] string buildUri,
      string name,
      List<string> requiredTags,
      TagComparison2010 tagComparison)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (ReserveAgent), MethodType.ReadWrite, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (buildUri), (object) buildUri);
        methodInformation.AddParameter(nameof (name), (object) name);
        methodInformation.AddArrayParameter<string>(nameof (requiredTags), (IList<string>) requiredTags);
        methodInformation.AddParameter(nameof (tagComparison), (object) tagComparison);
        this.EnterMethod(methodInformation);
        List<string> uris = new List<string>(1);
        uris.Add(buildUri);
        RosarioHelper.ConvertBuildUris(this.RequestContext, (IList<string>) uris, false, false);
        return RosarioHelper.Convert(this.BuildResourceService.ReserveBuildAgent(this.RequestContext, Guid.Empty, uris[0], name, (IList<string>) requiredTags, RosarioHelper.Convert(tagComparison)), buildUri);
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
