// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.JsonColumnConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  internal class JsonColumnConverter : JavaScriptConverter
  {
    private const string NameAttribute = "name";
    private const string WidthAttribute = "width";
    private const string NotAFieldAttribute = "notafield";
    private const string RollupAttribute = "rollup";
    private const string RollupCalculationAttribute = "rollupCalculation";

    public override IEnumerable<Type> SupportedTypes
    {
      get
      {
        yield return typeof (Column);
      }
    }

    public override IDictionary<string, object> Serialize(
      object obj,
      JavaScriptSerializer serializer)
    {
      if (!(obj is Column column))
        throw new ArgumentNullException(nameof (obj));
      return (IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "name",
          (object) column.FieldName
        },
        {
          "width",
          (object) column.ColumnWidth
        },
        {
          "notafield",
          (object) column.NotAField
        },
        {
          "rollup",
          (object) column.Rollup
        },
        {
          "rollupCalculation",
          (object) column.RollupCalculation
        }
      };
    }

    public override object Deserialize(
      IDictionary<string, object> dictionary,
      Type type,
      JavaScriptSerializer serializer)
    {
      if (dictionary == null)
        throw new ArgumentNullException(nameof (dictionary));
      if (!(type == typeof (Column)))
        return (object) null;
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>(dictionary, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Column column = new Column()
      {
        FieldName = dictionary1["name"] as string
      };
      if (dictionary1.ContainsKey("width"))
        column.ColumnWidth = (int) Convert.ToDouble(dictionary1["width"]);
      if (dictionary1.ContainsKey("notafield"))
        column.NotAField = Convert.ToBoolean(dictionary1["notafield"]);
      if (dictionary1.ContainsKey("rollup"))
        column.Rollup = Convert.ToBoolean(dictionary1["rollup"]);
      if (dictionary1.ContainsKey("rollupCalculation"))
        column.RollupCalculation = RollupUtils.GetRollupCalculation((Dictionary<string, object>) dictionary1["rollupCalculation"]);
      return (object) column;
    }
  }
}
