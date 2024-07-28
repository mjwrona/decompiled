// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Server.Common.Filter
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8190F04D-5888-4DB5-A838-8C98A67C6E45
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Server.Common
{
  public struct Filter
  {
    public readonly string Name;
    public readonly string Value;

    public Filter(string name, string value)
    {
      Filter.ValidateName(name);
      this.Name = name;
      this.Value = value;
    }

    private static void ValidateName(string name)
    {
      if (name.IndexOfAny(new char[3]{ ':', '-', '_' }) >= 0)
        throw new ArgumentException("Name contains illegal character in name: " + name);
    }
  }
}
