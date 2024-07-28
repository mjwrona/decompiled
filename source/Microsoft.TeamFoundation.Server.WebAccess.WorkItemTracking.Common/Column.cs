// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Column
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  public class Column
  {
    public Column()
    {
    }

    public Column(
      string fieldName,
      int width,
      bool notAField = false,
      bool rollup = false,
      RollupCalculation rollupCalculation = null)
    {
      this.FieldName = fieldName;
      this.ColumnWidth = width;
      this.NotAField = notAField;
      this.Rollup = rollup;
      this.RollupCalculation = rollupCalculation;
    }

    [XmlAttribute(AttributeName = "refname")]
    public string FieldName { get; set; }

    [XmlAttribute(AttributeName = "width")]
    public int ColumnWidth { get; set; }

    [XmlAttribute(AttributeName = "notafield")]
    public bool NotAField { get; set; }

    [XmlAttribute(AttributeName = "rollup")]
    public bool Rollup { get; set; }

    public RollupCalculation RollupCalculation { get; set; }
  }
}
