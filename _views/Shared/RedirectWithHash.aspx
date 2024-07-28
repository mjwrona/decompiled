<div class="header">
    <script type="text/javascript">
        (function () {
            var url = "<%: ViewData["Url"] %>",
                hash = window.location.hash;

            if (hash.length > 0) {
                url += "&" + hash.substring(1);
            }

            window.location = url;
        })();
    </script>
</div>
