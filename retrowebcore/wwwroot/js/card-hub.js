"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/cardhub").build();
var cardTypes = {};
cardTypes[1] = "liked";
cardTypes[2] = "lacked";
cardTypes[3] = "learned";
cardTypes[4] = "longedfor";
cardTypes[5] = "actionitem";

const CARD_ID_TEMPLATE = '<[[CARD_ID_TEMPLATE]]>';
const CARD_CONTENT_TEMPLATE = '<[[CARD_CONTENT_TEMPLATE]]>';
const SLUG_TEMPLATE = '<[[SLUG]]>';

var TEMPLATE = `
        <div class="card draggable shadow-sm dropzone" 
            id="${CARD_ID_TEMPLATE}" draggable="true" 
            ondragstart="drag(event)"
            ondrop="merge(event)" 
            ondragover="allowDrop(event)" 
            ondragleave="clearDrop(event)"
        >
            <div class="retro-card-body">
                <p>
                    ${CARD_CONTENT_TEMPLATE}
                </p>
                <div class="float-right">
                    <button class="float-right" data-feather="eye">view</button>
                    <span class="col-2" data-feather="thumbs-up"></span>
                </div>
            </div>
        </div>
        <div class="dropzone rounded" ondrop="drop(event)" ondragover="allowDrop(event)" ondragleave="clearDrop(event)"> &nbsp; </div>`;

//Disable send button until connection is established
for (var i = 1; i < 5; i++)
    document.getElementById(`${cardTypes[i]}LaneAddChild`).disabled = true;

connection.on(hubNewCardEvent, function (responseJson) {
    var response = JSON.parse(responseJson);
    var newBoard = TEMPLATE
        .replace(CARD_ID_TEMPLATE, response.squad)
        .replace(CARD_CONTENT_TEMPLATE, response.sprint);
    $(`#${cardTypes[response.CardType]}LaneAddChildSpinner`).addClass("hidden");
    $(`#${cardTypes[response.CardType]}Lane`).append(newBoard);
    feather.replace(); //to show icon in new card
    
});

connection.start().then(function () {
    for (var i = 1; i < 5; i++)
        document.getElementById(`${cardTypes[i]}LaneAddChild`).disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

$(".lane-add-button").click(function (event) {
    var id = $(this).attr("id");
    var spinnerIdSelector = `#${id}Spinner`;
    var type = $(this).attr("data-type");
    $(spinnerIdSelector).removeClass("hidden");
    var boardslug = $("#board-id").attr('data-name');
    connection.invoke(hubAddNewCard, boardslug, type).catch(function (err) {
        $(spinnerIdSelector).addClass("hidden");
        alert(err.toString());
        return console.error(err.toString());
    });
    event.preventDefault();
});
