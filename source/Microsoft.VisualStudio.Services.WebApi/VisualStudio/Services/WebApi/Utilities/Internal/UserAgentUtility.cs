// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Utilities.Internal.UserAgentUtility
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Microsoft.VisualStudio.Services.WebApi.Utilities.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class UserAgentUtility
  {
    private static Lazy<List<ProductInfoHeaderValue>> s_defaultRestUserAgent = new Lazy<List<ProductInfoHeaderValue>>(new Func<List<ProductInfoHeaderValue>>(UserAgentUtility.ConstructDefaultRestUserAgent));
    private static Lazy<string> s_defaultSoapUserAgent = new Lazy<string>(new Func<string>(UserAgentUtility.ConstructDefaultSoapUserAgent));
    private static readonly Dictionary<string, Tuple<string, int>> s_dev16SkuToAgentStringMap = new Dictionary<string, Tuple<string, int>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Enterprise",
        new Tuple<string, int>("Enterprise", 74)
      },
      {
        "Professional",
        new Tuple<string, int>("Pro", 70)
      },
      {
        "Community",
        new Tuple<string, int>("Community", 75)
      },
      {
        "V3|UNKNOWN",
        new Tuple<string, int>("TE", 76)
      },
      {
        "V4|UNKNOWN",
        new Tuple<string, int>("Sql", 77)
      },
      {
        "IntShell",
        new Tuple<string, int>("IntShell", 78)
      }
    };

    public static List<ProductInfoHeaderValue> GetDefaultRestUserAgent() => UserAgentUtility.s_defaultRestUserAgent.Value;

    private static List<ProductInfoHeaderValue> ConstructDefaultRestUserAgent()
    {
      string a;
      try
      {
        a = Path.GetFileName(Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.GetModuleFileName());
      }
      catch (Exception ex)
      {
        Trace.WriteLine("DefaultUserAgent: Unable to get exe.  " + ex.ToString());
        a = "unavailable";
      }
      Tuple<string, int> tuple = (Tuple<string, int>) null;
      if (string.Equals(a, "devenv.exe", StringComparison.OrdinalIgnoreCase))
        tuple = UserAgentUtility.GetCurrentSkuInfo();
      string str = string.Empty;
      if (AppDomain.CurrentDomain != null)
        str = (string) AppDomain.CurrentDomain.GetData("ApplicationName");
      if (!string.IsNullOrEmpty(str))
        a = a + "[" + str + "]";
      string productVersion = "unavailable";
      try
      {
        AssemblyFileVersionAttribute customAttribute = typeof (UserAgentUtility).GetTypeInfo().Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        if (customAttribute != null)
          productVersion = customAttribute.Version;
      }
      catch (Exception ex)
      {
        Trace.WriteLine("DefaultUserAgent: Unable to get fileVersion: " + ex.ToString());
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("(");
      stringBuilder.Append(a);
      if (tuple != null)
      {
        stringBuilder.Append(", ");
        stringBuilder.Append(tuple.Item1);
        stringBuilder.Append(", SKU:");
        stringBuilder.Append(tuple.Item2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      stringBuilder.Append(")");
      string comment = stringBuilder.ToString();
      return new List<ProductInfoHeaderValue>()
      {
        new ProductInfoHeaderValue("VSServices", productVersion),
        new ProductInfoHeaderValue(comment)
      };
    }

    public static string GetDefaultSoapUserAgent() => UserAgentUtility.s_defaultSoapUserAgent.Value;

    private static string ConstructDefaultSoapUserAgent()
    {
      string a;
      try
      {
        a = Path.GetFileName(Microsoft.VisualStudio.Services.Common.Internal.NativeMethods.GetModuleFileName());
      }
      catch (Exception ex)
      {
        Trace.WriteLine("DefaultUserAgent: Unable to get exe: " + ex.ToString());
        a = "unavailable";
      }
      Tuple<string, int> tuple = (Tuple<string, int>) null;
      if (string.Equals(a, "devenv.exe", StringComparison.OrdinalIgnoreCase))
        tuple = UserAgentUtility.GetCurrentSkuInfo();
      string str1 = string.Empty;
      if (AppDomain.CurrentDomain != null)
        str1 = (string) AppDomain.CurrentDomain.GetData("ApplicationName");
      if (!string.IsNullOrEmpty(str1))
        a = a + "[" + str1 + "]";
      string str2 = string.Empty;
      try
      {
        foreach (object customAttribute in typeof (UserAgentUtility).Assembly.GetCustomAttributes(false))
        {
          if (customAttribute is AssemblyFileVersionAttribute)
          {
            str2 = ((AssemblyFileVersionAttribute) customAttribute).Version;
            break;
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine("DefaultUserAgent: Unable to get fileVersion: " + ex.ToString());
        str2 = "unavailable";
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("Team Foundation (");
      stringBuilder.Append(a);
      stringBuilder.Append(", ");
      stringBuilder.Append(str2);
      if (tuple != null)
      {
        stringBuilder.Append(", ");
        stringBuilder.Append(tuple.Item1);
        stringBuilder.Append(", SKU:");
        stringBuilder.Append(tuple.Item2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private static Tuple<string, int> GetCurrentSkuInfo()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("VSSKUEDITION");
      Tuple<string, int> tuple;
      return !string.IsNullOrEmpty(environmentVariable) && UserAgentUtility.s_dev16SkuToAgentStringMap.TryGetValue(environmentVariable, out tuple) ? tuple : new Tuple<string, int>("Other", 79);
    }
  }
}
