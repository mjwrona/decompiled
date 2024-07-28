// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.ScaledAgileApiConstants
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ScaledAgileApiConstants
  {
    public static readonly Guid PlansLocationId = new Guid("0B42CB47-CD73-4810-AC90-19C9BA147453");
    public static readonly Guid DeliveryTimlineLocationId = new Guid("BDD0834E-101F-49F0-A6AE-509F384A12B4");
    public const string PlansPublisherName = "ms";
    public const string PlansExtensionName = "vss-plans";
  }
}
