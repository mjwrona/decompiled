// Decompiled with JetBrains decompiler
// Type: Nest.CatHealthDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatHealthDescriptor : 
    RequestDescriptorBase<CatHealthDescriptor, CatHealthRequestParameters, ICatHealthRequest>,
    ICatHealthRequest,
    IRequest<CatHealthRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatHealth;

    public CatHealthDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatHealthDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatHealthDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatHealthDescriptor IncludeTimestamp(bool? includetimestamp = true) => this.Qs("ts", (object) includetimestamp);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatHealthDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.5.0, reason: Removed from the server as it was never a valid option")]
    public CatHealthDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public CatHealthDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatHealthDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
