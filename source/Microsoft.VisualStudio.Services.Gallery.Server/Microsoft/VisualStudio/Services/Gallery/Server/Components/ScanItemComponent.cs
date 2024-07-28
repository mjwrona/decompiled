// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ScanItemComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class ScanItemComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "ScanItemComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ScanItemComponent>(0, true),
      (IComponentCreator) new ComponentCreator<ScanItemComponent1>(1),
      (IComponentCreator) new ComponentCreator<ScanItemComponent2>(2)
    }, "ScanItem");

    static ScanItemComponent()
    {
      ScanItemComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      ScanItemComponent.s_sqlExceptionFactories.Add(270027, new SqlExceptionFactory(typeof (InvalidScanItemException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ScanItemComponent.s_sqlExceptionFactories;

    protected override string TraceArea => nameof (ScanItemComponent);
  }
}
