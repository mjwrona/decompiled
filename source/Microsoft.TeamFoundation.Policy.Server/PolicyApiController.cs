// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyApiController
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace Microsoft.TeamFoundation.Policy.Server
{
  public class PolicyApiController : TfsProjectApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (FormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (JsonSerializationException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (NotSupportedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (TeamFoundationServiceException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ProjectException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PolicySettingsFormatException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PolicyTypeCannotBeChangedException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (PolicySettingsTooLargeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (CannotCreateNewDeprecatedScopeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (PolicyChangeRejectedByPolicyException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (PolicyNeedsPermissionException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (FeatureDisabledException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PolicyConfigurationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PolicyTypeNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProjectDoesNotExistWithNameException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProjectDoesNotExistException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ProjectNameNotRecognizedException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PolicyEvaluationNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (ArtifactNotFoundException),
        HttpStatusCode.NotFound
      },
      {
        typeof (PolicyConfigurationUpdatedByAnotherRequestException),
        HttpStatusCode.Conflict
      }
    };

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PolicyApiController.s_httpExceptions;

    [ExcludeFromCodeCoverage]
    public override string TraceArea => "Policy";

    [ExcludeFromCodeCoverage]
    public override string ActivityLogArea => "Policy";

    protected ITeamFoundationPolicyService PolicyService => this.TfsRequestContext.GetService<ITeamFoundationPolicyService>();

    protected Guid GetProjectId(string projectIdOrName)
    {
      ArgumentUtility.CheckForNull<string>(projectIdOrName, "projectId");
      IProjectService service = this.TfsRequestContext.GetService<IProjectService>();
      Guid result;
      if (Guid.TryParse(projectIdOrName, out result))
        service.GetProjectName(this.TfsRequestContext, result);
      else
        result = service.GetProjectId(this.TfsRequestContext, projectIdOrName);
      return result;
    }
  }
}
