// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Diagnostics, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6A4F2FF9-BE93-434B-9864-FE0D09D21D75
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Diagnostics.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Diagnostics
{
  public static class JsonExtensions
  {
    public static JsObject ToJson(this TraceEvent traceEvent)
    {
      JsObject json = new JsObject();
      json.Add("traceId", (object) traceEvent.TraceId);
      json.Add("serviceHost", traceEvent.ServiceHost != Guid.Empty ? (object) traceEvent.ServiceHost.ToString("D") : (object) (string) null);
      json.Add("tracePoint", (object) traceEvent.Tracepoint);
      json.Add("processName", (object) traceEvent.ProcessName);
      json.Add("userLogin", (object) traceEvent.UserLogin);
      json.Add("service", (object) traceEvent.Service);
      json.Add("method", (object) traceEvent.Method);
      json.Add("area", (object) traceEvent.Area);
      json.Add("level", (object) traceEvent.Level);
      json.Add("userAgent", (object) traceEvent.UserAgent);
      json.Add("layer", (object) traceEvent.Layer);
      json.Add("uri", (object) traceEvent.Uri);
      json.Add("path", (object) traceEvent.Path);
      json.Add("userDefined", traceEvent.Tags == null || traceEvent.Tags.Length == 0 ? (object) (string) null : (object) string.Join(":", traceEvent.Tags));
      json.Add("message", (object) traceEvent.GetMessage());
      json.Add("timeCreated", (object) traceEvent.TimeCreated);
      json.Add("activityId", (object) traceEvent.ActivityId);
      return json;
    }

    public static JsObject ToJson(this TraceFilter traceFilter)
    {
      JsObject json = new JsObject();
      json.Add("traceId", (object) traceFilter.TraceId);
      json.Add("enabled", (object) traceFilter.IsEnabled);
      json.Add("serviceHost", traceFilter.ServiceHost != Guid.Empty ? (object) traceFilter.ServiceHost.ToString("D") : (object) (string) null);
      json.Add("tracePoint", (object) traceFilter.Tracepoint);
      json.Add("processName", (object) traceFilter.ProcessName);
      json.Add("userLogin", (object) traceFilter.UserLogin);
      json.Add("service", (object) traceFilter.Service);
      json.Add("method", (object) traceFilter.Method);
      json.Add("area", (object) traceFilter.Area);
      json.Add("level", (object) traceFilter.Level);
      json.Add("userAgent", (object) traceFilter.UserAgent);
      json.Add("layer", (object) traceFilter.Layer);
      json.Add("uri", (object) traceFilter.Uri);
      json.Add("path", (object) traceFilter.Path);
      json.Add("userDefined", traceFilter.Tags == null || traceFilter.Tags.Length == 0 ? (object) (string) null : (object) string.Join(":", traceFilter.Tags));
      json.Add("owner", traceFilter.Owner != null ? (object) traceFilter.Owner.DisplayName : (object) (string) null);
      json.Add("timeCreated", (object) traceFilter.TimeCreated);
      return json;
    }
  }
}
