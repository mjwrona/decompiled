// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.TableResult
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

namespace Microsoft.Azure.Cosmos.Table
{
  public sealed class TableResult
  {
    public object Result { get; set; }

    public int HttpStatusCode { get; set; }

    public string Etag { get; set; }

    public string SessionToken { get; internal set; }

    public double? RequestCharge { get; internal set; }

    public string ActivityId { get; internal set; }
  }
}
