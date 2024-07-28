// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationControllerBase
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ApplyRequestLanguage]
  public abstract class NotificationControllerBase : TfsApiController
  {
    private static readonly IDictionary<Type, HttpStatusCode> s_baseHttpExceptions = (IDictionary<Type, HttpStatusCode>) new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (ArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentNullException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ArgumentOutOfRangeException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (AuthenticationException),
        HttpStatusCode.Unauthorized
      },
      {
        typeof (UnauthorizedAccessException),
        HttpStatusCode.Unauthorized
      }
    };
    private IVssRequestContext m_collectionContext;
    protected static readonly string Area = "Notifications";
    protected static readonly string Layer = "NotificationController";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => NotificationControllerBase.s_baseHttpExceptions;

    public IDictionary<string, object> LoggableDiagnosticParameters { get; } = (IDictionary<string, object>) new Dictionary<string, object>();

    public override Exception TranslateException(Exception ex)
    {
      Type type1 = ex.GetType();
      if (!this.ExceptionMapping.GetStatusCode(type1).HasValue)
      {
        string name = type1.Name;
        string str1 = this.Request.RequestUri.ToString();
        string method = this.Request.Method.Method;
        if (this.LoggableDiagnosticParameters.Any<KeyValuePair<string, object>>())
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (KeyValuePair<string, object> diagnosticParameter in (IEnumerable<KeyValuePair<string, object>>) this.LoggableDiagnosticParameters)
          {
            string key = diagnosticParameter.Key;
            string empty = string.Empty;
            Type type2 = diagnosticParameter.Value.GetType();
            string str2;
            if (!type2.IsPrimitive)
            {
              if (!(type2 == typeof (Guid)))
              {
                try
                {
                  str2 = JsonConvert.SerializeObject(diagnosticParameter.Value, NotificationsSerialization.JsonMinimalSerializerSettings);
                  goto label_9;
                }
                catch (Exception ex1)
                {
                  str2 = ex1.GetType().Name ?? "";
                  goto label_9;
                }
              }
            }
            str2 = diagnosticParameter.Value.ToString();
label_9:
            if (stringBuilder.Length > 0)
              stringBuilder.Append(", ");
            stringBuilder.Append(key + "='" + str2 + "'");
          }
          this.TfsRequestContext.Trace(1002050, TraceLevel.Error, "Notifications", "Controller", name + " " + method + " " + str1 + " " + stringBuilder.ToString());
        }
        else
          this.TfsRequestContext.Trace(1002050, TraceLevel.Error, "Notifications", "Controller", name + " " + method + " " + str1);
      }
      return base.TranslateException(ex);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.m_collectionContext != null)
        this.m_collectionContext.Dispose();
      base.Dispose(disposing);
    }

    public override string ActivityLogArea => "Notification";
  }
}
