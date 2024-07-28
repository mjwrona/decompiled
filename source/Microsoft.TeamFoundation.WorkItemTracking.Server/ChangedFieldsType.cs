// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ChangedFieldsType
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ChangedFieldsType
  {
    private IntegerField[] integerFieldsField;
    private StringField[] stringFieldsField;
    private BooleanField[] booleanFieldsField;

    [XmlArrayItem("Field", IsNullable = false)]
    public IntegerField[] IntegerFields
    {
      get => this.integerFieldsField;
      set => this.integerFieldsField = value;
    }

    [XmlArrayItem("Field", IsNullable = false)]
    public StringField[] StringFields
    {
      get => this.stringFieldsField;
      set => this.stringFieldsField = value;
    }

    [XmlArrayItem("Field", IsNullable = false)]
    public BooleanField[] BooleanFields
    {
      get => this.booleanFieldsField;
      set => this.booleanFieldsField = value;
    }
  }
}
