// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestReportsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public static class TestReportsHelper
  {
    internal static Dictionary<string, string> GetTransformFilterMap(string filterString)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(filterString, nameof (filterString));
      Dictionary<string, string> transformFilterMap = new Dictionary<string, string>();
      string str1 = filterString;
      char[] chArray1 = new char[1]{ '&' };
      foreach (string str2 in str1.Split(chArray1))
      {
        char[] chArray2 = new char[1]{ '=' };
        string[] strArray = str2.Split(chArray2);
        if (strArray.Length == 2)
          transformFilterMap.Add(strArray[0], strArray[1]);
      }
      return transformFilterMap;
    }

    internal static void ProcessResultsForCharts<RecordType>(
      IVssRequestContext requestContext,
      ITabulator<RecordType> tabulator,
      IEnumerable<RecordType> dataset)
    {
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      int capacity = 100;
      List<RecordType> recordBuffer = new List<RecordType>(capacity);
      foreach (RecordType recordType in dataset)
      {
        if ((double) stopwatch.ElapsedMilliseconds >= DataServicesTimeoutPolicy.GetTransformerTimeout(requestContext).TotalMilliseconds)
          throw new TimeoutException();
        recordBuffer.Add(recordType);
        if (recordBuffer.Count == capacity)
        {
          tabulator.Tabulate((IEnumerable<RecordType>) recordBuffer, requestContext);
          recordBuffer.Clear();
        }
      }
      if (recordBuffer.Count <= 0)
        return;
      tabulator.Tabulate((IEnumerable<RecordType>) recordBuffer, requestContext);
    }
  }
}
