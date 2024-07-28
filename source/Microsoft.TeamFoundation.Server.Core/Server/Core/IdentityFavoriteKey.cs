// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityFavoriteKey
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class IdentityFavoriteKey
  {
    public const string Prefix = "Microsoft.TeamFoundation.Framework.Server.IdentityFavorites";

    public static IdentityFavoriteKey Parse(IVssRequestContext requestContext, string key)
    {
      try
      {
        bool flag = false;
        string str1 = key.Substring(61, 36);
        string input = key.Length > 134 ? key.Substring(98, 36) : "";
        if (Guid.TryParse(input, out Guid _))
          flag = true;
        else
          input = (string) null;
        int startIndex = flag ? 135 : 98;
        string str2 = key.Substring(startIndex);
        string str3 = (string) null;
        string str4;
        if (str2.Contains("-"))
        {
          int length = str2.Length;
          str4 = str2.Substring(0, length - 36);
          str3 = str2.Substring(length - 36, 36);
        }
        else
          str4 = str2.Replace("*", "");
        return new IdentityFavoriteKey()
        {
          ProjectGuid = str1,
          TeamId = input,
          Scope = str4,
          Id = str3
        };
      }
      catch (Exception ex)
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Favorites", "IdentityFavoriteKey.ParseException", "Summary", ex.ToString());
        throw;
      }
    }

    public static IdentityFavoriteKey CreateUserkey(string projectId, string scope, Guid id) => new IdentityFavoriteKey()
    {
      ProjectGuid = projectId,
      Id = id.ToString(),
      Scope = scope
    };

    public static IdentityFavoriteKey CreateTeamkey(
      string projectId,
      string teamId,
      string scope,
      Guid id)
    {
      return new IdentityFavoriteKey()
      {
        ProjectGuid = projectId,
        TeamId = teamId,
        Id = id.ToString(),
        Scope = scope
      };
    }

    public override string ToString()
    {
      string str;
      if (!this.IsTeamFavorite)
        str = string.Format("{0}..{1}.{2}{3}", (object) "Microsoft.TeamFoundation.Framework.Server.IdentityFavorites", (object) this.ProjectGuid, (object) this.Scope, (object) this.Id);
      else
        str = string.Format("{0}..{1}.{2}.{3}{4}", (object) "Microsoft.TeamFoundation.Framework.Server.IdentityFavorites", (object) this.ProjectGuid, (object) this.TeamId, (object) this.Scope, (object) this.Id);
      return str;
    }

    public bool IsTeamFavorite => this.TeamId != null;

    public string ProjectGuid { get; private set; }

    public string TeamId { get; private set; }

    public string Id { get; private set; }

    public string Scope { get; private set; }
  }
}
