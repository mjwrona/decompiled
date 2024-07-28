// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.BitbucketUrl
// Assembly: Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 223C9BE7-A3E9-431B-86B7-A81B8A6447FF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.ExternalProviders.Bitbucket.WebApi
{
  public static class BitbucketUrl
  {
    public static class Api
    {
      private static readonly string root = "https://api.bitbucket.org";

      public static UriBuilder Root() => new UriBuilder(BitbucketUrl.Api.root).AppendPathSegments("2.0");

      public static UriBuilder Repository(string repository)
      {
        ArgumentUtility.CheckForNull<string>(repository, nameof (repository));
        return BitbucketUrl.Api.Root().AppendPathSegments("repositories", repository);
      }
    }

    public static class Html
    {
      private static readonly string root = "https://bitbucket.org";

      public static UriBuilder Root() => new UriBuilder(BitbucketUrl.Html.root);

      public static UriBuilder Content(string repository, string version, string path)
      {
        ArgumentUtility.CheckForNull<string>(repository, nameof (repository));
        ArgumentUtility.CheckForNull<string>(version, nameof (version));
        ArgumentUtility.CheckForNull<string>(path, nameof (path));
        return new UriBuilder(BitbucketUrl.Html.root).AppendPathSegments(repository, "src", version, path);
      }
    }
  }
}
