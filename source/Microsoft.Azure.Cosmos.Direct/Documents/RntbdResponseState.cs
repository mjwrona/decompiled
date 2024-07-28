// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdResponseState
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Documents
{
  internal class RntbdResponseState
  {
    private static readonly string[] StateNames = Enum.GetNames(typeof (RntbdResponseStateEnum));
    private RntbdResponseStateEnum state;
    private int headerAndMetadataRead;
    private int bodyRead;
    private DateTimeOffset lastReadTime;

    public RntbdResponseState()
    {
      this.state = RntbdResponseStateEnum.NotStarted;
      this.headerAndMetadataRead = 0;
      this.bodyRead = 0;
      this.lastReadTime = DateTimeOffset.MinValue;
    }

    public void SetState(RntbdResponseStateEnum newState) => this.state = newState >= this.state && newState <= RntbdResponseStateEnum.Done ? newState : throw new InternalServerErrorException();

    public void AddHeaderMetadataRead(int amountRead)
    {
      this.headerAndMetadataRead += amountRead;
      this.lastReadTime = DateTimeOffset.Now;
    }

    public void AddBodyRead(int amountRead)
    {
      this.bodyRead += amountRead;
      this.lastReadTime = DateTimeOffset.Now;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "State: {0}. Meta bytes read: {1}. Body bytes read: {2}. Last read completion: {3}", (object) RntbdResponseState.StateNames[(int) this.state], (object) this.headerAndMetadataRead, (object) this.bodyRead, (object) this.lastReadTime);
  }
}
