// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.StringField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class StringField
  {
    private string nameField;
    private string referenceNameField;
    private string oldValueField;
    private string newValueField;
    private string oldValueLocalField;
    private string newValueLocalField;
    private bool isIdentity;

    public string Name
    {
      get => this.nameField;
      set => this.nameField = value;
    }

    public string ReferenceName
    {
      get => this.referenceNameField;
      set => this.referenceNameField = value;
    }

    public string OldValue
    {
      get => this.oldValueField;
      set => this.oldValueField = value;
    }

    public string NewValue
    {
      get => this.newValueField;
      set => this.newValueField = value;
    }

    public string OldLocalValue
    {
      get => this.oldValueLocalField;
      set => this.oldValueLocalField = value;
    }

    public string NewLocalValue
    {
      get => this.newValueLocalField;
      set => this.newValueLocalField = value;
    }

    public bool IsIdentity
    {
      get => this.isIdentity;
      set => this.isIdentity = value;
    }
  }
}
