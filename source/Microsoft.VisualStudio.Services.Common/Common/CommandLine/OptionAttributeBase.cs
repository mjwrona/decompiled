// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.OptionAttributeBase
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  [DebuggerDisplay("{Name,nq}")]
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
  public abstract class OptionAttributeBase : Attribute
  {
    public static Type DefaultValueConverterType = typeof (NoValueConverter);
    private Type valueConverterType;

    protected OptionAttributeBase() => this.ConverterType = OptionAttributeBase.DefaultValueConverterType;

    public Type ConverterType
    {
      get => this.valueConverterType;
      set
      {
        if (value == (Type) null)
        {
          this.valueConverterType = OptionAttributeBase.DefaultValueConverterType;
        }
        else
        {
          ValueConverter.ValidateConverterType(value);
          this.valueConverterType = value;
        }
      }
    }

    public string Name { get; set; }

    public abstract Option ToOption(IValueConvertible valueConverter = null);

    internal static bool RequiresCollectionMember(OptionAttributeBase attribute)
    {
      bool flag = false;
      switch (attribute)
      {
        case OptionAttribute optionAttribute:
          flag = optionAttribute.AllowMultiple;
          break;
        case PositionalOptionAttribute _:
          flag = true;
          break;
      }
      return flag;
    }
  }
}
