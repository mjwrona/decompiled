// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContentSecurityPolicyNonceManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ContentSecurityPolicyNonceManagementService : 
    IContentSecurityPolicyNonceManagementService,
    IVssFrameworkService
  {
    private static readonly RNGCryptoServiceProvider s_rngCryptoServiceProvider = new RNGCryptoServiceProvider();
    private const string c_requestContextNonceValueKey = "$vssCspNonceValue";
    private const string c_area = "ContentSecurityPolicy";
    private const string c_layer = "NonceService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15140000, "ContentSecurityPolicy", "NonceService", nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      systemRequestContext.TraceLeave(15140001, "ContentSecurityPolicy", "NonceService", nameof (ServiceStart));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(15140002, "ContentSecurityPolicy", "NonceService", nameof (ServiceEnd));
      systemRequestContext.TraceLeave(15140003, "ContentSecurityPolicy", "NonceService", nameof (ServiceEnd));
    }

    public string GetNonceValue(IVssRequestContext requestContext, HttpContextBase httpContext)
    {
      requestContext.TraceEnter(15140010, "ContentSecurityPolicy", "NonceService", nameof (GetNonceValue));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        string empty = string.Empty;
        object obj = httpContext.Items[(object) "$vssCspNonceValue"];
        string nonceValue;
        if (obj != null)
        {
          nonceValue = obj as string;
          requestContext.Trace(15140015, TraceLevel.Info, "ContentSecurityPolicy", "NonceService", string.Format("Found nonce {0} in HttpContext.", (object) nonceValue));
        }
        else
        {
          byte[] numArray = new byte[16];
          ContentSecurityPolicyNonceManagementService.s_rngCryptoServiceProvider.GetNonZeroBytes(numArray);
          nonceValue = Convert.ToBase64String(numArray);
          httpContext.Items[(object) "$vssCspNonceValue"] = (object) nonceValue;
          requestContext.Trace(15140020, TraceLevel.Info, "ContentSecurityPolicy", "NonceService", string.Format("Generated nonce {0}.", (object) nonceValue));
        }
        return nonceValue;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15140025, "ContentSecurityPolicy", "NonceService", ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(15140030, "ContentSecurityPolicy", "NonceService", nameof (GetNonceValue));
      }
    }
  }
}
