// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.RuleVersionAttribute
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  internal sealed class RuleVersionAttribute : Attribute
  {
    private readonly int _version;

    public RuleVersionAttribute(int version) => this._version = version;

    public int Version => this._version;
  }
}
