// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.ScaleoutMessage
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class ScaleoutMessage
  {
    public ScaleoutMessage(IList<Message> messages)
    {
      this.Messages = messages;
      this.ServerCreationTime = DateTime.UtcNow;
    }

    public ScaleoutMessage()
    {
    }

    public IList<Message> Messages { get; set; }

    public DateTime ServerCreationTime { get; set; }

    public byte[] ToBytes()
    {
      using (MemoryStream output = new MemoryStream())
      {
        BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
        binaryWriter.Write(this.Messages.Count);
        for (int index = 0; index < this.Messages.Count; ++index)
          this.Messages[index].WriteTo((Stream) output);
        binaryWriter.Write(this.ServerCreationTime.Ticks);
        return output.ToArray();
      }
    }

    public static ScaleoutMessage FromBytes(byte[] data)
    {
      if (data == null)
        throw new ArgumentNullException(nameof (data));
      using (MemoryStream input = new MemoryStream(data))
      {
        BinaryReader binaryReader = new BinaryReader((Stream) input);
        ScaleoutMessage scaleoutMessage = new ScaleoutMessage();
        scaleoutMessage.Messages = (IList<Message>) new List<Message>();
        int num = binaryReader.ReadInt32();
        for (int index = 0; index < num; ++index)
          scaleoutMessage.Messages.Add(Message.ReadFrom((Stream) input));
        scaleoutMessage.ServerCreationTime = new DateTime(binaryReader.ReadInt64());
        return scaleoutMessage;
      }
    }
  }
}
