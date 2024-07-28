// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ExpressionException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Logging;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  public class ExpressionException : Exception
  {
    private readonly string m_message;

    internal ExpressionException(ISecretMasker secretMasker, string message)
    {
      if (secretMasker != null)
        message = secretMasker.MaskSecrets(message);
      this.m_message = message;
    }

    public override string Message => this.m_message;
  }
}
