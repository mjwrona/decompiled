// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.HandlebarsDataTemplateTransform
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class HandlebarsDataTemplateTransform
  {
    private ContributedDataTemplate m_template;

    public HandlebarsDataTemplateTransform(ContributedDataTemplate template) => this.m_template = template;

    public JObject Transform(IVssRequestContext requestContext, JObject templateContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      MustacheOptions options = HandlebarsTemplateEvaluationHelpers.CreateEvaluationOptions(requestContext) ?? new MustacheOptions();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      options.EncodeMethod = HandlebarsDataTemplateTransform.\u003C\u003EO.\u003C0\u003E__NoEncode ?? (HandlebarsDataTemplateTransform.\u003C\u003EO.\u003C0\u003E__NoEncode = new MustacheEncodeMethod(MustacheEncodeMethods.NoEncode));
      Dictionary<string, object> additionalEvaluationData = new Dictionary<string, object>();
      HandlebarsTemplateTransformExtensions.SetRequestContext((IDictionary<string, object>) additionalEvaluationData, requestContext);
      Dictionary<string, JObject> allTemplateInputs = new Dictionary<string, JObject>();
      foreach (KeyValuePair<string, TemplateFields> templateField in this.m_template.TemplateFields)
        allTemplateInputs[templateField.Key] = templateField.Value.GetObjectWithReplacements((object) templateContext, additionalEvaluationData, options);
      HandlebarsTemplateTransformExtensions.SetAllTemplateInputs((IDictionary<string, object>) additionalEvaluationData, (IDictionary<string, JObject>) allTemplateInputs);
      return allTemplateInputs[this.m_template.Id];
    }
  }
}
