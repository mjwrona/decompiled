// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WorkItemsViewODataJsonSchemaGenerator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.WebApi;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class WorkItemsViewODataJsonSchemaGenerator : ODataJsonSchemaGenerator
  {
    private WorkItemsViewODataJsonSchemaGeneratorData Data;
    private WorkItemsViewODataJsonSchemaGenerator.HistoricalTransformationsData HistoricalData;

    private WorkItemsViewODataJsonSchemaGenerator.HistoricalTransformationsData GetTransformationsData(
      IVssRequestContext requestContext,
      string queryTemplate)
    {
      IDictionary<string, SingleValueNode> parameterAliasNodes = this.Parser.ParseUri().ParameterAliasNodes;
      ICollectionDateTimeService service = requestContext.GetService<ICollectionDateTimeService>();
      string key1 = "@endDateSK";
      IVssRequestContext requestContext1 = requestContext;
      int num1 = int.Parse(service.GetCollectionCurrentDateTime(requestContext1).ToString("yyyyMMdd"));
      if (parameterAliasNodes.ContainsKey(key1))
      {
        int num2 = int.Parse(parameterAliasNodes[key1].Accept<string>((QueryNodeVisitor<string>) new ConstantAliasNodeVisitor()));
        if (num2 < num1)
          num1 = num2;
      }
      string key2 = "@startDateSK";
      int num3 = parameterAliasNodes.ContainsKey(key2) ? int.Parse(parameterAliasNodes[key2].Accept<string>((QueryNodeVisitor<string>) new ConstantAliasNodeVisitor())) : 19000101;
      string key3 = "@period";
      string str = WorkItemsViewODataJsonSchemaGenerator.ReinterpretPeriodForSchema(parameterAliasNodes.ContainsKey(key3) ? parameterAliasNodes[key3].Accept<string>((QueryNodeVisitor<string>) new ConstantAliasNodeVisitor()) : (string) null);
      bool flag1 = AnalyticsViewsQueryParsingUtilities.IsPropertySelected(queryTemplate, "DateSK");
      bool flag2 = AnalyticsViewsQueryParsingUtilities.IsPropertySelected(queryTemplate, "RevisedDateSK");
      return new WorkItemsViewODataJsonSchemaGenerator.HistoricalTransformationsData()
      {
        StartDateSK = new int?(num3),
        EndDateSK = new int?(num1),
        Period = str,
        IncludeDateSK = flag1,
        IncludeRevisedDateSK = flag2
      };
    }

    private static string ReinterpretPeriodForSchema(string period)
    {
      if (period == null)
        return (string) null;
      string str = new Regex("Microsoft.VisualStudio.Services.Analytics.Model.Period'(?<period>[A-Za-z]+)'").Match(period).Groups[nameof (period)].Value;
      return !(str == "None") ? str : "Day";
    }

    public WorkItemsViewODataJsonSchemaGenerator(
      IVssRequestContext requestContext,
      IEdmModel model,
      WorkItemsViewODataJsonSchemaGeneratorData data)
      : base(model, (ODataJsonSchemaGeneratorData) data)
    {
      this.Data = data;
      if (data == null || data.QueryType != WorkItemsViewType.Historical)
        return;
      this.HistoricalData = this.GetTransformationsData(requestContext, data.ODataQueryTemplate);
    }

    private void ThrowIfEndDateSKIsNotDefined(int? endDateSK, string mashupFunctionName)
    {
      if (!endDateSK.HasValue)
        throw new ViewSchemaMissingMashupFunctionParameterException("EndDateSK", mashupFunctionName);
    }

    protected override ODataMashupFunction[] GetListTransformationsAnnotation()
    {
      WorkItemsViewODataJsonSchemaGeneratorData data = this.Data;
      if ((data != null ? (data.QueryType == WorkItemsViewType.Historical ? 1 : 0) : 0) == 0)
        return (ODataMashupFunction[]) null;
      string mashupFunctionName = "VSTS.List.ToSnapshot";
      this.ThrowIfEndDateSKIsNotDefined(this.HistoricalData.EndDateSK, mashupFunctionName);
      ODataMashupFunction odataMashupFunction = new ODataMashupFunction()
      {
        Function = mashupFunctionName,
        Parameters = new Dictionary<string, object>()
        {
          {
            "endDateSK",
            (object) this.HistoricalData.EndDateSK
          }
        }
      };
      if (this.HistoricalData.StartDateSK.HasValue)
        odataMashupFunction.Parameters.Add("startDateSK", (object) this.HistoricalData.StartDateSK);
      if (this.HistoricalData.Period != null)
        odataMashupFunction.Parameters.Add("period", (object) this.HistoricalData.Period);
      return new ODataMashupFunction[1]
      {
        odataMashupFunction
      };
    }

    protected override ODataMashupFunction[] GetRecordTransformationsAnnotation()
    {
      WorkItemsViewODataJsonSchemaGeneratorData data = this.Data;
      if ((data != null ? (data.QueryType == WorkItemsViewType.Historical ? 1 : 0) : 0) == 0)
        return (ODataMashupFunction[]) null;
      List<ODataMashupFunction> odataMashupFunctionList1 = new List<ODataMashupFunction>();
      string mashupFunctionName = "VSTS.Record.AddIsCurrent";
      this.ThrowIfEndDateSKIsNotDefined(this.HistoricalData.EndDateSK, mashupFunctionName);
      odataMashupFunctionList1.Add(new ODataMashupFunction()
      {
        Function = mashupFunctionName,
        Parameters = new Dictionary<string, object>()
        {
          {
            "endDateSK",
            (object) this.HistoricalData.EndDateSK
          }
        }
      });
      if (this.HistoricalData.IncludeDateSK)
      {
        string str = "Date";
        List<ODataMashupFunction> odataMashupFunctionList2 = odataMashupFunctionList1;
        ODataMashupFunction odataMashupFunction1 = new ODataMashupFunction();
        odataMashupFunction1.Function = "VSTS.Record.DuplicateField";
        ODataMashupFunction odataMashupFunction2 = odataMashupFunction1;
        Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
        dictionary1.Add("sourceFieldName", (object) "DateSK");
        dictionary1.Add("fieldName", (object) str);
        Dictionary<string, object> dictionary2 = dictionary1;
        ODataJsonSchema odataJsonSchema1 = new ODataJsonSchema();
        odataJsonSchema1.Type = ODataJsonSchemaType.Integer;
        odataJsonSchema1.Format = ODataJsonSchemaFormat.Int32;
        odataJsonSchema1.DisplayNameAnnotation = str;
        odataJsonSchema1.TransformationsAnnotation = new ODataMashupFunction[1]
        {
          new ODataMashupFunction()
          {
            Function = "VSTS.Date.FromDateSK"
          }
        };
        ODataJsonSchema odataJsonSchema2 = odataJsonSchema1;
        dictionary2.Add("fieldSchema", (object) odataJsonSchema2);
        Dictionary<string, object> dictionary3 = dictionary1;
        odataMashupFunction2.Parameters = dictionary3;
        ODataMashupFunction odataMashupFunction3 = odataMashupFunction1;
        odataMashupFunctionList2.Add(odataMashupFunction3);
      }
      return odataMashupFunctionList1.ToArray();
    }

    protected override SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?> GetSelectItemTranslator(
      IEdmModel model)
    {
      HistoricalTranslatorData historicalTranslatorData = (HistoricalTranslatorData) null;
      if (this.Data.QueryType == WorkItemsViewType.Historical)
        historicalTranslatorData = new HistoricalTranslatorData()
        {
          IncludeDateSK = this.HistoricalData.IncludeDateSK,
          IncludeRevisedDateSK = this.HistoricalData.IncludeRevisedDateSK
        };
      WorkItemsViewODataJsonSchemaSelectItemTranslatorData data = new WorkItemsViewODataJsonSchemaSelectItemTranslatorData()
      {
        Type = this.Data.QueryType,
        HistoricalData = historicalTranslatorData
      };
      return (SelectItemTranslator<KeyValuePair<string, ODataJsonSchema>?>) new WorkItemsViewODataJsonSchemaSelectItemTranslator(model, data);
    }

    private class HistoricalTransformationsData
    {
      public int? StartDateSK;
      public int? EndDateSK;
      public string Period;
      public bool IncludeDateSK;
      public bool IncludeRevisedDateSK;
    }
  }
}
