// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.VsixIdManagerComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class VsixIdManagerComponent : TeamFoundationSqlResourceComponent
  {
    private const string s_area = "VsixIdManagerComponent";
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> SqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<VsixIdManagerComponent>(1),
      (IComponentCreator) new ComponentCreator<VsixIdManagerComponent2>(2)
    }, "VsixIdManager");

    protected override string TraceArea => nameof (VsixIdManagerComponent);

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) VsixIdManagerComponent.SqlExceptionFactories;

    public bool DoesMetadataPairExist(string keyName, string value)
    {
      this.PrepareStoredProcedure("Gallery.prc_DoesMetadataPairExist");
      this.BindString("keyname", keyName, 80, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString(nameof (value), value, 2000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      return (bool) this.ExecuteScalar();
    }

    public void AddReservedVsixId(string vsixId, ReservedVsixIdPurposeType purpose, string userId)
    {
      this.PrepareStoredProcedure("Gallery.prc_AddReservedVsixId");
      this.BindString(nameof (vsixId), vsixId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt(nameof (purpose), (int) purpose);
      this.BindString(nameof (userId), userId, 50, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public void DeleteReservedVsixId(string vsixId, ReservedVsixIdPurposeType purpose)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteReservedVsixId");
      this.BindString(nameof (vsixId), vsixId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt(nameof (purpose), (int) purpose);
      this.ExecuteNonQuery();
    }

    public List<ReservedVsixId> QueryReservedVsixIds(
      string vsixId,
      ReservedVsixIdPurposeType purpose)
    {
      this.PrepareStoredProcedure("Gallery.prc_QueryReservedVsixIds");
      this.BindString(nameof (vsixId), vsixId, 100, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt(nameof (purpose), (int) purpose);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_QueryReservedVsixIds", this.RequestContext))
      {
        resultCollection.AddBinder<ReservedVsixId>((ObjectBinder<ReservedVsixId>) new ReservedVsixIdBinder());
        return resultCollection.GetCurrent<ReservedVsixId>().Items;
      }
    }
  }
}
