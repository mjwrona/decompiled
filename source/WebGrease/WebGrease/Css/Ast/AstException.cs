// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.Ast.AstException
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Runtime.Serialization;

namespace WebGrease.Css.Ast
{
  [Serializable]
  public class AstException : Exception
  {
    public AstException()
    {
    }

    public AstException(string message)
      : base(message)
    {
    }

    public AstException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected AstException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
