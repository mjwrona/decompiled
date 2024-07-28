<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Account" %>
<script type="text/javascript"<%= Html.GenerateNonce(true) %>>
    var ActionUrls = {
        <%: Html.GenerateActionUrls() %>
    };
</script>
