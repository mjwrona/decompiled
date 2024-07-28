// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.MediaIdHelper
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

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
      byte[] buffer;
      try
      {
        buffer = ResourceId.FromBase64String(mediaId);
      }
      catch (FormatException ex)
      {
        return false;
      }
      if (buffer.Length != (int) ResourceId.Length && buffer.Length != (int) ResourceId.Length + 1)
        return false;
      if (buffer.Length == (int) ResourceId.Length)
      {
        storageIndex = (byte) 0;
        attachmentId = mediaId;
        return true;
      }
      storageIndex = buffer[buffer.Length - 1];
      attachmentId = ResourceId.ToBase64String(buffer, 0, (int) ResourceId.Length);
      return true;
    }
  }
}
