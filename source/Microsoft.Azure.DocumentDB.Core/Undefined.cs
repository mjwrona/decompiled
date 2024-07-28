// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Undefined
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Azure.Documents
{
  public sealed class Undefined : IEquatable<Undefined>
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
