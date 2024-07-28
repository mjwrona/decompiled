// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.WebApi.WidgetResponse
// Assembly: Microsoft.TeamFoundation.Dashboards.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 05786B5F-D2ED-4F72-80F1-EA5660131542
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Dashboards.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Dashboards.WebApi
{
  [DataContract]
  public class WidgetResponse : Widget
  {
    public WidgetResponse()
    {
    }

    public WidgetResponse(Widget w, string url)
    {
      this.Url = url;
      this.Id = w.Id;
      this.Name = w.Name;
      this.Position = w.Position;
      this.Size = w.Size;
      this.ContributionId = w.ContributionId;
      this.Settings = w.Settings;
      this.ArtifactId = w.ArtifactId;
      this.SettingsVersion = w.SettingsVersion;
      this.ETag = w.ETag;
    }
  }
}
