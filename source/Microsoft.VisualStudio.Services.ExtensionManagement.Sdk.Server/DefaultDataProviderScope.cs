// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.DefaultDataProviderScope
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class DefaultDataProviderScope : IDataProviderScope
  {
    public string Name { get; private set; }

    public string Value { get; private set; }

    public DefaultDataProviderScope(string name, string value)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(value, nameof (value));
      this.Name = name;
      this.Value = value;
    }

    public override string ToString() => string.Format("{0}: {1}", (object) this.Name, (object) this.Value);

    public object GetProperty(string propertyName) => (object) null;
  }
}
