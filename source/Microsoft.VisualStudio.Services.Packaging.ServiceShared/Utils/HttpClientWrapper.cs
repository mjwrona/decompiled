// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.HttpClientWrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public sealed class HttpClientWrapper : IRequestContextAwareHttpClient
  {
    private readonly string loggingName;
    private readonly IFactory<Uri, HttpClient> httpClientFactory;
    private static ImmutableList<ProductInfoHeaderValue> userAgent;
    private static readonly ConcurrentDictionary<string, string> ObservedUriAuthorities = new ConcurrentDictionary<string, string>();

    public HttpClientWrapper(IFactory<Uri, HttpClient> httpClientFactory, string loggingName)
    {
      this.httpClientFactory = httpClientFactory;
      this.loggingName = loggingName;
    }

    public async Task<HttpResponseMessage> SendAsync(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      HttpCompletionOption completionOption)
    {
      HttpClientWrapper sendInTheThisObject = this;
      HttpResponseMessage result;
      string exception;
      HttpResponseMessage httpResponseMessage;
      using (ITracerBlock tracer = requestContext.GetTracerFacade().Enter((object) sendInTheThisObject, nameof (SendAsync)))
      {
        result = (HttpResponseMessage) null;
        exception = (string) null;
        DateTime start = DateTime.UtcNow;
        DateTime end = DateTime.UtcNow;
        try
        {
          HttpClient httpClient = sendInTheThisObject.httpClientFactory.Get(request.RequestUri);
          string leftPart = request.RequestUri.GetLeftPart(UriPartial.Authority);
          if (HttpClientWrapper.ObservedUriAuthorities.TryAdd(leftPart, leftPart))
            ServicePointManager.FindServicePoint(new Uri(leftPart)).ConnectionLeaseTimeout = 60000;
          if (!request.Headers.Contains("X-VSS-E2EID"))
            request.Headers.Add("X-VSS-E2EID", requestContext.E2EId.ToString());
          foreach (ProductInfoHeaderValue productInfoHeaderValue in LazyInitializer.EnsureInitialized<ImmutableList<ProductInfoHeaderValue>>(ref HttpClientWrapper.userAgent, (Func<ImmutableList<ProductInfoHeaderValue>>) (() => HttpClientWrapper.InitializeUserAgent(requestContext))))
          {
            if (!request.Headers.UserAgent.Contains(productInfoHeaderValue))
              request.Headers.UserAgent.Add(productInfoHeaderValue);
          }
          tracer.TraceMarker("Before SendAsync");
          result = await httpClient.SendAsync(request, completionOption, requestContext.CancellationToken).EnforceCancellation<HttpResponseMessage>(requestContext.CancellationToken, file: "D:\\a\\_work\\1\\s\\Packaging\\Service\\Common\\Shared\\Utils\\IHttpClient.cs", member: nameof (SendAsync), line: 207);
          tracer.TraceMarker("After SendAsync");
          end = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
          end = DateTime.UtcNow;
          exception = ex.ToString();
          throw;
        }
        finally
        {
          string afdRefInfo = string.Empty;
          int responseCode = -1;
          if (result != null)
          {
            IEnumerable<string> values = (IEnumerable<string>) null;
            result.Headers?.TryGetValues("X-MSEdge-Ref", out values);
            afdRefInfo = (values != null ? values.FirstOrDefault<string>() : (string) null) ?? string.Empty;
            responseCode = (int) result.StatusCode;
          }
          TeamFoundationTracingService.TraceHttpOutgoingRequest(start, (int) (end - start).TotalMilliseconds, sendInTheThisObject.loggingName, "GET", request.RequestUri.Host, request.RequestUri.AbsolutePath, responseCode, exception ?? "", requestContext.E2EId, afdRefInfo, "", Guid.Empty, "", "");
        }
        httpResponseMessage = result;
      }
      result = (HttpResponseMessage) null;
      exception = (string) null;
      return httpResponseMessage;
    }

    private static ImmutableList<ProductInfoHeaderValue> InitializeUserAgent(
      IVssRequestContext requestContext)
    {
      List<string> values = new List<string>(4)
      {
        "Microsoft Azure Artifacts [Azure DevOps]"
      };
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      if (executionEnvironment.IsHostedDeployment)
      {
        values.Add("Hosted");
        string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) "/Configuration/Settings/HostedServiceName", string.Empty);
        if (!string.IsNullOrEmpty(str))
          values.Add(str);
      }
      if (executionEnvironment.IsOnPremisesDeployment)
        values.Add("On customer premises");
      ImmutableList<ProductInfoHeaderValue>.Builder builder = ImmutableList.CreateBuilder<ProductInfoHeaderValue>();
      builder.Add(new ProductInfoHeaderValue("AzureArtifacts", HttpClientWrapper.GetVersionString()));
      builder.Add(new ProductInfoHeaderValue("(" + string.Join("; ", (IEnumerable<string>) values) + ")"));
      return builder.ToImmutable();
    }

    private static string GetVersionString()
    {
      try
      {
        AssemblyFileVersionAttribute customAttribute = typeof (HttpClientWrapper).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        if (customAttribute != null)
          return customAttribute.Version;
      }
      catch (Exception ex)
      {
      }
      return "version-unavailable";
    }
  }
}
