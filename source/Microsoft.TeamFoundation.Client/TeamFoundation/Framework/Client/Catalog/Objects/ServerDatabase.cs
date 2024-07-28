// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.ServerDatabase
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
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
