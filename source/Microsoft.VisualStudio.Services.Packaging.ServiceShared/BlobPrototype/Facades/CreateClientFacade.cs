// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.CreateClientFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class CreateClientFacade : ICreateClient
  {
    private readonly IVssRequestContext requestContext;
    private readonly Microsoft.TeamFoundation.Framework.Server.ICreateClient createClient;

    public CreateClientFacade(IVssRequestContext requestContext, Microsoft.TeamFoundation.Framework.Server.ICreateClient createClient)
    {
      this.requestContext = requestContext;
      this.createClient = createClient;
    }

    public T CreateClient<T>(
      Uri baseUri,
      string logAs,
      ApiResourceLocationCollection resourceLocations,
      bool requiresResourceLocations = true,
      bool createAsElevated = false)
      where T : VssHttpClientBase
    {
      return this.createClient.CreateClient<T>(createAsElevated ? this.requestContext.Elevate() : this.requestContext, baseUri, logAs, resourceLocations, requiresResourceLocations);
    }
  }
}
