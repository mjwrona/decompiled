// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.HybridRowHeader
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow
{
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public readonly struct HybridRowHeader
  {
    public const int Size = 5;

    public HybridRowHeader(HybridRowVersion version, SchemaId schemaId)
    {
      this.Version = version;
      this.SchemaId = schemaId;
    }

    public HybridRowVersion Version { get; }

    public SchemaId SchemaId { get; }
  }
}
