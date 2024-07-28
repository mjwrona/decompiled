// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.EnforceAIRestrictionAction
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class EnforceAIRestrictionAction : IEventProcessorAction
  {
    private const int MaxPropertyNameLength = 150;
    private const int MaxPropertyValueLength = 1024;

    public int Priority => 200;

    public bool Execute(IEventProcessorContext eventProcessorContext)
    {
      List<string> stringList = (List<string>) null;
      List<Tuple<string, string>> tupleList = (List<Tuple<string, string>>) null;
      TelemetryEvent telemetryEvent = eventProcessorContext.TelemetryEvent;
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
      {
        if (!EnforceAIRestrictionAction.IsPropertyNameValid(property.Key))
        {
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(property.Key);
        }
        else if (property.Value != null && !(property.Value is TelemetryComplexProperty))
        {
          string str = property.Value.ToString();
          if (!EnforceAIRestrictionAction.IsPropertyValueValid(str))
          {
            if (tupleList == null)
              tupleList = new List<Tuple<string, string>>();
            tupleList.Add(Tuple.Create<string, string>(property.Key, str));
          }
        }
      }
      if (stringList != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string key in stringList)
        {
          telemetryEvent.Properties.Remove(key);
          if (stringBuilder.Length + 1 + key.Length <= 1024)
          {
            if (stringBuilder.Length > 0)
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ",{0}", new object[1]
              {
                (object) key
              });
            else
              stringBuilder.Append(key);
          }
          else if (stringBuilder.Length == 0)
            stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}...", new object[1]
            {
              (object) key.Substring(0, 147)
            });
        }
        telemetryEvent.Properties["Reserved.InvalidEvent.InvalidPropertyNames"] = (object) stringBuilder.ToString();
      }
      if (tupleList != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (Tuple<string, string> tuple in tupleList)
        {
          telemetryEvent.Properties[tuple.Item1] = (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}...", new object[1]
          {
            (object) tuple.Item2.Substring(0, 1021)
          });
          if (stringBuilder.Length + 1 + tuple.Item1.Length <= 150)
          {
            if (stringBuilder.Length > 0)
              stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, ",{0}", new object[1]
              {
                (object) tuple.Item1
              });
            else
              stringBuilder.Append(tuple.Item1);
          }
        }
        telemetryEvent.Properties["Reserved.InvalidEvent.TruncatedProperties"] = (object) stringBuilder.ToString();
      }
      return true;
    }

    private static bool IsPropertyValueValid(string value) => value.Length <= 1024;

    private static bool IsPropertyNameValid(string propertyName)
    {
      if (propertyName.Length > 150)
        return false;
      foreach (char c in propertyName)
      {
        if (!char.IsLetterOrDigit(c) && c != '_' && c != '/' && c != '\\' && c != '.' && c != '-')
          return false;
      }
      return true;
    }
  }
}
