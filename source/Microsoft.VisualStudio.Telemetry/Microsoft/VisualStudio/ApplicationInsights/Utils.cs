// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Utils
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Extensibility;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.Tracing;
using Microsoft.VisualStudio.Telemetry.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.ApplicationInsights
{
  internal static class Utils
  {
    private static readonly string[] RelativeFolderPath = new string[3]
    {
      "Microsoft",
      "ApplicationInsights",
      "Cache"
    };

    public static bool IsNullOrWhiteSpace(this string value) => value == null || value.All<char>(new Func<char, bool>(char.IsWhiteSpace));

    public static void CopyDictionary<TValue>(
      IDictionary<string, TValue> source,
      IDictionary<string, TValue> target)
    {
      foreach (KeyValuePair<string, TValue> keyValuePair in (IEnumerable<KeyValuePair<string, TValue>>) source)
      {
        if (!string.IsNullOrEmpty(keyValuePair.Key) && !target.ContainsKey(keyValuePair.Key))
          target[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public static string PopulateRequiredStringValue(
      string value,
      string parameterName,
      string telemetryType)
    {
      if (!string.IsNullOrEmpty(value))
        return value;
      CoreEventSource.Log.PopulateRequiredStringWithValue(parameterName, telemetryType);
      return parameterName + " is a required field for " + telemetryType;
    }

    public static TimeSpan ValidateDuration(string value)
    {
      TimeSpan output;
      if (TimeSpanEx.TryParse(value, CultureInfo.InvariantCulture, out output))
        return output;
      CoreEventSource.Log.RequestTelemetryIncorrectDuration();
      return TimeSpan.Zero;
    }

    public static TType ReadSerializedContext<TType>(string fileName) where TType : IFallbackContext, new()
    {
      string tempPath = Path.GetTempPath();
      string str = ((IEnumerable<string>) Utils.RelativeFolderPath).Aggregate<string, string>(tempPath, new Func<string, string, string>(Path.Combine));
      if (!Directory.Exists(str))
      {
        try
        {
          ReparsePointAware.CreateDirectory(str);
        }
        catch (IOException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        catch (NotSupportedException ex)
        {
        }
      }
      bool flag = true;
      string path = Path.Combine(str, fileName);
      if (File.Exists(path))
      {
        try
        {
          using (FileStream input = ReparsePointAware.OpenRead(path))
          {
            using (XmlReader reader = XmlReader.Create((Stream) input))
            {
              XDocument xdocument = XDocument.Load(reader);
              TType type = new TType();
              if (type.Deserialize(xdocument.Root))
              {
                flag = false;
                return type;
              }
            }
          }
        }
        catch (IOException ex)
        {
        }
        catch (SecurityException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        catch (XmlException ex)
        {
        }
        catch (NotSupportedException ex)
        {
        }
      }
      if (flag)
      {
        XDocument xdocument = new XDocument();
        xdocument.Add((object) new XElement(XName.Get(typeof (TType).Name)));
        TType type = new TType();
        type.Initialize();
        type.Serialize(xdocument.Root);
        try
        {
          using (StreamWriter text = ReparsePointAware.CreateText(path))
          {
            using (XmlWriter writer = XmlWriter.Create((TextWriter) text))
              xdocument.Save(writer);
            return type;
          }
        }
        catch (IOException ex)
        {
        }
        catch (SecurityException ex)
        {
        }
        catch (UnauthorizedAccessException ex)
        {
        }
        catch (XmlException ex)
        {
        }
        catch (NotSupportedException ex)
        {
        }
      }
      TType type1 = new TType();
      type1.Initialize();
      return type1;
    }
  }
}
