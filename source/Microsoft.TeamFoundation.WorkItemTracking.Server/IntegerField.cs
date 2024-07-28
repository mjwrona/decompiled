// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.IntegerField
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class IntegerField
  {
    private string nameField;
    private string referenceNameField;
    private int oldValueField;
    private int newValueField;

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

    public int OldValue
    {
      get => this.oldValueField;
      set => this.oldValueField = value;
    }

    public int NewValue
    {
      get => this.newValueField;
      set => this.newValueField = value;
    }

    public bool IsOldValueEmpty { get; set; }

    public bool IsNewValueEmpty { get; set; }

    public bool ShouldSerializeIsOldValueEmpty() => this.IsOldValueEmpty;

    public bool ShouldSerializeIsNewValueEmpty() => this.IsNewValueEmpty;
  }
}
