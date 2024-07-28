// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelAssignmentsController
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.PermissionLevel
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName("PermissionLevel", "PermissionLevelAssignments", 1)]
  public class PermissionLevelAssignmentsController : TfsApiController
  {
    private const string c_PermissionLevelAssignmentsApis = "Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable";
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (NotSupportedException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (NotImplementedException),
        HttpStatusCode.NotImplemented
      },
      {
        typeof (UnexpectedHostTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AccessCheckException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PermissionLevelAssignmentNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PermissionLevelAssignmentAlreadyExistsException),
        HttpStatusCode.Conflict
      },
      {
        typeof (PermissionLevelAssignmentBadRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PermissionLevelDefinitionNotFoundException),
        HttpStatusCode.BadRequest
      }
    };

    public override string ActivityLogArea => "PermissionLevelAssignment";

    public override string TraceArea => "PermissionLevelAssignment";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PermissionLevelAssignmentsController.s_httpExceptions;

    [HttpGet]
    public PagedPermissionLevelAssignment GetPermissionLevelAssignmentsByDefinitionId(
      string resourceId,
      Guid definitionId,
      int? pageSize = null,
      string continuationToken = null)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      if (pageSize.HasValue)
      {
        int? nullable = pageSize;
        int num = 1;
        if (nullable.GetValueOrDefault() < num & nullable.HasValue)
          throw new PermissionLevelAssignmentBadRequestException("Parameter 'pageSize' must either be omitted or must be a positive integer");
      }
      return this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>().GetPermissionLevelAssignments(this.TfsRequestContext, definitionId, resourceId, pageSize, continuationToken);
    }

    [HttpGet]
    public PagedPermissionLevelAssignment GetPermissionLevelAssignmentsByScope(
      string resourceId,
      PermissionLevelDefinitionScope scope,
      int? pageSize = null,
      string continuationToken = null)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      if (pageSize.HasValue)
      {
        int? nullable = pageSize;
        int num = 1;
        if (nullable.GetValueOrDefault() < num & nullable.HasValue)
          throw new PermissionLevelAssignmentBadRequestException("Parameter 'pageSize' must either be omitted or must be a positive integer");
      }
      return this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>().GetPermissionLevelAssignments(this.TfsRequestContext, scope, resourceId, pageSize, continuationToken);
    }

    [HttpGet]
    public IEnumerable<PermissionLevelAssignment> GetPermissionLevelAssignmentsByScopeAndSubject(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      string resourceId,
      PermissionLevelDefinitionScope scope)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>().GetPermissionLevelAssignments(this.TfsRequestContext, scope, resourceId, subjectDescriptor);
    }

    [HttpPost]
    public PermissionLevelAssignment AssignPermissionLevel(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      Guid definitionId,
      string resourceId)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      return this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>().AssignPermissionLevel(this.TfsRequestContext, definitionId, resourceId, subjectDescriptor);
    }

    [HttpPut]
    public PermissionLevelAssignment UpdatePermissionLevelAssignment(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      string resourceId,
      Guid definitionId,
      Guid newDefinitionId)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      IPermissionLevelAssignmentService service = this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>();
      PermissionLevelAssignment permissionLevelAssignment1 = new PermissionLevelAssignment(definitionId, resourceId, subjectDescriptor);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      PermissionLevelAssignment permissionLevelAssignment2 = permissionLevelAssignment1;
      Guid newDefinitionId1 = newDefinitionId;
      return service.UpdatePermissionLevelAssignment(tfsRequestContext, permissionLevelAssignment2, newDefinitionId1);
    }

    [HttpDelete]
    public void RemovePermissionLevelAssignment(
      [ClientParameterType(typeof (string), false)] SubjectDescriptor subjectDescriptor,
      Guid definitionId,
      string resourceId)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.PermissionLevel.PermissionLevelAssignmentApis.Enable"))
        throw new NotImplementedException();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      IPermissionLevelAssignmentService service = this.TfsRequestContext.GetService<IPermissionLevelAssignmentService>();
      PermissionLevelAssignment permissionLevelAssignment1 = new PermissionLevelAssignment(definitionId, resourceId, subjectDescriptor);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      PermissionLevelAssignment permissionLevelAssignment2 = permissionLevelAssignment1;
      service.RemovePermissionLevelAssignment(tfsRequestContext, permissionLevelAssignment2);
    }
  }
}
