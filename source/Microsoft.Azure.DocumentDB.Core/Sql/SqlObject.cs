// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObject
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlObject
  {
    protected SqlObject(SqlObjectKind kind) => this.Kind = kind;

    public SqlObjectKind Kind { get; }

    public abstract void Accept(SqlObjectVisitor visitor);

    public abstract TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor);

    public abstract TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input);

    public override string ToString() => this.Serialize(false);

    public override int GetHashCode() => this.Accept<int>((SqlObjectVisitor<int>) SqlObjectHasher.Singleton);

    public string PrettyPrint() => this.Serialize(true);

    public SqlObject GetObfuscatedObject() => this.Accept<SqlObject>((SqlObjectVisitor<SqlObject>) new SqlObjectObfuscator());

    private string Serialize(bool prettyPrint)
    {
      SqlObjectTextSerializer visitor = new SqlObjectTextSerializer(prettyPrint);
      this.Accept((SqlObjectVisitor) visitor);
      return visitor.ToString();
    }
  }
}
