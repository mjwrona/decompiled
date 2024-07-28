// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results.ProductBacklogQueryResults
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.Results
{
  [DataContract]
  public class ProductBacklogQueryResults : QueryResultModel
  {
    private string m_teamFieldReferenceName;

    internal ProductBacklogQueryResults()
    {
    }

    internal ProductBacklogQueryResults(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary<string, int> map,
      ProductBacklogGridOptions options,
      bool runQuery,
      IDictionary queryContext = null)
      : this(requestContext, wiql, map, options, (string) null, runQuery, queryContext)
    {
    }

    internal ProductBacklogQueryResults(
      IVssRequestContext requestContext,
      string wiql,
      IDictionary<string, int> map,
      ProductBacklogGridOptions options,
      string teamFieldReferenceName,
      bool runQuery,
      IDictionary queryContext = null)
      : base(requestContext, wiql, map, runQuery, false, (string) null, runQuery, queryContext, skipWiqlTextLimitValidation: true)
    {
      this.ProductBacklogGridOptions = options;
      this.m_teamFieldReferenceName = teamFieldReferenceName;
    }

    [DataMember(Name = "productBacklogGridOptions", EmitDefaultValue = false)]
    public virtual ProductBacklogGridOptions ProductBacklogGridOptions { get; set; }

    [DataMember(Name = "ownedIds", EmitDefaultValue = false)]
    public int[] OwnedIds { get; set; }

    [DataMember(Name = "backlogQueryResultType", EmitDefaultValue = false)]
    public BacklogQueryResultType BacklogQueryResultType { get; set; }

    [DataMember(Name = "expandIds", EmitDefaultValue = false)]
    public int[] ExpandIds { get; set; }

    public void UpdatePayload(IVssRequestContext requestContext) => this.GeneratePayload(requestContext);

    protected override void UpdatePageColumns()
    {
      base.UpdatePageColumns();
      if (this.m_teamFieldReferenceName == null)
        return;
      this.PageColumns = (IEnumerable<string>) this.PageColumns.Union<string>((IEnumerable<string>) new string[2]
      {
        this.m_teamFieldReferenceName,
        CoreFieldReferenceNames.State
      }).ToArray<string>();
    }
  }
}
