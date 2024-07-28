// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Transports.PersistentResponse
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNet.SignalR.Json;
using Microsoft.AspNet.SignalR.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.SignalR.Transports
{
  public sealed class PersistentResponse : IJsonWritable
  {
    private readonly Func<Message, bool> _exclude;
    private readonly Action<TextWriter> _writeCursor;

    public PersistentResponse()
      : this((Func<Message, bool>) (message => false), (Action<TextWriter>) (writer => { }))
    {
    }

    public PersistentResponse(Func<Message, bool> exclude, Action<TextWriter> writeCursor)
    {
      this._exclude = exclude;
      this._writeCursor = writeCursor;
    }

    public IList<ArraySegment<Message>> Messages { get; set; }

    public bool Terminal { get; set; }

    public int TotalCount { get; set; }

    public bool Initializing { get; set; }

    public bool Aborted { get; set; }

    public bool Reconnect { get; set; }

    public string GroupsToken { get; set; }

    public long? LongPollDelay { get; set; }

    void IJsonWritable.WriteJson(TextWriter writer)
    {
      JsonTextWriter jsonWriter = writer != null ? new JsonTextWriter(writer) : throw new ArgumentNullException(nameof (writer));
      jsonWriter.WriteStartObject();
      writer.Write('"');
      writer.Write("C");
      writer.Write('"');
      writer.Write(':');
      writer.Write('"');
      this._writeCursor(writer);
      writer.Write('"');
      writer.Write(',');
      if (this.Initializing)
      {
        jsonWriter.WritePropertyName("S");
        jsonWriter.WriteValue(1);
      }
      if (this.Reconnect)
      {
        jsonWriter.WritePropertyName("T");
        jsonWriter.WriteValue(1);
      }
      if (this.GroupsToken != null)
      {
        jsonWriter.WritePropertyName("G");
        jsonWriter.WriteValue(this.GroupsToken);
      }
      if (this.LongPollDelay.HasValue)
      {
        jsonWriter.WritePropertyName("L");
        jsonWriter.WriteValue(this.LongPollDelay.Value);
      }
      jsonWriter.WritePropertyName("M");
      jsonWriter.WriteStartArray();
      this.WriteMessages(writer, jsonWriter);
      jsonWriter.WriteEndArray();
      jsonWriter.WriteEndObject();
    }

    private void WriteMessages(TextWriter writer, JsonTextWriter jsonWriter)
    {
      if (this.Messages == null)
        return;
      IBinaryWriter binaryWriter = writer as IBinaryWriter;
      bool flag = true;
      for (int index = 0; index < this.Messages.Count; ++index)
      {
        ArraySegment<Message> message1 = this.Messages[index];
        for (int offset = message1.Offset; offset < message1.Offset + message1.Count; ++offset)
        {
          Message message2 = message1.Array[offset];
          if (!message2.IsCommand && !this._exclude(message2))
          {
            if (binaryWriter != null)
            {
              if (!flag)
                writer.Write(',');
              binaryWriter.Write(message2.Value);
              flag = false;
            }
            else
              jsonWriter.WriteRawValue(message2.GetString());
          }
        }
      }
    }
  }
}
