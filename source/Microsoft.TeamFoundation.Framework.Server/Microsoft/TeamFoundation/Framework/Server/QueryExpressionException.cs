// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.QueryExpressionException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;
using System.Security;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public abstract class QueryExpressionException : TeamFoundationServiceException
  {
    private int m_parseIndex;

    protected QueryExpressionException(string message, int parseIndex)
      : base(message)
    {
      this.m_parseIndex = parseIndex;
    }

    protected QueryExpressionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.m_parseIndex = info.GetInt32(nameof (m_parseIndex));
    }

    [SecurityCritical]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("m_parseIndex", this.m_parseIndex);
    }
  }
}
