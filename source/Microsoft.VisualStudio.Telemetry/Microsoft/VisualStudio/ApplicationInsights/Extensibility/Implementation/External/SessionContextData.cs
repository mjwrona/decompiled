// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.SessionContextData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  internal sealed class SessionContextData
  {
    private readonly IDictionary<string, string> tags;

    internal SessionContextData(IDictionary<string, string> tags) => this.tags = tags;

    public string Id
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.SessionId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.SessionId, value);
    }

    public bool? IsFirst
    {
      get => this.tags.GetTagBoolValueOrNull(ContextTagKeys.Keys.SessionIsFirst);
      set => this.tags.SetTagValueOrRemove<bool?>(ContextTagKeys.Keys.SessionIsFirst, value);
    }

    public bool? IsNewSession
    {
      get => this.tags.GetTagBoolValueOrNull(ContextTagKeys.Keys.SessionIsNew);
      set => this.tags.SetTagValueOrRemove<bool?>(ContextTagKeys.Keys.SessionIsNew, value);
    }

    internal void SetDefaults(SessionContextData source)
    {
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.SessionId, source.Id);
      this.tags.InitializeTagValue<bool?>(ContextTagKeys.Keys.SessionIsFirst, source.IsFirst);
      this.tags.InitializeTagValue<bool?>(ContextTagKeys.Keys.SessionIsNew, source.IsNewSession);
    }
  }
}
