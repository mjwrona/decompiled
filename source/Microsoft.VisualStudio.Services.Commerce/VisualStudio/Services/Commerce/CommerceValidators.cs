// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceValidators
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class CommerceValidators
  {
    private const string Area = "Commerce";
    private const string Layer = "CommerceValidators";

    public static void CheckForMissingContentType(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      if (request.Content.Headers == null || request.Content.Headers.ContentType == null)
      {
        string str1 = (string) null;
        string str2 = (string) null;
        try
        {
          str1 = CommerceCommons.GetRdfeOperationId(request);
        }
        catch (Exception ex)
        {
        }
        try
        {
          str2 = request.Content.ReadAsStringAsync().Result;
        }
        catch (Exception ex)
        {
        }
        string str3 = str1 ?? "(Not Available)";
        string str4 = str2 ?? "(Not Available)";
        requestContext.Trace(5105304, TraceLevel.Error, "Commerce", nameof (CommerceValidators), "Incoming request with RDFE operation id " + str3 + " does not have content-type header for request content \"" + str4 + "\"");
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = "Request missing Content-Type header, can not deserialize request message content if type unknown."
        });
      }
    }

    public static void CheckResourceTypeIsAccount(
      IVssRequestContext requestContext,
      string resourceType,
      bool strictlyEnforced = false)
    {
      if (string.Compare("account", resourceType, StringComparison.InvariantCultureIgnoreCase) == 0)
        return;
      resourceType = "account";
      requestContext.Trace(5105305, TraceLevel.Error, "Commerce", nameof (CommerceValidators), "Incoming request with RDFE operation id (Not Available) has incorrect resource type " + resourceType + ", expected account");
      if (strictlyEnforced)
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = "Resource type supplied failed validation, resource type given \"{0}\" is invalid."
        });
    }
  }
}
