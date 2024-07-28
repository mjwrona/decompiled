// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiAnalyzerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class KuromojiAnalyzerDescriptor : 
    AnalyzerDescriptorBase<KuromojiAnalyzerDescriptor, IKuromojiAnalyzer>,
    IKuromojiAnalyzer,
    IAnalyzer
  {
    protected override string Type => "kuromoji";

    KuromojiTokenizationMode? IKuromojiAnalyzer.Mode { get; set; }

    string IKuromojiAnalyzer.UserDictionary { get; set; }

    public KuromojiAnalyzerDescriptor Mode(KuromojiTokenizationMode? mode) => this.Assign<KuromojiTokenizationMode?>(mode, (Action<IKuromojiAnalyzer, KuromojiTokenizationMode?>) ((a, v) => a.Mode = v));

    public KuromojiAnalyzerDescriptor UserDictionary(string userDictionary) => this.Assign<string>(userDictionary, (Action<IKuromojiAnalyzer, string>) ((a, v) => a.UserDictionary = v));
  }
}
