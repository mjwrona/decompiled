// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.AcknowledgementRange
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Client
{
  [DataContract(Namespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2")]
  internal sealed class AcknowledgementRange
  {
    private AcknowledgementRange()
    {
    }

    public AcknowledgementRange(long messageId)
      : this(messageId, messageId)
    {
    }

    public AcknowledgementRange(long lower, long upper)
    {
      this.Lower = lower;
      this.Upper = upper;
    }

    [DataMember]
    public long Lower { get; private set; }

    [DataMember]
    public long Upper { get; private set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AcknowledgementRange[{0}, {1}]", (object) this.Lower, (object) this.Upper);
  }
}
