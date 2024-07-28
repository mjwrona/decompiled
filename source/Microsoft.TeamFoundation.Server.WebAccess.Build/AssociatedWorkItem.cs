// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Build.AssociatedWorkItem
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Build, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5B90139F-AF48-436C-9A4F-5104A3D8571F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Build.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Build
{
  public class AssociatedWorkItem
  {
    public int Id { get; set; }

    public int ParentId { get; set; }

    public string Type { get; set; }

    public string ParentPath { get; set; }

    public JsObject ToJson()
    {
      int num = 0;
      if (!string.IsNullOrEmpty(this.ParentPath))
        num = this.ParentPath.Split('\\').Length - 1;
      JsObject json = new JsObject();
      json.Add("id", (object) this.Id);
      json.Add("parentId", (object) this.ParentId);
      json.Add("parentPath", (object) this.ParentPath);
      json.Add("type", (object) this.Type);
      json.Add("indent", (object) num);
      return json;
    }
  }
}
