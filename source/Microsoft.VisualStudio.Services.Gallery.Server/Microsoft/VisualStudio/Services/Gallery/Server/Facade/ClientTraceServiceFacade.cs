// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.ClientTraceServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public sealed class ClientTraceServiceFacade
  {
    private const string area = "gallery";
    private readonly ClientTraceService clientTraceService;
    private readonly IVssRequestContext requestContext;

    public ClientTraceServiceFacade(
      ClientTraceService clientTraceService,
      IVssRequestContext requestContext)
    {
      this.clientTraceService = clientTraceService ?? throw new ArgumentNullException(nameof (clientTraceService));
      this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));
    }

    public void Publish(string feature, IDictionary<string, object> properties)
    {
      ClientTraceData properties1 = new ClientTraceData();
      if (properties != null)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
          properties1.Add(property.Key, property.Value);
      }
      this.clientTraceService.Publish(this.requestContext, "gallery", feature, properties1);
    }
  }
}
