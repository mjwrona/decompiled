// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ErrorEventModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ErrorEventModule : IHttpModule
  {
    private static readonly string s_Area = "EarlyErrorEventModule";
    private static readonly string s_Layer = "WebServices";

    public void Init(HttpApplication context) => context.BeginRequest += new EventHandler(this.Module_BeginRequest);

    protected void Module_BeginRequest(object sender, EventArgs e)
    {
      TeamFoundationTracingService.TraceEnterRaw(60075, ErrorEventModule.s_Area, ErrorEventModule.s_Layer, nameof (Module_BeginRequest), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
      try
      {
        new HttpApplicationWrapper((HttpApplication) sender).Context.Items[(object) "OnErrorFormatEvent"] = (object) this.OnFormatError;
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(60076, ErrorEventModule.s_Area, ErrorEventModule.s_Layer, nameof (Module_BeginRequest));
      }
    }

    public event ErrorFormatterDelegate OnFormatError;

    public void Dispose() => this.OnFormatError = (ErrorFormatterDelegate) null;
  }
}
