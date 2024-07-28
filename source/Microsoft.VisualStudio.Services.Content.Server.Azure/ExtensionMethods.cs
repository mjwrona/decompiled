// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.ExtensionMethods
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class ExtensionMethods
  {
    public const int MaxBlobNameLength = 1024;
    private static readonly Regex AlphaNumericCharactersExpression = new Regex("[a-zA-Z0-9]+", RegexOptions.Compiled);
    private static Regex rfc5646 = new Regex("^(([A-Za-z]{2,3}(\\-[A-Za-z]{3,3}(\\-[A-Za-z]{3,3}){,2}){0,1}|[A-Za-z]{4,4}|[A-Za-z]{5,8})(\\-[A-Za-z]{4,4}){0,1}(\\-([A-Za-z]{2,2}|[0-9]{3,3})){0,1}(\\-([A-Za-z0-9]{5,8}|([0-9][A-Za-z0-9]{3,3})))*(\\-[0-9A-WY-Za-wy-z](\\-([A-Za-z0-9]{2,8}))+)*(\\-x(\\-([A-Za-z0-9]{1,8}))+){0,1}|x(\\-([A-Za-z0-9]{1,8}))+|((en\\-gb\\-oed|i\\-ami|i\\-bnn|i\\-default|i\\-enochian|i\\-hak|i\\-klingon|i\\-lux|i\\-mingo|i\\-navajo|i\\-pwn|i\\-tao|i\\-tay|i\\-tsu|sgn\\-be\\-fr|sgn\\-be\\-nl|sgn\\-ch\\-de)|(art\\-lojban|cel\\-gaulish|no\\-bok|no\\-nyn|zh\\-guoyu|zh\\-hakka|zh\\-min|zh\\-min\\-nan|zh\\-xiang)))$", RegexOptions.Compiled);

    public static bool ContainsStorageExceptionWithHttpStatus(
      this Exception exc,
      params HttpStatusCode[] statusCodes)
    {
      AggregateException exc1 = exc as AggregateException;
      Microsoft.Azure.Storage.StorageException exc2 = exc as Microsoft.Azure.Storage.StorageException;
      if (exc1 != null)
        return ExtensionMethods.ContainsStorageExceptionWithHttpStatus(exc1, statusCodes);
      return exc2 != null && exc2.HasHttpStatus(statusCodes);
    }

    public static bool ContainsStorageExceptionWithHttpStatus(
      this AggregateException exc,
      params HttpStatusCode[] statusCodes)
    {
      return exc != null && exc.InnerExceptions.OfType<Microsoft.Azure.Storage.StorageException>().Any<Microsoft.Azure.Storage.StorageException>((Func<Microsoft.Azure.Storage.StorageException, bool>) (se => se.HasHttpStatus(statusCodes)));
    }

    public static bool HasHttpStatus(this Microsoft.Azure.Storage.StorageException exc, params HttpStatusCode[] statusCodes) => exc != null && statusCodes != null && ((IEnumerable<HttpStatusCode>) statusCodes).Any<HttpStatusCode>((Func<HttpStatusCode, bool>) (statusCode => exc.RequestInformation?.HttpStatusCode.Equals((int) statusCode).GetValueOrDefault(false)));

    public static bool HasHttpStatus(this Microsoft.Azure.Storage.StorageException exc, HttpStatusCode statusCode)
    {
      int? httpStatusCode = exc?.RequestInformation?.HttpStatusCode;
      int num = (int) statusCode;
      return httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue;
    }

    public static bool HasHttpStatus(this Microsoft.Azure.Cosmos.Table.StorageException exc, params HttpStatusCode[] statusCodes) => exc != null && statusCodes != null && ((IEnumerable<HttpStatusCode>) statusCodes).Any<HttpStatusCode>((Func<HttpStatusCode, bool>) (statusCode => exc.RequestInformation?.HttpStatusCode.Equals((int) statusCode).GetValueOrDefault(false)));

    public static bool HasHttpStatus(this Microsoft.Azure.Cosmos.Table.StorageException exc, HttpStatusCode statusCode)
    {
      int? httpStatusCode = exc?.RequestInformation?.HttpStatusCode;
      int num = (int) statusCode;
      return httpStatusCode.GetValueOrDefault() == num & httpStatusCode.HasValue;
    }

    public static string ParseAlphaNumericCharacters(this string value)
    {
      StringBuilder stringBuilder = (StringBuilder) null;
      if (!string.IsNullOrWhiteSpace(value))
      {
        MatchCollection matchCollection = ExtensionMethods.AlphaNumericCharactersExpression.Matches(value);
        if (matchCollection != null && matchCollection.Count > 0)
        {
          stringBuilder = new StringBuilder();
          foreach (Match match in matchCollection)
            stringBuilder.Append(match.Value);
        }
      }
      return stringBuilder?.ToString();
    }

    public static string ConvertToAzureCompatibleString(this Guid guid) => guid.ToString("N");

    public static void AssertContentLanguageFormat(string contentLanguage)
    {
      if (!ExtensionMethods.rfc5646.IsMatch(contentLanguage))
        throw new FormatException("'" + contentLanguage + "' does not match RFC-5646.");
    }

    public static string GetContentLanguageCompatibleSegment(
      this Guid guid,
      string alphanumPrefixOneToEightChars)
    {
      StringBuilder b = new StringBuilder();
      b.AppendContentLanguageCompatibleSegment(alphanumPrefixOneToEightChars, guid);
      return b.ToString();
    }

    public static void AppendContentLanguageCompatibleSegment(
      this StringBuilder b,
      string alphanumPrefixOneToEightChars,
      Guid guid)
    {
      b.Append(alphanumPrefixOneToEightChars);
      foreach (List<char> page in ((IEnumerable<char>) guid.ConvertToAzureCompatibleString().ToCharArray()).GetPages<char>(8))
      {
        b.Append('-');
        foreach (char ch in page)
          b.Append(ch);
      }
    }

    public static void ApplyTracingGuids(
      this SharedAccessBlobHeaders headers,
      params (string, Guid)[] tracingGuids)
    {
      if (tracingGuids.Length == 0)
        return;
      StringBuilder b = new StringBuilder();
      b.Append("x");
      foreach ((string, Guid) tracingGuid in tracingGuids)
      {
        b.Append("-");
        b.AppendContentLanguageCompatibleSegment(tracingGuid.Item1, tracingGuid.Item2);
      }
      string contentLanguage = b.ToString();
      ExtensionMethods.AssertContentLanguageFormat(contentLanguage);
      headers.ContentLanguage = contentLanguage;
    }

    public static string GetAccountName(this CloudBlobContainer container) => container.ServiceClient?.BaseUri?.Host;

    public static string GetTableName(
      this IVssRequestContext requestContext,
      string prefix,
      bool isHosted)
    {
      return string.Format("{0}{1}", (object) prefix, isHosted ? (object) requestContext.ServiceHost.InstanceId.ConvertToAzureCompatibleString() : (object) string.Empty);
    }
  }
}
