// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.TomlPropertiesMetadata
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Tomlyn.Model
{
  [DebuggerDisplay("{_properties}")]
  public class TomlPropertiesMetadata
  {
    private readonly Dictionary<string, TomlPropertyMetadata> _properties;

    public TomlPropertiesMetadata() => this._properties = new Dictionary<string, TomlPropertyMetadata>();

    public void Clear() => this._properties.Clear();

    public bool ContainsProperty(string propertyKey) => this._properties.ContainsKey(propertyKey);

    public bool TryGetProperty(string propertyKey, [NotNullWhen(true)] out TomlPropertyMetadata? propertyMetadata) => this._properties.TryGetValue(propertyKey, out propertyMetadata);

    public void SetProperty(string propertyKey, TomlPropertyMetadata propertyMetadata) => this._properties[propertyKey] = propertyMetadata;
  }
}
