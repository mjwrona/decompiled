// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.InternalContextData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  internal sealed class InternalContextData
  {
    private readonly IDictionary<string, string> tags;

    internal InternalContextData(IDictionary<string, string> tags) => this.tags = tags;

    public string SdkVersion
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.InternalSdkVersion);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.InternalSdkVersion, value);
    }

    public string AgentVersion
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.InternalAgentVersion);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.InternalAgentVersion, value);
    }
  }
}
