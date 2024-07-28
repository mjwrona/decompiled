// Decompiled with JetBrains decompiler
// Type: Nest.CatNodeAttributesRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatNodeAttributesRecord : ICatRecord
  {
    [DataMember(Name = "attr")]
    public string Attribute { get; set; }

    [DataMember(Name = "host")]
    public string Host { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "ip")]
    public string Ip { get; set; }

    [DataMember(Name = "node")]
    public string Node { get; set; }

    [DataMember(Name = "port")]
    public long Port { get; set; }

    [DataMember(Name = "pid")]
    public long ProcessId { get; set; }

    [DataMember(Name = "value")]
    public string Value { get; set; }
  }
}
