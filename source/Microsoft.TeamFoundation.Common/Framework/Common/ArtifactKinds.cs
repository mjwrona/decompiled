// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.ArtifactKinds
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class ArtifactKinds
  {
    public static readonly Guid Generic = new Guid("{435B8548-EE47-47bb-A3C7-DA6574D8BAA9}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid Catalog = new Guid("{AD359EF1-FDE8-4c78-B19F-4AB30E74E53C}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid Identity = new Guid("{2B258B5D-D41F-4657-8585-8BBBFEC1052B}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid InternalIdentity = new Guid("{CC712A4E-3290-4CD1-9AD8-E86E8497C3FE}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid LocalIdentity = new Guid("{5A282F28-FBB1-4A6E-9D8E-E437C5EA90B1}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid Account = new Guid("{04E1698C-10ED-4CA6-AD04-56DBDA8615F9}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ProcessTemplate = new Guid("{0A9C9FAB-BE9F-423F-8730-2E98691554E1}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid Location = new Guid("{77951F77-F29D-4A9B-8311-1B13E4B89C42}");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid ServiceHost = new Guid("{42FEDFDD-11BF-4F3B-91CA-CD6A4FC7B13C}");
  }
}
