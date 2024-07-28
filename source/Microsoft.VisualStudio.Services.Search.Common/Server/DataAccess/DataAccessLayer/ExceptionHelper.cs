// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.ExceptionHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public static class ExceptionHelper
  {
    public static string ExceptionToString(Exception ex)
    {
      if (ex == null)
        throw new ArgumentNullException(nameof (ex));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception message -----> {0}", (object) ex.Message));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception type -----> {0}", (object) ex.GetType()));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception source -----> {0}", (object) ex.Source));
      stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Stack trace,\r\n{0}", (object) ex.StackTrace.Indent(3)));
      if (ex.InnerException != null)
      {
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "INNER EXCEPTION,\r\n{0}", (object) ex.InnerException.ToString().Indent(6)));
      }
      return stringBuilder.ToString();
    }
  }
}
