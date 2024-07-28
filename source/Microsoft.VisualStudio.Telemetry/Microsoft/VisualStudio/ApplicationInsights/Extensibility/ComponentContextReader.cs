// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.ComponentContextReader
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Threading;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility
{
  internal class ComponentContextReader : IComponentContextReader
  {
    internal const string UnknownComponentVersion = "Unknown";
    private static IComponentContextReader instance;

    internal ComponentContextReader()
    {
    }

    public static IComponentContextReader Instance
    {
      get
      {
        if (ComponentContextReader.instance != null)
          return ComponentContextReader.instance;
        Interlocked.CompareExchange<IComponentContextReader>(ref ComponentContextReader.instance, (IComponentContextReader) new ComponentContextReader(), (IComponentContextReader) null);
        ComponentContextReader.instance.Initialize();
        return ComponentContextReader.instance;
      }
      internal set => ComponentContextReader.instance = value;
    }

    public void Initialize()
    {
    }

    public string GetVersion() => string.Empty;
  }
}
