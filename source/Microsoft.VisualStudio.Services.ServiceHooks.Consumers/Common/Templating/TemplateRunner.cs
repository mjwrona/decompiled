// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.TemplateRunner
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating
{
  public class TemplateRunner
  {
    private Event m_event;
    private JObject m_model;

    public TemplateRunner(Event @event) => this.m_event = @event != null ? @event : throw new ArgumentNullException(nameof (@event));

    public string Run(string template) => string.IsNullOrWhiteSpace(template) ? template : TemplateEngineFactory.CreateDefault().ApplyTemplate(template, this.Model);

    public static string Run(string template, Event @event) => string.IsNullOrWhiteSpace(template) ? template : new TemplateRunner(@event).Run(template);

    private JObject BuildModel() => TemplateModelBuilderFactory.Create(this.m_event.EventType).Build(this.m_event);

    private JObject Model
    {
      get
      {
        if (this.m_model == null)
          this.m_model = this.BuildModel();
        return this.m_model;
      }
    }
  }
}
