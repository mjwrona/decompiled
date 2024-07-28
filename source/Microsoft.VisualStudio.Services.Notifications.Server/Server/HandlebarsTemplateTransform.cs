// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.HandlebarsTemplateTransform
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
  internal class HandlebarsTemplateTransform
  {
    private ContributedTemplate m_template;
    private const string c_allTemplateInputsKey = "allTemplateInputs";

    public HandlebarsTemplateTransform(ContributedTemplate template) => this.m_template = template;

    public NotificationTransformResult Transform(
      IVssRequestContext requestContext,
      JObject templateContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      MustacheOptions evaluationOptions = HandlebarsTemplateEvaluationHelpers.CreateEvaluationOptions(requestContext);
      NotificationTransformResult notificationTransformResult = new NotificationTransformResult();
      Dictionary<string, object> additionalEvaluationData = new Dictionary<string, object>();
      HandlebarsTemplateTransformExtensions.SetRequestContext((IDictionary<string, object>) additionalEvaluationData, requestContext);
      Dictionary<string, JObject> dictionary = new Dictionary<string, JObject>();
      additionalEvaluationData["allTemplateInputs"] = (object) dictionary;
      foreach (KeyValuePair<string, TemplateFields> templateField in this.m_template.TemplateFields)
        dictionary[templateField.Key] = templateField.Value.GetObjectWithReplacements((object) templateContext, additionalEvaluationData, evaluationOptions);
      JObject content;
      if (dictionary.TryGetValue(this.m_template.Id, out content))
      {
        templateContext.Merge((object) content);
        notificationTransformResult.Properties = content;
      }
      notificationTransformResult.Content = this.m_template.RootExpression.Evaluate((object) templateContext, additionalEvaluationData, (MustacheEvaluationContext) null, (Dictionary<string, MustacheRootExpression>) null, evaluationOptions);
      return notificationTransformResult;
    }
  }
}
