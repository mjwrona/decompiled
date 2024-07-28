// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.ExceptionsUtilities
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class ExceptionsUtilities
  {
    public static string GetAllInnerExceptionsMessages(AggregateException ex)
    {
      if (ex == null)
        return string.Empty;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Exception innerException in ex.InnerExceptions)
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0}\r\n", (object) innerException.Message);
      return stringBuilder.ToString();
    }
  }
}
