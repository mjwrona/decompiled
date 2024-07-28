// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.TfsMessageQueueConstantsV2
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TfsMessageQueueConstantsV2
  {
    public const string Namespace = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2";
    public const string Acknowledge = "Acknowledge";
    public const string AcknowledgeAction = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Acknowledge";
    public const string AcknowledgeReplyAction = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/AcknowledgeResponse";
    public const string Dequeue = "Dequeue";
    public const string DequeueAction = "http://schemas.microsoft.com/2010/TeamFoundation/Framework/MessageQueue/2/Dequeue";
    public const string MessageId = "MessageId";
    public const string LastMessage = "LastMessage";
    public const string Acknowledgement = "Acknowledgement";
    public const string AcknowledgementRange = "AcknowledgementRange";
    public const string AcknowledgementRangeLower = "Lower";
    public const string AcknowledgementRangeUpper = "Upper";
  }
}
