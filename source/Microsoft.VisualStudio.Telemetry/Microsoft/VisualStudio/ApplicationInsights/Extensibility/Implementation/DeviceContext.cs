// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.DeviceContext
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation.External;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.ApplicationInsights.Extensibility.Implementation
{
  public sealed class DeviceContext : IJsonSerializable
  {
    private readonly IDictionary<string, string> tags;

    internal DeviceContext(IDictionary<string, string> tags) => this.tags = tags;

    public string Type
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceType);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceType, value);
    }

    public string Id
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceId);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceId, value);
    }

    public string OperatingSystem
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceOSVersion);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceOSVersion, value);
    }

    public string OemName
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceOEMName);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceOEMName, value);
    }

    public string Model
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceModel);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceModel, value);
    }

    public string NetworkType
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceNetwork);
      set => this.tags.SetTagValueOrRemove<string>(ContextTagKeys.Keys.DeviceNetwork, value);
    }

    public string ScreenResolution
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceScreenResolution);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceScreenResolution, value);
    }

    public string Language
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceLanguage);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceLanguage, value);
    }

    public string RoleName
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceRoleName);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceRoleName, value);
    }

    public string RoleInstance
    {
      get => this.tags.GetTagValueOrNull(ContextTagKeys.Keys.DeviceRoleInstance);
      set => this.tags.SetStringValueOrRemove(ContextTagKeys.Keys.DeviceRoleInstance, value);
    }

    void IJsonSerializable.Serialize(IJsonWriter writer)
    {
      writer.WriteStartObject();
      writer.WriteProperty("type", this.Type);
      writer.WriteProperty("id", Utils.PopulateRequiredStringValue(this.Id, "id", typeof (DeviceContext).FullName));
      writer.WriteProperty("osVersion", this.OperatingSystem);
      writer.WriteProperty("oemName", this.OemName);
      writer.WriteProperty("model", this.Model);
      writer.WriteProperty("network", this.NetworkType);
      writer.WriteProperty("resolution", this.ScreenResolution);
      writer.WriteProperty("locale", this.Language);
      writer.WriteProperty("roleName", this.RoleName);
      writer.WriteProperty("roleInstance", this.RoleInstance);
      writer.WriteEndObject();
    }
  }
}
