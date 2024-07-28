// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PlatformJsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;
using System.Web.Configuration;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class PlatformJsonExtensions
  {
    private static readonly bool s_isHosted;

    static PlatformJsonExtensions()
    {
      try
      {
        string appSetting = WebConfigurationManager.AppSettings["IsHosted"];
        PlatformJsonExtensions.s_isHosted = !string.IsNullOrEmpty(appSetting) && string.Equals(appSetting, "true", StringComparison.OrdinalIgnoreCase);
      }
      catch
      {
      }
    }

    public static JsObject ToServiceHostJson(this IVssRequestContext requestContext)
    {
      if (requestContext == null)
        return (JsObject) null;
      JsObject serviceHostJson = new JsObject();
      serviceHostJson.Add("instanceId", (object) requestContext.ServiceHost.InstanceId);
      serviceHostJson.Add("name", (object) requestContext.ServiceHost.Name);
      serviceHostJson.Add("hostType", (object) requestContext.ServiceHost.HostType);
      serviceHostJson.Add("vDir", (object) requestContext.VirtualPath());
      serviceHostJson.Add("relVDir", (object) requestContext.TrimmedVirtualDirectory());
      return serviceHostJson;
    }

    public static JsObject ToJson(this Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (identity == null)
        return (JsObject) null;
      JsObject json = new JsObject();
      json.Add("id", (object) identity.Id);
      json.Add("isContainer", (object) identity.IsContainer);
      json.Add("isActive", (object) identity.IsActive);
      json.Add("displayName", (object) identity.DisplayName);
      json.Add("customDisplayName", (object) identity.CustomDisplayName);
      json.Add("providerDisplayName", (object) identity.ProviderDisplayName);
      json.Add("uniqueName", (object) IdentityHelper.GetUniqueName(identity));
      json.Add("email", (object) identity.GetProperty<string>("Mail", string.Empty));
      return json;
    }

    public static JsObject ToJson(this Exception exception) => exception.ToJson(true);

    public static JsObject ToJson(this Exception exception, bool details)
    {
      bool stackTrace = false;
      HttpContext current = HttpContext.Current;
      if (current != null)
        stackTrace = current.IsDebuggingEnabled;
      return exception.ToJson(details, stackTrace);
    }

    public static JsObject ToJson(this Exception exception, bool details, bool stackTrace)
    {
      if (PlatformJsonExtensions.s_isHosted)
        stackTrace = false;
      JsObject json = new JsObject();
      json["message"] = (object) UserFriendlyError.GetMessageFromException(exception);
      json["type"] = (object) exception.GetType().FullName;
      if (details)
      {
        if (exception is TeamFoundationServerException foundationServerException)
          json["isRemoteException"] = (object) foundationServerException.IsRemoteException;
        if (exception is TeamFoundationServiceException serviceException)
        {
          json["errorCode"] = (object) serviceException.ErrorCode;
          json["eventId"] = (object) serviceException.EventId;
        }
        if (exception is SoapException soapException)
        {
          json["actor"] = (object) soapException.Actor;
          json["code"] = (object) soapException.Code;
          json["detail"] = (object) soapException.Detail;
          json["lang"] = (object) soapException.Lang;
          json["node"] = (object) soapException.Node;
          json["role"] = (object) soapException.Role;
          if (soapException.SubCode != null)
            json["subCode"] = (object) soapException.SubCode.ToJson();
        }
        if (stackTrace)
          json["stack"] = (object) exception.StackTrace;
        if (exception.InnerException != null)
          json["innerException"] = (object) exception.InnerException.ToJson(details, false);
      }
      return json;
    }

    public static JsObject ToJson(this SoapFaultSubCode faultSubCode)
    {
      JsObject json = new JsObject();
      json["code"] = (object) faultSubCode.Code;
      if (faultSubCode.SubCode != null)
        json["subCode"] = (object) faultSubCode.SubCode.ToJson();
      return json;
    }
  }
}
