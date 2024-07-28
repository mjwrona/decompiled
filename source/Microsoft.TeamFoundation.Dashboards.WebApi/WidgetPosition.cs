// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetPosition
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class WidgetPosition : DashboardSecuredObject
  {
    [DataMember]
    public int Row { get; set; }

    [DataMember]
    public int Column { get; set; }

    public WidgetPosition Clone() => new WidgetPosition()
    {
      Row = this.Row,
      Column = this.Column
    };

    public bool IsPositioned() => this.Row != 0 || this.Column != 0;
  }
}
