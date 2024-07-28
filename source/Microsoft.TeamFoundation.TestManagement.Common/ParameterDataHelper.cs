// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Common.ParameterDataHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1401105B-6771-499A-8DF3-F3CBE1BB3AE4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.Common.dll

using Microsoft.VisualStudio.Services.Common.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ParameterDataHelper
  {
    public static void PopulateDataSetFromSharedParameterData(
      DataSet data,
      TestCaseParameterDataInfo parameterDataInfo,
      ISharedParameterData parameterData)
    {
      DataTable table;
      if (data.Tables.Count == 0)
      {
        table = new DataTable("Table1");
        data.Tables.Add(table);
      }
      else
      {
        table = data.Tables[0];
        table.Reset();
      }
      List<ISharedParameterDataRow> dataRows = (List<ISharedParameterDataRow>) null;
      if (parameterData != null && parameterData.ParameterValues != null)
        dataRows = parameterData.ParameterValues.Rows;
      ParameterDataHelper.PopulateDataSetColumNames(parameterDataInfo, table);
      ParameterDataHelper.PopulateDataSetRows(parameterDataInfo, table, dataRows);
      data.AcceptChanges();
    }

    private static void PopulateDataSetRows(
      TestCaseParameterDataInfo parameterDataInfo,
      DataTable table,
      List<ISharedParameterDataRow> dataRows)
    {
      Dictionary<string, int> localParameterNameToIndexMap = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      if (dataRows == null)
        return;
      for (int index = 0; index < dataRows.Count; ++index)
      {
        List<string> stringList = new List<string>();
        if (parameterDataInfo.ParameterMap != null)
        {
          foreach (ParameterDefinition parameter in parameterDataInfo.ParameterMap)
          {
            if (parameter is SharedParameterDefinition sharedParamDef && dataRows[index].ParameterValues != null)
              stringList.Add(ParameterDataHelper.GetParameterValueForParameterDefinition(sharedParamDef, localParameterNameToIndexMap, dataRows[index].ParameterValues));
          }
        }
        table.Rows.Add((object[]) stringList.ToArray());
      }
    }

    private static string GetParameterValueForParameterDefinition(
      SharedParameterDefinition sharedParamDef,
      Dictionary<string, int> localParameterNameToIndexMap,
      List<IKeyValuePair> rowValues)
    {
      string parameterDefinition = string.Empty;
      if (sharedParamDef != null && rowValues != null)
      {
        int index = -1;
        string key = XmlConvert.EncodeName(sharedParamDef.LocalParamName);
        if (!localParameterNameToIndexMap.TryGetValue(key, out index))
        {
          string encodedSharedParamName = XmlConvert.EncodeName(sharedParamDef.SharedParameterName);
          index = rowValues.FindIndex((Predicate<IKeyValuePair>) (paramValue => string.Equals(paramValue.Key, encodedSharedParamName, StringComparison.CurrentCultureIgnoreCase)));
          if (index != -1)
            localParameterNameToIndexMap.Add(key, index);
        }
        if (index != -1)
        {
          IKeyValuePair keyValuePair = rowValues.ElementAtOrDefault<IKeyValuePair>(index);
          if (keyValuePair != null)
            parameterDefinition = UriUtility.HtmlDecode(keyValuePair.Value);
        }
      }
      return parameterDefinition;
    }

    private static void PopulateDataSetColumNames(
      TestCaseParameterDataInfo parameterDataInfo,
      DataTable table)
    {
      List<string> localParamNames = parameterDataInfo.GetLocalParamNames();
      if (localParamNames == null)
        return;
      foreach (string columnName in localParamNames)
      {
        if (!string.IsNullOrEmpty(columnName))
          table.Columns.Add(columnName);
      }
    }

    public static TestCaseParameterDataInfo ReadParameterDataInfoFromJson(string dataRowValue)
    {
      try
      {
        return JsonConvert.DeserializeObject<TestCaseParameterDataInfo>(dataRowValue, new JsonSerializerSettings()
        {
          TypeNameHandling = TypeNameHandling.None
        });
      }
      catch (JsonReaderException ex)
      {
        return (TestCaseParameterDataInfo) null;
      }
    }

    public static bool TryParseDataSetFromXml(DataSet data, string dataRowXml)
    {
      try
      {
        if (!string.IsNullOrEmpty(dataRowXml))
        {
          using (XmlTextReader safeXmlTextReader = Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.CreateSafeXmlTextReader((TextReader) new StringReader(dataRowXml)))
          {
            safeXmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
            int content = (int) safeXmlTextReader.MoveToContent();
            int num = (int) data.ReadXml((XmlReader) safeXmlTextReader, XmlReadMode.ReadSchema);
            data.AcceptChanges();
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(TraceKeywords.TestManagement, "TryParseDataFromXml", ex);
        return false;
      }
    }

    public static SharedParameterData GetSharedParameterDataFromParametersFieldValue(
      string parametersFieldValue)
    {
      parametersFieldValue1 = (SharedParameterData) null;
      using (XmlReader safeReader = Microsoft.TeamFoundation.TestManagement.Common.Internal.XmlUtility.CreateSafeReader((TextReader) new StringReader(parametersFieldValue)))
      {
        object obj = new XmlSerializer(typeof (SharedParameterData)).Deserialize(safeReader);
        if (obj != null)
        {
          if (obj is SharedParameterData parametersFieldValue1)
            return parametersFieldValue1;
        }
      }
      return parametersFieldValue1;
    }
  }
}
