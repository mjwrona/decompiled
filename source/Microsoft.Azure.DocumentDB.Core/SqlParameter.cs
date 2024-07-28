// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.SqlParameter
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents
{
  [DataContract]
  public sealed class SqlParameter
  {
    private string name;
    private object value;

    public SqlParameter()
    {
    }

    public SqlParameter(string name) => this.name = name;

    public SqlParameter(string name, object value)
    {
      this.name = name;
      this.value = value;
    }

    [DataMember(Name = "name")]
    public string Name
    {
      get => this.name;
      set => this.name = value;
    }

    [DataMember(Name = "value")]
    public object Value
    {
      get => this.value;
      set => this.value = value;
    }
  }
}
