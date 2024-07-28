// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTrackingData
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationTrackingData
  {
    private JObject m_data;

    public NotificationTrackingData()
    {
    }

    public NotificationTrackingData(string encodedData) => this.m_data = JObject.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(encodedData)));

    public NotificationTrackingData(JObject data) => this.Data = data;

    public JToken GetValue(string propertyName) => this.Data[propertyName];

    public void SetValue(string name, JToken value)
    {
      if (value == null)
        this.Data.Remove(name);
      else
        this.Data[name] = value;
    }

    public string GetEncodedString(bool includeAdvanced = false)
    {
      JObject jobject = this.Data;
      if (!includeAdvanced)
      {
        jobject = (JObject) jobject.DeepClone();
        jobject.Remove("_xci");
      }
      JsonSerializerSettings settings = new JsonSerializerSettings();
      return Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) jobject, settings)));
    }

    private JObject Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = new JObject();
        return this.m_data;
      }
      set => this.m_data = value;
    }
  }
}
