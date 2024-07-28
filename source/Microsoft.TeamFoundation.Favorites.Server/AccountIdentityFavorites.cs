// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Favorites.AccountIdentityFavorites
// Assembly: Microsoft.TeamFoundation.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4742830F-DF0E-4509-8C2D-2540DAED73F4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Favorites
{
  internal class AccountIdentityFavorites : IdentityFavorites, IAccountIdentityFavorites
  {
    private const int c_ProjectGuidStartIndex = 2;
    private const int c_guidStringLength = 36;

    protected override string Namespace => "Microsoft.TeamFoundation.Framework.Server.IdentityFavorites";

    public IEnumerable<TfsFavorite> GetAccountScopedFavorites(
      IVssRequestContext requestContext,
      OwnerScope ownerScope)
    {
      using (TimedCiEvent ciEvent = this.StartCIEvent(requestContext, "AccountFavoritesQueried"))
      {
        if (this.Identity == null)
          throw new UnknownOwnerIdentityException(FavoritesWebApiResources.UnknownOwnerIdentityExceptionMessage());
        IEnumerable<KeyValuePair<string, object>> viewProperties = this.GetViewProperties(IdentityPropertyScope.Local);
        List<TfsFavorite> results = new List<TfsFavorite>();
        HashSet<Guid> guidSet1 = new HashSet<Guid>();
        foreach (KeyValuePair<string, object> keyValuePair in viewProperties)
        {
          TfsFavorite tfsFavorite = (TfsFavorite) null;
          try
          {
            string str = keyValuePair.Value as string;
            if (!string.IsNullOrEmpty(str))
              tfsFavorite = TfsFavorite.Deserialize(str);
          }
          catch (SerializationException ex)
          {
          }
          try
          {
            string input = keyValuePair.Key.Substring(2, 36);
            string str1 = keyValuePair.Key.Substring(39);
            string str2 = str1.Substring(0, str1.Length - 36);
            if (ownerScope.IsTeam)
              str2 = str2.Substring(37);
            if (!string.IsNullOrEmpty(input))
            {
              Guid result;
              if (Guid.TryParse(input, out result))
              {
                if (tfsFavorite != null)
                {
                  HashSet<Guid> guidSet2 = guidSet1;
                  Guid? id = tfsFavorite.Id;
                  Guid guid1 = id.Value;
                  if (!guidSet2.Contains(guid1))
                  {
                    tfsFavorite.ProjectId = result;
                    tfsFavorite.Scope = str2;
                    results.Add(tfsFavorite);
                    HashSet<Guid> guidSet3 = guidSet1;
                    id = tfsFavorite.Id;
                    Guid guid2 = id.Value;
                    guidSet3.Add(guid2);
                  }
                }
              }
            }
          }
          catch (ArgumentOutOfRangeException ex)
          {
          }
        }
        this.AnnotateEventDetails(ciEvent, results);
        return (IEnumerable<TfsFavorite>) results;
      }
    }

    private void AnnotateEventDetails(TimedCiEvent ciEvent, List<TfsFavorite> results)
    {
      int num1 = results.Select<TfsFavorite, Guid>((Func<TfsFavorite, Guid>) (o => o.ProjectId)).Distinct<Guid>().Count<Guid>();
      ciEvent["projectCount"] = (object) num1;
      int num2 = results.Select<TfsFavorite, string>((Func<TfsFavorite, string>) (o => o.Type)).Distinct<string>().Count<string>();
      ciEvent["typeCount"] = (object) num2;
      results.Select<TfsFavorite, Tuple<Guid, string>>((Func<TfsFavorite, Tuple<Guid, string>>) (o => Tuple.Create<Guid, string>(o.ProjectId, o.Type))).Distinct<Tuple<Guid, string>>().Count<Tuple<Guid, string>>();
      ciEvent["projectTypeCount"] = (object) num1;
    }
  }
}
