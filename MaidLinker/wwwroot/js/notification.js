"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

//Disable the send button until connection is established.
//document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    //var li = document.createElement("li");
    //document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    $.ajax({
        url: '/Notification/GetNotificationsCount',  // Replace 'your-api-url' with the actual API endpoint URL
        method: 'GET',
        success: function (response) {
            // On success, loop through the response data and populate the dropdown options
            //dropdown.append($('<option>').text(degree.titleEn).val(degree.id));
            $(".notificationsCount").text(response);
            //li.textContent = `${response} says `;
            if ($(".notificationsCount").text() > 0 || response > 0) {
                $("#notifications-count").removeClass("badge-light").addClass("badge-danger");
            }
            //$.each(response, function (index, item) {
            //    li.textContent = `${item.titleEn} says `;
            //});


        },
        error: function () {
            console.log('Error occurred while retrieving degree data.');
        }
    });
});

connection.start().then(function () {
    //document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error("connection error");
});

//document.getElementById("sendButton").addEventListener("click", function (event) {
//    //var user = document.getElementById("userInput").value;
//    //var message = document.getElementById("messageInput").value;
//    connection.invoke("SendMessage", user, message).catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});