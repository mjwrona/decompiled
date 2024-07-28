// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Data.ShallowScaledAgileViewRecord
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using System;

namespace Microsoft.TeamFoundation.Agile.Web.Data
{
  internal class ShallowScaledAgileViewRecord
  {
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public string Name { get; set; }

    public int Type { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime ModifiedDate { get; set; }

    public Guid ModifiedBy { get; set; }

    public string Description { get; set; }

    public int Revision { get; set; }

    public DateTime? LastAccessed { get; set; }
  }
}
