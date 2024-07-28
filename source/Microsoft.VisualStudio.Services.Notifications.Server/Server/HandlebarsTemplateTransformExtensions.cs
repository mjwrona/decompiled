// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.HandlebarsTemplateTransformExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class HandlebarsTemplateTransformExtensions
  {
    private const string c_vssRequestContextKey = "IVssRequestContext";
    private const string c_allTemplateInputsKey = "allTemplateInputs";

    public static IVssRequestContext GetRequestContext(
      this MustacheEvaluationContext evaluationContext)
    {
      IVssRequestContext vssRequestContext = (IVssRequestContext) null;
      object obj;
      if (evaluationContext.AdditionalEvaluationData != null && evaluationContext.AdditionalEvaluationData.TryGetValue("IVssRequestContext", out obj))
        vssRequestContext = obj as IVssRequestContext;
      return vssRequestContext == null && evaluationContext.ParentContext != null ? evaluationContext.ParentContext.GetRequestContext() : vssRequestContext;
    }

    public static void SetRequestContext(
      IDictionary<string, object> additionalEvaluationData,
      IVssRequestContext requestContext)
    {
      additionalEvaluationData["IVssRequestContext"] = (object) requestContext;
    }

    public static IDictionary<string, JObject> GetAllTemplateInputs(
      this MustacheEvaluationContext evaluationContext)
    {
      IDictionary<string, JObject> dictionary = (IDictionary<string, JObject>) null;
      object obj;
      if (evaluationContext.AdditionalEvaluationData != null && evaluationContext.AdditionalEvaluationData.TryGetValue("allTemplateInputs", out obj))
        dictionary = obj as IDictionary<string, JObject>;
      return dictionary == null && evaluationContext.ParentContext != null ? evaluationContext.ParentContext.GetAllTemplateInputs() : dictionary;
    }

    public static void SetAllTemplateInputs(
      IDictionary<string, object> additionalEvaluationData,
      IDictionary<string, JObject> allTemplateInputs)
    {
      additionalEvaluationData[nameof (allTemplateInputs)] = (object) allTemplateInputs;
    }
  }
}
