// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Warehouse.WarehouseSecurity
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Warehouse
{
  public static class WarehouseSecurity
  {
    public static readonly Guid WarehouseNamespaceId = new Guid("b8fbab8b-69c8-4cd9-98b5-873656788efb");
    public static readonly string WarehouseNamespaceToken = "Warehouse";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string WarehouseNamespaceName = "Warehouse";
  }
}
