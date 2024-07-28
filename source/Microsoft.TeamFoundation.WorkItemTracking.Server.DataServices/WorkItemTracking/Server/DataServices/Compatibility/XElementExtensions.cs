// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility.XElementExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using System;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.Compatibility
{
  internal static class XElementExtensions
  {
    private static ulong GetXmlPosition(this XElement element)
    {
      IXmlLineInfo xmlLineInfo = element != null ? (IXmlLineInfo) element : throw new ArgumentNullException(nameof (element));
      if (xmlLineInfo == null)
        throw new InvalidCastException();
      if (!xmlLineInfo.HasLineInfo())
        throw new InvalidOperationException();
      return (ulong) xmlLineInfo.LineNumber << 32 | (ulong) (uint) xmlLineInfo.LinePosition;
    }

    public static string GetCorrelationId(this XElement element) => element != null ? element.GetXmlPosition().ToString("x16", (IFormatProvider) CultureInfo.InvariantCulture) : throw new ArgumentNullException(nameof (element));
  }
}
