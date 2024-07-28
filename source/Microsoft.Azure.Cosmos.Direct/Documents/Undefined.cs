// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Undefined
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Documents
{
  internal sealed class Undefined : IEquatable<Undefined>
  {
    [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Undefined is infact immutable")]
    public static readonly Undefined Value = new Undefined();

    private Undefined()
    {
    }

    public bool Equals(Undefined other) => other != null;

    public override bool Equals(object other) => this.Equals(other as Undefined);

    public override int GetHashCode() => 0;
  }
}
