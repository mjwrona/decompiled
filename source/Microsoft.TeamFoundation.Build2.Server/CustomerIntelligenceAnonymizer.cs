// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CustomerIntelligenceAnonymizer
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class CustomerIntelligenceAnonymizer
  {
    public static string AnonymizeRepositoryUri(Uri uri, bool createHash = false)
    {
      if (uri == (Uri) null || !uri.IsAbsoluteUri)
        return (string) null;
      UriBuilder uriBuilder = new UriBuilder(uri)
      {
        UserName = (string) null,
        Password = (string) null
      };
      return !createHash ? uriBuilder.AbsoluteUri() : CustomerIntelligenceAnonymizer.HashUriString(uriBuilder.AbsoluteUri());
    }

    public static string AnonymizeRepositoryUri(string uri, bool createHash = false)
    {
      Uri result;
      if (!string.IsNullOrEmpty(uri) && Uri.TryCreate(uri, UriKind.Absolute, out result))
        return CustomerIntelligenceAnonymizer.AnonymizeRepositoryUri(result, createHash);
      return !createHash ? uri : CustomerIntelligenceAnonymizer.HashUriString(uri);
    }

    private static string HashUriString(string uri)
    {
      string input;
      if (uri == null)
        input = (string) null;
      else
        input = uri.ToLowerInvariant().TrimEnd('/');
      return CustomerIntelligenceAnonymizer.GetHash(input);
    }

    public static string GetTruncatedHash(string input, int count = 4) => !string.IsNullOrEmpty(input) ? CustomerIntelligenceAnonymizer.Truncate(CustomerIntelligenceAnonymizer.GetHash(input), count) : input;

    private static string GetHash(string input)
    {
      if (string.IsNullOrEmpty(input))
        return input;
      using (SHA512CryptoServiceProvider cryptoServiceProvider = new SHA512CryptoServiceProvider())
        return HexConverter.ToString(cryptoServiceProvider.ComputeHash(Encoding.ASCII.GetBytes(input)));
    }

    private static string Truncate(string data, int count)
    {
      ArgumentUtility.CheckForNull<string>(data, nameof (data));
      ArgumentUtility.CheckForNonnegativeInt(count, nameof (count));
      return count >= data.Length ? string.Empty : data.Substring(0, data.Length - count);
    }
  }
}
