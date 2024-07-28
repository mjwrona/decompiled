// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.FavoriteItemModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class FavoriteItemModel
  {
    private const int WebAccessExceptionEaten = 599999;

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(Name = "version", EmitDefaultValue = false)]
    public string Version { get; set; }

    [DataMember(Name = "repositoryId", EmitDefaultValue = false)]
    public Guid RepositoryId { get; set; }

    public static FavoriteItemModel Deserialize(
      IVssRequestContext requestContext,
      string favoriteData)
    {
      FavoriteItemModel favoriteItemModel = (FavoriteItemModel) null;
      if (VersionControlPath.IsServerItem(favoriteData))
      {
        favoriteItemModel = new FavoriteItemModel()
        {
          Path = favoriteData
        };
      }
      else
      {
        try
        {
          favoriteItemModel = JsonExtensions.FromJson<FavoriteItemModel>(favoriteData);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
        }
      }
      return favoriteItemModel;
    }
  }
}
