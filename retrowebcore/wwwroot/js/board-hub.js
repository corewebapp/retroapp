"use strict";

const disabled = 'disabled';
//Disable buttons until connection is established
$('#addNewBoard').prop(disabled, true);
$('.archive-action').prop(disabled, true);

var connection = new signalR.HubConnectionBuilder().withUrl("/boardhub").build();

connection.on(hubNewBoardEvent, function (responseJson) {
    var response = JSON.parse(responseJson);
    var newBoard = buildNewBoard(response);
    $('#sendButtonSpinner').addClass("hidden");
    $('#boardContainer').append(newBoard);
    $('.archive-action').off('click');
    attachArchive();
});

connection.on(hubArchiveEvent, function (slug) {
    $(`#container-${slug}`).remove();
});

connection.start().then(function () {
    $('#addNewBoard').prop(disabled, false);
    $('.archive-action').prop(disabled, false);
}).catch(function (err) {
    return console.error(err.toString());
});

$("#addNewBoard").click(function (event) {
    $('#sendButtonSpinner').removeClass("hidden");
    var squad = document.getElementById("squadName").value;
    var sprint = document.getElementById("sprintName").value;
    connection.invoke(hubAddNewBoard, squad, sprint).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

function attachArchive() {
    $('.archive-action').click(function (event) {
        var slug = $(this).attr('id');
        $(`#spinner-${slug}`).removeClass("hidden");
        connection.invoke(hubArchiveBoard, slug).catch(function (err) {
            return console.error(err.toString());
        });
        event.preventDefault();
    });
}

attachArchive();