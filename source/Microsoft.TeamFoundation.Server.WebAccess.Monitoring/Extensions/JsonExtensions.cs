// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Extensions.JsonExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Monitoring, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2931506-B8BC-4923-B99C-2CD8E1087ABB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Monitoring.dll

using System;
using System.IO;
using System.Web.UI.DataVisualization.Charting;

namespace Microsoft.TeamFoundation.Server.WebAccess.Monitoring.Extensions
{
  public static class JsonExtensions
  {
    public static JsObject ToJson(this Chart chart)
    {
      string name = Guid.NewGuid().ToString();
      using (MemoryStream imageStream = new MemoryStream(2048))
      {
        chart.SaveImage((Stream) imageStream, ChartImageFormat.Png);
        imageStream.Seek(0L, SeekOrigin.Begin);
        JsObject json = new JsObject();
        json.Add("imageData", (object) ("data:image/png;base64," + Convert.ToBase64String(imageStream.ToArray())));
        json.Add("imageMap", (object) chart.GetHtmlImageMap(name));
        json.Add("imageMapName", (object) name);
        return json;
      }
    }
  }
}
