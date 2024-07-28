// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.TemplateNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class TemplateNotification : Notification
  {
    private IDictionary<string, string> templateProperties;

    public TemplateNotification(IDictionary<string, string> templateProperties)
      : base((IDictionary<string, string>) null, (string) null)
    {
      this.templateProperties = templateProperties != null ? templateProperties : throw new ArgumentNullException("properties");
    }

    [Obsolete("This method is obsolete.")]
    public TemplateNotification(IDictionary<string, string> templateProperties, string tag)
      : base((IDictionary<string, string>) null, tag)
    {
      this.templateProperties = templateProperties != null ? templateProperties : throw new ArgumentNullException("properties");
    }

    protected override string PlatformType => "template";

    protected override void OnValidateAndPopulateHeaders() => this.Body = new JavaScriptSerializer().Serialize((object) this.templateProperties);
  }
}
