// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.VssJsonPatchMediaTypeFormatter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class VssJsonPatchMediaTypeFormatter : VssJsonMediaTypeFormatter
  {
    private MediaTypeHeaderValue JsonPatch = new MediaTypeHeaderValue("application/json-patch+json");

    public VssJsonPatchMediaTypeFormatter()
      : base()
    {
      this.SupportedMediaTypes.Clear();
      this.SupportedMediaTypes.Add(this.JsonPatch);
    }

    public override bool CanReadType(Type type) => type.IsOfType(typeof (IPatchDocument<>));

    public override bool CanWriteType(Type type) => false;

    public override async Task<object> ReadFromStreamAsync(
      Type type,
      Stream readStream,
      HttpContent content,
      IFormatterLogger formatterLogger)
    {
      object obj1 = await base.ReadFromStreamAsync(typeof (JsonPatchDocument), readStream, content, formatterLogger).ConfigureAwait(false);
      MethodInfo methodInfo = type.GetTypeInfo().DeclaredMethods.First<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name.Equals("CreateFromJson") && m.Attributes.HasFlag((Enum) (MethodAttributes.Public | MethodAttributes.Static))));
      object obj2;
      try
      {
        obj2 = methodInfo.Invoke((object) null, new object[1]
        {
          obj1
        });
      }
      catch (Exception ex)
      {
        if (ex is TargetInvocationException && ex.InnerException != null)
          throw ex.InnerException;
        throw;
      }
      return obj2;
    }
  }
}
