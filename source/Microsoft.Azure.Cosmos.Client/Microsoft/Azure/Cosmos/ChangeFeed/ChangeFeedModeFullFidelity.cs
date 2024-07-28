// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedModeFullFidelity
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedModeFullFidelity : ChangeFeedMode
  {
    public static ChangeFeedMode Instance { get; } = (ChangeFeedMode) new ChangeFeedModeFullFidelity();

    internal override void Accept(RequestMessage requestMessage)
    {
      requestMessage.UseGatewayMode = new bool?(true);
      requestMessage.Headers.Add("A-IM", "Full-Fidelity Feed");
      requestMessage.Headers.Add("x-ms-cosmos-changefeed-wire-format-version", Constants.ChangeFeedWireFormatVersions.SeparateMetadataWithCrts);
    }
  }
}
