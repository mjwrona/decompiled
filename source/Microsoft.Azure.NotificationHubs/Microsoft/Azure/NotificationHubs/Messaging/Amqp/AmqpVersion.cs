// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.AmqpVersion
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal struct AmqpVersion : IEquatable<AmqpVersion>
  {
    private byte major;
    private byte minor;
    private byte revision;

    public AmqpVersion(byte major, byte minor, byte revision)
    {
      this.major = major;
      this.minor = minor;
      this.revision = revision;
    }

    public AmqpVersion(Version version)
      : this((byte) version.Major, (byte) version.Minor, (byte) version.Revision)
    {
    }

    public byte Major => this.major;

    public byte Minor => this.minor;

    public byte Revision => this.revision;

    public bool Equals(AmqpVersion other) => (int) this.Major == (int) other.Major && (int) this.Minor == (int) other.Minor;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", new object[3]
    {
      (object) this.Major,
      (object) this.Minor,
      (object) this.Revision
    });
  }
}
