// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.InheritLevel
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Location
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public enum InheritLevel : byte
  {
    None = 0,
    Deployment = 1,
    Account = 2,
    Collection = 4,
    All = 7,
  }
}
