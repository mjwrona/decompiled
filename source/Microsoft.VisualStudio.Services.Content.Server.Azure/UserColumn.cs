// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.UserColumn
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class UserColumn : IUserColumn, IColumn
  {
    public UserColumn(string name)
    {
      if (name.Equals(PartitionKeyColumn.Instance.Name, StringComparison.Ordinal) || name.Equals(RowKeyColumn.Instance.Name, StringComparison.Ordinal))
        throw new ArgumentException(name + " cannot be used as a user column.", nameof (name));
      this.Name = name;
    }

    public string Name { get; private set; }
  }
}
