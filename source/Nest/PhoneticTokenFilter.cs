// Decompiled with JetBrains decompiler
// Type: Nest.PhoneticTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PhoneticTokenFilter : TokenFilterBase, IPhoneticTokenFilter, ITokenFilter
  {
    public PhoneticTokenFilter()
      : base("phonetic")
    {
    }

    public PhoneticEncoder? Encoder { get; set; }

    public IEnumerable<PhoneticLanguage> LanguageSet { get; set; }

    public int? MaxCodeLength { get; set; }

    public PhoneticNameType? NameType { get; set; }

    public bool? Replace { get; set; }

    public PhoneticRuleType? RuleType { get; set; }
  }
}
