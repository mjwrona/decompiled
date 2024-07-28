// Decompiled with JetBrains decompiler
// Type: Nest.StoredScriptDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class StoredScriptDescriptor : 
    DescriptorBase<StoredScriptDescriptor, IStoredScript>,
    IStoredScript
  {
    string IStoredScript.Lang { get; set; }

    string IStoredScript.Source { get; set; }

    public StoredScriptDescriptor Source(string source) => this.Assign<string>(source, (Action<IStoredScript, string>) ((a, v) => a.Source = v));

    public StoredScriptDescriptor Lang(string lang) => this.Assign<string>(lang, (Action<IStoredScript, string>) ((a, v) => a.Lang = v));

    public StoredScriptDescriptor Lang(ScriptLang lang) => this.Assign<string>(lang.GetStringValue(), (Action<IStoredScript, string>) ((a, v) => a.Lang = v));
  }
}
