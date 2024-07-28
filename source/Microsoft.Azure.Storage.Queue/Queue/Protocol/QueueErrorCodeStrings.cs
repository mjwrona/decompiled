// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.Protocol.QueueErrorCodeStrings
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

namespace Microsoft.Azure.Storage.Queue.Protocol
{
  public static class QueueErrorCodeStrings
  {
    public static readonly string QueueNotFound = nameof (QueueNotFound);
    public static readonly string QueueDisabled = nameof (QueueDisabled);
    public static readonly string QueueAlreadyExists = nameof (QueueAlreadyExists);
    public static readonly string QueueNotEmpty = nameof (QueueNotEmpty);
    public static readonly string QueueBeingDeleted = nameof (QueueBeingDeleted);
    public static readonly string PopReceiptMismatch = nameof (PopReceiptMismatch);
    public static readonly string InvalidParameter = nameof (InvalidParameter);
    public static readonly string MessageNotFound = nameof (MessageNotFound);
    public static readonly string MessageTooLarge = nameof (MessageTooLarge);
    public static readonly string InvalidMarker = nameof (InvalidMarker);
  }
}
