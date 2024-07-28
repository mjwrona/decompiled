// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO.Segment
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas;
using System;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO
{
  public struct Segment
  {
    public int Length;
    public string Comment;
    public string SDL;
    public Namespace Schema;

    [Obsolete("Use object-model constructor instead.")]
    public Segment(string comment, string sdl)
    {
      this.Length = 0;
      this.Comment = comment;
      this.SDL = sdl;
      this.Schema = (Namespace) null;
    }

    public Segment(string comment, Namespace ns)
    {
      this.Length = 0;
      this.Comment = comment;
      this.SDL = (string) null;
      this.Schema = ns;
    }
  }
}
