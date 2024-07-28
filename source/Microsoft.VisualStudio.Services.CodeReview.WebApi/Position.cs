// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Position
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class Position
  {
    [DataMember]
    public int Line { get; set; }

    [DataMember]
    public int Offset { get; set; }

    public bool Equals(Position position) => position != null && this.Line == position.Line && this.Offset == position.Offset;

    public override bool Equals(object obj) => this.Equals(obj as Position);

    public override int GetHashCode()
    {
      int num = this.Line;
      int hashCode1 = num.GetHashCode();
      num = this.Offset;
      int hashCode2 = num.GetHashCode();
      return hashCode1 ^ hashCode2;
    }
  }
}
