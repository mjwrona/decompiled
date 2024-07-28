// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.SequenceNumber
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Globalization;
using System.Threading;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp
{
  internal struct SequenceNumber : IComparable<SequenceNumber>, IEquatable<SequenceNumber>
  {
    private int sequenceNumber;

    public SequenceNumber(uint value) => this.sequenceNumber = (int) value;

    public uint Value => (uint) this.sequenceNumber;

    public static SequenceNumber Increment(ref int sn) => (SequenceNumber) (uint) Interlocked.Increment(ref sn);

    public static int Compare(int x, int y)
    {
      int num = x - y;
      return num != int.MinValue ? num : throw new InvalidOperationException(SRAmqp.AmqpInvalidSequenceNumberComparison((object) x, (object) y));
    }

    public int CompareTo(SequenceNumber value) => SequenceNumber.Compare(this.sequenceNumber, value.sequenceNumber);

    public bool Equals(SequenceNumber obj) => this.sequenceNumber == obj.sequenceNumber;

    public static implicit operator SequenceNumber(uint value) => new SequenceNumber(value);

    public static SequenceNumber operator +(SequenceNumber value1, int delta) => (SequenceNumber) (uint) (value1.sequenceNumber + delta);

    public static SequenceNumber operator -(SequenceNumber value1, int delta) => (SequenceNumber) (uint) (value1.sequenceNumber - delta);

    public static int operator -(SequenceNumber value1, SequenceNumber value2) => value1.sequenceNumber - value2.sequenceNumber;

    public static bool operator ==(SequenceNumber value1, SequenceNumber value2) => value1.sequenceNumber == value2.sequenceNumber;

    public static bool operator !=(SequenceNumber value1, SequenceNumber value2) => value1.sequenceNumber != value2.sequenceNumber;

    public static bool operator >(SequenceNumber value1, SequenceNumber value2) => value1.CompareTo(value2) > 0;

    public static bool operator >=(SequenceNumber value1, SequenceNumber value2) => value1.CompareTo(value2) >= 0;

    public static bool operator <(SequenceNumber value1, SequenceNumber value2) => value1.CompareTo(value2) < 0;

    public static bool operator <=(SequenceNumber value1, SequenceNumber value2) => value1.CompareTo(value2) <= 0;

    public uint Increment() => (uint) ++this.sequenceNumber;

    public override int GetHashCode() => this.sequenceNumber.GetHashCode();

    public override bool Equals(object obj) => obj is SequenceNumber sequenceNumber && this.Equals(sequenceNumber);

    public override string ToString() => this.sequenceNumber.ToString((IFormatProvider) CultureInfo.InvariantCulture);
  }
}
