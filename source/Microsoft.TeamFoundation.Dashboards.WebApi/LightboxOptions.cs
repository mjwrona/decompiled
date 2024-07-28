// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.LightboxOptions
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  public class LightboxOptions : DashboardSecuredObject
  {
    public int? Width;
    public int? Height;
    public bool Resizable;

    public LightboxOptions(int? width, int? height, bool? resizable)
    {
      this.Width = width;
      this.Height = height;
      if (resizable.HasValue)
        this.Resizable = resizable.Value;
      else
        this.Resizable = false;
    }
  }
}
