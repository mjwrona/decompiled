// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing.Multiple`1
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Messaging.Amqp.Encoding;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.NotificationHubs.Messaging.Amqp.Framing
{
  internal sealed class Multiple<T>
  {
    private List<T> value;

    public Multiple() => this.value = new List<T>();

    public Multiple(IList<T> value) => this.value = new List<T>((IEnumerable<T>) value);

    public void Add(T item) => this.value.Add(item);

    public bool Contains(T item) => this.value.Contains(item);

    public static int GetEncodeSize(Multiple<T> multiple)
    {
      if (multiple == null)
        return 1;
      return multiple.value.Count == 1 ? AmqpEncoding.GetObjectEncodeSize((object) multiple.value[0]) : ArrayEncoding.GetEncodeSize<T>(multiple.value.ToArray());
    }

    public static void Encode(Multiple<T> multiple, ByteBuffer buffer)
    {
      if (multiple == null)
        AmqpEncoding.EncodeNull(buffer);
      else if (multiple.value.Count == 1)
        AmqpEncoding.EncodeObject((object) multiple.value[0], buffer);
      else
        ArrayEncoding.Encode<T>(multiple.value.ToArray(), buffer);
    }

    public static Multiple<T> Decode(ByteBuffer buffer)
    {
      object obj1 = AmqpEncoding.DecodeObject(buffer);
      if (obj1 == null)
        return (Multiple<T>) null;
      if (obj1 is T obj2)
      {
        Multiple<T> multiple = new Multiple<T>();
        multiple.Add(obj2);
        return multiple;
      }
      return obj1.GetType().IsArray ? new Multiple<T>((IList<T>) (T[]) obj1) : throw new AmqpException(AmqpError.InvalidField);
    }

    public static IList<T> Intersect(Multiple<T> multiple1, Multiple<T> multiple2)
    {
      List<T> objList = new List<T>();
      if (multiple1 == null || multiple2 == null)
        return (IList<T>) objList;
      foreach (T obj in multiple1.value)
      {
        if (multiple2.value.Contains(obj))
          objList.Add(obj);
      }
      return (IList<T>) objList;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder("[");
      bool flag = true;
      foreach (T obj1 in this.value)
      {
        object obj2 = (object) obj1;
        if (!flag)
          stringBuilder.Append(',');
        stringBuilder.Append(obj2.ToString());
        flag = false;
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }
  }
}
