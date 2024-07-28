// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Rntbd.ServerKey
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Rntbd
{
  internal sealed class ServerKey
  {
    public ServerKey(Uri uri)
    {
      this.Server = uri.DnsSafeHost;
      this.Port = uri.Port;
    }

    public string Server { get; private set; }

    public int Port { get; private set; }

    public override string ToString() => string.Format("{0}:{1}", (object) this.Server, (object) this.Port);

    public override bool Equals(object obj) => obj != null && this.Equals(obj as ServerKey);

    public bool Equals(ServerKey key) => key != null && this.Server.Equals(key.Server) && this.Port == key.Port;

    public override int GetHashCode() => ((580324321 ^ this.Server.GetHashCode()) * 302869537 ^ ServerKey.HashInt32(this.Port)) * 302869537;

    private static int HashInt32(int key)
    {
      int num = 266758603;
      for (int index = 0; index < 4; ++index)
      {
        num = (num ^ key & (int) byte.MaxValue) * 646844749;
        key >>= 8;
      }
      return num;
    }
  }
}
