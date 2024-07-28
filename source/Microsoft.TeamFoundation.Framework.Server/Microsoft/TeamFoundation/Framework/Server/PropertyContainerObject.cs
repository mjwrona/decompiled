// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyContainerObject
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract]
  public abstract class PropertyContainerObject
  {
    protected PropertyContainerObject() => this.Properties = new PropertiesCollection();

    [DataMember]
    protected internal PropertiesCollection Properties { get; set; }

    public T GetProperty<T>(string name, T defaultValue)
    {
      T obj;
      return this.Properties != null && this.Properties.TryGetValue<T>(name, out obj) ? obj : defaultValue;
    }

    public bool TryGetProperty(string name, out object value)
    {
      value = (object) null;
      return this.Properties != null && this.Properties.TryGetValue(name, out value);
    }

    public void SetProperty(string name, object value) => this.Properties[name] = value;

    public bool RemoveProperty(string name) => this.Properties.Remove(name);
  }
}
