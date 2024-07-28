// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemsViewODataJsonSchemaSelectItemTranslator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class WorkItemsViewODataJsonSchemaSelectItemTranslator : ODataJsonSchemaSelectItemTranslator
  {
    private WorkItemsViewODataJsonSchemaSelectItemTranslatorData data;

    public WorkItemsViewODataJsonSchemaSelectItemTranslator(
      IEdmModel model,
      WorkItemsViewODataJsonSchemaSelectItemTranslatorData data)
      : base(model)
    {
      this.data = data;
    }

    public override KeyValuePair<string, ODataJsonSchema>? Translate(
      ExpandedNavigationSelectItem item)
    {
      KeyValuePair<string, ODataJsonSchema>? nullable = base.Translate(item);
      if (nullable.HasValue)
      {
        ODataJsonSchema odataJsonSchema = nullable.Value.Value;
        if (item.NavigationSource.EntityType().FullName() == typeof (User).FullName && odataJsonSchema.Properties.ContainsKey("UserName"))
        {
          string displayNameAnnotation = odataJsonSchema.DisplayNameAnnotation;
          odataJsonSchema.DisplayNameAnnotation = (string) null;
          odataJsonSchema.Properties["UserName"].DisplayNameAnnotation = displayNameAnnotation;
        }
        else if (!string.IsNullOrWhiteSpace(odataJsonSchema.DisplayNameAnnotation))
        {
          foreach (KeyValuePair<string, ODataJsonSchema> property in (IEnumerable<KeyValuePair<string, ODataJsonSchema>>) odataJsonSchema.Properties)
          {
            string str1 = property.Value.DisplayNameAnnotation;
            if (str1.StartsWith(odataJsonSchema.DisplayNameAnnotation))
            {
              str1 = str1.Replace(odataJsonSchema.DisplayNameAnnotation, string.Empty).Trim();
              property.Value.DisplayNameAnnotation = str1;
            }
            string str2 = odataJsonSchema.DisplayNameAnnotation + " " + str1;
            property.Value.DisplayNameAnnotation = str2;
          }
          odataJsonSchema.DisplayNameAnnotation = (string) null;
        }
      }
      return nullable;
    }

    public override KeyValuePair<string, ODataJsonSchema>? Translate(PathSelectItem item)
    {
      KeyValuePair<string, ODataJsonSchema>? nullable = base.Translate(item);
      if (nullable.HasValue)
      {
        KeyValuePair<string, ODataJsonSchema> keyValuePair = nullable.Value;
        string key = keyValuePair.Key;
        keyValuePair = nullable.Value;
        ODataJsonSchema odataJsonSchema = keyValuePair.Value;
        if (this.data.Type == WorkItemsViewType.Historical)
        {
          if (key == "DateSK" && !this.data.HistoricalData.IncludeDateSK)
          {
            List<ODataMashupFunction> odataMashupFunctionList = odataJsonSchema.TransformationsAnnotation != null ? new List<ODataMashupFunction>((IEnumerable<ODataMashupFunction>) odataJsonSchema.TransformationsAnnotation) : new List<ODataMashupFunction>();
            odataMashupFunctionList.Add(new ODataMashupFunction()
            {
              Function = "VSTS.Date.FromDateSK"
            });
            odataJsonSchema.DisplayNameAnnotation = "Date";
            odataJsonSchema.TransformationsAnnotation = odataMashupFunctionList.ToArray();
          }
          else if (key == "RevisedDateSK" && !this.data.HistoricalData.IncludeRevisedDateSK)
            odataJsonSchema.DisplayNameAnnotation = (string) null;
        }
      }
      return nullable;
    }
  }
}
