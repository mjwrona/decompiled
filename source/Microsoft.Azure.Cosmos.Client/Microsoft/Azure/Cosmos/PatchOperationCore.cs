// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PatchOperationCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class PatchOperationCore : PatchOperation
  {
    public PatchOperationCore(PatchOperationType operationType, string path)
    {
      this.OperationType = operationType;
      this.Path = !string.IsNullOrWhiteSpace(path) ? path : throw new ArgumentNullException(nameof (path));
    }

    public override PatchOperationType OperationType { get; }

    public override string Path { get; }
  }
}
