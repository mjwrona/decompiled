// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SequenceContext
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class SequenceContext
  {
    internal const long UnspecifiedSequenceId = -1;
    internal static SequenceContext MaxSequenceContext = new SequenceContext(long.MaxValue, long.MaxValue, long.MaxValue, 0L);
    internal static SequenceContext InitSequenceContext = new SequenceContext(-1L, -1L, -1L, 0L);

    public SequenceContext(long identitySequenceId, long groupSequenceId)
      : this(identitySequenceId, groupSequenceId, -1L)
    {
    }

    public SequenceContext(
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId)
      : this(identitySequenceId, groupSequenceId, organizationIdentitySequenceId, 0L)
    {
    }

    [JsonConstructor]
    public SequenceContext(
      long identitySequenceId,
      long groupSequenceId,
      long organizationIdentitySequenceId,
      long pageSize)
    {
      this.IdentitySequenceId = identitySequenceId;
      this.GroupSequenceId = groupSequenceId;
      this.OrganizationIdentitySequenceId = organizationIdentitySequenceId;
      this.PageSize = pageSize;
    }

    [JsonProperty]
    internal long IdentitySequenceId { get; }

    [JsonProperty]
    internal long GroupSequenceId { get; }

    [JsonProperty]
    internal long OrganizationIdentitySequenceId { get; }

    [JsonProperty]
    internal long PageSize { get; }

    internal SequenceContext Clone() => new SequenceContext(this.IdentitySequenceId, this.GroupSequenceId, this.OrganizationIdentitySequenceId);

    public bool IsUnspecified => this.IdentitySequenceId == -1L && this.GroupSequenceId == -1L && this.OrganizationIdentitySequenceId == -1L;

    public override string ToString() => string.Format("[{0}:{1}, {2}:{3}, {4}:{5}]", (object) "IdentitySequenceId", (object) this.IdentitySequenceId, (object) "GroupSequenceId", (object) this.GroupSequenceId, (object) "OrganizationIdentitySequenceId", (object) this.OrganizationIdentitySequenceId);

    internal class HeadersUtils
    {
      internal const string MinIdentitySequenceId = "X-VSSF-MinIdentitySequenceId";
      internal const string MinGroupSequenceId = "X-VSSF-MinGroupSequenceId";
      internal const string MinOrgIdentitySequenceId = "X-VSSF-MinOrgIdentitySequenceId";
      internal const string PageSize = "X-VSSF-PagingSize";

      internal static bool TryExtractSequenceContext(
        HttpRequestHeaders httpRequestHeaders,
        out SequenceContext sequenceContext)
      {
        sequenceContext = (SequenceContext) null;
        IEnumerable<string> values1;
        bool flag1 = httpRequestHeaders.TryGetValues("X-VSSF-MinIdentitySequenceId", out values1) && values1 != null;
        IEnumerable<string> values2;
        int num1 = !httpRequestHeaders.TryGetValues("X-VSSF-MinGroupSequenceId", out values2) ? 0 : (values2 != null ? 1 : 0);
        IEnumerable<string> values3;
        bool flag2 = httpRequestHeaders.TryGetValues("X-VSSF-MinOrgIdentitySequenceId", out values3) && values3 != null;
        IEnumerable<string> values4;
        int num2 = !httpRequestHeaders.TryGetValues("X-VSSF-PagingSize", out values4) ? 0 : (values4 != null ? 1 : 0);
        if (num1 == 0 && !flag1 && !flag2)
          return false;
        long orGetDefault1 = SequenceContext.HeadersUtils.ParseOrGetDefault(values1 != null ? values1.FirstOrDefault<string>() : (string) null);
        long orGetDefault2 = SequenceContext.HeadersUtils.ParseOrGetDefault(values2 != null ? values2.FirstOrDefault<string>() : (string) null);
        long orGetDefault3 = SequenceContext.HeadersUtils.ParseOrGetDefault(values3 != null ? values3.FirstOrDefault<string>() : (string) null);
        long orGetDefault4 = SequenceContext.HeadersUtils.ParseOrGetDefault(values4 != null ? values4.FirstOrDefault<string>() : (string) null);
        sequenceContext = new SequenceContext(orGetDefault1, orGetDefault2, orGetDefault3, orGetDefault4);
        return true;
      }

      internal static KeyValuePair<string, string>[] PopulateRequestHeaders(
        SequenceContext sequenceContext)
      {
        if (sequenceContext == null)
          return new KeyValuePair<string, string>[0];
        return new KeyValuePair<string, string>[4]
        {
          new KeyValuePair<string, string>("X-VSSF-MinIdentitySequenceId", sequenceContext.IdentitySequenceId.ToString()),
          new KeyValuePair<string, string>("X-VSSF-MinGroupSequenceId", sequenceContext.GroupSequenceId.ToString()),
          new KeyValuePair<string, string>("X-VSSF-MinOrgIdentitySequenceId", sequenceContext.OrganizationIdentitySequenceId.ToString()),
          new KeyValuePair<string, string>("X-VSSF-PagingSize", sequenceContext.PageSize.ToString())
        };
      }

      private static long ParseOrGetDefault(string s)
      {
        long result;
        return !string.IsNullOrWhiteSpace(s) && long.TryParse(s, out result) ? result : -1L;
      }
    }
  }
}
