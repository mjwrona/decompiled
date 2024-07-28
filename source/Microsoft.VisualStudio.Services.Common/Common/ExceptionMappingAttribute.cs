// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.ExceptionMappingAttribute
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class ExceptionMappingAttribute : Attribute
  {
    public ExceptionMappingAttribute(
      string minApiVersion,
      string exclusiveMaxApiVersion,
      string typeKey,
      string typeName)
    {
      this.MinApiVersion = new Version(minApiVersion);
      this.ExclusiveMaxApiVersion = new Version(exclusiveMaxApiVersion);
      this.TypeKey = typeKey;
      this.TypeName = typeName;
    }

    public Version MinApiVersion { get; private set; }

    public Version ExclusiveMaxApiVersion { get; private set; }

    public string TypeKey { get; private set; }

    public string TypeName { get; private set; }
  }
}
