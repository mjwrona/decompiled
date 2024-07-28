// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.GetUserStatusWithIdRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Graph;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph
{
  public class GetUserStatusWithIdRequest : MicrosoftGraphClientRequest<GetUserStatusWithIdResponse>
  {
    private const string PropertiesToFetch = "accountEnabled,signInSessionsValidFromDateTime";
    private const string EnableDeletedUsersVerboseResponseLogging = "VisualStudio.Services.Graph.EnableDeletedUsersVerboseResponseLogging";
    protected const string Layer = "GetUserStatusWithIdRequest";

    public Guid ObjectId { get; set; }

    internal override GetUserStatusWithIdResponse Execute(
      IVssRequestContext context,
      GraphServiceClient graphServiceClient)
    {
      try
      {
        return new GetUserStatusWithIdResponse(context.RunSynchronously<User>((Func<Task<User>>) (() => graphServiceClient.Users[this.ObjectId.ToString()].Request().Select("accountEnabled,signInSessionsValidFromDateTime").GetAsync(new CancellationToken()))));
      }
      catch (ServiceException ex)
      {
        if (ex.IsResourceNotFoundError())
        {
          string str;
          if (context.IsFeatureEnabled("VisualStudio.Services.Graph.EnableDeletedUsersVerboseResponseLogging"))
          {
            str = string.Format("Error Response: {0}", (object) ex.Error);
          }
          else
          {
            Error innerError = ex.Error.InnerError;
            KeyValuePair<string, object>? nullable;
            if (innerError == null)
            {
              nullable = new KeyValuePair<string, object>?();
            }
            else
            {
              IDictionary<string, object> additionalData = innerError.AdditionalData;
              nullable = additionalData != null ? new KeyValuePair<string, object>?(additionalData.FirstOrDefault<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (c => c.Key == "request-id"))) : new KeyValuePair<string, object>?();
            }
            str = string.Format("RequestId: {0}", (object) nullable);
          }
          JwtSecurityToken accessToken1 = this.AccessToken;
          Claim claim1;
          if (accessToken1 == null)
          {
            claim1 = (Claim) null;
          }
          else
          {
            IEnumerable<Claim> claims = accessToken1.Claims;
            claim1 = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "iss")) : (Claim) null;
          }
          Claim claim2 = claim1;
          JwtSecurityToken accessToken2 = this.AccessToken;
          Claim claim3;
          if (accessToken2 == null)
          {
            claim3 = (Claim) null;
          }
          else
          {
            IEnumerable<Claim> claims = accessToken2.Claims;
            claim3 = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "tid")) : (Claim) null;
          }
          Claim claim4 = claim3;
          context.TraceAlways(44750012, TraceLevel.Info, "MicrosoftGraph", nameof (GetUserStatusWithIdRequest), string.Format("Microsoft Graph API returned Not Found for user {0} oid. Iss Claim: {1}, Tid Claim {2}, {3}", (object) this.ObjectId, (object) claim2, (object) claim4, (object) str));
          return new GetUserStatusWithIdResponse();
        }
        context.TraceException(44750010, "MicrosoftGraph", nameof (GetUserStatusWithIdRequest), (Exception) ex);
        throw;
      }
      finally
      {
        context.Trace(44750013, TraceLevel.Info, "MicrosoftGraph", nameof (GetUserStatusWithIdRequest), "Leaving Execute method for {0} oid", (object) this.ObjectId);
      }
    }

    public override string ToString() => "GetUserStatusWithIdRequest{" + string.Format("ObjectId={0}", (object) this.ObjectId) + "}";
  }
}
