// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.IterationContext
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class IterationContext
  {
    [DataMember]
    public short FirstComparingIteration { get; set; }

    [DataMember]
    public short SecondComparingIteration { get; set; }

    public bool Equals(IterationContext iterationContext) => iterationContext != null && (int) this.FirstComparingIteration == (int) iterationContext.FirstComparingIteration && (int) this.SecondComparingIteration == (int) iterationContext.SecondComparingIteration;

    public override bool Equals(object obj) => this.Equals(obj as IterationContext);

    public override int GetHashCode()
    {
      short comparingIteration = this.FirstComparingIteration;
      int hashCode1 = comparingIteration.GetHashCode();
      comparingIteration = this.SecondComparingIteration;
      int hashCode2 = comparingIteration.GetHashCode();
      return hashCode1 ^ hashCode2;
    }
  }
}
