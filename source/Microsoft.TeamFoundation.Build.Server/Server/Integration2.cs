// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Integration2
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Services;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Build.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Linking/03", Description = "DevOps Build Integration web service")]
  [WebServiceBinding(Name = "IProjectMaintenanceBinding", Namespace = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03")]
  internal class Integration2 : BuildWebServiceBase, ILinkingProvider
  {
    [WebMethod]
    public Artifact[] GetArtifacts(string[] artifactUris)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (GetArtifacts), MethodType.Tool, EstimatedMethodCost.Low);
        methodInformation.AddArrayParameter<string>(nameof (artifactUris), (IList<string>) artifactUris);
        this.EnterMethod(methodInformation);
        ArgumentValidation.CheckUriArray(nameof (artifactUris), (IList<string>) artifactUris, true, (string) null);
        return BisQuery.GetV2Artifacts(this.RequestContext, artifactUris);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }

    [WebMethod]
    [SoapDocumentMethod(Binding = "IProjectMaintenanceBinding", Action = "http://schemas.microsoft.com/TeamFoundation/2005/06/Services/Admin/03/DeleteProject")]
    public bool DeleteProject(string projectUri)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (DeleteProject), MethodType.Tool, EstimatedMethodCost.Moderate);
        methodInformation.AddParameter(nameof (projectUri), (object) projectUri);
        this.EnterMethod(methodInformation);
        return this.BuildService.DeleteTeamProject(this.RequestContext, projectUri);
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceException(0, "Build", "Service", ex);
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
