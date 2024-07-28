// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactHttpClientFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class ArtifactHttpClientFactory
  {
    private readonly CancellationToken verifyConnectionCancellationToken;
    private readonly VssHttpRetryOptions Options;
    private static readonly Uri SpsProductionUri = new Uri("https://app.vssps.visualstudio.com");

    public ArtifactHttpClientFactory(
      VssCredentials credentials,
      TimeSpan? httpSendTimeout,
      IAppTraceSource tracer,
      CancellationToken verifyConnectionCancellationToken)
      : this(tracer, credentials, httpSendTimeout, verifyConnectionCancellationToken)
    {
      ArgumentUtility.CheckForNull<IAppTraceSource>(tracer, nameof (tracer));
      ArgumentUtility.CheckForNull<VssCredentials>(credentials, nameof (credentials));
    }

    internal ArtifactHttpClientFactory(
      IAppTraceSource tracer = null,
      VssCredentials credentials = null,
      TimeSpan? httpSendTimeout = null,
      CancellationToken verifyConnectionCancellationToken = default (CancellationToken),
      VssHttpRetryOptions options = null)
    {
      this.Credentials = credentials;
      this.Tracer = tracer ?? (IAppTraceSource) NoopAppTraceSource.Instance;
      this.ClientSettings = VssClientHttpRequestSettings.Default.Clone();
      this.ClientSettings.ClientCertificateManager = (IVssClientCertificateManager) null;
      if (httpSendTimeout.HasValue)
        this.ClientSettings.SendTimeout = httpSendTimeout.Value;
      this.DelegatingHandlerFactoryMethods = (IList<Func<DelegatingHandler>>) new List<Func<DelegatingHandler>>();
      this.verifyConnectionCancellationToken = verifyConnectionCancellationToken;
      if (options == null)
      {
        this.Options = ArtifactHttpRetryMessageHandler.DefaultRetryOptions;
      }
      else
      {
        options.RetryableStatusCodes.AddRange<HttpStatusCode, ICollection<HttpStatusCode>>((IEnumerable<HttpStatusCode>) ArtifactHttpRetryMessageHandler.DefaultRetryOptions.RetryableStatusCodes);
        this.Options = options;
      }
    }

    protected ArtifactHttpClientFactory(ArtifactHttpClientFactory factory)
    {
      this.Credentials = factory.Credentials;
      this.Tracer = factory.Tracer;
      this.ClientSettings = factory.ClientSettings;
      this.ClientSettings.SendTimeout = factory.ClientSettings.SendTimeout;
      this.DelegatingHandlerFactoryMethods = factory.DelegatingHandlerFactoryMethods;
      this.verifyConnectionCancellationToken = factory.verifyConnectionCancellationToken;
      this.Options = factory.Options;
    }

    public VssCredentials Credentials { get; private set; }

    public VssClientHttpRequestSettings ClientSettings { get; private set; }

    public VssHttpRequestSettings RequestSettings => (VssHttpRequestSettings) this.ClientSettings;

    protected IList<Func<DelegatingHandler>> DelegatingHandlerFactoryMethods { get; set; }

    protected IAppTraceSource Tracer { get; private set; }

    public RequiredInterface CreateVssHttpClient<RequiredInterface, PreferredConcrete>(Uri baseUri)
      where RequiredInterface : IArtifactHttpClient
      where PreferredConcrete : IVssHttpClient, RequiredInterface
    {
      this.Tracer.Verbose(string.Format("{0}.{1}: {2} with BaseUri: {3}, MaxRetries:{4}, SendTimeout:{5}", (object) nameof (ArtifactHttpClientFactory), (object) nameof (CreateVssHttpClient), (object) typeof (PreferredConcrete).Name, (object) baseUri, (object) ArtifactHttpRetryMessageHandler.DefaultRetryOptions.MaxRetries, (object) this.RequestSettings.SendTimeout));
      return (RequiredInterface) this.CreateVssHttpClient(typeof (RequiredInterface), typeof (PreferredConcrete), baseUri);
    }

    internal object CreateVssHttpClient(
      Type requiredInterface,
      Type preferredType,
      Uri baseUri,
      ArtifactHttpRetryMessageHandler retryHandler = null)
    {
      ArgumentUtility.CheckForNull<Type>(requiredInterface, nameof (requiredInterface));
      ArgumentUtility.CheckForNull<Type>(preferredType, nameof (preferredType));
      ArgumentUtility.CheckForNull<Uri>(baseUri, nameof (baseUri));
      if (!typeof (IArtifactHttpClient).IsAssignableFrom(requiredInterface))
        throw new ArgumentException(string.Format("Type {0} is not assignable from {1}", (object) typeof (IArtifactHttpClient), (object) requiredInterface.Name));
      if (!requiredInterface.IsAssignableFrom(preferredType))
        throw new ArgumentException(string.Format("Type {0} is not assignable from {1}", (object) requiredInterface, (object) preferredType));
      Type[] types = new Type[4]
      {
        typeof (Uri),
        typeof (VssCredentials),
        typeof (VssHttpRequestSettings),
        typeof (DelegatingHandler[])
      };
      if (preferredType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, (Binder) null, types, (ParameterModifier[]) null) == (ConstructorInfo) null)
        throw new ArgumentException(preferredType.Name + " must be a non-abstract class with a constructor accepting Uri, VssCredentials, VssHttpRequestSettings, DelegatingHandler[], IAppTraceSource types in order to use it as parameter 'T'.");
      DelegatingHandler[] delegatingHandlers = this.CreateDelegatingHandlers(retryHandler);
      Type type = preferredType;
      object[] objArray = new object[4]
      {
        (object) baseUri,
        (object) this.Credentials,
        (object) this.RequestSettings,
        (object) delegatingHandlers
      };
      object instance;
      ((IArtifactHttpClient) (instance = Activator.CreateInstance(type, objArray))).SetTracer(this.Tracer);
      return instance;
    }

    public static bool CanRetryOnNotFoundForVssServiceResponseException(
      bool retryOnNotFoundTestOnly,
      Exception exception)
    {
      return retryOnNotFoundTestOnly && exception is VssServiceResponseException && ((VssServiceResponseException) exception).HttpStatusCode.Equals((object) HttpStatusCode.NotFound);
    }

    public virtual object CreateVssHttpClient(
      Type requiredInterface,
      Type preferredType,
      Uri baseUri)
    {
      return this.CreateVssHttpClient(requiredInterface, preferredType, baseUri, (ArtifactHttpRetryMessageHandler) null);
    }

    public virtual ArtifactHttpClientFactory.VerifyConnectionResult VerifyConnection(
      IArtifactHttpClient httpClient)
    {
      return TaskSafety.SyncResultOnThreadPool<ArtifactHttpClientFactory.VerifyConnectionResult>((Func<Task<ArtifactHttpClientFactory.VerifyConnectionResult>>) (() => this.VerifyConnectionAsync(httpClient)));
    }

    public virtual async Task<ArtifactHttpClientFactory.VerifyConnectionResult> VerifyConnectionAsync(
      IArtifactHttpClient httpClient)
    {
      return await new AsyncHttpRetryHelper<ArtifactHttpClientFactory.VerifyConnectionResult>((Func<Task<ArtifactHttpClientFactory.VerifyConnectionResult>>) (async () => await this.VerifyConnectionInternalAsync(httpClient)), 2, this.Tracer, false, nameof (VerifyConnectionAsync)).InvokeAsync(new CancellationToken());
    }

    protected async Task<ArtifactHttpClientFactory.VerifyConnectionResult> VerifyConnectionInternalAsync(
      IArtifactHttpClient httpClient)
    {
      ArtifactHttpClientFactory.VerifyConnectionResult result = ArtifactHttpClientFactory.VerifyConnectionResult.Uninitialized;
      this.Tracer.Verbose("ArtifactHttpClientFactory.VerifyConnectionAsync: " + httpClient.GetType().Name + ".GetOptionsAsync starting");
      try
      {
        await httpClient.GetOptionsAsync(this.verifyConnectionCancellationToken).ConfigureAwait(false);
        result = ArtifactHttpClientFactory.VerifyConnectionResult.InitialRequestSucceeded;
      }
      catch (VssUnauthorizedException ex)
      {
        VssConnection connection = new VssConnection(ArtifactHttpClientFactory.SpsProductionUri, this.Credentials, (VssHttpRequestSettings) this.ClientSettings);
        await connection.ConnectAsync(this.verifyConnectionCancellationToken).ConfigureAwait(false);
        IdentitySelf identitySelf = await connection.GetClient<IdentityHttpClient>().GetIdentitySelfAsync((object) null, this.verifyConnectionCancellationToken).ConfigureAwait(false);
        await httpClient.GetOptionsAsync(this.verifyConnectionCancellationToken).ConfigureAwait(false);
        result = ArtifactHttpClientFactory.VerifyConnectionResult.RequestSucceededAfterProfileCreation;
        connection = (VssConnection) null;
      }
      this.Tracer.Verbose(string.Format("{0}.{1}: {2}.{3} completed with {4}", (object) nameof (ArtifactHttpClientFactory), (object) "VerifyConnectionAsync", (object) httpClient.GetType().Name, (object) "GetOptionsAsync", (object) result));
      return result;
    }

    private DelegatingHandler[] CreateDelegatingHandlers(
      ArtifactHttpRetryMessageHandler retryHandler = null)
    {
      List<DelegatingHandler> handlers = new List<DelegatingHandler>();
      this.AddRetryHandler((IList<DelegatingHandler>) handlers, retryHandler);
      foreach (Func<DelegatingHandler> handlerFactoryMethod in (IEnumerable<Func<DelegatingHandler>>) this.DelegatingHandlerFactoryMethods)
      {
        DelegatingHandler delegatingHandler = handlerFactoryMethod();
        handlers.Add(delegatingHandler);
      }
      return handlers.ToArray();
    }

    private void AddRetryHandler(
      IList<DelegatingHandler> handlers,
      ArtifactHttpRetryMessageHandler retryHandler = null)
    {
      if (handlers.OfType<ArtifactHttpRetryMessageHandler>().Any<ArtifactHttpRetryMessageHandler>())
        throw new InvalidOperationException("handlers already contains a ArtifactHttpRetryMessageHandler");
      if (retryHandler == null)
        retryHandler = new ArtifactHttpRetryMessageHandler(this.Tracer, this.Options);
      handlers.Add((DelegatingHandler) retryHandler);
      if (this.ClientSettings.MaxRetryRequest == this.Options.MaxRetries)
        return;
      this.ClientSettings.MaxRetryRequest = this.Options.MaxRetries;
    }

    public enum VerifyConnectionResult
    {
      Uninitialized,
      InitialRequestSucceeded,
      RequestSucceededAfterProfileCreation,
    }
  }
}
