// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.OperationContextData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  internal sealed class OperationContextData
  {
    private readonly IDictionary<string, string> tags;

    internal OperationContextData(IDictionary<string, string> tags) => this.tags = tags;

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

    public string ParentId
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationParentId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationParentId, value);
    }

    public string RootId
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationRootId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationRootId, value);
    }

    public string SyntheticSource
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.OperationSyntheticSource);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.OperationSyntheticSource, value);
    }

    public bool? IsSynthetic
    {
      get => this.tags.GetTagBoolValueOrNull(ContextTagKeys.Keys.OperationIsSynthetic);
      set => this.tags.SetTagValueOrRemove<bool?>(ContextTagKeys.Keys.OperationIsSynthetic, value);
    }

    internal void SetDefaults(OperationContextData source)
    {
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.OperationId, source.Id);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.OperationName, source.Name);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.OperationParentId, source.ParentId);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.OperationRootId, source.RootId);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.OperationSyntheticSource, source.SyntheticSource);
      this.tags.InitializeTagValue<bool?>(ContextTagKeys.Keys.OperationIsSynthetic, source.IsSynthetic);
    }
  }
}
