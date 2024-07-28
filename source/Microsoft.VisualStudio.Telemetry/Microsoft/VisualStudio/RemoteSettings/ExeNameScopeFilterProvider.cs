// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.ExeNameScopeFilterProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Utilities.Internal;
using System;

namespace Microsoft.VisualStudio.RemoteSettings
{
  internal sealed class ExeNameScopeFilterProvider : 
    ISingleValueScopeFilterProvider<StringScopeValue>,
    IScopeFilterProvider
  {
    private readonly Lazy<StringScopeValue> exeName;

    public string Name => "ExeName";

    public ExeNameScopeFilterProvider(RemoteSettingsFilterProvider filterProvider)
    {
      filterProvider.RequiresArgumentNotNull<RemoteSettingsFilterProvider>(nameof (filterProvider));
      this.exeName = new Lazy<StringScopeValue>((Func<StringScopeValue>) (() =>
      {
        string applicationName = filterProvider.GetApplicationName();
        return new StringScopeValue(string.IsNullOrEmpty(applicationName) ? "Unknown" : applicationName);
      }));
    }

    public StringScopeValue Provide() => this.exeName.Value;
  }
}
