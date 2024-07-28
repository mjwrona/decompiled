// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.QueryResultPayload
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class QueryResultPayload
  {
    public QueryResultPayload()
    {
    }

    public QueryResultPayload(
      IVssRequestContext requestContext,
      GenericDataReader dataReader,
      bool omitHeaders,
      IList<int> orderedWorkItemIds = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<GenericDataReader>(dataReader, nameof (dataReader));
      int columnCount = dataReader.FieldCount;
      string[] array = Enumerable.Range(0, columnCount).Select<int, string>((System.Func<int, string>) (i => dataReader.GetName(i))).ToArray<string>();
      if (!omitHeaders)
        this.Columns = (IEnumerable<string>) array;
      this.Rows = (IEnumerable<object[]>) dataReader.Select<IDataRecord, object[]>((System.Func<IDataRecord, object[]>) (record =>
      {
        object[] objArray = new object[columnCount];
        record.GetValues(objArray);
        return ((IEnumerable<object>) objArray).Select<object, object>((System.Func<object, object>) (value => value)).ToArray<object>();
      })).ToArray<object[]>();
      if (orderedWorkItemIds == null || !orderedWorkItemIds.Any<int>())
        return;
      this.Rows = (IEnumerable<object[]>) this.Rows.OrderBy<object[], int>((System.Func<object[], int>) (x => orderedWorkItemIds.IndexOf((int) x[0]))).ToArray<object[]>();
    }

    [DataMember(Name = "columns", EmitDefaultValue = false)]
    public IEnumerable<string> Columns { get; set; }

    [DataMember(Name = "rows", EmitDefaultValue = false)]
    public IEnumerable<object[]> Rows { get; set; }
  }
}
