function generateHoard() {
    $("#error").text("");
    $("#results").html("");

    $.post("/TreasureHoardGenerator/GenerateHoard",
        {
            challengeLevel: $("input[name=ChallengeLevel]").val(),
            currencyRollingGuideJson: $("textarea[name=currencyRollingJson]").val()
        },
        function (result) {
            if (result.error) {
                $("#error").text(result.error);
            }
            else {
                var html = '';

                html += '<div class="col-3">';
                html += '<h3>Total Value: </h3>';
                html += '<p>';
                html += result.currency.valueInGold + " in gold";
                html += '</p>';
                html += '</div>';


                html += '<div class="col-3">';
                html += '<h3>Coins: </h3>';
                for (var i = 0; i < result.currency.pieces.length; i++) {
                    html += '<p>';
                    html += result.currency.pieces[i].value + " " + result.currency.pieces[i].typeAsString;
                    html += '</p>';
                }
                html += '</div>';

                $("#results").html(html);
            }
        });
}