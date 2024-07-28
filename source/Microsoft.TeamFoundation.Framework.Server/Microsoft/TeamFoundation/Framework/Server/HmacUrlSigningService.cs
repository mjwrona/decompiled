// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HmacUrlSigningService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class HmacUrlSigningService : IUrlSigningService, IVssFrameworkService
  {
    private static readonly TimeSpan m_maxTTL = TimeSpan.FromDays(14.0);
    private static readonly string m_primaryHmacSecret = "UrlSigningHmacKeyPrimary";
    private static readonly string m_secondaryHmacSecret = "UrlSigningHmacKeySecondary";
    private const string Area = "UrlSigning";
    private const string Layer = "HmacUrlSigningService";

    public UrlSigningAlgorithm SigningAlgorithm => UrlSigningAlgorithm.HMACV1;

    public TimeSpan MaxTimeToLive => HmacUrlSigningService.m_maxTTL;

    public string Sign(IVssRequestContext requestContext, Uri uri, TimeSpan timeToLive)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      if (timeToLive > HmacUrlSigningService.m_maxTTL)
        throw new ArgumentOutOfRangeException("TTL exceeds max TTL allowed for this UrlSigningProvider");
      DateTime expiresAt = DateTime.UtcNow + timeToLive;
      return this.GetAbsoluteEscapedUri(this.GetSignedUri(requestContext, uri, expiresAt));
    }

    public string Sign(IVssRequestContext requestContext, Uri uri, DateTime expires)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      DateTime universalTime = expires.ToUniversalTime();
      if (universalTime > DateTime.UtcNow + HmacUrlSigningService.m_maxTTL)
        throw new ArgumentOutOfRangeException("TTL exceeds max TTL allowed for this UrlSigningProvider");
      return this.GetAbsoluteEscapedUri(this.GetSignedUri(requestContext, uri, universalTime));
    }

    internal UriBuilder GetSignedUri(
      IVssRequestContext requestContext,
      Uri uri,
      DateTime expiresAt)
    {
      requestContext.Trace(2100000, TraceLevel.Info, "UrlSigning", nameof (HmacUrlSigningService), "Signing " + uri.AbsoluteUri + ", expires at: " + expiresAt.ToString("o"));
      NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
      queryString.Remove(UrlSigningConstants.Expires);
      queryString.Remove(UrlSigningConstants.SigningMethod);
      queryString.Remove(UrlSigningConstants.Signature);
      string str = expiresAt.ToString("o");
      queryString.Add(UrlSigningConstants.Expires, str);
      queryString.Add(UrlSigningConstants.SigningMethod, this.SigningAlgorithm.ToString());
      UriBuilder uriBuilder = this.GetUriBuilder(requestContext, uri, queryString);
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(this.GetAbsoluteEscapedUri(uriBuilder), this.GetKey(requestContext, HmacUrlSigningService.m_primaryHmacSecret)))
        uriBuilder.AppendQuery(UrlSigningConstants.Signature, hmacshA256Hash.HashBase64Encoded);
      return uriBuilder;
    }

    public UrlValidationResult Validate(
      IVssRequestContext requestContext,
      Uri uri,
      bool throwIfNotValid)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      NameValueCollection queryString = HttpUtility.ParseQueryString(uri.Query);
      string s1 = queryString[UrlSigningConstants.Signature];
      string s2 = queryString[UrlSigningConstants.Expires];
      string a = queryString[UrlSigningConstants.SigningMethod];
      if (string.IsNullOrEmpty(s1))
        return this.HandleFailure(UrlValidationFailureReason.MissingRequiredParameter, FrameworkResources.MissingParameter((object) UrlSigningConstants.Signature), throwIfNotValid);
      if (string.IsNullOrEmpty(s2))
        return this.HandleFailure(UrlValidationFailureReason.MissingRequiredParameter, FrameworkResources.MissingParameter((object) UrlSigningConstants.Expires), throwIfNotValid);
      if (string.IsNullOrEmpty(a))
        return this.HandleFailure(UrlValidationFailureReason.MissingRequiredParameter, FrameworkResources.MissingParameter((object) UrlSigningConstants.SigningMethod), throwIfNotValid);
      if (!string.Equals(a, this.SigningAlgorithm.ToString(), StringComparison.Ordinal))
        return this.HandleFailure(UrlValidationFailureReason.SignedByOthers, FrameworkResources.NotSignedByMe(), throwIfNotValid);
      DateTime result;
      if (!DateTime.TryParse(s2, (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
        return this.HandleFailure(UrlValidationFailureReason.InvalidParameter, FrameworkResources.InvalidParameter((object) UrlSigningConstants.Expires), throwIfNotValid);
      if (result.ToUniversalTime() < DateTime.UtcNow)
        return this.HandleFailure(UrlValidationFailureReason.Expired, FrameworkResources.SignedUriExpired(), throwIfNotValid);
      byte[] lhs;
      try
      {
        lhs = Convert.FromBase64String(s1);
      }
      catch (FormatException ex)
      {
        requestContext.TraceException(0, "UrlSigning", nameof (HmacUrlSigningService), (Exception) ex);
        return this.HandleFailure(UrlValidationFailureReason.InvalidParameter, FrameworkResources.InvalidParameter((object) UrlSigningConstants.Signature), throwIfNotValid);
      }
      queryString.Remove(UrlSigningConstants.Signature);
      string absoluteEscapedUri = this.GetAbsoluteEscapedUri(this.GetUriBuilder(requestContext, uri, queryString));
      byte[] key1 = this.GetKey(requestContext, HmacUrlSigningService.m_primaryHmacSecret);
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(absoluteEscapedUri, key1))
      {
        if (SecureCompare.TimeInvariantEquals(lhs, hmacshA256Hash.Hash))
          return UrlValidationResult.Success();
      }
      requestContext.TraceAlways(0, TraceLevel.Info, "UrlSigning", nameof (HmacUrlSigningService), "Url validation failed with primary key, trying secondary hash key.");
      byte[] key2 = this.GetKey(requestContext, HmacUrlSigningService.m_secondaryHmacSecret);
      using (HMACSHA256Hash hmacshA256Hash = new HMACSHA256Hash(absoluteEscapedUri, key2))
      {
        if (SecureCompare.TimeInvariantEquals(lhs, hmacshA256Hash.Hash))
        {
          requestContext.TraceAlways(0, TraceLevel.Info, "UrlSigning", nameof (HmacUrlSigningService), "Validation with secondary hash key succeeded.");
          return UrlValidationResult.Success();
        }
      }
      return this.HandleFailure(UrlValidationFailureReason.InvalidSignature, FrameworkResources.SignedUrlSignatureNotMatch(), throwIfNotValid);
    }

    internal string GetAbsoluteEscapedUri(UriBuilder uriBuilder) => uriBuilder.Uri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);

    private UriBuilder GetUriBuilder(
      IVssRequestContext requestContext,
      Uri uri,
      NameValueCollection queryParams)
    {
      UriBuilder uriBuilder = new UriBuilder(uri);
      uriBuilder.Query = string.Empty;
      foreach (string name in (IEnumerable<string>) ((IEnumerable<string>) queryParams.AllKeys).OrderBy<string, string>((Func<string, string>) (k => k)))
      {
        if (name == string.Empty)
          throw new ArgumentException("Query parameter with empty key, e.g. '?=foo', is not allowed.");
        if (name == null)
          uriBuilder.AppendQueryValueOnly(queryParams[name]);
        else
          uriBuilder.AppendQuery(name, queryParams[name]);
      }
      return uriBuilder;
    }

    private byte[] GetKey(IVssRequestContext requestContext, string keyName)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate().To(TeamFoundationHostType.Deployment);
      ITeamFoundationStrongBoxService service = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service.GetItemInfo(vssRequestContext, "ConfigurationSecrets", keyName, true);
      return Convert.FromBase64String(service.GetString(vssRequestContext, itemInfo));
    }

    private UrlValidationResult HandleFailure(
      UrlValidationFailureReason reason,
      string errorMessage,
      bool shouldThrow)
    {
      if (shouldThrow)
        throw new UrlSigningValidationException(errorMessage);
      return UrlValidationResult.Failure(reason, errorMessage);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
