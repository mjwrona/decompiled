// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributedRequestHandler`1
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public class ContributedRequestHandler<T> where T : IContributedRequestHandler
  {
    public ContributedRequestHandler(string id, T handler, JObject properties, int order)
    {
      this.Id = id;
      this.Handler = handler;
      this.Properties = properties;
      this.Order = order;
    }

    public string Id { get; private set; }

    public T Handler { get; private set; }

    public int Order { get; private set; }

    public JObject Properties { get; private set; }
  }
}
