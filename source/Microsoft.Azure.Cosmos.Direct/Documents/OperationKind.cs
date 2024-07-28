// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.OperationKind
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents
{
  internal enum OperationKind
  {
    Invalid,
    Create,
    Replace,
    Delete,
    [Obsolete("This item is obsolete as it does not apply to Conflict.")] Read,
  }
}
