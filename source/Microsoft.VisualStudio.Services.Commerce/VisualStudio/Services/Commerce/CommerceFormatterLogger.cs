// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceFormatterLogger
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Net.Http.Formatting;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceFormatterLogger : IFormatterLogger
  {
    private const string Area = "Commerce";
    private const string Layer = "CommerceFormatterLogger";

    internal CommerceFormatterLogger(IVssRequestContext requestContext, int tracePoint)
    {
      this.RequestContext = requestContext;
      this.TracePoint = tracePoint;
    }

    public void LogError(string errorPath, Exception exception) => this.RequestContext.TraceException(this.TracePoint, "Commerce", nameof (CommerceFormatterLogger), exception);

    public void LogError(string errorPath, string errorMessage) => this.RequestContext.Trace(this.TracePoint, TraceLevel.Error, "Commerce", nameof (CommerceFormatterLogger), "An error occurred in a MediaTypeFormatter at location " + errorPath + " with message " + errorMessage);

    private IVssRequestContext RequestContext { get; set; }

    private int TracePoint { get; set; }
  }
}
