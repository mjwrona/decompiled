// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VsoCreateDatabaseOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VsoCreateDatabaseOptions
  {
    public string Collation { get; set; }

    public int? MaxTenants { get; set; }

    public TeamFoundationDatabaseFlags Flags { get; set; }

    public string ServiceObjective { get; set; }

    public int MaxSizeInGB { get; set; }

    public SetQueryStoreOptions QueryStoreOptions { get; set; } = new SetQueryStoreOptions();

    public static VsoCreateDatabaseOptions DefaultOptions => new VsoCreateDatabaseOptions();
  }
}
