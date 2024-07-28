// Decompiled with JetBrains decompiler
// Type: Nest.ExecutionResultAction
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ExecutionResultAction
  {
    [DataMember(Name = "email")]
    public EmailActionResult Email { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "index")]
    public IndexActionResult Index { get; set; }

    [DataMember(Name = "logging")]
    public LoggingActionResult Logging { get; set; }

    [DataMember(Name = "pagerduty")]
    public PagerDutyActionResult PagerDuty { get; set; }

    [DataMember(Name = "reason")]
    public string Reason { get; set; }

    [DataMember(Name = "slack")]
    public SlackActionResult Slack { get; set; }

    [DataMember(Name = "status")]
    public Status Status { get; set; }

    [DataMember(Name = "type")]
    public ActionType Type { get; set; }

    [DataMember(Name = "webhook")]
    public WebhookActionResult Webhook { get; set; }
  }
}
