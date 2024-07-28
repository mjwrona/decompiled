// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.GitHubUserProvider
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.ExternalProviders.Common
{
  public static class GitHubUserProvider
  {
    private const string Area = "Github";
    private const string Layer = "GitHubUserProvider";

    public static GitHubData.V3.Owner GetCurrentUser(
      IVssRequestContext requestContext,
      GraphFederatedProviderData providerData = null)
    {
      return providerData != null ? GitHubUserProvider.Execute<GitHubData.V3.Owner>(requestContext, (Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.Owner>>) ((client, auth) => client.GetCurrentUser(auth)), providerData, (Func<GitHubResult<GitHubData.V3.Owner>, GitHubData.V3.Owner>) null) : GitHubUserProvider.Execute<GitHubData.V3.Owner>(requestContext, (Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.Owner>>) ((client, auth) => client.GetCurrentUser(auth)), new SubjectDescriptor(), (Func<GitHubResult<GitHubData.V3.Owner>, GitHubData.V3.Owner>) null);
    }

    public static GitHubData.V3.User[] SearchUsers(
      IVssRequestContext requestContext,
      string searchQuery,
      int maxResults)
    {
      return GitHubUserProvider.Execute<GitHubData.V3.User[]>(requestContext, (Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.User[]>>) ((client, auth) => client.SearchUsers(searchQuery, maxResults, auth)), new SubjectDescriptor(), (Func<GitHubResult<GitHubData.V3.User[]>, GitHubData.V3.User[]>) null);
    }

    public static bool TryGetGitHubUserAuthentication(
      IVssRequestContext context,
      out GitHubAuthentication authentication,
      out string errorMessage)
    {
      errorMessage = (string) null;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = context.GetUserIdentity();
      if (userIdentity == null)
      {
        authentication = (GitHubAuthentication) null;
        errorMessage = string.Format("Not able to find the login user. UserContext: {0}.", (object) context.UserContext);
        context.TraceDataConditionally(61520314, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), errorMessage, methodName: nameof (TryGetGitHubUserAuthentication));
        return false;
      }
      SubjectDescriptor subjectDescriptor = userIdentity.SubjectDescriptor;
      if (!subjectDescriptor.IsFederatableUserType())
      {
        authentication = (GitHubAuthentication) null;
        errorMessage = string.Format("User is not federatable. UserContext: {0}.", (object) context.UserContext);
        context.TraceDataConditionally(61520315, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), errorMessage, methodName: nameof (TryGetGitHubUserAuthentication));
        return false;
      }
      GraphFederatedProviderData federatedProviderData = context.GetService<IGraphFederatedProviderService>().AcquireProviderData(context, subjectDescriptor, "github.com");
      if (federatedProviderData == null || federatedProviderData.AccessToken == null)
      {
        authentication = (GitHubAuthentication) null;
        errorMessage = string.Format("GitHub User authentication AccessToken is not found. User VSID: {0}, SubjectDescriptor: {1}", (object) userIdentity.Id, (object) subjectDescriptor);
        context.TraceDataConditionally(61520316, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), errorMessage, methodName: nameof (TryGetGitHubUserAuthentication));
        return false;
      }
      authentication = new GitHubAuthentication(federatedProviderData.AccessToken);
      return true;
    }

    public static GitHubData.V3.User GetAnyUser(
      IVssRequestContext requestContext,
      string userId,
      GraphFederatedProviderData providerData = null)
    {
      return providerData != null ? GitHubUserProvider.Execute<GitHubData.V3.User>(requestContext, (Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.User>>) ((client, auth) => client.GetUserById(userId, auth)), providerData, (Func<GitHubResult<GitHubData.V3.User>, GitHubData.V3.User>) null) : GitHubUserProvider.Execute<GitHubData.V3.User>(requestContext, (Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<GitHubData.V3.User>>) ((client, auth) => client.GetUserById(userId, auth)), new SubjectDescriptor(), (Func<GitHubResult<GitHubData.V3.User>, GitHubData.V3.User>) null);
    }

    private static TResult Execute<TResult>(
      IVssRequestContext context,
      Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<TResult>> request,
      SubjectDescriptor actor,
      Func<GitHubResult<TResult>, TResult> onError)
    {
      IGraphFederatedProviderService service = context.GetService<IGraphFederatedProviderService>();
      SubjectDescriptor subjectDescriptor = FederatedProviderExtensions.SelectProvidedOrAuthenticatedDescriptor(context, actor);
      IVssRequestContext context1 = context;
      SubjectDescriptor descriptor = subjectDescriptor;
      GraphFederatedProviderData providerData = service.AcquireProviderData(context1, descriptor, "github.com");
      return GitHubUserProvider.Execute<TResult>(context, request, providerData, onError);
    }

    private static TResult Execute<TResult>(
      IVssRequestContext context,
      Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<TResult>> request,
      GraphFederatedProviderData providerData,
      Func<GitHubResult<TResult>, TResult> onError)
    {
      GitHubResult<TResult> response = GitHubUserProvider.ExecuteWithAuthRetry<TResult>(context, request, providerData);
      return GitHubUserProvider.GetResult<TResult>(context, response, onError);
    }

    private static GitHubResult<TResult> ExecuteWithAuthRetry<TResult>(
      IVssRequestContext context,
      Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<TResult>> request,
      GraphFederatedProviderData providerData)
    {
      Lazy<HashedGraphFederatedProviderData> lazilyHashedProviderData = new Lazy<HashedGraphFederatedProviderData>((Func<HashedGraphFederatedProviderData>) (() => providerData.Hashed(context)));
      GitHubResult<TResult> response = GitHubUserProvider.ExecuteOnce<TResult>(context, request, providerData, lazilyHashedProviderData);
      if (context.IsFeatureEnabled("VisualStudio.Services.Identity.DisableGithubAuthRetry"))
      {
        context.TraceDataConditionally(61520321, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Retry feature is disabled; returning response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          feature = "VisualStudio.Services.Identity.DisableGithubAuthRetry"
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      if (response == null)
      {
        context.TraceDataConditionally(61520322, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Could not complete Github request; returning empty response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      if (response.IsSuccessful)
      {
        context.TraceDataConditionally(61520323, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Successfully completed Github request on first attempt; returning success response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      if (response.StatusCode != HttpStatusCode.Unauthorized)
      {
        context.TraceDataConditionally(61520324, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Failed to complete Github request due to non-authentication error; returning error response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      context.TraceDataConditionally(61520325, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Failed to complete Github request due to authentication error; attempting to retry with new credentials", (Func<object>) (() => (object) new
      {
        response = response,
        providerData = lazilyHashedProviderData.Value
      }), nameof (ExecuteWithAuthRetry));
      GraphFederatedProviderData latestProviderData = context.GetService<IGraphFederatedProviderService>().AcquireProviderData(context, providerData == null ? FederatedProviderExtensions.SelectProvidedOrAuthenticatedDescriptor(context, new SubjectDescriptor()) : providerData.SubjectDescriptor, "github.com", long.MaxValue);
      Lazy<HashedGraphFederatedProviderData> lazilyHashedLatestProviderData = new Lazy<HashedGraphFederatedProviderData>((Func<HashedGraphFederatedProviderData>) (() => latestProviderData.Hashed(context)));
      if (string.IsNullOrEmpty(latestProviderData?.AccessToken))
      {
        context.TraceDataConditionally(61520326, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Latest access token is null or empty; returning previous auth error response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          latestProviderData = lazilyHashedLatestProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      if (latestProviderData.AccessToken == providerData?.AccessToken)
      {
        context.TraceDataConditionally(61520327, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Latest access token is the same as previous one; returning previous auth error response without retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          latestProviderData = lazilyHashedLatestProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
        return response;
      }
      context.TraceDataConditionally(61520328, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Received a new access token; retrying request that previously receive auth error response", (Func<object>) (() => (object) new
      {
        response = response,
        providerData = lazilyHashedProviderData.Value,
        latestProviderData = lazilyHashedLatestProviderData.Value
      }), nameof (ExecuteWithAuthRetry));
      response = GitHubUserProvider.ExecuteOnce<TResult>(context, request, latestProviderData, lazilyHashedLatestProviderData);
      if (response == null)
        context.TraceDataConditionally(615203231, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Could not complete Github reqeust on second attempt; returning empty response following retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          latestProviderData = lazilyHashedLatestProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
      else if (!response.IsSuccessful)
        context.TraceDataConditionally(615203232, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Completed Github request with error on second attempt; returning error response following retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          latestProviderData = lazilyHashedLatestProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
      else
        context.TraceDataConditionally(615203233, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Successfully completed Github request on second attempt; returning success response following retry", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value,
          latestProviderData = lazilyHashedLatestProviderData.Value
        }), nameof (ExecuteWithAuthRetry));
      return response;
    }

    private static GitHubResult<TResult> ExecuteOnce<TResult>(
      IVssRequestContext context,
      Func<GitHubHttpClient, GitHubAuthentication, GitHubResult<TResult>> request,
      GraphFederatedProviderData providerData,
      Lazy<HashedGraphFederatedProviderData> lazilyHashedProviderData)
    {
      if (string.IsNullOrEmpty(providerData?.AccessToken))
      {
        GitHubResult<TResult> shortCircuitResponse = GitHubResult<TResult>.Error("Skipped call due to missing access token", HttpStatusCode.Unauthorized, (HttpResponseHeaders) null);
        context.TraceDataConditionally(61520301, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Short-circuited Github request due to missing access token", (Func<object>) (() => (object) new
        {
          response = shortCircuitResponse,
          providerData = lazilyHashedProviderData.Value
        }), nameof (ExecuteOnce));
        return shortCircuitResponse;
      }
      context.TraceDataConditionally(61520302, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Calling Github with access token", (Func<object>) (() => (object) new
      {
        providerData = lazilyHashedProviderData.Value
      }), nameof (ExecuteOnce));
      GitHubHttpClient gitHubHttpClient = GitHubHttpClientFactory.Create(context);
      GitHubAuthentication hubAuthentication = new GitHubAuthentication(providerData.AccessToken);
      GitHubResult<TResult> response = request(gitHubHttpClient, hubAuthentication);
      if (response == null)
      {
        context.TraceDataConditionally(61520303, TraceLevel.Error, "Github", nameof (GitHubUserProvider), "Failed to complete Github request", (Func<object>) (() => (object) new
        {
          response = response,
          providerData = lazilyHashedProviderData.Value
        }), nameof (ExecuteOnce));
        return (GitHubResult<TResult>) null;
      }
      context.TraceDataConditionally(61520304, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Completed Github request", (Func<object>) (() => (object) new
      {
        response = response,
        providerData = lazilyHashedProviderData.Value
      }), nameof (ExecuteOnce));
      return response;
    }

    private static TResult GetResult<TResult>(
      IVssRequestContext context,
      GitHubResult<TResult> response,
      Func<GitHubResult<TResult>, TResult> onError)
    {
      TResult result;
      if (response != null && response.IsSuccessful)
      {
        result = response.Result;
        context.TraceDataConditionally(61520341, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Response was success; returning inner result", (Func<object>) (() => (object) new
        {
          response = response,
          result = result
        }), nameof (GetResult));
      }
      else if (onError == null)
      {
        result = default (TResult);
        context.TraceDataConditionally(61520342, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Response was non-success with no custom error handler; returning default error result", (Func<object>) (() => (object) new
        {
          response = response,
          result = result
        }), nameof (GetResult));
      }
      else
      {
        result = onError(response);
        context.TraceDataConditionally(61520343, TraceLevel.Verbose, "Github", nameof (GitHubUserProvider), "Response was non-success with a custom error handler; returning custom error result", (Func<object>) (() => (object) new
        {
          response = response,
          result = result
        }), nameof (GetResult));
      }
      return result;
    }

    private static class Features
    {
      public const string DisableGithubAuthRetry = "VisualStudio.Services.Identity.DisableGithubAuthRetry";
    }
  }
}
