// Decompiled with JetBrains decompiler
// Type: WebGrease.Css.ImageAssemblyAnalysis.ImageAssembleException
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace WebGrease.Css.ImageAssemblyAnalysis
{
  [Serializable]
  public class ImageAssembleException : Exception
  {
    public ImageAssembleException()
    {
    }

    public ImageAssembleException(string message)
      : base(message)
    {
    }

    public ImageAssembleException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    internal ImageAssembleException(string imageName, string spriteName, string message)
      : base(message)
    {
      this.ImageName = imageName;
      this.SpriteName = spriteName;
    }

    internal ImageAssembleException(
      string imageName,
      string spriteName,
      string message,
      Exception innerException)
      : base(message, innerException)
    {
      this.ImageName = imageName;
      this.SpriteName = spriteName;
    }

    protected ImageAssembleException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ImageName = info != null ? info.GetString(nameof (ImageName)) : throw new ArgumentNullException(nameof (info));
      this.SpriteName = info.GetString(nameof (SpriteName));
    }

    public string ImageName { get; private set; }

    public string SpriteName { get; private set; }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
        throw new ArgumentNullException(nameof (info));
      base.GetObjectData(info, context);
      info.AddValue("ImageName", (object) this.ImageName);
      info.AddValue("SpriteName", (object) this.SpriteName);
    }
  }
}
