// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding.EncodingBase
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding
{
  internal abstract class EncodingBase
  {
    private FormatCode formatCode;

    protected EncodingBase(FormatCode formatCode) => this.formatCode = formatCode;

    public FormatCode FormatCode => this.formatCode;

    public abstract int GetObjectEncodeSize(object value, bool arrayEncoding);

    public abstract void EncodeObject(object value, bool arrayEncoding, ByteBuffer buffer);

    public abstract object DecodeObject(ByteBuffer buffer, FormatCode formatCode);

    public static void VerifyFormatCode(FormatCode formatCode, FormatCode expected, int offset)
    {
      if (formatCode != expected)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) offset));
    }

    public static void VerifyFormatCode(
      FormatCode formatCode,
      int offset,
      params FormatCode[] expected)
    {
      bool flag = false;
      foreach (FormatCode formatCode1 in expected)
      {
        if (formatCode == formatCode1)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        throw AmqpEncoding.GetEncodingException(SRAmqp.AmqpInvalidFormatCode((object) formatCode, (object) offset));
    }
  }
}
