// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings.ConstantPackagingSetting`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings
{
  public class ConstantPackagingSetting<TValue> : 
    IOrgLevelPackagingSetting<TValue>,
    IFrotocolLevelPackagingSetting<TValue>,
    IFrotocolLevelPackagingSettingDefinition<TValue>
  {
    private readonly TValue value;

    public ConstantPackagingSetting(TValue value) => this.value = value;

    public TValue Get() => this.value;

    public TValue Get(IFeedRequest feedRequest) => this.value;

    public ConstantPackagingSetting<TValue> Bootstrap(IVssRequestContext requestContext) => this;

    IFrotocolLevelPackagingSetting<TValue> IFrotocolLevelPackagingSettingDefinition<TValue>.Bootstrap(
      IVssRequestContext requestContext)
    {
      return (IFrotocolLevelPackagingSetting<TValue>) this.Bootstrap(requestContext);
    }
  }
}
