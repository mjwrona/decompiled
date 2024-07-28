// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.VsGalleryMigrationComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class VsGalleryMigrationComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe("Grandfathered")]
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    [StaticSafe("Grandfathered")]
    private static string s_area = nameof (VsGalleryMigrationComponent);
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<VsGalleryMigrationComponent>(1)
    }, "VsGalleryMigration");

    static VsGalleryMigrationComponent()
    {
      VsGalleryMigrationComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      VsGalleryMigrationComponent.s_sqlExceptionFactories.Add(270002, new SqlExceptionFactory(typeof (ExtensionExistsException)));
      VsGalleryMigrationComponent.s_sqlExceptionFactories.Add(270003, new SqlExceptionFactory(typeof (ExtensionDoesNotExistException)));
      VsGalleryMigrationComponent.s_sqlExceptionFactories.Add(270015, new SqlExceptionFactory(typeof (ExtensionDoesNotExistException)));
      VsGalleryMigrationComponent.s_sqlExceptionFactories.Add(270006, new SqlExceptionFactory(typeof (ExtensionMustBePrivateException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) VsGalleryMigrationComponent.s_sqlExceptionFactories;

    protected override string TraceArea => VsGalleryMigrationComponent.s_area;

    public virtual void UpdateExtensionMetadata(
      Guid extensionId,
      IList<KeyValuePair<string, string>> metadata)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionMetadata");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindKeyValuePairStringTable(nameof (metadata), (IEnumerable<KeyValuePair<string, string>>) metadata);
      this.ExecuteNonQuery();
    }
  }
}
