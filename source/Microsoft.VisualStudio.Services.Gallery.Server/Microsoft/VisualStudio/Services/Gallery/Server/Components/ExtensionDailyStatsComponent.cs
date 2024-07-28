// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[13]
    {
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent>(0, true),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent1>(1),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent2>(2),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent3>(3),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent4>(4),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent5>(5),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent6>(6),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent7>(7),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent8>(8),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent9>(9),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent10>(10),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent11>(11),
      (IComponentCreator) new ComponentCreator<ExtensionDailyStatsComponent12>(12)
    }, "ExtensionDailyStats");
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
    private const string s_area = "ExtensionDailyStatsComponent";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ExtensionDailyStatsComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (ExtensionDailyStatsComponent);
  }
}
