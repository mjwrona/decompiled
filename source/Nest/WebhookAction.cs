// Decompiled with JetBrains decompiler
// Type: Nest.WebhookAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class WebhookAction : ActionBase, IWebhookAction, IAction, IHttpInputRequest
  {
    public WebhookAction(string name)
      : base(name)
    {
    }

    public override ActionType ActionType => ActionType.Webhook;

    public IHttpInputAuthentication Authentication { get; set; }

    public string Body { get; set; }

    public Time ConnectionTimeout { get; set; }

    public IDictionary<string, string> Headers { get; set; }

    public string Host { get; set; }

    public HttpInputMethod? Method { get; set; }

    public IDictionary<string, string> Params { get; set; }

    public string Path { get; set; }

    public int? Port { get; set; }

    public IHttpInputProxy Proxy { get; set; }

    public Time ReadTimeout { get; set; }

    public ConnectionScheme? Scheme { get; set; }

    public string Url { get; set; }
  }
}
