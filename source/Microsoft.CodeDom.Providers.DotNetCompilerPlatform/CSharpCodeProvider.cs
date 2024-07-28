// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  [DesignerCategory("code")]
  public sealed class CSharpCodeProvider : Microsoft.CSharp.CSharpCodeProvider
  {
    private readonly IProviderOptions _providerOptions;

    public CSharpCodeProvider()
      : this((IProviderOptions) null)
    {
    }

    [Obsolete("ICompilerSettings is obsolete. Please update code to use IProviderOptions instead.", false)]
    public CSharpCodeProvider(ICompilerSettings compilerSettings = null) => this._providerOptions = compilerSettings == null ? CompilationUtil.CSC2 : (IProviderOptions) new ProviderOptions(compilerSettings);

    public CSharpCodeProvider(IProviderOptions providerOptions = null) => this._providerOptions = providerOptions ?? CompilationUtil.CSC2;

    public CSharpCodeProvider(IDictionary<string, string> providerOptions)
      : this(CompilationUtil.CreateProviderOptions(providerOptions, CompilationUtil.CSC2))
    {
    }

    [Obsolete("Callers should not use the ICodeCompiler interface and should instead use the methods directly on the CodeDomProvider class.")]
    public override ICodeCompiler CreateCompiler() => (ICodeCompiler) new CSharpCompiler((CodeDomProvider) this, this._providerOptions);
  }
}
