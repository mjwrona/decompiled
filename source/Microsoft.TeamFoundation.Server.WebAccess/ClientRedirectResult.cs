// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ClientRedirectResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security.AntiXss;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ClientRedirectResult : CustomRedirectResult
  {
    public ClientRedirectResult(string redirectAddress)
      : base(redirectAddress, HttpStatusCode.OK)
    {
    }

    protected override void DoRedirect(ControllerContext context, string redirectLocation)
    {
      string s = !context.RequestContext.TfsRequestContext().IsFeatureEnabled("VisualStudio.Services.EnableSafeDeserializer") ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><head><meta http-equiv=\"refresh\" content=\"0; url={0}\"><script type=\"text/javascript\">window.location.replace({1});</script></head></html>", (object) AntiXssEncoder.XmlAttributeEncode(redirectLocation), (object) new JavaScriptSerializer().Serialize((object) redirectLocation)) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "<html><head><meta http-equiv=\"refresh\" content=\"0; url={0}\"><script type=\"text/javascript\">window.location.replace({1});</script></head></html>", (object) AntiXssEncoder.XmlAttributeEncode(redirectLocation), (object) JsonConvert.SerializeObject((object) redirectLocation));
      context.HttpContext.Response.Write(s);
    }
  }
}
