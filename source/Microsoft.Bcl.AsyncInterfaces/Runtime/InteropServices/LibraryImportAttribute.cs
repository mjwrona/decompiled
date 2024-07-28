// Decompiled with JetBrains decompiler
// Type: System.Runtime.InteropServices.LibraryImportAttribute
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

namespace System.Runtime.InteropServices
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  internal sealed class LibraryImportAttribute : Attribute
  {
    public LibraryImportAttribute(string libraryName) => this.LibraryName = libraryName;

    public string LibraryName { get; }

    public string EntryPoint { get; set; }

    public StringMarshalling StringMarshalling { get; set; }

    public Type StringMarshallingCustomType { get; set; }

    public bool SetLastError { get; set; }
  }
}
