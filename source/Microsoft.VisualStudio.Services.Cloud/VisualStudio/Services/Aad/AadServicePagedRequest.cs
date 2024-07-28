// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadServicePagedRequest
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Aad
{
  public abstract class AadServicePagedRequest : AadServiceRequest
  {
    public virtual int? MaxResults { get; set; }

    public virtual string PagingToken { get; set; }
  }
}
