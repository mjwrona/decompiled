// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SQLServerPropertiesComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class SQLServerPropertiesComponent : TeamFoundationSqlResourceComponent
  {
    public List<KeyValuePair<string, string>> QuerySQLServerProperties(
      IEnumerable<string> propertyNames)
    {
      TeamFoundationDatabaseXmlWriter xmlWriter = new TeamFoundationDatabaseXmlWriter("serverProperties");
      int num = 1;
      foreach (string propertyName in propertyNames)
      {
        xmlWriter.WriteStartElement("p");
        xmlWriter.WriteAttributeString("n", propertyName);
        xmlWriter.WriteAttributeString("o", num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        xmlWriter.WriteEndElement();
      }
      this.PrepareStoredProcedure("prc_QuerySQLServerProperties");
      this.BindXml("@propertyNameXML", xmlWriter);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QuerySQLServerProperties", this.RequestContext);
      resultCollection.AddBinder<KeyValuePair<string, string>>((ObjectBinder<KeyValuePair<string, string>>) new SQLServerPropertiesComponent.SQLServerPropertyColumns());
      return resultCollection.GetCurrent<KeyValuePair<string, string>>().Items;
    }

    internal class SQLServerPropertyColumns : ObjectBinder<KeyValuePair<string, string>>
    {
      private SqlColumnBinder nameColumn = new SqlColumnBinder("Name");
      private SqlColumnBinder valueColumn = new SqlColumnBinder("Value");

      protected override KeyValuePair<string, string> Bind() => new KeyValuePair<string, string>(this.nameColumn.GetString((IDataReader) this.Reader, false), this.valueColumn.GetString((IDataReader) this.Reader, true));
    }
  }
}
