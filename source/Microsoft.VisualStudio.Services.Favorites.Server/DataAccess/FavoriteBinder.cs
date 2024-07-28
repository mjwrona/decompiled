// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.DataAccess.FavoriteBinder
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Favorites.Server.DataAccess
{
  internal class FavoriteBinder : ObjectBinder<Favorite>
  {
    private SqlColumnBinder Id = new SqlColumnBinder(nameof (Id));
    private SqlColumnBinder OwnerIdentity = new SqlColumnBinder(nameof (OwnerIdentity));
    private SqlColumnBinder DataspaceIdentifier = new SqlColumnBinder(nameof (DataspaceIdentifier));
    private SqlColumnBinder ArtifactType = new SqlColumnBinder(nameof (ArtifactType));
    private SqlColumnBinder ArtifactId = new SqlColumnBinder(nameof (ArtifactId));
    private SqlColumnBinder CreationDate = new SqlColumnBinder(nameof (CreationDate));
    private SqlColumnBinder ArtifactName = new SqlColumnBinder(nameof (ArtifactName));
    private SqlColumnBinder ArtifactProperties = new SqlColumnBinder(nameof (ArtifactProperties));
    private SqlColumnBinder ArtifactIsDeleted = new SqlColumnBinder(nameof (ArtifactIsDeleted));

    protected override Favorite Bind()
    {
      Guid guid = this.DataspaceIdentifier.GetGuid((IDataReader) this.Reader, false);
      string str;
      string empty;
      if (guid == Guid.Empty)
      {
        str = "Collection";
        empty = string.Empty;
      }
      else
      {
        str = "Project";
        empty = guid.ToString();
      }
      Microsoft.VisualStudio.Services.Favorites.WebApi.ArtifactProperties artifactProperties = (Microsoft.VisualStudio.Services.Favorites.WebApi.ArtifactProperties) null;
      JsonUtilities.TryDeserialize<Microsoft.VisualStudio.Services.Favorites.WebApi.ArtifactProperties>(this.ArtifactProperties.GetString((IDataReader) this.Reader, true), out artifactProperties);
      if (artifactProperties == null)
        artifactProperties = new Microsoft.VisualStudio.Services.Favorites.WebApi.ArtifactProperties();
      return new Favorite()
      {
        Id = new Guid?(this.Id.GetGuid((IDataReader) this.Reader)),
        ArtifactScope = new ArtifactScope()
        {
          Type = str,
          Id = empty
        },
        Owner = new IdentityRef()
        {
          Id = this.OwnerIdentity.GetGuid((IDataReader) this.Reader).ToString()
        },
        ArtifactType = this.ArtifactType.GetString((IDataReader) this.Reader, false),
        ArtifactId = this.ArtifactId.GetString((IDataReader) this.Reader, false),
        CreationDate = new DateTime?(this.CreationDate.GetDateTime((IDataReader) this.Reader)),
        ArtifactName = this.ArtifactName.GetString((IDataReader) this.Reader, false),
        ArtifactProperties = artifactProperties,
        ArtifactIsDeleted = this.ArtifactIsDeleted.GetBoolean((IDataReader) this.Reader, false)
      };
    }
  }
}
