// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.OperationContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  public sealed class OperationContext : IJsonSerializable
  {
    private readonly IDictionary<string, string> tags;

    internal OperationContext(IDictionary<string, string> tags) => this.tags = tags;

    public string Id
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationId, value);
    }

    public string Name
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationName);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationName, value);
    }

    public string SyntheticSource
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationSyntheticSource);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationSyntheticSource, value);
    }

    void IJsonSerializable.Serialize(IJsonWriter writer)
    {
      writer.WriteStartObject();
      writer.WriteProperty("id", this.Id);
      writer.WriteProperty("name", this.Name);
      writer.WriteProperty("syntheticSource", this.SyntheticSource);
      writer.WriteEndObject();
    }
  }
}
