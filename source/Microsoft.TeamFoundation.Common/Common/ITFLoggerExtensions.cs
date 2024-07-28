// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ITFLoggerExtensions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Common
{
  public static class ITFLoggerExtensions
  {
    public static void Heading(this ITFLogger logger, string message)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info(string.Empty);
      logger.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
      logger.Info(message);
      logger.Info("-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+");
    }

    public static void Heading(this ITFLogger logger, string message, params object[] args) => logger.Heading(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public static void Heading2(this ITFLogger logger, string message)
    {
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info(string.Empty);
      logger.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "+-+-+-+-+-| {0} |+-+-+-+-+-", (object) message));
    }

    public static void Heading2(this ITFLogger logger, string message, params object[] args) => logger.Heading2(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));
  }
}
