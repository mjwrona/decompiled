// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.ServerDatabase
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServerDatabase : CatalogObject
  {
    public string InitialCatalog
    {
      get => this.GetProperty<string>(nameof (InitialCatalog));
      set => this.SetProperty<string>(nameof (InitialCatalog), value);
    }

    public bool IsOnline
    {
      get => this.GetProperty<bool>(nameof (IsOnline));
      set => this.SetProperty<bool>(nameof (IsOnline), value);
    }

    public static class Fields
    {
      public const string InitialCatalog = "InitialCatalog";
      public const string IsOnline = "IsOnline";
    }
  }
}
