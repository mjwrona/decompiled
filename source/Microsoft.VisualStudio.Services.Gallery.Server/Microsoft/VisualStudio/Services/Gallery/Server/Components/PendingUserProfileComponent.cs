// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PendingUserProfileComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PendingUserProfileComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "PendingUserProfileComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PendingUserProfileComponent>(1)
    }, "PendingUserProfile");

    static PendingUserProfileComponent()
    {
      PendingUserProfileComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      PendingUserProfileComponent.s_sqlExceptionFactories.Add(270018, new SqlExceptionFactory(typeof (PuidExistsException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PendingUserProfileComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (PendingUserProfileComponent);

    public virtual void CreateUserPendingProfile(string puid, Guid pendingID, string displayName)
    {
      this.PrepareStoredProcedure("Gallery.prc_CreatePendingProfile");
      this.BindString(nameof (puid), puid, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (pendingID), pendingID);
      this.BindString(nameof (displayName), displayName, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual Guid QueryUserPendingProfile(string puid)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryPendingProfile");
      this.BindString(nameof (puid), puid, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryPendingProfile", this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new PendingUserProfileComponent.PendingProfilePendingIDBinder());
        return resultCollection.GetCurrent<Guid>().Items.Count != 0 ? resultCollection.GetCurrent<Guid>().Items[0] : Guid.Empty;
      }
    }

    public virtual void LinkPendingProfile(string puid, Guid vsid)
    {
      this.PrepareStoredProcedure("Gallery.prc_LinkPendingProfile");
      this.BindString(nameof (puid), puid, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindGuid(nameof (vsid), vsid);
      this.BindGuid("writerIdentifier", this.Author);
      this.ExecuteNonQuery();
    }

    public virtual Dictionary<Guid, string> GetPendingProfileNames(List<Guid> userIds)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetPendingProfileNames");
      this.BindGuidTable(nameof (userIds), (IEnumerable<Guid>) userIds);
      Dictionary<Guid, string> pendingProfileNames = new Dictionary<Guid, string>();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetPendingProfileNames", this.RequestContext))
      {
        resultCollection.AddBinder<PendingUserProfileComponent.PendingProfileNameRow>((ObjectBinder<PendingUserProfileComponent.PendingProfileNameRow>) new PendingUserProfileComponent.PendingProfileNameBinder());
        if (resultCollection.GetCurrent<PendingUserProfileComponent.PendingProfileNameRow>().Items.Count != 0)
        {
          foreach (PendingUserProfileComponent.PendingProfileNameRow pendingProfileNameRow in resultCollection.GetCurrent<PendingUserProfileComponent.PendingProfileNameRow>().Items)
            pendingProfileNames.Add(pendingProfileNameRow.UserId, pendingProfileNameRow.DisplayName);
        }
      }
      return pendingProfileNames;
    }

    internal class PendingProfilePendingIDBinder : ObjectBinder<Guid>
    {
      protected SqlColumnBinder PendingIDColumn = new SqlColumnBinder("PendingID");

      protected override Guid Bind() => this.PendingIDColumn.GetGuid((IDataReader) this.Reader, false);
    }

    internal class PendingProfileNameRow
    {
      public Guid UserId { get; set; }

      public string DisplayName { get; set; }
    }

    internal class PendingProfileNameBinder : 
      ObjectBinder<PendingUserProfileComponent.PendingProfileNameRow>
    {
      protected SqlColumnBinder userIdColumn = new SqlColumnBinder("PendingID");
      protected SqlColumnBinder displayNameColumn = new SqlColumnBinder("DisplayName");

      protected override PendingUserProfileComponent.PendingProfileNameRow Bind() => new PendingUserProfileComponent.PendingProfileNameRow()
      {
        UserId = this.userIdColumn.GetGuid((IDataReader) this.Reader, false),
        DisplayName = this.displayNameColumn.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
