// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AcknowledgementRange
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DataContract(Namespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2")]
  public sealed class AcknowledgementRange
  {
    internal int QueueId { get; set; }

    [DataMember]
    public long Lower { get; set; }

    [DataMember]
    public long Upper { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "AcknowledgementRange[{0}, {1}]", (object) this.Lower, (object) this.Upper);
  }
}
