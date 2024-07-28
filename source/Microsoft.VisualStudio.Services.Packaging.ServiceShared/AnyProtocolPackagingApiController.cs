// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.AnyProtocolPackagingApiController
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ValidationUtils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public abstract class AnyProtocolPackagingApiController : PackagingApiControllerBase
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) PackagingExceptionMappings.HttpExceptionMapping;

    protected override string GetClientSessionIdFrom(HttpRequestMessage requestMessage) => (string) null;

    protected IFeedRequest GetFeedRequest(
      string protocolName,
      string feedNameOrId,
      IValidator<FeedCore> validator = null)
    {
      return this.GetFeedRequest(AnyProtocolPackagingApiController.GetProtocol(protocolName), feedNameOrId, validator);
    }

    protected static IProtocol GetProtocol(string protocolName) => ProtocolRegistrar.Instance.GetProtocolOrDefault(protocolName) ?? throw new UnknownPackagingProtocolException(Resources.Error_UnknownProtocol((object) protocolName));
  }
}
