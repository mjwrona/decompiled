// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.ProductBacklogGridOptions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  [DataContract]
  public class ProductBacklogGridOptions
  {
    [DataMember(Name = "showOrderColumn", EmitDefaultValue = true)]
    public bool ShowOrderColumn { get; set; }

    [DataMember(Name = "enableReorder", EmitDefaultValue = true)]
    public bool EnableReorder { get; set; }

    [DataMember(Name = "enableReparent", EmitDefaultValue = true)]
    public bool EnableReparent { get; set; }

    [DataMember(Name = "enableForecast", EmitDefaultValue = true)]
    public bool EnableForecast { get; set; }

    [DataMember(Name = "columnOptionsKey", EmitDefaultValue = true)]
    public string ColumnOptionsKey { get; set; }
  }
}
