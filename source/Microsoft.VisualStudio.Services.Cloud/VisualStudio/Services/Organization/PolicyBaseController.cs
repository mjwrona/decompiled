// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.PolicyBaseController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class PolicyBaseController : TfsApiController
  {
    private static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (NotSupportedException),
        HttpStatusCode.ServiceUnavailable
      },
      {
        typeof (OrganizationException),
        HttpStatusCode.InternalServerError
      },
      {
        typeof (UnexpectedHostTypeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (OrganizationBadRequestException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (OrganizationServiceSecurityException),
        HttpStatusCode.Forbidden
      }
    };

    public override string ActivityLogArea => "Organization";

    public override string TraceArea => "Organization";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PolicyBaseController.s_httpExceptions;

    protected static void ValidateContext(IVssRequestContext requestContext)
    {
      requestContext.CheckHostedDeployment();
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.OrganizationPolicy.RestApi.Disable"))
        throw new NotSupportedException();
    }

    protected static IList<string> ParseCommaSeparatedString(string commaSeparatedPolicyNames)
    {
      if (string.IsNullOrWhiteSpace(commaSeparatedPolicyNames))
        return (IList<string>) new string[0];
      return (IList<string>) commaSeparatedPolicyNames.Split(',');
    }
  }
}
