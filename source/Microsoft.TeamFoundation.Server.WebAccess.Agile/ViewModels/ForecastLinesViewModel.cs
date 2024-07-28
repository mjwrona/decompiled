// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.ForecastLinesViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class ForecastLinesViewModel
  {
    [DataMember(Name = "visibleState", EmitDefaultValue = true)]
    public string VisibleState { get; set; }

    [DataMember(Name = "effortData", EmitDefaultValue = true)]
    public EffortDataViewModel EffortData { get; set; }

    [DataMember(Name = "velocity", EmitDefaultValue = true)]
    public int Velocity { get; set; }
  }
}
