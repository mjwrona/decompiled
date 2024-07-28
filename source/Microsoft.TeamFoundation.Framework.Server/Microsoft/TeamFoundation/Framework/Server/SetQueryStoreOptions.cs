// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SetQueryStoreOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class SetQueryStoreOptions : QueryStoreOptionsBase
  {
    public SetQueryStoreOptions()
    {
    }

    public SetQueryStoreOptions(QueryStoreOptions queryStoreOptions)
      : base((QueryStoreOptionsBase) queryStoreOptions)
    {
      ArgumentUtility.CheckForNull<QueryStoreOptions>(queryStoreOptions, nameof (queryStoreOptions));
      this.OperationMode = queryStoreOptions.DesiredState == QueryStoreState.ReadWrite ? QueryStoreOperationMode.ReadWrite : QueryStoreOperationMode.ReadOnly;
    }

    public QueryStoreOperationMode OperationMode { get; set; } = QueryStoreOperationMode.ReadWrite;

    public override string ToString() => string.Format("{0}, OperationMode: {1}", (object) base.ToString(), (object) this.OperationMode);
  }
}
