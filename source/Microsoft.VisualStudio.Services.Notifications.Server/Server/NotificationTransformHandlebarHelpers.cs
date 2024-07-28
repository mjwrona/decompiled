// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTransformHandlebarHelpers
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class NotificationTransformHandlebarHelpers
  {
    private const string c_trackingDataQueryParam = "tracking_data";
    private const string c_notificationIconsPathCdn = "https://cdn.vsassets.io/content/notifications";
    private const string c_notificationIconsPathLocal = "/_static/content/notifications";

    public static void RegisterHelpers(MustacheTemplateParser templateParser)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("contributedPartial", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C0\u003E__HandlebarContributedPartialHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C0\u003E__HandlebarContributedPartialHelper = new MustacheTemplateHelperMethod(NotificationTransformHandlebarHelpers.HandlebarContributedPartialHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("iconUrl", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C1\u003E__IconUrlHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C1\u003E__IconUrlHelper = new MustacheTemplateHelperMethod(NotificationTransformHandlebarHelpers.IconUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("icon", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C2\u003E__IconHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C2\u003E__IconHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.IconHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("assetUrl", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C3\u003E__AssetUrlHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C3\u003E__AssetUrlHelper = new MustacheTemplateHelperMethod(NotificationTransformHandlebarHelpers.AssetUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("trackUrl", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C4\u003E__TrackUrlHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C4\u003E__TrackUrlHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.TrackUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("isHosted", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C5\u003E__IsHostedHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C5\u003E__IsHostedHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.IsHostedHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("style", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C6\u003E__StyleHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C6\u003E__StyleHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.StyleHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("grid", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C7\u003E__HtmlGridHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C7\u003E__HtmlGridHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.HtmlGridHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("gridRow", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C8\u003E__HtmlGridRowHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C8\u003E__HtmlGridRowHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.HtmlGridRowHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("gridCell", NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C9\u003E__HtmlGridCellHelper ?? (NotificationTransformHandlebarHelpers.\u003C\u003EO.\u003C9\u003E__HtmlGridCellHelper = new MustacheTemplateHelperWriter(NotificationTransformHandlebarHelpers.HtmlGridCellHelper)));
    }

    private static void IconHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      if (string.IsNullOrWhiteSpace(helperArgument1))
        return;
      string str1 = helperArgument1.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase) || helperArgument1.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ? helperArgument1 : NotificationTransformHandlebarHelpers.GetIconUrl(context.GetRequestContext(), helperArgument1);
      int helperArgument2 = expression.GetHelperArgument<int>(context, 1);
      int helperArgument3 = expression.GetHelperArgument<int>(context, 2);
      string helperArgument4 = expression.GetHelperArgument<string>(context, 3);
      string str2 = (string) null;
      if (expression.IsBlockExpression)
        str2 = expression.EvaluateChildExpressions(context);
      TagBuilder builder = new TagBuilder("img").Attribute("src", str1);
      if (helperArgument2 > 0)
        builder.Attribute("width", helperArgument2.ToString());
      if (helperArgument3 > 0)
        builder.Attribute("height", helperArgument3.ToString());
      if (!string.IsNullOrWhiteSpace(helperArgument4))
        builder.Attribute("style", helperArgument4);
      if (!string.IsNullOrWhiteSpace(str2))
        builder.Attribute("alt", str2);
      writer.Write(builder.ToString(TagRenderMode.StartTag));
    }

    public static string IconUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string helperArgument = expression.GetHelperArgument<string>(context, 0);
      return NotificationTransformHandlebarHelpers.GetIconUrl(context.GetRequestContext(), helperArgument);
    }

    public static string IconCdnUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string helperArgument = expression.GetHelperArgument<string>(context, 0);
      return NotificationTransformHandlebarHelpers.GetIconUrl(context.GetRequestContext(), helperArgument, true);
    }

    private static string GetIconUrl(
      IVssRequestContext requestContext,
      string filePath,
      bool returnCdnPathOnly = false)
    {
      string str = "https://cdn.vsassets.io/content/notifications";
      if (!returnCdnPathOnly && requestContext != null && requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        string accessPoint = requestContext.GetService<ILocationService>().DetermineAccessMapping(requestContext)?.AccessPoint;
        if (!string.IsNullOrEmpty(accessPoint))
          str = accessPoint.TrimEnd('"', '\'', '/') + "/_static/content/notifications";
      }
      return str + "/" + filePath;
    }

    private static string AssetUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string str = (string) null;
      string helperArgument1 = expression.GetHelperArgument<string>(context, 0);
      string helperArgument2 = expression.GetHelperArgument<string>(context, 1);
      if (!string.IsNullOrEmpty(helperArgument1) && !string.IsNullOrEmpty(helperArgument2))
      {
        IVssRequestContext requestContext = context.GetRequestContext();
        if (requestContext != null)
          str = requestContext.GetService<IContributionService>().QueryAssetLocation(requestContext, helperArgument1, helperArgument2, "Cdn");
      }
      return str;
    }

    private static void TrackUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string uriString;
      string helperArgument;
      if (expression.IsBlockExpression)
      {
        uriString = expression.EvaluateChildExpressions(context);
        helperArgument = expression.GetHelperArgument<string>(context, 0);
      }
      else
      {
        uriString = expression.GetHelperArgument<string>(context, 0);
        helperArgument = expression.GetHelperArgument<string>(context, 1);
      }
      if (string.IsNullOrEmpty(uriString))
        return;
      Uri uri = new Uri(uriString);
      if ((uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && !uri.Host.Equals("go.microsoft.com", StringComparison.InvariantCultureIgnoreCase) && !uri.Host.Equals("aka.ms", StringComparison.InvariantCultureIgnoreCase))
      {
        StringBuilder stringBuilder = new StringBuilder(helperArgument);
        bool? nullable = new bool?();
        MustacheEvaluationContext evaluationContext = context;
        do
        {
          JObject jobject;
          if (evaluationContext.ReplacementObject != null && evaluationContext.ReplacementObject.GetType().Equals(typeof (JObject)) && ((JObject) evaluationContext.ReplacementObject).TryGetValue<JObject>("tracking", out jobject))
          {
            JToken jtoken1;
            if (jobject.TryGetValue("id", out jtoken1))
            {
              if (stringBuilder.Length > 0)
                stringBuilder.Insert(0, '/');
              stringBuilder.Insert(0, jtoken1.ToString());
            }
            JToken jtoken2;
            if (!nullable.HasValue && jobject.TryGetValue("includeAdvanced", out jtoken2) && jtoken2.Type.Equals((object) JTokenType.Boolean))
              nullable = new bool?(jtoken2.Value<bool>());
          }
          evaluationContext = evaluationContext.ParentContext;
        }
        while (evaluationContext != null);
        JToken currentJtoken = expression.GetCurrentJToken("@root.system.tracking", context);
        if (currentJtoken != null && currentJtoken.Type.Equals((object) JTokenType.String))
        {
          NotificationTrackingData notificationTrackingData = new NotificationTrackingData(currentJtoken.ToString());
          if (stringBuilder.Length > 0)
            notificationTrackingData.SetValue("Element", (JToken) stringBuilder.ToString());
          string encodedString = notificationTrackingData.GetEncodedString(nullable.HasValue && nullable.Value);
          if (!string.IsNullOrEmpty(encodedString))
          {
            NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
            queryString["tracking_data"] = encodedString;
            string str = string.Empty;
            if (uriString.Contains("#"))
            {
              str = uriString.Substring(uriString.IndexOf("#"));
              uriString = uriString.Substring(0, uriString.Length - str.Length);
            }
            if (uriString.Contains("?"))
              uriString = uriString.Substring(0, uriString.IndexOf("?"));
            uriString = uriString + "?" + queryString.ToString() + str;
          }
        }
      }
      writer.Write(uriString);
    }

    private static void StyleHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      string empty = string.Empty;
      List<string> stringList = new List<string>();
      for (int index = 0; index < expression.HelperArguments.Count; ++index)
      {
        string helperArgument = expression.GetHelperArgument<string>(context, index);
        if (!string.IsNullOrEmpty(helperArgument) && !helperArgument.Contains(".."))
          stringList.Add(helperArgument);
      }
      string computedStyleString;
      if (!stringList.Any<string>() || !NotificationTransformHandlebarHelpers.ComputeStyleStrings((IEnumerable<string>) stringList, context, out computedStyleString, out string _))
        return;
      writer.Write(computedStyleString);
    }

    private static bool ComputeStyleStrings(
      IEnumerable<string> selectors,
      MustacheEvaluationContext context,
      out string computedStyleString,
      out string computedClassString)
    {
      computedStyleString = string.Empty;
      computedClassString = string.Empty;
      JObject jobject;
      if (context.GetAllTemplateInputs().TryGetValue("ms.vss-notifications.standard-email-template-v2", out jobject))
      {
        Dictionary<string, string> collection = (Dictionary<string, string>) null;
        List<string> source = (List<string>) null;
        foreach (string selector in selectors)
        {
          JToken jtoken = jobject.SelectToken("styles." + selector);
          if (jtoken != null)
          {
            if (jtoken.Type.Equals((object) JTokenType.String))
            {
              computedStyleString = jtoken.ToString();
              break;
            }
            if (jtoken.Type.Equals((object) JTokenType.Object))
            {
              foreach (KeyValuePair<string, JToken> keyValuePair in (JObject) jtoken)
              {
                if (keyValuePair.Value != null && keyValuePair.Value.Type.Equals((object) JTokenType.String))
                {
                  if (string.Equals(keyValuePair.Key, "_class"))
                  {
                    if (source == null)
                      source = new List<string>();
                    source.Add(keyValuePair.Value.ToString());
                  }
                  if (collection == null)
                    collection = new Dictionary<string, string>();
                  collection[keyValuePair.Key] = keyValuePair.Value.ToString();
                }
              }
            }
          }
        }
        if (collection != null && string.IsNullOrEmpty(computedStyleString))
        {
          StringBuilder resultBuilder = new StringBuilder();
          collection.ForEach<KeyValuePair<string, string>>((Action<KeyValuePair<string, string>>) (stylePair => resultBuilder.Append(string.Format("{0}: {1}; ", (object) stylePair.Key, (object) stylePair.Value))));
          computedStyleString = resultBuilder.ToString().Trim();
        }
        if (source != null)
          computedClassString = string.Join(" ", source.Distinct<string>());
      }
      return !string.IsNullOrEmpty(computedStyleString) || !string.IsNullOrEmpty(computedClassString);
    }

    private static void HtmlGridHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      TagBuilder builder = new TagBuilder("table").Attribute("cellpadding", "0").Attribute("cellspacing", "0").Attribute("border", "0");
      string computedStyleString;
      string computedClassString;
      if (NotificationTransformHandlebarHelpers.ComputeStyleStrings((IEnumerable<string>) new string[1]
      {
        "grid"
      }, context, out computedStyleString, out computedClassString))
      {
        builder.Attribute("style", computedStyleString);
        builder.Attribute("class", computedClassString);
      }
      if (expression.IsBlockExpression)
        builder.InnerHtml = expression.EvaluateChildExpressions(context);
      writer.Write(builder.ToString());
    }

    private static void HtmlGridRowHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      TagBuilder tagBuilder = new TagBuilder("tr");
      if (expression.IsBlockExpression)
        tagBuilder.InnerHtml = expression.EvaluateChildExpressions(context);
      writer.Write(tagBuilder.ToString());
    }

    private static void HtmlGridCellHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      List<string> styleSelectors = new List<string>()
      {
        "grid.cell"
      };
      if (context.CurrentIndex == context.ParentItemsCount - 1)
        styleSelectors.Add("grid.cell.lastRow");
      if (expression.HelperArguments.Count > 0)
        ((IEnumerable<string>) expression.GetHelperArgument<string>(context, 0).Split(new char[2]
        {
          ',',
          ' '
        }, StringSplitOptions.RemoveEmptyEntries)).ForEach<string>((Action<string>) (s => styleSelectors.Add(string.Format("grid.cell.{0}", (object) s))));
      TagBuilder builder = new TagBuilder("td");
      string computedStyleString;
      string computedClassString;
      if (NotificationTransformHandlebarHelpers.ComputeStyleStrings((IEnumerable<string>) styleSelectors, context, out computedStyleString, out computedClassString))
      {
        builder.Attribute("style", computedStyleString);
        builder.Attribute("class", computedClassString);
      }
      if (expression.IsBlockExpression)
        builder.InnerHtml = expression.EvaluateChildExpressions(context);
      writer.Write(builder.ToString());
    }

    private static string HandlebarContributedPartialHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string rawHelperArgument1 = expression.GetRawHelperArgument(0);
      if (string.IsNullOrEmpty(rawHelperArgument1))
        return string.Empty;
      if (rawHelperArgument1[0].Equals('(') && rawHelperArgument1[rawHelperArgument1.Length - 1].Equals(')'))
      {
        JToken currentJtoken = expression.GetCurrentJToken(rawHelperArgument1.Substring(1, rawHelperArgument1.Length - 2), context);
        if (currentJtoken == null || !currentJtoken.Type.Equals((object) JTokenType.String))
          return string.Empty;
        rawHelperArgument1 = currentJtoken.ToString();
      }
      MustacheRootExpression expression1;
      if (!context.PartialExpressions.TryGetValue(rawHelperArgument1, out expression1))
        return string.Empty;
      MustacheEvaluationContext.CombinePartialsDictionaries(context, (MustacheAggregateExpression) expression1);
      object obj = context.ReplacementObject;
      MustacheEvaluationContext parentContext = context.ParentContext;
      Dictionary<string, MustacheRootExpression> partialExpressions = context.PartialExpressions;
      Dictionary<string, object> additionalEvaluationData = context.AdditionalEvaluationData;
      string rawHelperArgument2 = expression.GetRawHelperArgument(1);
      if (!string.IsNullOrEmpty(rawHelperArgument2))
      {
        obj = (object) expression.GetCurrentJToken(rawHelperArgument2, context);
        parentContext = context;
      }
      IDictionary<string, JObject> allTemplateInputs = context.GetAllTemplateInputs();
      JObject content;
      if (allTemplateInputs != null && allTemplateInputs.TryGetValue(rawHelperArgument1, out content))
      {
        if (obj == null)
        {
          obj = (object) content;
        }
        else
        {
          obj = (object) JObject.FromObject(obj);
          ((JContainer) obj).Merge((object) content);
        }
      }
      return expression1.Evaluate(obj, additionalEvaluationData, parentContext, partialExpressions, context.Options);
    }

    private static void IsHostedHelper(
      MustacheTemplatedExpression expression,
      MustacheTextWriter writer,
      MustacheEvaluationContext context)
    {
      if (context.GetRequestContext().ExecutionEnvironment.IsHostedDeployment)
      {
        if (expression.IsBlockExpression)
          expression.EvaluateChildExpressions(context, writer);
        else
          writer.Write("true");
      }
      else
      {
        if (expression.IsBlockExpression)
          return;
        writer.Write("false");
      }
    }

    private static TagBuilder Attribute(this TagBuilder builder, string name, string value)
    {
      builder.MergeAttribute(name, value);
      return builder;
    }
  }
}
