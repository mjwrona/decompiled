// Decompiled with JetBrains decompiler
// Type: Microsoft.CodeDom.Providers.DotNetCompilerPlatform.ICompilerSettings
// Assembly: Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Version=4.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 4D7629DB-EBA2-4B9A-A3BA-C2E83ED1F745
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.dll

using System;

namespace Microsoft.CodeDom.Providers.DotNetCompilerPlatform
{
  [Obsolete("ICompilerSettings is obsolete. Please update code to use IProviderOptions instead.", false)]
  public interface ICompilerSettings
  {
    string CompilerFullPath { get; }

    int CompilerServerTimeToLive { get; }
  }
}
