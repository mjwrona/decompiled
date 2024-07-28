// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.TFLogger
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Common
{
  public abstract class TFLogger : MarshalByRefObject, ITFLogger
  {
    public abstract void Info(string message);

    public void Info(string message, params object[] args) => this.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public abstract void Warning(string message);

    public void Warning(string message, params object[] args) => this.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public void Warning(Exception exception)
    {
      if (exception == null)
        return;
      this.Warning(TeamFoundationExceptionFormatter.FormatException(exception, false));
    }

    public abstract void Error(string message);

    public void Error(string message, params object[] args) => this.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public void Error(Exception exception)
    {
      if (exception == null)
        return;
      this.Error(TeamFoundationExceptionFormatter.FormatException(exception, false));
    }
  }
}
