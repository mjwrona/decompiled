// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks.WebHookEventPayloadInputMapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.WebHooks
{
  public class WebHookEventPayloadInputMapper
  {
    private MustacheTemplateParser m_parser = new MustacheTemplateParser();
    private JObject m_eventPayload;
    private IDictionary<string, string> m_payloadMapping;

    public WebHookEventPayloadInputMapper(string eventPayload, IArtifactType artifactType)
    {
      if (string.IsNullOrEmpty(eventPayload))
        eventPayload = "{}";
      this.m_eventPayload = JObject.Parse(eventPayload);
      this.m_eventPayload.Add(nameof (eventPayload), (JToken) eventPayload);
      this.m_payloadMapping = (IDictionary<string, string>) (artifactType.ArtifactTriggerConfiguration?.WebhookPayloadMapping ?? new Dictionary<string, string>());
    }

    public bool GetValue(string key, out string value)
    {
      value = string.Empty;
      string template;
      if (!this.m_payloadMapping.TryGetValue(key, out template))
        return false;
      value = this.m_parser.ReplaceValues(template, (object) this.m_eventPayload);
      return true;
    }

    public string GetValueFromEventPaylaod(string template)
    {
      JToken jtoken = this.m_eventPayload.SelectToken(template);
      return jtoken != null ? jtoken.ToString() : string.Empty;
    }

    public IDictionary<string, string> GetAllInputValues()
    {
      Dictionary<string, string> allInputValues = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) this.m_payloadMapping)
        allInputValues.Add(keyValuePair.Key, this.m_parser.ReplaceValues(keyValuePair.Value, (object) this.m_eventPayload));
      return (IDictionary<string, string>) allInputValues;
    }
  }
}
