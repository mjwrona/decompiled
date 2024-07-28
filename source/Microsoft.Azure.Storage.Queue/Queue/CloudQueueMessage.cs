// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Queue.CloudQueueMessage
// Assembly: Microsoft.Azure.Storage.Queue, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 3D35BFA0-638A-4C3C-8E74-B592D3B60EFD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Queue.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Azure.Storage.Queue
{
  public sealed class CloudQueueMessage
  {
    private const long MaximumMessageSize = 65536;
    private static readonly TimeSpan MaximumVisibilityTimeout = TimeSpan.FromDays(7.0);
    private const int MaximumNumberOfMessagesToPeek = 32;
    private static UTF8Encoding utf8Encoder = new UTF8Encoding(false, true);

    [Obsolete("Use SetMessageContent2(byte[])")]
    public void SetMessageContent(byte[] content) => this.SetMessageContent2(content);

    public static long MaxMessageSize => 65536;

    public static TimeSpan MaxVisibilityTimeout => CloudQueueMessage.MaximumVisibilityTimeout;

    public static int MaxNumberOfMessagesToPeek => 32;

    internal CloudQueueMessage()
    {
    }

    public CloudQueueMessage(byte[] content) => this.SetMessageContent2(content);

    public CloudQueueMessage(string messageId, string popReceipt)
    {
      this.Id = messageId;
      this.PopReceipt = popReceipt;
    }

    public CloudQueueMessage(string content, bool isBase64Encoded = false) => this.SetMessageContent2(content, isBase64Encoded);

    public byte[] AsBytes
    {
      get
      {
        if (this.MessageType == QueueMessageType.RawString)
          return Encoding.UTF8.GetBytes(this.RawString);
        return this.MessageType == QueueMessageType.Base64Encoded ? Convert.FromBase64String(this.RawString) : this.RawBytes;
      }
    }

    public string Id { get; internal set; }

    public string PopReceipt { get; internal set; }

    public DateTimeOffset? InsertionTime { get; internal set; }

    public DateTimeOffset? ExpirationTime { get; internal set; }

    public DateTimeOffset? NextVisibleTime { get; internal set; }

    public string AsString
    {
      get
      {
        if (this.MessageType == QueueMessageType.RawString)
          return this.RawString;
        if (this.MessageType != QueueMessageType.Base64Encoded)
          return CloudQueueMessage.utf8Encoder.GetString(this.RawBytes, 0, this.RawBytes.Length);
        byte[] bytes = Convert.FromBase64String(this.RawString);
        return CloudQueueMessage.utf8Encoder.GetString(bytes, 0, bytes.Length);
      }
    }

    public int DequeueCount { get; internal set; }

    internal QueueMessageType MessageType { get; private set; }

    internal string RawString { get; set; }

    internal byte[] RawBytes { get; set; }

    internal string GetMessageContentForTransfer(
      bool shouldEncodeMessage,
      QueueRequestOptions options = null)
    {
      if (!shouldEncodeMessage && this.MessageType != QueueMessageType.RawString)
        throw new ArgumentException("EncodeMessage should be true for binary message.");
      if (options != null)
      {
        options.AssertPolicyIfRequired();
        if (options.EncryptionPolicy != null)
        {
          string contentForTransfer = options.EncryptionPolicy.EncryptMessage(this.AsBytes);
          if ((long) contentForTransfer.Length <= CloudQueueMessage.MaxMessageSize)
            return contentForTransfer;
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Encrypted Messages cannot be larger than {0} bytes. Please note that encrypting queue messages can increase their size.", (object) CloudQueueMessage.MaxMessageSize));
        }
      }
      string s;
      if (this.MessageType != QueueMessageType.Base64Encoded)
      {
        if (shouldEncodeMessage)
        {
          s = Convert.ToBase64String(this.AsBytes);
          if ((long) s.Length > CloudQueueMessage.MaxMessageSize)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Messages cannot be larger than {0} bytes.", (object) CloudQueueMessage.MaxMessageSize));
        }
        else
        {
          s = this.RawString;
          if ((long) Encoding.UTF8.GetBytes(s).Length > CloudQueueMessage.MaxMessageSize)
            throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Messages cannot be larger than {0} bytes.", (object) CloudQueueMessage.MaxMessageSize));
        }
      }
      else
      {
        s = this.RawString;
        if ((long) s.Length > CloudQueueMessage.MaxMessageSize)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Messages cannot be larger than {0} bytes.", (object) CloudQueueMessage.MaxMessageSize));
      }
      return s;
    }

    [Obsolete("Use SetMessageContent2(string, false)")]
    public void SetMessageContent(string content) => this.SetMessageContent2(content, false);

    public void SetMessageContent2(byte[] content)
    {
      this.RawBytes = content;
      this.RawString = (string) null;
      this.MessageType = QueueMessageType.RawBytes;
    }

    public void SetMessageContent2(string content, bool isBase64Encoded)
    {
      this.RawBytes = (byte[]) null;
      this.RawString = content ?? string.Empty;
      this.MessageType = isBase64Encoded ? QueueMessageType.Base64Encoded : QueueMessageType.RawString;
    }
  }
}
