// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingLogger
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingLogger : ITFLogger
  {
    private IServicingContext m_context;

    public ServicingLogger(IServicingContext context)
    {
      ArgumentUtility.CheckForNull<IServicingContext>(context, nameof (context));
      this.m_context = context;
    }

    public void Info(string message) => this.m_context.LogInfo(message);

    public void Info(string message, params object[] args) => this.Info(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public void Warning(string message) => this.m_context.Warn(message);

    public void Warning(string message, params object[] args) => this.Warning(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public void Warning(Exception exception)
    {
      if (exception == null)
        return;
      this.Warning(TeamFoundationExceptionFormatter.FormatException(exception, false));
    }

    public void Error(string message) => this.m_context.Error(message);

    public void Error(string message, params object[] args) => this.Error(string.Format((IFormatProvider) CultureInfo.InvariantCulture, message, args));

    public void Error(Exception exception)
    {
      if (exception == null)
        return;
      this.Error(TeamFoundationExceptionFormatter.FormatException(exception, false));
    }
  }
}
