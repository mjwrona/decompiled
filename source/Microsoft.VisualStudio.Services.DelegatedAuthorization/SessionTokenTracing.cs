// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionTokenTracing
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal static class SessionTokenTracing
  {
    private static readonly Func<SessionTokenError, string> ErrorLookup = LookupGenerator.CreateLookupWithDefault<SessionTokenError, string>(string.Empty, (IReadOnlyDictionary<SessionTokenError, string>) Enum.GetValues(typeof (SessionTokenError)).Cast<SessionTokenError>().ToDictionary<SessionTokenError, SessionTokenError, string>((Func<SessionTokenError, SessionTokenError>) (key => key), (Func<SessionTokenError, string>) (value => value.ToString())));
    private static readonly Func<SessionTokenType, string> TokenTypeLookup = LookupGenerator.CreateLookupWithDefault<SessionTokenType, string>(string.Empty, (IReadOnlyDictionary<SessionTokenType, string>) Enum.GetValues(typeof (SessionTokenType)).Cast<SessionTokenType>().ToDictionary<SessionTokenType, SessionTokenType, string>((Func<SessionTokenType, SessionTokenType>) (key => key), (Func<SessionTokenType, string>) (value => value.ToString())));

    public static void TraceTokenIssuance(
      SessionTokenType tokenType,
      SessionTokenError error,
      SessionToken token)
    {
      if (token == null)
        TeamFoundationTracingService.TraceIdentitySessionTokenOperation("TokenIssuance", SessionTokenTracing.TokenTypeLookup(tokenType), SessionTokenTracing.ErrorLookup(error));
      else
        TeamFoundationTracingService.TraceIdentitySessionTokenOperation("TokenIssuance", SessionTokenTracing.TokenTypeLookup(tokenType), SessionTokenTracing.ErrorLookup(error), token.ClientId, token.AccessId, token.AuthorizationId, token.HostAuthorizationId, token.UserId, token.ValidFrom, token.ValidTo, token.DisplayName, token.Scope, token.TargetAccountString(), token.IsValid, token.IsPublic, token.PublicData, token.Source);
    }

    public static void TraceTokenUpdate(SessionTokenType tokenType, SessionToken token)
    {
      if (token == null)
        return;
      TeamFoundationTracingService.TraceIdentitySessionTokenOperation("TokenUpdate", SessionTokenTracing.TokenTypeLookup(tokenType), string.Empty, token.ClientId, token.AccessId, token.AuthorizationId, token.HostAuthorizationId, token.UserId, token.ValidFrom, token.ValidTo, token.DisplayName, token.Scope, token.TargetAccountString(), token.IsValid, token.IsPublic, token.PublicData, token.Source);
    }

    public static void TracePublicKeyRemoval(string publicData, string error = "")
    {
      string publicData1 = publicData;
      TeamFoundationTracingService.TraceIdentitySessionTokenOperation("TokenRevocation", error: error, publicData: publicData1);
    }

    public static void TraceTokenRevocation(Guid authorizationId) => TeamFoundationTracingService.TraceIdentitySessionTokenOperation("TokenRevocation", authorizationId: authorizationId);

    public static void TraceAllTokensRevokeOrRemovalForUser(Guid userId, bool isPublic = false) => TeamFoundationTracingService.TraceIdentitySessionTokenOperation("RevokeOrRemoveAllSessionTokens", userId: userId, isPublic: (isPublic ? 1 : 0) != 0);

    private static string TargetAccountString(this SessionToken token)
    {
      if (token == null || token.TargetAccounts.IsNullOrEmpty<Guid>())
        return string.Empty;
      return token.TargetAccounts.Count == 1 ? token.TargetAccounts[0].ToString() : string.Join<Guid>(",", (IEnumerable<Guid>) token.TargetAccounts);
    }
  }
}
