// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration.TcmDataMigrationControllerBase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Tcm.TCMServiceDataMigration
{
  public abstract class TcmDataMigrationControllerBase : TfsApiController
  {
    private TestManagementRequestContext m_testManagementRequestContext;
    private static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static TcmDataMigrationControllerBase()
    {
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (InvalidPropertyException), HttpStatusCode.BadRequest);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.AccessDeniedException), HttpStatusCode.Forbidden);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectInUseException), HttpStatusCode.Forbidden);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TeamProjectNotFoundException), HttpStatusCode.NotFound);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException), HttpStatusCode.NotFound);
      TcmDataMigrationControllerBase.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
    }

    public override string ActivityLogArea => "Test Results";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TcmDataMigrationControllerBase.s_httpExceptions;

    protected TestManagementRequestContext TestManagementRequestContext
    {
      get
      {
        if (this.m_testManagementRequestContext == null)
          this.m_testManagementRequestContext = new TestManagementRequestContext(this.TfsRequestContext);
        return this.m_testManagementRequestContext;
      }
    }
  }
}
