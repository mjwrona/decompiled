// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.ResourceProviderUtility
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider
{
  [ExcludeFromCodeCoverage]
  internal static class ResourceProviderUtility
  {
    public static TimeSpan ParseTimeSpan(string value)
    {
      TimeSpan result = new TimeSpan();
      if (!string.IsNullOrEmpty(value) && !TimeSpan.TryParse(value, (IFormatProvider) CultureInfo.InvariantCulture, out result))
        result = XmlConvert.ToTimeSpan(value);
      return result;
    }
  }
}
