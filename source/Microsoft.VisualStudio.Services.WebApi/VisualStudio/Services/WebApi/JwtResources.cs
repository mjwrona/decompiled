// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.JwtResources
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.VisualStudio.Services.WebApi
{
  internal static class JwtResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (JwtResources), typeof (JwtResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => JwtResources.s_resMgr;

    private static string Get(string resourceName) => JwtResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? JwtResources.Get(resourceName) : JwtResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) JwtResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? JwtResources.GetInt(resourceName) : (int) JwtResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) JwtResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? JwtResources.GetBool(resourceName) : (bool) JwtResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => JwtResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = JwtResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string ActorValidationException() => JwtResources.Get(nameof (ActorValidationException));

    public static string ActorValidationException(CultureInfo culture) => JwtResources.Get(nameof (ActorValidationException), culture);

    public static string DeserializationException() => JwtResources.Get(nameof (DeserializationException));

    public static string DeserializationException(CultureInfo culture) => JwtResources.Get(nameof (DeserializationException), culture);

    public static string DigestUnsupportedException(object arg0, object arg1) => JwtResources.Format(nameof (DigestUnsupportedException), arg0, arg1);

    public static string DigestUnsupportedException(object arg0, object arg1, CultureInfo culture) => JwtResources.Format(nameof (DigestUnsupportedException), culture, arg0, arg1);

    public static string EncodedTokenDataMalformed() => JwtResources.Get(nameof (EncodedTokenDataMalformed));

    public static string EncodedTokenDataMalformed(CultureInfo culture) => JwtResources.Get(nameof (EncodedTokenDataMalformed), culture);

    public static string InvalidAudienceException() => JwtResources.Get(nameof (InvalidAudienceException));

    public static string InvalidAudienceException(CultureInfo culture) => JwtResources.Get(nameof (InvalidAudienceException), culture);

    public static string InvalidClockSkewException() => JwtResources.Get(nameof (InvalidClockSkewException));

    public static string InvalidClockSkewException(CultureInfo culture) => JwtResources.Get(nameof (InvalidClockSkewException), culture);

    public static string InvalidIssuerException() => JwtResources.Get(nameof (InvalidIssuerException));

    public static string InvalidIssuerException(CultureInfo culture) => JwtResources.Get(nameof (InvalidIssuerException), culture);

    public static string InvalidSignatureAlgorithm() => JwtResources.Get(nameof (InvalidSignatureAlgorithm));

    public static string InvalidSignatureAlgorithm(CultureInfo culture) => JwtResources.Get(nameof (InvalidSignatureAlgorithm), culture);

    public static string InvalidValidFromValueException() => JwtResources.Get(nameof (InvalidValidFromValueException));

    public static string InvalidValidFromValueException(CultureInfo culture) => JwtResources.Get(nameof (InvalidValidFromValueException), culture);

    public static string InvalidValidToValueException() => JwtResources.Get(nameof (InvalidValidToValueException));

    public static string InvalidValidToValueException(CultureInfo culture) => JwtResources.Get(nameof (InvalidValidToValueException), culture);

    public static string ProviderTypeUnsupported(object arg0) => JwtResources.Format(nameof (ProviderTypeUnsupported), arg0);

    public static string ProviderTypeUnsupported(object arg0, CultureInfo culture) => JwtResources.Format(nameof (ProviderTypeUnsupported), culture, arg0);

    public static string SerializationException() => JwtResources.Get(nameof (SerializationException));

    public static string SerializationException(CultureInfo culture) => JwtResources.Get(nameof (SerializationException), culture);

    public static string SignatureAlgorithmUnsupportedException(object arg0) => JwtResources.Format(nameof (SignatureAlgorithmUnsupportedException), arg0);

    public static string SignatureAlgorithmUnsupportedException(object arg0, CultureInfo culture) => JwtResources.Format(nameof (SignatureAlgorithmUnsupportedException), culture, arg0);

    public static string SignatureNotFound() => JwtResources.Get(nameof (SignatureNotFound));

    public static string SignatureNotFound(CultureInfo culture) => JwtResources.Get(nameof (SignatureNotFound), culture);

    public static string SignatureValidationException() => JwtResources.Get(nameof (SignatureValidationException));

    public static string SignatureValidationException(CultureInfo culture) => JwtResources.Get(nameof (SignatureValidationException), culture);

    public static string SymmetricSecurityKeyNotFound() => JwtResources.Get(nameof (SymmetricSecurityKeyNotFound));

    public static string SymmetricSecurityKeyNotFound(CultureInfo culture) => JwtResources.Get(nameof (SymmetricSecurityKeyNotFound), culture);

    public static string TokenExpiredException() => JwtResources.Get(nameof (TokenExpiredException));

    public static string TokenExpiredException(CultureInfo culture) => JwtResources.Get(nameof (TokenExpiredException), culture);

    public static string TokenNotYetValidException() => JwtResources.Get(nameof (TokenNotYetValidException));

    public static string TokenNotYetValidException(CultureInfo culture) => JwtResources.Get(nameof (TokenNotYetValidException), culture);

    public static string ValidFromAfterValidToException() => JwtResources.Get(nameof (ValidFromAfterValidToException));

    public static string ValidFromAfterValidToException(CultureInfo culture) => JwtResources.Get(nameof (ValidFromAfterValidToException), culture);

    public static string SigningTokenExpired() => JwtResources.Get(nameof (SigningTokenExpired));

    public static string SigningTokenExpired(CultureInfo culture) => JwtResources.Get(nameof (SigningTokenExpired), culture);

    public static string SigningTokenNoPrivateKey() => JwtResources.Get(nameof (SigningTokenNoPrivateKey));

    public static string SigningTokenNoPrivateKey(CultureInfo culture) => JwtResources.Get(nameof (SigningTokenNoPrivateKey), culture);

    public static string SigningTokenKeyTooSmall() => JwtResources.Get(nameof (SigningTokenKeyTooSmall));

    public static string SigningTokenKeyTooSmall(CultureInfo culture) => JwtResources.Get(nameof (SigningTokenKeyTooSmall), culture);

    public static string TokenScopeNotAuthorizedException() => JwtResources.Get(nameof (TokenScopeNotAuthorizedException));

    public static string TokenScopeNotAuthorizedException(CultureInfo culture) => JwtResources.Get(nameof (TokenScopeNotAuthorizedException), culture);
  }
}
