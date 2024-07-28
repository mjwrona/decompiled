// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SignInContextFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SignInContextFactory
  {
    private static readonly Regex ClientVersionRegex = new Regex("^[\\sA-Za-z0-9]+/?(\\d{2}(?:.\\d{1,5}){1,5}\\s)?\\(devenv.exe\\s?,\\s?(\\d{2}(?:.\\d{1,5}){1,5})?,?\\s?([A-Za-z]+)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100.0));
    private const string Area = "SignIn";
    private const string Layer = "SignInContextFactory";

    public static SignInContext Construct(IVssRequestContext requestContext)
    {
      IVssRequestContext compatibleRequestContext = SignInContextFactory.GetCompatibleRequestContext(requestContext);
      string validationHash;
      string redirectRealm = SignInContextFactory.GetRedirectRealm(requestContext, out validationHash);
      string clientVersion;
      string clientSku;
      SignInContextFactory.GetClientVersionAndSku(requestContext, out clientVersion, out clientSku);
      return new SignInContext()
      {
        HostType = compatibleRequestContext.ServiceHost.HostType,
        HostId = compatibleRequestContext.ServiceHost.InstanceId,
        QueryString = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
        MsmRedirectHost = redirectRealm,
        ValidationHash = validationHash,
        ClientVersion = clientVersion,
        ClientSku = clientSku
      };
    }

    public static IVssRequestContext GetCompatibleRequestContext(IVssRequestContext requestContext) => !SignInContextFactory.IsCompatibleRequestContext(requestContext) ? requestContext.To(TeamFoundationHostType.Application) : requestContext;

    public static bool IsCompatibleRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IVssServiceHost serviceHost = requestContext.ServiceHost;
      if (serviceHost == null)
        throw new InvalidOperationException(FrameworkResources.NoHostAvailableForRequest());
      return serviceHost.IsOnly(TeamFoundationHostType.Application) || serviceHost.IsOnly(TeamFoundationHostType.Application | TeamFoundationHostType.Deployment);
    }

    private static void GetClientVersionAndSku(
      IVssRequestContext requestContext,
      out string clientVersion,
      out string clientSku)
    {
      clientSku = "";
      clientVersion = "";
      if (string.IsNullOrEmpty(requestContext.UserAgent))
        return;
      try
      {
        Match match = SignInContextFactory.ClientVersionRegex.Match(requestContext.UserAgent);
        if (!match.Success)
          return;
        if (!string.IsNullOrEmpty(match.Groups[1].Value))
          clientVersion = match.Groups[1].Value;
        else if (!string.IsNullOrEmpty(match.Groups[2].Value))
          clientVersion = match.Groups[2].Value;
        if (string.IsNullOrEmpty(match.Groups[3].Value))
          return;
        clientSku = match.Groups[3].Value;
      }
      catch (RegexMatchTimeoutException ex)
      {
        requestContext.Trace(999997, TraceLevel.Error, "SignIn", nameof (SignInContextFactory), "The string '{0}' exceeded the regex timeout of 100 ms.", (object) requestContext.UserAgent);
      }
    }

    public static SignInContext Construct(string context)
    {
      SignInContextFactory.Context context1;
      try
      {
        context1 = JsonConvert.DeserializeObject<SignInContextFactory.Context>(Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(context)));
        if (context1 == null)
          return (SignInContext) null;
      }
      catch
      {
        return (SignInContext) null;
      }
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> keyValuePair in context1.qs.AsEmptyIfNull<Dictionary<string, string>>())
        dictionary.Add(keyValuePair.Key, keyValuePair.Value);
      SignInContext signInContext = new SignInContext()
      {
        HostType = (TeamFoundationHostType) context1.ht,
        QueryString = dictionary,
        MsmRedirectHost = context1.rr,
        ValidationHash = context1.vh,
        ClientVersion = context1.cv,
        ClientSku = context1.cs
      };
      Guid result;
      if (Guid.TryParse(context1.hid, out result))
        signInContext.HostId = result;
      return signInContext;
    }

    public static string Deconstruct(SignInContext context)
    {
      try
      {
        return HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) new SignInContextFactory.Context(context))));
      }
      catch
      {
        return (string) null;
      }
    }

    private static string GetRedirectRealm(
      IVssRequestContext requestContext,
      out string validationHash)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string s1 = vssRequestContext.GetService<ICachedRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) "/OrgId/Authentication/MSMRedirectorKey", false, (string) null);
      string s2 = "";
      validationHash = "";
      if (!string.IsNullOrEmpty(s1))
      {
        Uri uri = new Uri(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker));
        s2 = new UriBuilder(uri.Scheme, uri.Host, uri.Port).Uri.AbsoluteUri;
        byte[] bytes = Encoding.UTF8.GetBytes(s2);
        byte[] hash = new HMACSHA256(Encoding.UTF8.GetBytes(s1)).ComputeHash(bytes);
        validationHash = HttpServerUtility.UrlTokenEncode(hash);
      }
      return s2;
    }

    private class Context
    {
      public int ht { get; set; }

      public string hid { get; set; }

      public Dictionary<string, string> qs { get; set; }

      public string rr { get; set; }

      public string vh { get; set; }

      public string cv { get; set; }

      public string cs { get; set; }

      public Context()
      {
      }

      public Context(SignInContext context)
      {
        this.ht = (int) context.HostType;
        this.hid = context.HostId.ToString();
        this.qs = context.QueryString;
        this.rr = context.MsmRedirectHost;
        this.vh = context.ValidationHash;
        this.cv = context.ClientVersion;
        this.cs = context.ClientSku;
      }
    }
  }
}
