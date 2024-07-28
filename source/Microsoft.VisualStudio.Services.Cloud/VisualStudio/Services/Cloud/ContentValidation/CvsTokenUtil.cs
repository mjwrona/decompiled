// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentValidation.CvsTokenUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.IdentityModel.Tokens;
using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.SecureToken;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.VisualStudio.Services.Cloud.ContentValidation
{
  internal static class CvsTokenUtil
  {
    public const string TokenAudience = "00000002-0000-8888-8000-000000000000";
    public const string TokenIssuer = "HostedContentValidationService";
    public const string KeyNamespace = "cvs";
    public static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7.0);

    public static JwtSecurityToken Get(
      IVssRequestContext rc,
      Guid sourceHostId,
      ContentValidationTakedownTarget targetWithinHost,
      Guid? sourceProjectId)
    {
      List<Claim> claimList = new List<Claim>()
      {
        new Claim("SourceHostId", sourceHostId.ToString()),
        new Claim("TargetWithinHost", targetWithinHost.ToString())
      };
      if (sourceProjectId.HasValue)
        claimList.Add(new Claim("SourceProjectId", sourceProjectId.Value.ToString()));
      IVssRequestContext vssRequestContext = rc.Elevate();
      return vssRequestContext.GetService<ISecureTokenService>().IssueToken(vssRequestContext, "00000002-0000-8888-8000-000000000000", "HostedContentValidationService", (IEnumerable<Claim>) claimList, "cvs", CvsTokenUtil.TokenLifetime);
    }

    public static void PopulateInContentItem(ContentItem contentItem, JwtSecurityToken token)
    {
      ArgumentUtility.CheckForNull<JwtSecurityToken>(token, nameof (token));
      ContentValidationExternalId validationExternalId = JsonUtilities.Deserialize<ContentValidationExternalId>(contentItem.ExternalId);
      validationExternalId.Token = token.RawData;
      contentItem.ExternalId = validationExternalId.Serialize<ContentValidationExternalId>();
    }

    public static string ExtractFirstRaw(IEnumerable<ContentItem> contentItems)
    {
      foreach (ContentItem contentItem in contentItems)
      {
        ContentValidationExternalId validationExternalId = JsonUtilities.Deserialize<ContentValidationExternalId>(contentItem.ExternalId);
        if (validationExternalId.Token != null)
          return validationExternalId.Token;
      }
      return (string) null;
    }

    public static void Validate(
      IVssRequestContext rc,
      string rawToken,
      Guid sourceHostId,
      ContentValidationTakedownTarget targetWithinHost,
      Guid? sourceProjectId)
    {
      IVssRequestContext vssRequestContext = rc.Elevate();
      SecureTokenValidationResult validationResult = vssRequestContext.GetService<ISecureTokenService>().ValidateToken(vssRequestContext, rawToken, new TokenValidationParameters()
      {
        ValidIssuer = "HostedContentValidationService",
        ValidAudience = "00000002-0000-8888-8000-000000000000"
      });
      if (!validationResult.Success)
        throw new InvalidTokenException("The supplied token was invalid.", validationResult.Failure);
      IReadOnlyDictionary<string, string> dictionary = (IReadOnlyDictionary<string, string>) validationResult.validatedJwt.Claims.ToDictionary<Claim, string, string>((Func<Claim, string>) (c => c.Type), (Func<Claim, string>) (c => c.Value));
      if (dictionary["SourceHostId"] != sourceHostId.ToString())
        throw new InvalidTokenException("The source host IDs did not match.");
      if (dictionary["TargetWithinHost"] != targetWithinHost.ToString())
        throw new InvalidTokenException("The takedown target did not match.");
      if (sourceProjectId.HasValue)
      {
        if (dictionary["SourceProjectId"] != sourceProjectId.ToString())
          throw new InvalidTokenException("The project IDs did not match.");
      }
      else if (dictionary.ContainsKey("SourceProjectId"))
        throw new InvalidTokenException("Source project ID has a claim but no value was supplied.");
    }

    private static byte[] GetEncodedBytes(ContentItem item) => new TokenizeableContentItem(item)
    {
      ExternalId = {
        Token = ((string) null)
      }
    }.AsEncodedMessage();

    private static class ClaimNames
    {
      public const string SourceHostId = "SourceHostId";
      public const string TargetWithinHost = "TargetWithinHost";
      public const string SourceProjectId = "SourceProjectId";
    }
  }
}
