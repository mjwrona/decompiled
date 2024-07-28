// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.SignInFederatedProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class SignInFederatedProvider
  {
    public const string Provider = "provider";
    public const string GithubSignIn = "githubsi";

    public static UriBuilder AppendProvider(this UriBuilder uriBuilder, string provider) => string.IsNullOrEmpty(provider) ? uriBuilder : uriBuilder.AppendQuery(nameof (provider), provider);

    public static UriBuilder AppendGithubSignIn(this UriBuilder uriBuilder) => uriBuilder.AppendQuery("githubsi", "true");
  }
}
