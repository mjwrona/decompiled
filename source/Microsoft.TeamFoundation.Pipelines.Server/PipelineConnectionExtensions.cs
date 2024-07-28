// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelineConnectionExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Claims;
using System.Text;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class PipelineConnectionExtensions
  {
    private const int c_maxTokenLength = 2024;
    private const string c_layer = "PipelineConnectionExtensions";
    private const string c_strongbox_drawer = "XLaunchTokens";
    private const string c_jwt_audience = "vsts";
    private const string c_jwt_issuer = "vsts.xlaunch";
    private const string c_jwt_accountId = "accountId";
    private const string c_jwt_projectId = "projectId";
    private const string c_jwt_definitionId = "definitionId";

    public static PipelineConnection ConnectionFromToken(
      IVssRequestContext requestContext,
      string token,
      bool throwIfInvalid = true,
      bool skipValidation = false)
    {
      PipelineConnection pipelineConnection = (PipelineConnection) null;
      if (!string.IsNullOrEmpty(token))
      {
        if (token.Contains("."))
        {
          try
          {
            JsonWebToken webToken = JsonWebToken.Create(token);
            pipelineConnection = PipelineConnectionExtensions.GetConnectionFromWebToken(webToken);
            if (!skipValidation)
            {
              requestContext.CheckProjectCollectionOrOrganizationRequestContext();
              string generateStrongBoxSecret = requestContext.Elevate().GetOrGenerateStrongBoxSecret("XLaunchTokens", pipelineConnection.TeamProjectId.ToString());
              PipelineConnectionExtensions.ValidateWebToken(webToken, generateStrongBoxSecret);
            }
          }
          catch (JsonWebTokenValidationException ex)
          {
            ((IVssRequestContext) null).TraceError(TracePoints.Connections.Extensions, nameof (PipelineConnectionExtensions), "Unable to create pipeline connection from token: " + token + ". Details: " + ex.ToString());
            if (throwIfInvalid)
              throw new PipelineConnectionException(PipelinesResources.ExceptionInvalidToken());
            return (PipelineConnection) null;
          }
        }
        else
        {
          try
          {
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            if (!JsonUtilities.TryDeserialize<PipelineConnection>(json, out pipelineConnection, true) & throwIfInvalid)
            {
              ((IVssRequestContext) null).TraceError(TracePoints.Connections.Extensions, nameof (PipelineConnectionExtensions), "Unable to create pipeline connection from json: " + json);
              throw new PipelineConnectionException(PipelinesResources.ExceptionInvalidToken());
            }
          }
          catch (FormatException ex)
          {
            if (throwIfInvalid)
            {
              ((IVssRequestContext) null).TraceError(TracePoints.Connections.Extensions, nameof (PipelineConnectionExtensions), "Unable to create pipeline connection from token: " + token);
              throw new PipelineConnectionException(PipelinesResources.ExceptionInvalidToken());
            }
          }
        }
      }
      return pipelineConnection;
    }

    public static string ToToken(
      this PipelineConnection connection,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<PipelineConnection>(connection, nameof (connection));
      requestContext.CheckProjectCollectionOrOrganizationRequestContext();
      string generateStrongBoxSecret = requestContext.Elevate().GetOrGenerateStrongBoxSecret("XLaunchTokens", connection.TeamProjectId.ToString());
      Claim[] additionalClaims = new Claim[2]
      {
        new Claim("accountId", connection.AccountId.ToString()),
        new Claim("projectId", connection.TeamProjectId.ToString())
      };
      DateTime dateTime = new DateTime(1, 1, 1, 1, 1, 1, DateTimeKind.Utc);
      string encodedToken = JsonWebToken.Create("vsts.xlaunch", "vsts", dateTime, dateTime, (IEnumerable<Claim>) additionalClaims, VssSigningCredentials.Create(Encoding.UTF8.GetBytes(generateStrongBoxSecret))).EncodedToken;
      if (encodedToken.Length <= 2024)
        return encodedToken;
      ((IVssRequestContext) null).TraceError(TracePoints.Connections.Extensions, nameof (PipelineConnectionExtensions), string.Format("Generated token is too large. Reduce the size of the strings in the connection. MaxLength={0}", (object) 2024));
      throw new PipelineConnectionException(PipelinesResources.ExceptionTokenGenerationFailed((object) 2024));
    }

    public static PipelineConnection Clone(this PipelineConnection connection)
    {
      ArgumentUtility.CheckForNull<PipelineConnection>(connection, nameof (connection));
      return new PipelineConnection()
      {
        AccountId = connection.AccountId,
        ServiceEndpointId = connection.ServiceEndpointId,
        TeamProjectId = connection.TeamProjectId,
        RedirectUrl = connection.RedirectUrl
      };
    }

    private static void ValidateWebToken(JsonWebToken webToken, string secret)
    {
      JsonWebTokenValidationParameters parameters = new JsonWebTokenValidationParameters()
      {
        ValidateActor = false,
        ValidateAudience = true,
        AllowedAudiences = (IEnumerable<string>) new string[1]
        {
          "vsts"
        },
        ValidateExpiration = false,
        ValidateIssuer = true,
        ValidIssuers = (IEnumerable<string>) new string[1]
        {
          "vsts.xlaunch"
        },
        ValidateNotBefore = false,
        ValidateSignature = true,
        SigningCredentials = VssSigningCredentials.Create(Encoding.UTF8.GetBytes(secret))
      };
      webToken.ValidateToken(parameters);
    }

    private static PipelineConnection GetConnectionFromWebToken(JsonWebToken webToken)
    {
      JsonWebTokenValidationParameters parameters = new JsonWebTokenValidationParameters()
      {
        ValidateActor = false,
        ValidateAudience = false,
        ValidateExpiration = false,
        ValidateIssuer = false,
        ValidateNotBefore = false,
        ValidateSignature = false
      };
      return PipelineConnectionExtensions.GetConnectionFromClaims(webToken.ValidateToken(parameters));
    }

    private static PipelineConnection GetConnectionFromClaims(ClaimsPrincipal claimsPrincipal)
    {
      PipelineConnection connectionFromClaims = new PipelineConnection();
      foreach (Claim claim in claimsPrincipal.Claims)
      {
        try
        {
          switch (claim.Type)
          {
            case "accountId":
              connectionFromClaims.AccountId = new Guid?(new Guid(claim.Value));
              continue;
            case "projectId":
              connectionFromClaims.TeamProjectId = new Guid?(new Guid(claim.Value));
              continue;
            default:
              continue;
          }
        }
        catch (Exception ex) when (
        {
          // ISSUE: unable to correctly present filter
          int num;
          switch (ex)
          {
            case ArgumentNullException _:
            case FormatException _:
              num = 1;
              break;
            default:
              num = ex is OverflowException ? 1 : 0;
              break;
          }
          if ((uint) num > 0U)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
        }
      }
      if (!connectionFromClaims.AccountId.HasValue || !connectionFromClaims.TeamProjectId.HasValue)
        throw new JsonWebTokenValidationException(PipelinesResources.ExceptionInvalidToken());
      return connectionFromClaims;
    }
  }
}
