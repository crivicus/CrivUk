"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub", { accessTokenFactory: () => this.loginToken }).build();

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " says " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.on("WhisperMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = user + " whispers " + msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    document.getElementById("messagesList").appendChild(li);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    var toUser = "";

    if (message.startsWith('<'))
        toUser = message.substring(message.indexOf('<'), message.indexOf(' '));

    toUser = toUser.replace('<', '');

    if (toUser.length > 0) {
        connection.invoke("SendPrivateMessage", user, toUser, message).catch(function (err) {
            return console.error(err.toString());
        });
    } else {
        connection.invoke("SendMessage", user, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});