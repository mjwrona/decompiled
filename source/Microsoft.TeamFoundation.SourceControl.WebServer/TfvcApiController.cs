// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcApiController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ApiTelemetry(true, false)]
  public class TfvcApiController : TfsProjectApiController
  {
    public const int c_defaultShallowCommentLength = 80;
    public const int c_defaultDeepCommentLength = 2000;
    public const string c_linkHeader = "Link";
    public static readonly char[] s_orderByFieldSeparator = new char[1]
    {
      ','
    };
    public static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();
    protected const int c_defaultTop = 100;

    static TfvcApiController()
    {
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidPathException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidVersionSpecException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidArgumentValueException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidVersionException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (ItemBatchNotFoundException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (DateVersionSpecBeforeBeginningOfRepositoryException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (IllegalLabelNameException), HttpStatusCode.BadRequest);
      TfvcApiController.s_httpExceptions.Add(typeof (AuthorizationException), HttpStatusCode.Unauthorized);
      TfvcApiController.s_httpExceptions.Add(typeof (UnauthorizedAccessException), HttpStatusCode.Forbidden);
      TfvcApiController.s_httpExceptions.Add(typeof (ItemNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (ShelvesetNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (IdentityNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (LabelNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (ChangesetBatchNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (ChangesetNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (BranchNotFoundException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (Microsoft.TeamFoundation.VersionControl.Server.DestroyedContentUnavailableException), HttpStatusCode.NotFound);
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidItemAtRootException), HttpStatusCode.InternalServerError);
      TfvcApiController.s_httpExceptions.Add(typeof (InvalidSqlDateException), HttpStatusCode.InternalServerError);
      TfvcApiController.s_httpExceptions.Add(typeof (ProjectCatalogNodeMissingException), HttpStatusCode.InternalServerError);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) TfvcApiController.s_httpExceptions;

    public override string ActivityLogArea => "Version Control";

    public override string TraceArea => "TfvcRestService";

    internal string ProjectScopedPath(string path)
    {
      if (string.IsNullOrEmpty(path))
        return "$/" + this.ProjectInfo.Name;
      if (path[0] == '$')
      {
        if (!this.TfsRequestContext.IsFeatureEnabled("Tfvc.ProjectScopedPaths") || path.Equals("$/" + this.ProjectInfo.Name, StringComparison.OrdinalIgnoreCase) || path.StartsWith("$/" + this.ProjectInfo.Name + (object) '/', StringComparison.OrdinalIgnoreCase))
          return path;
        throw new InvalidPathException(Resources.Format("RequestProjectScopeConflict", (object) path, (object) ("$/" + this.ProjectInfo.Name)));
      }
      if (path[0] == '/')
        return "$/" + this.ProjectInfo.Name + path;
      return "$/" + this.ProjectInfo.Name + (object) '/' + path;
    }
  }
}
