// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ExtensionUtilityTypeCreateException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ExtensionUtilityTypeCreateException : ExtensionUtilityException
  {
    public ExtensionUtilityTypeCreateException(string message, Type type, Exception innerException)
      : base(message, innerException)
    {
      this.Type = type;
    }

    public ExtensionUtilityTypeCreateException(Type type, Exception innerException)
      : base("Failed to instantiate type " + type.FullName + ". Check InnerException for details.", innerException)
    {
      this.Type = type;
    }

    protected ExtensionUtilityTypeCreateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public Type Type { get; }
  }
}
