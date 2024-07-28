// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.XPath;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class ConfigUtil
  {
    private const string c_area = "FileCacheService";
    private const string c_layer = "ProxyConfiguration";

    internal static bool TryReadNode(XPathNavigator navigator, string node, out string value)
    {
      XPathNavigator xpathNavigator = navigator.SelectSingleNode(node);
      if (xpathNavigator != null && xpathNavigator.Value != null)
      {
        value = xpathNavigator.Value;
        return true;
      }
      value = (string) null;
      return false;
    }

    internal static long ReadLong(
      XPathNavigator navigator,
      string node,
      long defaultValue,
      long lowerBound,
      long upperBound)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ConfigUtil.ReadValue<long>(navigator, ConfigUtil.\u003C\u003EO.\u003C0\u003E__TryParse ?? (ConfigUtil.\u003C\u003EO.\u003C0\u003E__TryParse = new ConfigUtil.TryParse<long>(long.TryParse)), node, defaultValue, lowerBound, upperBound);
    }

    internal static double ReadDouble(
      XPathNavigator navigator,
      string node,
      double defaultValue,
      double lowerBound,
      double upperBound)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return ConfigUtil.ReadValue<double>(navigator, ConfigUtil.\u003C\u003EO.\u003C1\u003E__TryParse ?? (ConfigUtil.\u003C\u003EO.\u003C1\u003E__TryParse = new ConfigUtil.TryParse<double>(double.TryParse)), node, defaultValue, lowerBound, upperBound);
    }

    private static T ReadValue<T>(
      XPathNavigator navigator,
      ConfigUtil.TryParse<T> tryParseFunc,
      string node,
      T defaultValue,
      T lowerBound,
      T upperBound)
    {
      T obj = defaultValue;
      string str;
      if (ConfigUtil.TryReadNode(navigator, node, out str))
      {
        T x;
        if (tryParseFunc(str, out x))
        {
          Comparer<T> comparer = Comparer<T>.Default;
          if (comparer.Compare(x, lowerBound) < 0 || comparer.Compare(x, upperBound) > 0)
            TeamFoundationEventLog.Default.Log(FrameworkResources.ConfigurationNumericValueOutOfRange((object) node, (object) lowerBound, (object) upperBound, (object) defaultValue), TeamFoundationEventId.ConfigurationWarning, EventLogEntryType.Warning);
          else
            obj = x;
        }
        else
          TeamFoundationEventLog.Default.Log(FrameworkResources.ConfigValueNotNumeric((object) node, (object) str, (object) defaultValue), TeamFoundationEventId.ConfigurationWarning, EventLogEntryType.Warning);
      }
      TeamFoundationTracingService.TraceRaw(13207, TraceLevel.Verbose, "FileCacheService", "ProxyConfiguration", "{0}={1}", (object) node, (object) obj);
      return obj;
    }

    private delegate bool TryParse<T>(string str, out T value);
  }
}
