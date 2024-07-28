// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PagedSessionTokens
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class PagedSessionTokens
  {
    public int NextRowNumber { get; set; }

    public IList<SessionToken> SessionTokens { get; set; }

    public static implicit operator PagedPatTokens(PagedSessionTokens s)
    {
      IList<PatToken> patTokenList = (IList<PatToken>) null;
      if (s.SessionTokens != null)
      {
        patTokenList = (IList<PatToken>) new List<PatToken>();
        foreach (SessionToken sessionToken in (IEnumerable<SessionToken>) s.SessionTokens)
        {
          PatToken patToken = PagedSessionTokens.ConvertSessionTokenToPatToken(sessionToken);
          patTokenList.Add(patToken);
        }
      }
      string str = PagedSessionTokens.EncodeContinuationToken(s.NextRowNumber);
      return new PagedPatTokens()
      {
        PatTokens = patTokenList,
        ContinuationToken = str
      };
    }

    public static string EncodeContinuationToken(int nextRowNumber) => nextRowNumber > 0 ? Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}-patapitoken", (object) nextRowNumber))) : "";

    public static int DecodeContinuationToken(string continuationToken)
    {
      string str = Encoding.UTF8.GetString(Convert.FromBase64String(continuationToken));
      return int.Parse(str.Substring(0, str.IndexOf('-')));
    }

    private static PatToken ConvertSessionTokenToPatToken(SessionToken s)
    {
      PatToken patToken = (PatToken) null;
      if (s != null)
        patToken = new PatToken()
        {
          DisplayName = s.DisplayName,
          ValidTo = s.ValidTo,
          Scope = s.Scope,
          TargetAccounts = s.TargetAccounts,
          ValidFrom = s.ValidFrom,
          AuthorizationId = s.AuthorizationId,
          Token = s.Token
        };
      return patToken;
    }
  }
}
