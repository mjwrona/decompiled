// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.RuleDependencyAttribute
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime
{
  [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
  internal sealed class RuleDependencyAttribute : Attribute
  {
    private readonly Type _recognizer;
    private readonly int _rule;
    private readonly int _version;
    private readonly Dependents _dependents;

    public RuleDependencyAttribute(Type recognizer, int rule, int version)
    {
      this._recognizer = recognizer;
      this._rule = rule;
      this._version = version;
      this._dependents = Dependents.Self | Dependents.Parents;
    }

    public RuleDependencyAttribute(Type recognizer, int rule, int version, Dependents dependents)
    {
      this._recognizer = recognizer;
      this._rule = rule;
      this._version = version;
      this._dependents = dependents | Dependents.Self;
    }

    public Type Recognizer => this._recognizer;

    public int Rule => this._rule;

    public int Version => this._version;

    public Dependents Dependents => this._dependents;
  }
}
