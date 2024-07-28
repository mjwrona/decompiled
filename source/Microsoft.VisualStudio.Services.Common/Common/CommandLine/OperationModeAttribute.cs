// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OperationModeAttribute
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class OperationModeAttribute : Attribute
  {
    public static StringComparison DefaultCaseSensitivity = StringComparison.OrdinalIgnoreCase;

    public OperationModeAttribute() => this.CaseSensitivity = OperationModeAttribute.DefaultCaseSensitivity;

    public OperationModeAttribute(string name)
      : this()
    {
      this.Name = name;
    }

    public OperationModeAttribute(string name, StringComparison caseSensitivity)
      : this(name)
    {
      this.CaseSensitivity = caseSensitivity;
    }

    public StringComparison CaseSensitivity { get; set; }

    public bool IsDefault { get; set; }

    public string Name { get; set; }

    public IEnumerable<string> Split()
    {
      Collection<string> collection = new Collection<string>();
      if (!string.IsNullOrWhiteSpace(this.Name))
      {
        string name = this.Name;
        char[] separator = new char[1]{ ' ' };
        foreach (string str in name.Split(separator, StringSplitOptions.RemoveEmptyEntries))
          collection.Add(str.Trim());
      }
      return (IEnumerable<string>) collection;
    }
  }
}
