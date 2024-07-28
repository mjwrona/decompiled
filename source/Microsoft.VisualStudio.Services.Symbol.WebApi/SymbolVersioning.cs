// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Symbol.WebApi.SymbolVersioning
// Assembly: Microsoft.VisualStudio.Services.Symbol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52CDDA61-EF2D-4AD8-A25B-09A8F04FE8C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Symbol.WebApi.dll

using Microsoft.VisualStudio.Services.Content.Common;

namespace Microsoft.VisualStudio.Services.Symbol.WebApi
{
  public static class SymbolVersioning
  {
    public static readonly string VersionAndComments = FileVersionHelpers<SymbolVersioning.DummyClass>.GetAssemblyVersionAndComments();
    public static readonly string AssemblyVersion = FileVersionHelpers<SymbolVersioning.DummyClass>.GetAssemblyVersion();

    private sealed class DummyClass
    {
    }
  }
}
