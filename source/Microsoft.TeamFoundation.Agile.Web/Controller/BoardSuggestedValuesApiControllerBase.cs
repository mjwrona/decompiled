// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardSuggestedValuesApiControllerBase
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Net;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public abstract class BoardSuggestedValuesApiControllerBase : TfsProjectApiController
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlTypeException>(HttpStatusCode.BadRequest);
    }

    protected Guid? ProjectGuid
    {
      get
      {
        Guid? projectGuid = new Guid?();
        if (this.ProjectId != Guid.Empty)
          projectGuid = new Guid?(this.ProjectId);
        return projectGuid;
      }
    }

    public override string TraceArea => "Boards";

    public override string ActivityLogArea => "Boards";
  }
}
