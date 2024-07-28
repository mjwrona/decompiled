// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.ReleaseClaimSessionRequestProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Releases;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class ReleaseClaimSessionRequestProvider : ISessionRequestProvider
  {
    private readonly IVssRequestContext requestContext;

    public ReleaseClaimSessionRequestProvider(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task<(bool Success, SessionRequest Request)> TryGetSessionRequest()
    {
      IVssRequestContext requestContext = this.requestContext;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, new TraceData()
      {
        Area = "Packaging",
        Layer = nameof (TryGetSessionRequest)
      }, 5726900, nameof (TryGetSessionRequest)))
      {
        IPrincipal user = HttpContextFactory.Current?.User;
        if (user != null)
        {
          if (user is ClaimsPrincipal claimsPrincipal)
          {
            IEnumerable<Claim> claims = claimsPrincipal.Claims;
            Guid projectId;
            int releaseId1;
            if (claims != null && claims.TryGetRelease(out projectId, out releaseId1))
            {
              try
              {
                IVssRequestContext vssRequestContext = this.requestContext.Elevate();
                ReleaseHttpClient client = this.requestContext.GetClient<ReleaseHttpClient>();
                Guid project = projectId;
                int releaseId2 = releaseId1;
                ApprovalFilters? approvalFilters = new ApprovalFilters?(ApprovalFilters.None);
                SingleReleaseExpands? expand = new SingleReleaseExpands?(SingleReleaseExpands.None);
                object obj = (object) vssRequestContext;
                int? topGateRecords = new int?();
                object userState = obj;
                CancellationToken cancellationToken = new CancellationToken();
                Release releaseAsync = await client.GetReleaseAsync(project, releaseId2, approvalFilters, expand: expand, topGateRecords: topGateRecords, userState: userState, cancellationToken: cancellationToken);
                if (releaseAsync != null)
                  return (true, ReleaseClaimSessionRequestProvider.ConvertReleaseToSessionRequest(releaseAsync, this.requestContext.ServiceHost.InstanceId));
                tracer.TraceWarning("Failed to retrieve Release Session Information from releaseClient.");
              }
              catch (Exception ex)
              {
                this.requestContext.TraceCatch(5726900, "Packaging", nameof (TryGetSessionRequest), ex);
              }
            }
            else if (claims == null)
              tracer.TraceWarning("Failed to retrieve Release Session Information for null pipeline claims.");
            else if (!claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "ReleaseId")))
              tracer.TraceWarning("Failed to retrieve Release Session Information, claims are not ReleaseId type.");
            else
              tracer.TraceWarning("Failed to retrieve releaseId and projectId from pipeline claims.");
          }
          else
            tracer.TraceWarning("Failed to retrieve Release Session Information for null ClaimsPrincipal.");
        }
        else
          tracer.TraceWarning("Failed to retrieve Release Session Information for null contextUser.");
      }
      return (false, (SessionRequest) null);
    }

    private static SessionRequest ConvertReleaseToSessionRequest(Release release, Guid collectionId) => new SessionRequest()
    {
      Source = "InternalRelease",
      Data = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "System.CollectionId",
          collectionId.ToString()
        },
        {
          "Release.DefinitionId",
          release.ReleaseDefinitionReference?.Id.ToString()
        },
        {
          "Release.DefinitionName",
          release.ReleaseDefinitionReference?.Name?.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "System.TeamProjectId",
          release.ProjectReference?.Id.ToString()
        },
        {
          "Release.ReleaseName",
          release.Name?.ToString((IFormatProvider) CultureInfo.InvariantCulture)
        },
        {
          "Release.ReleaseId",
          release.Id.ToString()
        }
      }
    };
  }
}
