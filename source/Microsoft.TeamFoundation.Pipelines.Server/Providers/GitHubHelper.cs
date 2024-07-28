// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public static class GitHubHelper
  {
    private const string c_layer = "GitHubHelper";
    private const string c_authInstallationId = "installationId";
    private const string c_authJwtSignature = "JwtSignature";
    private const string c_dockerTag = "tag";
    private const string c_dockerRegistry = "registry";
    private const string c_dockerServer = "server";

    public static string GetRepoOwner(string repositoryFullName)
    {
      string[] strArray = !string.IsNullOrEmpty(repositoryFullName) ? repositoryFullName.Split('/') : throw new ArgumentException("Unable to determine the owner from repository " + repositoryFullName + ".");
      if (strArray.Length == 2)
        return strArray[0];
    }

    public static GitHubAuthentication GetAuthentication(
      this IVssRequestContext requestContext,
      string serviceEndpointGuid,
      Guid projectId)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ServiceEndpoint serviceEndpoint = vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(vssRequestContext, projectId, new Guid(serviceEndpointGuid));
      if (serviceEndpoint != null)
        return serviceEndpoint.GetGitHubAuthentication(requestContext, projectId);
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.UpdateStatus, nameof (GitHubHelper), string.Format("No service endpoint id found with id {0}; project id = {1}", (object) serviceEndpointGuid, (object) projectId));
      throw new PipelineStatusException(PipelinesResources.ExceptionServiceEndpointNotFound((object) serviceEndpointGuid));
    }

    public static JObject GetProviderAuthentication(
      IVssRequestContext requestContext,
      JObject eventPayload)
    {
      JObject providerAuthentication = new JObject();
      object obj1 = (object) eventPayload;
      // ISSUE: reference to a compiler-generated field
      if (GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubHelper)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target = GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p2 = GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "installation", typeof (GitHubHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__0.Target((CallSite) GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__0, obj1);
      object obj3;
      if (obj2 == null)
      {
        obj3 = (object) null;
      }
      else
      {
        // ISSUE: reference to a compiler-generated field
        if (GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object>>.Create(Binder.GetMember(CSharpBinderFlags.None, "id", typeof (GitHubHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        obj3 = GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__1.Target((CallSite) GitHubHelper.\u003C\u003Eo__8.\u003C\u003Ep__1, obj2);
      }
      string installationId = target((CallSite) p2, obj3);
      providerAuthentication["installationId"] = (JToken) installationId;
      providerAuthentication["JwtSignature"] = (JToken) GitHubHelper.GetInMemoryJwtSignature(requestContext, installationId);
      return providerAuthentication;
    }

    public static void GetAuthenticationElements(
      JObject providerAuthentication,
      out string installationId)
    {
      object obj1 = (object) providerAuthentication;
      ref string local = ref installationId;
      // ISSUE: reference to a compiler-generated field
      if (GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string>>.Create(Binder.Convert(CSharpBinderFlags.None, typeof (string), typeof (GitHubHelper)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string> target = GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string>> p1 = GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, string, object>>.Create(Binder.GetIndex(CSharpBinderFlags.None, typeof (GitHubHelper), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj2 = GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__0.Target((CallSite) GitHubHelper.\u003C\u003Eo__9.\u003C\u003Ep__0, obj1, nameof (installationId));
      string str = target((CallSite) p1, obj2);
      local = str;
      ArgumentUtility.CheckStringForNullOrEmpty(installationId, nameof (installationId));
    }

    private static string GetInMemoryJwtSignature(
      IVssRequestContext requestContext,
      string installationId)
    {
      string str = requestContext.ServiceHost.InstanceId.ToString("D");
      return installationId.ToJwtToken(Encoding.UTF8.GetBytes(str + installationId));
    }
  }
}
