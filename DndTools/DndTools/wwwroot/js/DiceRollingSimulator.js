function rollDice(number) {
    $("#error").text("");
    $("#results").html("");

    $.post("/DiceRollingSimulator/RollDice",
        { advancedDiceNotation: $("input[name=AdvancedDiceNotation" + number + "]").val() },
        function (result) {
            if (result.error) {
                $("#error").text(result.error);
            }
            else {
                var html = '';

                html += '<div class="col-3">';
                html += '<h3>Value: </h3>';
                html += '<p>';
                html += result.value;
                html += '</p>';
                html += '<h3>Multiplier mod:</h3>';
                html += '<p>';
                html += result.multiplier;
                html += '</p>';
                html += '<h3>Adding mod: </h3>';
                html += '<p>';
                html += result.addition;
                html += '</p>';
                html += '</div>';


                html += '<div class="col-3">';
                html += '<h3>Kept Rolls</h3>';
                for (var i = 0; i < result.keptRolls.length; i++) {
                    html += '<p>';
                    html += result.keptRolls[i];
                    html += '</p>';
                }
                html += '</div>';


                html += '<div class="col-3">';
                html += '<h3>Discarded Rolls</h3>';
                for (var i = 0; i < result.discardedRolls.length; i++) {
                    html += '<p>';
                    html += result.discardedRolls[i];
                    html += '</p>';
                }
                html += '</div>';



                $("#results").html(html);
            }
        });
}