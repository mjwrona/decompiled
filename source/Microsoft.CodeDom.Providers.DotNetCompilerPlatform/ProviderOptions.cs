// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.ProviderOptions
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  public sealed class ProviderOptions : IProviderOptions, ICompilerSettings
  {
    private IDictionary<string, string> _allOptions;

    public ProviderOptions()
    {
      this.CompilerFullPath = (string) null;
      this.CompilerVersion = (string) null;
      this.WarnAsError = false;
      this.AllOptions = (IDictionary<string, string>) new ReadOnlyDictionary<string, string>((IDictionary<string, string>) new Dictionary<string, string>());
      this.CompilerServerTimeToLive = 0;
      this.UseAspNetSettings = false;
    }

    public ProviderOptions(IProviderOptions opts)
    {
      this.CompilerFullPath = opts.CompilerFullPath;
      this.CompilerServerTimeToLive = opts.CompilerServerTimeToLive;
      this.CompilerVersion = opts.CompilerVersion;
      this.WarnAsError = opts.WarnAsError;
      this.UseAspNetSettings = opts.UseAspNetSettings;
      this.AllOptions = (IDictionary<string, string>) new ReadOnlyDictionary<string, string>(opts.AllOptions);
    }

    public ProviderOptions(string compilerFullPath, int compilerServerTimeToLive)
      : this()
    {
      this.CompilerFullPath = compilerFullPath;
      this.CompilerServerTimeToLive = compilerServerTimeToLive;
    }

    internal ProviderOptions(ICompilerSettings settings)
      : this(settings.CompilerFullPath, settings.CompilerServerTimeToLive)
    {
    }

    public string CompilerFullPath { get; internal set; }

    public int CompilerServerTimeToLive { get; internal set; }

    public string CompilerVersion { get; internal set; }

    public bool WarnAsError { get; internal set; }

    public bool UseAspNetSettings { get; internal set; }

    public IDictionary<string, string> AllOptions
    {
      get => this._allOptions;
      internal set => this._allOptions = value != null ? (IDictionary<string, string>) new ReadOnlyDictionary<string, string>(value) : (IDictionary<string, string>) null;
    }
  }
}
