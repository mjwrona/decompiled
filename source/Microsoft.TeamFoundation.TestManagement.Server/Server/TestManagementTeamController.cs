// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementTeamController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public class TestManagementTeamController : TfsTeamApiController
  {
    private TfsTestManagementRequestContext m_testManagementRequestContext;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static TestManagementTeamController()
    {
      TestManagementTeamController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      TestManagementTeamController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      TestManagementTeamController.s_httpExceptions.Add(typeof (TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementTeamController.s_httpExceptions.Add(typeof (InvalidPropertyException), HttpStatusCode.BadRequest);
      TestManagementTeamController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      TestManagementTeamController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException), HttpStatusCode.Forbidden);
      TestManagementTeamController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementTeamController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementTeamController.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      TestManagementTeamController.s_httpExceptions.Add(typeof (ProjectNotFoundException), HttpStatusCode.NotFound);
      TestManagementTeamController.s_httpExceptions.Add(typeof (InvalidStructurePathException), HttpStatusCode.BadRequest);
      TestManagementTeamController.s_httpExceptions.Add(typeof (TestManagementValidationException), HttpStatusCode.BadRequest);
      TestManagementTeamController.s_httpExceptions.Add(typeof (MissingLicenseException), HttpStatusCode.Forbidden);
      TestManagementTeamController.s_httpExceptions.Add(typeof (WorkItemTrackingFieldDefinitionNotFoundException), HttpStatusCode.BadRequest);
    }

    public override string ActivityLogArea => "Test Management REST";

    internal TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TfsTestManagementRequestContext(this.TfsRequestContext);
        return (TestManagementRequestContext) this.m_testManagementRequestContext;
      }
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TestManagementTeamController.s_httpExceptions;
  }
}
