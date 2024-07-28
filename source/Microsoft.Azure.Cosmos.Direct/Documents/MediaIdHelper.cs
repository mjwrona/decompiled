// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MediaIdHelper
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal sealed class MediaIdHelper
  {
    public static string NewMediaId(string attachmentId, byte storageIndex)
    {
      if (storageIndex == (byte) 0)
        return attachmentId;
      ResourceId resourceId = ResourceId.Parse(attachmentId);
      byte[] buffer = new byte[(int) ResourceId.Length + 1];
      resourceId.Value.CopyTo((Array) buffer, 0);
      buffer[buffer.Length - 1] = storageIndex;
      return ResourceId.ToBase64String(buffer);
    }

    public static bool TryParseMediaId(
      string mediaId,
      out string attachmentId,
      out byte storageIndex)
    {
      storageIndex = (byte) 0;
      attachmentId = string.Empty;
      byte[] bytes;
      if (!ResourceId.TryDecodeFromBase64String(mediaId, out bytes) || bytes.Length != (int) ResourceId.Length && bytes.Length != (int) ResourceId.Length + 1)
        return false;
      if (bytes.Length == (int) ResourceId.Length)
      {
        storageIndex = (byte) 0;
        attachmentId = mediaId;
        return true;
      }
      storageIndex = bytes[bytes.Length - 1];
      attachmentId = ResourceId.ToBase64String(bytes, 0, (int) ResourceId.Length);
      return true;
    }
  }
}
