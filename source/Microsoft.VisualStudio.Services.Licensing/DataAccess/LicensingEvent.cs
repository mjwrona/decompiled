// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.DataAccess.LicensingEvent
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using System;

namespace Microsoft.VisualStudio.Services.Licensing.DataAccess
{
  internal class LicensingEvent : ILicensingEvent
  {
    private const string s_eventTypeFamily = "LicensingEvent";
    private static readonly Version s_eventTypeVersion = new Version(1, 0);

    public string EventTypeFamily => new
    {
      Family = nameof (LicensingEvent),
      Version = LicensingEvent.s_eventTypeVersion.ToString(2)
    }.Serialize();

    public string EventTypeDescriptor => this.EventData.EventDataType;

    public ILicensingEventData EventData { get; internal set; }

    public DateTimeOffset TimeStamp { get; internal set; }
  }
}
