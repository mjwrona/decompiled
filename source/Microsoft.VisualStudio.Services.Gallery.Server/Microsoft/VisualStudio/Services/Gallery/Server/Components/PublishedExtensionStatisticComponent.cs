// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionStatisticComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionStatisticComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "PublishedExtensionStatisticComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PublishedExtensionStatisticComponent>(1, true)
    }, "ExtensionStatistic");

    static PublishedExtensionStatisticComponent() => PublishedExtensionStatisticComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PublishedExtensionStatisticComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (PublishedExtensionStatisticComponent);

    public virtual void UpdateStatistics(IEnumerable<ExtensionStatisticUpdate> statistics)
    {
      this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionStatistic");
      this.BindExtensionStatisticTable(nameof (statistics), statistics);
      this.ExecuteNonQuery();
    }
  }
}
