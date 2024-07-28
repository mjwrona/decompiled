// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.WindowsNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.Azure.NotificationHubs
{
  public sealed class WindowsNotification : Notification, INativeNotification
  {
    private const string WnsTypeName = "X-WNS-Type";
    private const string Raw = "wns/raw";
    private const string Badge = "wns/badge";
    private const string Tile = "wns/tile";
    private const string Toast = "wns/toast";

    public WindowsNotification(XmlDocument payLoad)
      : this(payLoad, (IDictionary<string, string>) null)
    {
    }

    public WindowsNotification(string payLoad)
      : this(payLoad, (IDictionary<string, string>) null)
    {
    }

    public WindowsNotification(XmlDocument payLoad, IDictionary<string, string> wnsHeaders)
      : this(payLoad.InnerXml, wnsHeaders)
    {
    }

    public WindowsNotification(string payLoad, IDictionary<string, string> wnsHeaders)
      : base(wnsHeaders, (string) null)
    {
      this.Body = !string.IsNullOrWhiteSpace(payLoad) ? payLoad : throw new ArgumentNullException(nameof (payLoad));
    }

    [Obsolete("This method is obsolete.")]
    public WindowsNotification(XmlDocument payLoad, string tag)
      : this(payLoad.InnerXml, (IDictionary<string, string>) null, tag)
    {
    }

    [Obsolete("This method is obsolete.")]
    public WindowsNotification(string payLoad, string tag)
      : this(payLoad, (IDictionary<string, string>) null, tag)
    {
    }

    [Obsolete("This method is obsolete.")]
    public WindowsNotification(
      XmlDocument payLoad,
      IDictionary<string, string> wnsHeaders,
      string tag)
      : this(payLoad.InnerXml, wnsHeaders, tag)
    {
    }

    [Obsolete("This method is obsolete.")]
    public WindowsNotification(string payLoad, IDictionary<string, string> wnsHeaders, string tag)
      : base(wnsHeaders, tag)
    {
      this.Body = !string.IsNullOrWhiteSpace(payLoad) ? payLoad : throw new ArgumentNullException(nameof (payLoad));
    }

    protected override string PlatformType => "windows";

    protected override void OnValidateAndPopulateHeaders()
    {
      if (this.Headers.ContainsKey("X-WNS-Type") && this.Headers["X-WNS-Type"].Equals("wns/raw", StringComparison.OrdinalIgnoreCase))
      {
        this.AddNotificationTypeHeader(WindowsTemplateBodyType.Raw);
        this.ContentType = "application/octet-stream";
      }
      else
      {
        this.AddNotificationTypeHeader(RegistrationSDKHelper.DetectWindowsTemplateRegistationType(this.Body, SRClient.NotSupportedXMLFormatAsPayload));
        this.Body = RegistrationSDKHelper.AddDeclarationToXml(this.Body);
      }
    }

    private void AddNotificationTypeHeader(WindowsTemplateBodyType bodyType)
    {
      switch (bodyType)
      {
        case WindowsTemplateBodyType.Toast:
          this.AddOrUpdateHeader("X-WNS-Type", "wns/toast");
          break;
        case WindowsTemplateBodyType.Tile:
          this.AddOrUpdateHeader("X-WNS-Type", "wns/tile");
          break;
        case WindowsTemplateBodyType.Badge:
          this.AddOrUpdateHeader("X-WNS-Type", "wns/badge");
          break;
        case WindowsTemplateBodyType.Raw:
          this.AddOrUpdateHeader("X-WNS-Type", "wns/raw");
          break;
      }
    }
  }
}
