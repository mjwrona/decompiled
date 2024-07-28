// Decompiled with JetBrains decompiler
// Type: Nest.HttpInputResponseResult
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class HttpInputResponseResult
  {
    [DataMember(Name = "body")]
    public string Body { get; set; }

    [DataMember(Name = "headers")]
    public IDictionary<string, string[]> Headers { get; set; }

    [DataMember(Name = "status")]
    public int StatusCode { get; set; }
  }
}
