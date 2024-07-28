// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.NameValueObjectCreator`1
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public class NameValueObjectCreator<T> : DefaultObjectCreator<T>
  {
    public NameValueObjectCreator(string name, string version, Type type)
      : base(type)
    {
      this.Name = name;
      this.Version = version;
    }

    public NameValueObjectCreator(string name, string version, T instance)
      : base(instance)
    {
      this.Name = name;
      this.Version = version;
    }
  }
}
