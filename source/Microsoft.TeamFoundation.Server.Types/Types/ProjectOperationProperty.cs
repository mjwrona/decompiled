// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ProjectOperationProperty
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

namespace Microsoft.TeamFoundation.Server.Types
{
  public class ProjectOperationProperty
  {
    public ProjectOperationProperty()
    {
    }

    public ProjectOperationProperty(string name, object value)
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; set; }

    public object Value { get; set; }
  }
}
