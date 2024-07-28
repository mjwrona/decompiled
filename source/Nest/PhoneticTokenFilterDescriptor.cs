// Decompiled with JetBrains decompiler
// Type: Nest.PhoneticTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PhoneticTokenFilterDescriptor : 
    TokenFilterDescriptorBase<PhoneticTokenFilterDescriptor, IPhoneticTokenFilter>,
    IPhoneticTokenFilter,
    ITokenFilter
  {
    protected override string Type => "phonetic";

    PhoneticEncoder? IPhoneticTokenFilter.Encoder { get; set; }

    IEnumerable<PhoneticLanguage> IPhoneticTokenFilter.LanguageSet { get; set; }

    int? IPhoneticTokenFilter.MaxCodeLength { get; set; }

    PhoneticNameType? IPhoneticTokenFilter.NameType { get; set; }

    bool? IPhoneticTokenFilter.Replace { get; set; }

    PhoneticRuleType? IPhoneticTokenFilter.RuleType { get; set; }

    public PhoneticTokenFilterDescriptor Replace(bool? replace = true) => this.Assign<bool?>(replace, (Action<IPhoneticTokenFilter, bool?>) ((a, v) => a.Replace = v));

    public PhoneticTokenFilterDescriptor Encoder(PhoneticEncoder? encoder) => this.Assign<PhoneticEncoder?>(encoder, (Action<IPhoneticTokenFilter, PhoneticEncoder?>) ((a, v) => a.Encoder = v));

    public PhoneticTokenFilterDescriptor MaxCodeLength(int? maxCodeLength) => this.Assign<int?>(maxCodeLength, (Action<IPhoneticTokenFilter, int?>) ((a, v) => a.MaxCodeLength = v));

    public PhoneticTokenFilterDescriptor RuleType(PhoneticRuleType? ruleType) => this.Assign<PhoneticRuleType?>(ruleType, (Action<IPhoneticTokenFilter, PhoneticRuleType?>) ((a, v) => a.RuleType = v));

    public PhoneticTokenFilterDescriptor NameType(PhoneticNameType? nameType) => this.Assign<PhoneticNameType?>(nameType, (Action<IPhoneticTokenFilter, PhoneticNameType?>) ((a, v) => a.NameType = v));

    public PhoneticTokenFilterDescriptor LanguageSet(params PhoneticLanguage[] languageSet) => this.Assign<PhoneticLanguage[]>(languageSet, (Action<IPhoneticTokenFilter, PhoneticLanguage[]>) ((a, v) => a.LanguageSet = (IEnumerable<PhoneticLanguage>) v));

    public PhoneticTokenFilterDescriptor LanguageSet(IEnumerable<PhoneticLanguage> languageSet) => this.Assign<IEnumerable<PhoneticLanguage>>(languageSet, (Action<IPhoneticTokenFilter, IEnumerable<PhoneticLanguage>>) ((a, v) => a.LanguageSet = v));
  }
}
