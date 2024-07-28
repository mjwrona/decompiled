// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ServerCoreApiController
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class ServerCoreApiController : TfsApiController
  {
    private const int c_minTop = 1;
    private const int c_minSkip = 0;
    private const int c_defaultTop = 100;
    private const int c_defaultSkip = 0;
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();

    static ServerCoreApiController()
    {
      ServerCoreApiController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ArgumentException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (UnexpectedHostTypeException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (InvalidProjectNameException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectAlreadyExistsException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectNameNotRecognizedException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectWorkPendingException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (CannotCreateProjectsWithDisableProcessException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (InvalidProjectDeleteException), HttpStatusCode.BadRequest);
      ServerCoreApiController.s_httpExceptions.Add(typeof (UnauthorizedAccessException), HttpStatusCode.Forbidden);
      ServerCoreApiController.s_httpExceptions.Add(typeof (CollectionDoesNotExistException), HttpStatusCode.NotFound);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectDoesNotExistWithNameException), HttpStatusCode.NotFound);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectDoesNotExistException), HttpStatusCode.NotFound);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProjectNotFoundException), HttpStatusCode.NotFound);
      ServerCoreApiController.s_httpExceptions.Add(typeof (TeamNotFoundException), HttpStatusCode.NotFound);
      ServerCoreApiController.s_httpExceptions.Add(typeof (ProcessNotFoundByTypeIdException), HttpStatusCode.NotFound);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) ServerCoreApiController.s_httpExceptions;

    public override string TraceArea => "ServerCoreRestService";

    public override string ActivityLogArea => "Framework";

    protected static void EvaluateTopSkip(
      int? top,
      int? skip,
      out int topValue,
      out int skipValue)
    {
      ref int local1 = ref topValue;
      int? nullable;
      int num1;
      if (top.HasValue)
      {
        nullable = top;
        int num2 = 1;
        if (!(nullable.GetValueOrDefault() < num2 & nullable.HasValue))
        {
          num1 = top.Value;
          goto label_4;
        }
      }
      num1 = 100;
label_4:
      local1 = num1;
      ref int local2 = ref skipValue;
      int num3;
      if (skip.HasValue)
      {
        nullable = skip;
        int num4 = 0;
        if (!(nullable.GetValueOrDefault() < num4 & nullable.HasValue))
        {
          num3 = skip.Value;
          goto label_8;
        }
      }
      num3 = 0;
label_8:
      local2 = num3;
    }
  }
}
