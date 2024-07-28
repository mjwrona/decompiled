// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.ModelBuilders.WorkItemModelBuilder
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Consumers, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEF54E7-7304-4071-B5F1-22428BB21801
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Consumers.dll

using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Consumers.Common.Templating.ModelBuilders
{
  public class WorkItemModelBuilder : JsonFriendlyModelBuilder
  {
    private const string c_fieldsPropertyName = "fields";
    private const string c_fieldsWorkItemUpdatePropertyName = "revision.fields";
    private const string c_revisionWorkItemUpdatedPropertyName = "revision";
    private const string c_newValueFieldName = "newValue";
    private const string c_oldValueFieldName = "oldValue";
    private const string c_oldValuePromotedFieldSuffix = "_oldValue";
    private const string c_systemFieldNamespace = "System.";
    private const string c_vstsCommonFieldNamespace = "Microsoft.VSTS.Common.";
    private const string c_resourceName = "workitem";
    private const string c_charsToRemoveInNormalizedFieldName = "(\\.|_)";

    protected override JObject TransformResource(JObject resource, string eventType)
    {
      resource = base.TransformResource(resource, eventType);
      string fieldsToPromoteJsonPath;
      Action<JObject, string, string> fieldPromoter = WorkItemModelBuilder.GetFieldPromoter(eventType, out fieldsToPromoteJsonPath);
      WorkItemModelBuilder.PromoteWorkItemFieldsToRoot(resource, "System.", fieldPromoter, fieldsToPromoteJsonPath);
      WorkItemModelBuilder.PromoteWorkItemFieldsToRoot(resource, "Microsoft.VSTS.Common.", fieldPromoter, fieldsToPromoteJsonPath);
      if (eventType == "workitem.updated" && resource["workItemId"] != null)
        resource["id"] = resource["workItemId"];
      return resource;
    }

    private static Action<JObject, string, string> GetFieldPromoter(
      string eventType,
      out string fieldsToPromoteJsonPath)
    {
      if (eventType == "workitem.updated")
      {
        fieldsToPromoteJsonPath = "revision.fields";
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        return WorkItemModelBuilder.\u003C\u003EO.\u003C0\u003E__PromoteWorkItemUpdatedField ?? (WorkItemModelBuilder.\u003C\u003EO.\u003C0\u003E__PromoteWorkItemUpdatedField = new Action<JObject, string, string>(WorkItemModelBuilder.PromoteWorkItemUpdatedField));
      }
      fieldsToPromoteJsonPath = "fields";
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return WorkItemModelBuilder.\u003C\u003EO.\u003C1\u003E__PromoteWorkItemField ?? (WorkItemModelBuilder.\u003C\u003EO.\u003C1\u003E__PromoteWorkItemField = new Action<JObject, string, string>(WorkItemModelBuilder.PromoteWorkItemField));
    }

    private static void PromoteWorkItemUpdatedField(
      JObject resource,
      string fieldName,
      string fieldNamespace)
    {
      string propertyName = WorkItemModelBuilder.NormalizeFieldName(fieldName, fieldNamespace);
      if (resource["fields"] != null && resource["fields"][(object) fieldName] != null)
      {
        resource[propertyName] = resource["fields"][(object) fieldName][(object) "newValue"];
        resource[propertyName + "_oldValue"] = resource["fields"][(object) fieldName][(object) "oldValue"];
      }
      else
        resource[propertyName] = resource["revision"][(object) "fields"][(object) fieldName];
    }

    private static void PromoteWorkItemField(
      JObject resource,
      string fieldName,
      string fieldNamespace)
    {
      resource[WorkItemModelBuilder.NormalizeFieldName(fieldName, fieldNamespace)] = resource["fields"][(object) fieldName];
    }

    private static void PromoteWorkItemFieldsToRoot(
      JObject resource,
      string fieldNamespace,
      Action<JObject, string, string> fieldPromoter,
      string fieldsToPromoteJsonPath)
    {
      JToken jtoken = resource.SelectToken(fieldsToPromoteJsonPath, false);
      if (jtoken == null)
        return;
      jtoken.Children<JProperty>().Where<JProperty>((Func<JProperty, bool>) (p => p.Name.StartsWith(fieldNamespace))).Select<JProperty, string>((Func<JProperty, string>) (p => p.Name)).ToList<string>().ForEach((Action<string>) (fieldName => fieldPromoter(resource, fieldName, fieldNamespace)));
    }

    private static string NormalizeFieldName(string fieldName, string fieldNamespace)
    {
      string str = Regex.Replace(fieldName.Substring(fieldNamespace.Length), "(\\.|_)", string.Empty);
      return char.ToLower(str[0]).ToString() + str.Substring(1);
    }
  }
}
