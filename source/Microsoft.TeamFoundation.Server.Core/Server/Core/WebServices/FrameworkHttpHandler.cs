// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.FrameworkHttpHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public abstract class FrameworkHttpHandler : TeamFoundationHttpHandler
  {
    protected FrameworkHttpHandler() => this.Initialize();

    protected FrameworkHttpHandler(HttpContextBase context)
      : base(context)
    {
      this.Initialize();
    }

    protected abstract void Execute();

    protected override sealed void ProcessRequestImpl(HttpContext context) => this.Execute();

    private void Initialize() => this.RequestContext.ServiceName = "Framework";

    protected static bool ParseRangeHeader(
      string range,
      out long totalLength,
      out long start,
      out long end)
    {
      int num1 = range.IndexOf('-');
      int num2 = range.IndexOf('/');
      if (range.StartsWith("bytes=", StringComparison.OrdinalIgnoreCase) && num1 > -1)
      {
        if (num2 > num1)
        {
          try
          {
            start = long.Parse(range.Substring(6, num1 - 6), (IFormatProvider) CultureInfo.InvariantCulture);
            end = long.Parse(range.Substring(num1 + 1, num2 - num1 - 1), (IFormatProvider) CultureInfo.InvariantCulture);
            totalLength = long.Parse(range.Substring(num2 + 1), (IFormatProvider) CultureInfo.InvariantCulture);
            return true;
          }
          catch (ArgumentOutOfRangeException ex)
          {
          }
          catch (FormatException ex)
          {
          }
        }
      }
      totalLength = 0L;
      start = 0L;
      end = 0L;
      return false;
    }
  }
}
