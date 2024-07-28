// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas.SchemaException
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Schemas
{
  [ExcludeFromCodeCoverage]
  [Serializable]
  public sealed class SchemaException : Exception
  {
    public SchemaException()
    {
    }

    public SchemaException(string message)
      : base(message)
    {
    }

    public SchemaException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    private SchemaException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
