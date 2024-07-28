// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Metrics.QueryLanguageResponseToDatatable
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Cloud.Metrics.Client.Metrics
{
  internal static class QueryLanguageResponseToDatatable
  {
    public static JArray GetResponseAsTable(Stream responseFromMetrics)
    {
      using (StreamReader reader = new StreamReader(responseFromMetrics, Encoding.UTF8))
      {
        JObject jobject1 = (JObject) JsonSerializer.Create().Deserialize((TextReader) reader, typeof (JObject));
        JArray responseAsTable = JArray.Parse("[]");
        DateTime dateTime = DateTime.Parse(jobject1["startTimeUtc"].Value<string>());
        TimeSpan timeSpan = TimeSpan.FromMinutes((double) jobject1["timeResolutionInMinutes"].Value<int>());
        foreach (JToken jtoken1 in jobject1["filteredTimeSeriesList"][(object) "$values"] as JArray)
        {
          JArray jarray1 = jtoken1[(object) "timeSeriesValues"][(object) "$values"] as JArray;
          string str1 = jtoken1[(object) "metricIdentifier"][(object) "monitoringAccount"].Value<string>();
          string str2 = jtoken1[(object) "metricIdentifier"][(object) "metricNamespace"].Value<string>();
          string str3 = jtoken1[(object) "metricIdentifier"][(object) "metricName"].Value<string>();
          JArray jarray2 = jtoken1[(object) "dimensionList"][(object) "$values"] as JArray;
          Dictionary<DateTime, JObject> dictionary = new Dictionary<DateTime, JObject>();
          foreach (JToken jtoken2 in jarray1)
          {
            string propertyName = jtoken2[(object) "key"][(object) "name"].Value<string>();
            JArray jarray3 = jtoken2[(object) "value"] as JArray;
            DateTime key = dateTime;
            foreach (JToken jtoken3 in jarray3)
            {
              JObject jobject2;
              if (dictionary.ContainsKey(key))
              {
                jobject2 = dictionary[key];
              }
              else
              {
                jobject2 = JObject.Parse("{}");
                jobject2.Add("TimestampUtc", (JToken) key.ToString((IFormatProvider) DateTimeFormatInfo.InvariantInfo));
                jobject2.Add("i_AccountName", (JToken) str1);
                jobject2.Add("i_MetricNamespace", (JToken) str2);
                jobject2.Add("i_MetricName", (JToken) str3);
                foreach (JToken jtoken4 in jarray2)
                  jobject2.Add(jtoken4[(object) "key"].Value<string>(), (JToken) jtoken4[(object) "value"].Value<string>());
                dictionary[key] = jobject2;
                responseAsTable.Add((JToken) jobject2);
              }
              double d = jtoken3.Value<double>();
              if (double.IsNaN(d))
                d = 0.0;
              jobject2.Add(propertyName, (JToken) d);
              key = key.Add(timeSpan);
            }
          }
        }
        return responseAsTable;
      }
    }
  }
}
