// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.MpnsNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class MpnsNotification : Notification, INativeNotification
  {
    public MpnsNotification(XmlDocument payLoad)
      : this(payLoad, (IDictionary<string, string>) null)
    {
    }

    [Obsolete("This method is obsolete.")]
    public MpnsNotification(XmlDocument payLoad, string tag)
      : this(payLoad, (IDictionary<string, string>) null, tag)
    {
    }

    public MpnsNotification(string payLoad)
      : this(payLoad, (IDictionary<string, string>) null)
    {
    }

    [Obsolete("This method is obsolete.")]
    public MpnsNotification(string payLoad, string tag)
      : this(payLoad, (IDictionary<string, string>) null, tag)
    {
    }

    public MpnsNotification(XmlDocument payLoad, IDictionary<string, string> mpnsHeaders)
      : this(payLoad.InnerXml, mpnsHeaders)
    {
    }

    public MpnsNotification(string payLoad, IDictionary<string, string> mpnsHeaders)
      : base(mpnsHeaders, (string) null)
    {
      this.Body = !string.IsNullOrWhiteSpace(payLoad) ? payLoad : throw new ArgumentNullException(nameof (payLoad));
    }

    [Obsolete("This method is obsolete.")]
    public MpnsNotification(
      XmlDocument payLoad,
      IDictionary<string, string> mpnsHeaders,
      string tag)
      : this(payLoad.InnerXml, mpnsHeaders, tag)
    {
    }

    [Obsolete("This method is obsolete.")]
    public MpnsNotification(string payLoad, IDictionary<string, string> mpnsHeaders, string tag)
      : base(mpnsHeaders, tag)
    {
      this.Body = !string.IsNullOrWhiteSpace(payLoad) ? payLoad : throw new ArgumentNullException(nameof (payLoad));
    }

    protected override string PlatformType => "windowsphone";

    protected override void OnValidateAndPopulateHeaders()
    {
      if (this.Headers.ContainsKey("X-NotificationClass"))
        return;
      this.AddNotificationTypeHeader(RegistrationSDKHelper.DetectMpnsTemplateRegistationType(this.Body, SRClient.NotSupportedXMLFormatAsPayloadForMpns));
      this.Body = RegistrationSDKHelper.AddDeclarationToXml(this.Body);
    }

    private void AddNotificationTypeHeader(MpnsTemplateBodyType bodyType)
    {
      switch (bodyType)
      {
        case MpnsTemplateBodyType.Toast:
          this.AddOrUpdateHeader("X-WindowsPhone-Target", "toast");
          this.AddOrUpdateHeader("X-NotificationClass", "2");
          break;
        case MpnsTemplateBodyType.Tile:
          this.AddOrUpdateHeader("X-WindowsPhone-Target", "token");
          this.AddOrUpdateHeader("X-NotificationClass", "1");
          break;
        case MpnsTemplateBodyType.Raw:
          this.AddOrUpdateHeader("X-NotificationClass", "3");
          break;
      }
    }
  }
}
