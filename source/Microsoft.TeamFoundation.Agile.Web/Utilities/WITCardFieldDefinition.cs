// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.WITCardFieldDefinition
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public class WITCardFieldDefinition : ICardFieldDefinition
  {
    private FieldDefinition m_fieldDefinition;

    public string Name => this.m_fieldDefinition.Name;

    public string ReferenceName => this.m_fieldDefinition.ReferenceName;

    public int Id => this.m_fieldDefinition.Id;

    public int Type => (int) this.m_fieldDefinition.FieldType;

    public bool IsEditable => this.m_fieldDefinition.IsEditable;

    public bool IsIdentity => this.m_fieldDefinition.IsIdentity;

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["Id"] = (object) this.Id;
      json["Name"] = (object) this.Name;
      json["ReferenceName"] = (object) this.ReferenceName;
      json["Type"] = (object) this.Type;
      json["IsEditable"] = (object) this.IsEditable;
      json["IsIdentity"] = (object) this.IsIdentity;
      return json;
    }

    public WITCardFieldDefinition(FieldDefinition definition) => this.m_fieldDefinition = definition;
  }
}
