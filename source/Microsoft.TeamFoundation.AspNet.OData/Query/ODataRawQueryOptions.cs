// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.ODataRawQueryOptions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

namespace Microsoft.AspNet.OData.Query
{
  public class ODataRawQueryOptions
  {
    public string Filter { get; internal set; }

    public string Apply { get; internal set; }

    public string OrderBy { get; internal set; }

    public string Top { get; internal set; }

    public string Skip { get; internal set; }

    public string Select { get; internal set; }

    public string Expand { get; internal set; }

    public string Count { get; internal set; }

    public string Format { get; internal set; }

    public string SkipToken { get; internal set; }

    public string DeltaToken { get; internal set; }

    public string Compute { get; internal set; }
  }
}
