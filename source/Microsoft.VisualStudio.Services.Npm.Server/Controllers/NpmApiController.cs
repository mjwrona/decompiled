// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Controllers.NpmApiController
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Exceptions;
using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace Microsoft.VisualStudio.Services.Npm.Server.Controllers
{
  [NpmExceptionFilter]
  public abstract class NpmApiController : PackagingApiController
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) NpmExceptionMappings.HttpExceptionMapping;

    public override string TraceArea => "npm";

    public override string ActivityLogArea => "npm";

    protected override bool ExemptFromGlobalExceptionFormatting { get; } = true;

    protected JsonMediaTypeFormatter JsonFormatter
    {
      get
      {
        JsonMediaTypeFormatter jsonFormatter = new JsonMediaTypeFormatter();
        jsonFormatter.SerializerSettings.ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver();
        return jsonFormatter;
      }
    }

    protected HttpContent GetNpmSuccessMessageContent() => (HttpContent) new ObjectContent(typeof (NpmResponseMessage), (object) new NpmResponseMessage()
    {
      Success = "true"
    }, (MediaTypeFormatter) this.JsonFormatter, "application/json");

    protected override IProtocol GetProtocol() => (IProtocol) Protocol.npm;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage)
    {
      IEnumerable<string> values;
      return requestMessage.Headers.TryGetValues("npm-session", out values) ? values.FirstOrDefault<string>() : (string) null;
    }

    public virtual IFeedJobDefinitionProvider ChangeProcessingFeedJobDefinitionProvider { get; set; }

    protected IFeedJobDefinitionProvider GetChangeProcessingFeedJobDefinitionProvider()
    {
      if (this.ChangeProcessingFeedJobDefinitionProvider == null)
        this.ChangeProcessingFeedJobDefinitionProvider = (IFeedJobDefinitionProvider) new Microsoft.VisualStudio.Services.Packaging.ServiceShared.ChangeProcessing.ChangeProcessingFeedJobDefinitionProvider(new NpmCommitLogFacadeBootstrapper(this.TfsRequestContext).Bootstrap(), "Microsoft.VisualStudio.Services.Npm.Server.Plugins.ChangeProcessing.NpmFeedChangeProcessingJob");
      return this.ChangeProcessingFeedJobDefinitionProvider;
    }
  }
}
