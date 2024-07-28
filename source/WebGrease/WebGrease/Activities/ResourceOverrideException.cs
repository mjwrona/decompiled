// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.ResourceOverrideException
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Runtime.Serialization;

namespace WebGrease.Activities
{
  [Serializable]
  public class ResourceOverrideException : Exception
  {
    public ResourceOverrideException()
    {
    }

    public ResourceOverrideException(string message)
      : base(message)
    {
    }

    public ResourceOverrideException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public ResourceOverrideException(string fileName, string tokenKey)
    {
      this.TokenKey = tokenKey;
      this.FileName = fileName;
    }

    protected ResourceOverrideException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public string FileName { get; private set; }

    public string TokenKey { get; private set; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      info.AddValue("FileName", (object) (this.FileName ?? string.Empty));
      info.AddValue("TokenKey", (object) (this.TokenKey ?? string.Empty));
      base.GetObjectData(info, context);
    }
  }
}
