// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ExperimentDto`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  [ExcludeFromCodeCoverage]
  internal sealed class ExperimentDto<T>
  {
    public T Value { get; set; }

    public int Percentage { get; set; } = 100;

    public ExperimentIdDto VSID { get; set; }

    public ExperimentIdDto CollectionHostId { get; set; }

    public ExperimentIdDto TenantId { get; set; }
  }
}
