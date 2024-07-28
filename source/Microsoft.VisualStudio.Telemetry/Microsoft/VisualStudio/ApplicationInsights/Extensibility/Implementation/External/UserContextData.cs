// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External.UserContextData
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External
{
  internal sealed class UserContextData
  {
    private readonly IDictionary<string, string> tags;

    internal UserContextData(IDictionary<string, string> tags) => this.tags = tags;

    public string Id
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.UserId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.UserId, value);
    }

    public string AccountId
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.UserAccountId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.UserAccountId, value);
    }

    public string UserAgent
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.UserAgent);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.UserAgent, value);
    }

    public string StoreRegion
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.UserStoreRegion);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.UserStoreRegion, value);
    }

    public DateTimeOffset? AcquisitionDate
    {
      get => this.tags.GetTagDateTimeOffsetValueOrNull(ContextTagKeys.Keys.UserAccountAcquisitionDate);
      set => this.tags.SetDateTimeOffsetValueOrRemove(ContextTagKeys.Keys.UserAccountAcquisitionDate, value);
    }

    internal void SetDefaults(UserContextData source)
    {
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.UserId, source.Id);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.UserAgent, source.UserAgent);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.UserAccountId, source.AccountId);
      this.tags.InitializeTagDateTimeOffsetValue(ContextTagKeys.Keys.UserAccountAcquisitionDate, source.AcquisitionDate);
      this.tags.InitializeTagValue<string>(ContextTagKeys.Keys.UserStoreRegion, source.StoreRegion);
    }
  }
}
