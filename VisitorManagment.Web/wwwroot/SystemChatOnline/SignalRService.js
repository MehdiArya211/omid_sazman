var chatBox = $("#ChatBox");

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.start();



function Init() {


    // هر زمان که دکمه ارسال در چت باکس کلیک شور کد های زیر اجرا می شود
    var NewMessageForm = $("#NewMessageForm");
    NewMessageForm.on("submit", function (e) {
        e.preventDefault();
        var message = e.target[0].value;
        e.target[0].value = '';
        sendMessage(message);
    });

}

//ارسال پیام به سرور
function sendMessage(text) {
    //var fullName = $("#fullNameUser").innerhtml();
    var fullName = document.getElementById("fullNameUser").value;
    connection.invoke('SendNewMessage', fullName, text);
}

//درسافت پیام از سرور
connection.on('getNewMessage', getMessage);

function getMessage(sender, message, time) {


    $("#Messages").append("<li><div><span>" + sender + "</span><span>" + "(" + time + ")" + "</span></div><div class='messages'>" + message.replace(/(.{1,300})/g, '$1<br/>') +"</div></li>")
};


$(document).ready(function () {
    Init();
});
