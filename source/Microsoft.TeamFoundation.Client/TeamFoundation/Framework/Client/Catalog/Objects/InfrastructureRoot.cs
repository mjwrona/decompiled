// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.InfrastructureRoot
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class InfrastructureRoot : CatalogObject
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.InfrastructureRoot;
    private static readonly Type[] KnownChildTypes = new Type[1]
    {
      typeof (Machine)
    };
    private ICollection<Machine> m_Machines;

    protected override void Reset()
    {
      base.Reset();
      this.m_Machines = (ICollection<Machine>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_Machines = this.PreloadChildren<Machine>(bulkData);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<Machine> Machines => this.GetChildCollection<Machine>(ref this.m_Machines);
  }
}
