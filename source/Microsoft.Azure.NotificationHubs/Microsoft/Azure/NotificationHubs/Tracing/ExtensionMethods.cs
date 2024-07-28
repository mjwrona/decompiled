// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.ExtensionMethods
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal static class ExtensionMethods
  {
    private const string ExceptionIdentifierName = "ExceptionId";

    public static int ToStringLength(this object param) => param == null ? 0 : param.ToString().Length;

    public static string ToStringSlim(this Exception exception)
    {
      if (exception.Data != null && exception.Data.Contains((object) "ExceptionId"))
        return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExceptionId: {0}-{1}: {2}", new object[3]
        {
          exception.Data[(object) "ExceptionId"],
          (object) exception.GetType(),
          (object) exception.Message
        });
      if (exception.Data == null)
        return exception.ToString();
      string str = Guid.NewGuid().ToString();
      exception.Data[(object) "ExceptionId"] = (object) str;
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExceptionId: {0}-{1}", new object[2]
      {
        (object) str,
        (object) exception.ToString()
      });
    }
  }
}
