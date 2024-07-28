// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.BuildClaimSessionRequestProvider
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class BuildClaimSessionRequestProvider : ISessionRequestProvider
  {
    private readonly IVssRequestContext requestContext;

    public BuildClaimSessionRequestProvider(IVssRequestContext requestContext) => this.requestContext = requestContext;

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
            int buildId;
            if (claims != null && claims.TryGetBuildId(out projectId, out buildId))
            {
              try
              {
                Microsoft.TeamFoundation.Build.WebApi.Build buildAsync = await this.requestContext.GetClient<BuildHttpClient>().GetBuildAsync(projectId, buildId);
                if (buildAsync != null)
                  return (true, BuildClaimSessionRequestProvider.ConvertBuildToSessionRequest(buildAsync, this.requestContext.ServiceHost.InstanceId));
                tracer.TraceWarning("Failed to retrieve Build Session Information from buildClient.");
              }
              catch (Exception ex)
              {
                this.requestContext.TraceCatch(5726900, "Packaging", nameof (TryGetSessionRequest), ex);
              }
            }
            else if (claims == null)
              tracer.TraceWarning("Failed to retrieve Build Session Information for null pipeline claims.");
            else if (!claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "BuildId")))
              tracer.TraceWarning("Failed to retrieve Build Session Information, claims are not BuildId type.");
            else
              tracer.TraceWarning("Failed to retrieve buildId and projectId from pipeline claims.");
          }
          else
            tracer.TraceWarning("Failed to retrieve Build Session Information for null ClaimsPrincipal.");
        }
        else
          tracer.TraceWarning("Failed to retrieve Build Session Information for null contextUser.");
      }
      return (false, (SessionRequest) null);
    }

    private static SessionRequest ConvertBuildToSessionRequest(Microsoft.TeamFoundation.Build.WebApi.Build build, Guid collectionId)
    {
      SessionRequest sessionRequest = new SessionRequest();
      sessionRequest.Source = "InternalBuild";
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      dictionary1.Add("System.CollectionId", collectionId.ToString());
      dictionary1.Add("System.DefinitionId", build.Definition.Id.ToString());
      dictionary1.Add("System.TeamProjectId", build.Definition.Project?.Id.ToString());
      dictionary1.Add("System.TeamProject", build.Definition.Project?.Name.ToString());
      dictionary1.Add("Build.BuildId", build.Id.ToString());
      dictionary1.Add("Build.BuildNumber", build.BuildNumber);
      dictionary1.Add("Build.DefinitionName", build.Definition.Name);
      int? revision = build.Definition.Revision;
      ref int? local = ref revision;
      dictionary1.Add("Build.DefinitionRevision", local.HasValue ? local.GetValueOrDefault().ToString() : (string) null);
      dictionary1.Add("Build.Repository.Name", build.Repository.Name ?? build.Repository.Id);
      dictionary1.Add("Build.Repository.Provider", build.Repository.Type);
      dictionary1.Add("Build.Repository.Id", build.Repository.Id);
      dictionary1.Add("Build.SourceBranch", build.SourceBranch);
      dictionary1.Add("Build.SourceBranchName", BuildClaimSessionRequestProvider.GetShortBranchName(build.SourceBranch));
      dictionary1.Add("Build.SourceVersion", build.SourceVersion);
      Dictionary<string, string> dictionary2 = dictionary1;
      if (build.Repository.Url != (Uri) null)
        dictionary2.Add("Build.Repository.Uri", build.Repository.Url.ToString());
      sessionRequest.Data = (IDictionary<string, string>) dictionary2;
      return sessionRequest;
    }

    private static string GetShortBranchName(string branchName)
    {
      if (!string.IsNullOrEmpty(branchName))
      {
        branchName = branchName.TrimEnd('/');
        int num = branchName.LastIndexOf('/');
        if (num >= 0 && num < branchName.Length)
          branchName = branchName.Substring(num + 1);
      }
      if (branchName == null)
        branchName = string.Empty;
      return branchName;
    }
  }
}
