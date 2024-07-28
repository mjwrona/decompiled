// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceAntiXssEncoder
// Assembly: Microsoft.VisualStudio.Services.Commerce.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A442E579-88AD-441C-B92A-FDB0C6C9E30B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.Common.dll

using System.Web.Security.AntiXss;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CommerceAntiXssEncoder : AntiXssEncoder
  {
    public new string UrlPathEncode(string value) => base.UrlPathEncode(value);
  }
}
