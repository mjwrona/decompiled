// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.SearchMigrationComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public class SearchMigrationComponent : TeamFoundationSqlResourceComponent
  {
    private const string ServiceName = "Search_HostMigration";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<SearchMigrationComponent>(1)
    }, "Search_HostMigration");

    public void DeleteSearchIndexingStateTables()
    {
      this.Logger.Info("Executing stored proc: prc_DeleteSearchIndexingStateTables");
      this.PrepareStoredProcedure("Search.prc_DeleteSearchIndexingStateTables", true);
      this.ExecuteNonQuery(false);
    }
  }
}
