// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTypeExtensionComponent2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTypeExtensionComponent2 : WorkItemTypeExtensionComponent
  {
    public override IEnumerable<IGrouping<Guid, string>> GetWorkItemCategoryDetailsForWITExtensionReconciliation(
      IList<int> categoryIds,
      IList<int> categoryMemberIds,
      IList<Tuple<int, string>> categoryReferenceNames)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemCategoryDetailsForWITExtensionReconciliation");
      this.BindInt32Table("@categoryIds", (IEnumerable<int>) categoryIds);
      this.BindInt32Table("@categoryMemberIds", (IEnumerable<int>) categoryMemberIds);
      this.BindInt32StringTable("@categoryReferenceNames", (IEnumerable<Tuple<int, string>>) categoryReferenceNames);
      IDataReader dataReader = this.ExecuteReader();
      List<Tuple<Guid, string>> source = new List<Tuple<Guid, string>>();
      while (dataReader.Read())
      {
        string g = dataReader.GetString(0);
        string str = dataReader.GetString(1);
        source.Add(new Tuple<Guid, string>(new Guid(g), str));
      }
      return source.GroupBy<Tuple<Guid, string>, Guid, string>((System.Func<Tuple<Guid, string>, Guid>) (tuple => tuple.Item1), (System.Func<Tuple<Guid, string>, string>) (tuple => tuple.Item2));
    }
  }
}
