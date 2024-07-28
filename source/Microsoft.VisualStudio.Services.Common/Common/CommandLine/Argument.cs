// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.Argument
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [DebuggerDisplay("{Option != null ? /Option.Name + \":\" : \"(positional)\",nq}{Value != null ? Value.ToString() : \"\",nq}")]
  public class Argument
  {
    public Argument(object value) => this.Value = value;

    public Argument(Option description) => this.Option = description;

    public Argument(object value, OptionMetadata metadata)
      : this(value)
    {
      this.Metadata = metadata;
    }

    public Argument(Option description, OptionMetadata metadata)
      : this(description)
    {
      this.Metadata = metadata;
    }

    public Argument(Option description, object value)
      : this(description)
    {
      this.Value = value;
    }

    public Argument(Option description, object value, OptionMetadata metadata)
      : this(description, metadata)
    {
      this.Value = value;
    }

    public Option Option { get; private set; }

    public OptionMetadata Metadata { get; internal set; }

    public object Value { get; set; }
  }
}
