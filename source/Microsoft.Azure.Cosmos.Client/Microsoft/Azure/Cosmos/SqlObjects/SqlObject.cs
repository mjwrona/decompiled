// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlObject
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal abstract class SqlObject : IEquatable<SqlObject>
  {
    public abstract void Accept(SqlObjectVisitor visitor);

    public abstract TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor);

    public abstract TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input);

    public override string ToString() => this.Serialize(false);

    public override int GetHashCode() => this.Accept<int>((SqlObjectVisitor<int>) SqlObjectHasher.Singleton);

    public override bool Equals(object obj)
    {
      SqlObject other = obj as SqlObject;
      return (object) other != null && this.Equals(other);
    }

    public bool Equals(SqlObject other) => SqlObject.Equals(this, other);

    public string PrettyPrint() => this.Serialize(true);

    public SqlObject GetObfuscatedObject() => this.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) new SqlObjectObfuscator());

    private string Serialize(bool prettyPrint)
    {
      SqlObjectTextSerializer visitor = new SqlObjectTextSerializer(prettyPrint);
      this.Accept((SqlObjectVisitor) visitor);
      return visitor.ToString();
    }

    public static bool Equals(SqlObject first, SqlObject second)
    {
      if ((object) first == (object) second)
        return true;
      return (object) first != null && (object) second != null && first.Accept<SqlObject, bool>((SqlObjectVisitor<SqlObject, bool>) SqlObjectEqualityVisitor.Singleton, second);
    }

    public static bool operator ==(SqlObject first, SqlObject second) => SqlObject.Equals(first, second);

    public static bool operator !=(SqlObject first, SqlObject second) => !(first == second);
  }
}
