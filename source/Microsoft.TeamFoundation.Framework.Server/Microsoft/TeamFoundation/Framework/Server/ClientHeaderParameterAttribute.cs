// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ClientHeaderParameterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public sealed class ClientHeaderParameterAttribute : Attribute
  {
    public ClientHeaderParameterAttribute(
      string header,
      Type parameterType,
      string parameterName,
      string parameterDescription,
      bool isOptional,
      bool isSensitive = false)
    {
      this.Header = header;
      this.ParameterType = parameterType;
      this.ParameterName = parameterName;
      this.ParameterDescription = parameterDescription;
      this.IsOptional = isOptional;
      this.IsSensitive = isSensitive;
    }

    public string Header { get; }

    public Type ParameterType { get; }

    public string ParameterName { get; }

    public string ParameterDescription { get; }

    public bool IsOptional { get; }

    public bool IsSensitive { get; }
  }
}
