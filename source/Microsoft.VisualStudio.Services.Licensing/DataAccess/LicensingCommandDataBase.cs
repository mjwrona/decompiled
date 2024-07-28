// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.LicensingCommandDataBase
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal abstract class LicensingCommandDataBase : LicensingEventDataBase
  {
    [JsonIgnore]
    public override string EventDataType { get; set; }

    [JsonIgnore]
    public EventDataDescriptor EventDataDescriptor { get; set; }

    public string Description => this.GetType().Name;

    protected LicensingCommandDataBase()
    {
      this.EventDataDescriptor = new EventDataDescriptor()
      {
        DataType = this.GetType().AssemblyQualifiedName
      };
      this.EventDataType = this.EventDataDescriptor.Serialize<EventDataDescriptor>();
    }
  }
}
