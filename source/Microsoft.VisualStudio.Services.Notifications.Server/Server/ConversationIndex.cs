// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ConversationIndex
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class ConversationIndex
  {
    private ConversationIndex()
    {
    }

    public byte[] Bytes { get; private set; }

    public Guid Id { get; private set; }

    public DateTime Time { get; private set; }

    public string ToBase64() => Convert.ToBase64String(this.Bytes);

    public override string ToString() => string.Format("{0} {1} {2}", (object) this.Id, (object) this.Time, (object) this.ToBase64());

    public static ConversationIndex Create() => ConversationIndex.FromId(Guid.NewGuid());

    public static ConversationIndex FromId(Guid id) => ConversationIndex.FromId(id, DateTime.UtcNow);

    public static ConversationIndex FromId(Guid id, DateTime time)
    {
      long fileTimeUtc = DateTime.UtcNow.ToFileTimeUtc();
      time = DateTime.FromFileTimeUtc(fileTimeUtc & 9223372036837998592L);
      ConversationIndex conversationIndex = new ConversationIndex()
      {
        Id = id,
        Time = time
      };
      byte[] numArray = new byte[22];
      numArray[0] = (byte) 1;
      numArray[1] = (byte) ((ulong) (fileTimeUtc >> 48) & (ulong) byte.MaxValue);
      numArray[2] = (byte) ((ulong) (fileTimeUtc >> 40) & (ulong) byte.MaxValue);
      numArray[3] = (byte) ((ulong) (fileTimeUtc >> 32) & (ulong) byte.MaxValue);
      numArray[4] = (byte) ((ulong) (fileTimeUtc >> 24) & (ulong) byte.MaxValue);
      numArray[5] = (byte) ((ulong) (fileTimeUtc >> 16) & (ulong) byte.MaxValue);
      byte[] byteArray = id.ToByteArray();
      for (int index = 0; index < byteArray.Length; ++index)
        numArray[6 + index] = byteArray[index];
      conversationIndex.Bytes = numArray;
      return conversationIndex;
    }

    public static ConversationIndex FromBase64(string base64Value) => ConversationIndex.FromBytes(Convert.FromBase64String(base64Value));

    public static ConversationIndex FromBytes(byte[] bytes)
    {
      ConversationIndex conversationIndex = (bytes != null ? (bytes.Length != 22 ? 1 : 0) : 1) == 0 ? new ConversationIndex()
      {
        Bytes = bytes
      } : throw new ArgumentException(nameof (bytes));
      long num = 1;
      for (int index = 1; index <= 5; ++index)
        num = (num << 8) + (long) bytes[index];
      long fileTime = num << 16;
      conversationIndex.Time = DateTime.FromFileTimeUtc(fileTime);
      byte[] numArray = new byte[16];
      Array.Copy((Array) bytes, 6, (Array) numArray, 0, 16);
      conversationIndex.Id = new Guid(numArray);
      return conversationIndex;
    }
  }
}
