// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.TemplateModelBuilderFactory
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.ModelBuilders;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating
{
  public static class TemplateModelBuilderFactory
  {
    private static readonly ITemplateModelBuilder s_defaultModelBuilder = (ITemplateModelBuilder) new JsonFriendlyModelBuilder();
    private static readonly ITemplateModelBuilder s_workItemModelBuilder = (ITemplateModelBuilder) new WorkItemModelBuilder();
    private static readonly Dictionary<string, ITemplateModelBuilder> m_customEventModelBuilders = new Dictionary<string, ITemplateModelBuilder>()
    {
      {
        "workitem.created",
        TemplateModelBuilderFactory.s_workItemModelBuilder
      },
      {
        "workitem.updated",
        TemplateModelBuilderFactory.s_workItemModelBuilder
      },
      {
        "workitem.commented",
        TemplateModelBuilderFactory.s_workItemModelBuilder
      }
    };

    public static ITemplateModelBuilder Create(string eventType)
    {
      if (eventType == null)
        throw new ArgumentNullException(nameof (eventType));
      return !TemplateModelBuilderFactory.m_customEventModelBuilders.ContainsKey(eventType) ? TemplateModelBuilderFactory.s_defaultModelBuilder : TemplateModelBuilderFactory.m_customEventModelBuilders[eventType];
    }
  }
}
