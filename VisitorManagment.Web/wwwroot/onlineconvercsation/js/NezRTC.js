/*
 *  Copyright (c) 2022 NezRTC authors. All Rights Reserved.
 *  Date:       2022-10-11
 *  Date:       2023-01-23
 */

'use strict';

/****************************************************************************
* تنظیمات پایه
****************************************************************************/
//جهت برقراری ارتباط با هاب
var connection = new signalR.HubConnectionBuilder().withUrl("/NezRTCHub").build();
let peerConn = null;
// متغیر ها برای کار کردن برای هندل کردن سیگنالینگ وب آر تی سی
let makingOffer = false;
let ignoreOffer = false;
let polite = false;

// current active element that user clicked
let ansarMeetingActiveElement = null;

let dataChannel;

const ip = '10.128.155.121'
const configuration = {

    'iceServers': [{
        'urls': `stun:${ip}:3478?transport=udp`
    },
    {
        'urls': `turn:${ip}:3478?transport=udp`,
        'username': 'themmaty',
        'credential': 'qwer@123123'
    },
    ]
};

//جهت گرفتن آیدی جلسه
function getId(id) {
    return document.getElementById(id)
}


// return new rtc connection
function factoryPeerConnection() {
    return new RTCPeerConnection(configuration);
}


//المان های اصلی  
const txtRoomName = getId('txtRoomName');
const btnCreateRoom = getId('btnCreateRoom');
const tblRooms = getId('tblRooms');
const connectionStatusMessage = getId('connectionStatusMessage');
const localVideo = getId('localVideo');
const remoteVideo = getId('remoteVideo');

//المان های مربوط به انتخاب ابزار های متصل شده توسط کاربر
const audioInputSelect = document.querySelector('select#audioSource');
const audioOutputSelect = document.querySelector('select#audioOutput');
const videoSelect = document.querySelector('select#videoSource');
const selectors = [audioInputSelect, audioOutputSelect, videoSelect].filter(Boolean);


//متغیر های سراسری
let myRoomId;
let localStream;
let remoteStream;
let isInitiator = false;
let hasRoomJoined = false;
let cameraSituation = false;
let tmpLocalStream = null;
let videoSrcFrame = 'default';
let isPicToPic = false;
let usersList = [];


$(tblRooms).DataTable({
    columns: [
        { data: 'RoomId', "width": "30%" },
        { data: 'Name', "width": "50%" },
        { data: 'Button', "width": "15%" }
    ],
    "lengthChange": false,
    "searching": false,
    "language": {
        "emptyTable": "جلسه ای موجود نمی باشد"
    }
});



// برای اتصال به اتاق گفتگو
function joinCall(roomId) {
    isPicToPic = true;
    if (hasRoomJoined) {
        // toastr.warning('شما در حال حاضر در این گفتگو حضور دارید');
    }
    else {
        if (cameraSituation == false) {
            toastr.warning('لطفاً پیش از برقراری ارتباط دوربین خود را فعال کنید.');
        }
        else {
            localStorage.setItem("currentRoomId", roomId);
            connection.invoke("Join", roomId).catch(function (err) {
                return console.log(err);
            });


        }
    }

}



// تغییر وضعیت کاربر به آنلاین و یا افلاین
function setUserStatus(personalCodeNeedToBeOnline) {
    // 1. گرفتن لبست کاربران
    const users = document.querySelectorAll(".onlineUsersList__item");
    const onlineUsersArray = [];


    users.forEach((user) => {
        const fPersonalCode = atob(user.dataset.pcode);

        // console.log(user.parentElement.parentElement.children[0].children[2]);


        // همه کاربران به صورت پیش فرض آفلاین هستند
        const statusElement = user.parentElement.parentElement.children[0].children[2];
        statusElement.style.borderColor = "red";
        statusElement.classList.remove("online");



        // کد پرسنلی هر شخص با کد افرادی که باید آنلاین شوند چک میشود و اگر با هم برابر بودنند آن کاربر آنلاین میشود
        personalCodeNeedToBeOnline.forEach((onlineUsers) => {

            if (onlineUsers.personalCode === fPersonalCode) {
                onlineUsersArray.push(user);
                statusElement.style.borderColor = "green";
                statusElement.classList.add("online");
                user.addEventListener("click", () => {
                    // اگر کاربر آنلاین باشد میتواند با کلیک بروی  اسم خود وارد اتاق گفتگو شود
                    if (statusElement.classList[0] === "online") {
                        joinCall(onlineUsers.roomId);
                    }

                })
            }

        })

    })

    // نمایش افراد آنلاین در بالای افراد افلاین
    onlineUsersArray.forEach((onlineUserElements) => {
        const parent = onlineUserElements.parentElement;
        console.log("parent", parent);
        parent.removeChild(onlineUserElements);
        const firstElementOnTheList = parent.firstChild.nextElementSibling;
        parent.insertBefore(onlineUserElements, firstElementOnTheList)
    })
}







//با فراخوانی این تابع مدیا های کاربر شامل صوت و تصویر از ابزار های متصل
//شده توسط کابر به سیستم دریافت می شوند. این ابزار شامل 
//grabWebCamVideo();
/****************************************************************************
 * دریافت مدیا از کاربر
****************************************************************************/

//انتخابگر ابزار های های متصل
if (audioOutputSelect) {
    audioOutputSelect.disabled = !('sinkId' in HTMLMediaElement.prototype);
}

function gotDevices(deviceInfos) {
    // شناسایی ابزار های متصل شده به سیستم شامل میکروفن، اسپیکر و دوربین
    const values = selectors.map(select => select.value);
    selectors.forEach(select => {
        while (select.firstChild) {
            select.removeChild(select.firstChild);
        }
    });
    for (let i = 0; i !== deviceInfos.length; ++i) {
        const deviceInfo = deviceInfos[i];
        const option = document.createElement('option');
        option.value = deviceInfo.deviceId;
        if (deviceInfo.kind === 'audioinput') {
            option.text = deviceInfo.label || `microphone ${audioInputSelect.length + 1}`;
            audioInputSelect.appendChild(option);
        } else if (deviceInfo.kind === 'audiooutput') {
            option.text = deviceInfo.label || `speaker ${audioOutputSelect.length + 1}`;
            audioOutputSelect.appendChild(option);
        } else if (deviceInfo.kind === 'videoinput') {
            option.text = deviceInfo.label || `camera ${videoSelect.length + 1}`;
            videoSelect.appendChild(option);
        } else {
            console.log('Some other kind of source/device: ', deviceInfo);
        }
    }
    selectors.forEach((select, selectorIndex) => {
        if (Array.prototype.slice.call(select.childNodes).some(n => n.value === values[selectorIndex])) {
            select.value = values[selectorIndex];
        }
    });
}

navigator.mediaDevices.enumerateDevices().then(gotDevices).catch(handleError);



// Attach audio output device to video element using device/sink ID.
function attachSinkId(element, sinkId) {
    if (typeof element.sinkId !== 'undefined') {
        element.setSinkId(sinkId)
            .then(() => {
                console.log(`Success, audio output device attached: ${sinkId}`);
            })
            .catch(error => {
                let errorMessage = error;
                if (error.name === 'SecurityError') {
                    errorMessage = `You need to use HTTPS for selecting audio output device: ${error}`;
                }
                console.error(errorMessage);
                // Jump back to first output device in the list as it's the default.
                audioOutputSelect.selectedIndex = 0;
            });
    } else {
        console.warn('Browser does not support output device selection.');
    }
}
function handleError(error) {
    console.log('navigator.MediaDevices.getUserMedia error: ', error.message, error.name);
}
function changeAudioDestination() {
    if (!audioOutputSelect || !localVideo) return;
    const audioDestination = audioOutputSelect.value;
    attachSinkId(localVideo, audioDestination);
}

//هنگام تغییر ابزار های متصل، فراخوانی جدید ابزار صورت می گیرد.
if (audioInputSelect) audioInputSelect.onchange = grabWebCamVideo;
if (audioOutputSelect) audioOutputSelect.onchange = changeAudioDestination;
if (videoSelect) videoSelect.onchange = grabWebCamVideo;


function changeVideoTextStyle() {
    getId('pictureInPicture_user1').style.borderRadius = "4";

}

// گرفتن اطلاعات وب کم کاربر و خواندن آن
async function grabWebCamVideo() {
    cameraSituation = false;

    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        toastr.error('مرورگر شما امکان دسترسی به دوربین و میکروفن را پشتیبانی نمی‌کند.');
        return false;
    }

    if (window.stream) {
        window.stream.getTracks().forEach(track => track.stop());
    }

    const audioSource = audioInputSelect ? audioInputSelect.value : null;
    const videoSource = videoSelect ? videoSelect.value : null;
    const constraints = {
        audio: {
            deviceId: audioSource ? { exact: audioSource } : undefined,
            echoCancellation: true
        },
        video: {
            deviceId: videoSource ? { exact: videoSource } : undefined
        }
    };

    try {
        const stream = await navigator.mediaDevices.getUserMedia(constraints);
        gotStream(stream);
        await navigator.mediaDevices.enumerateDevices().then(gotDevices);
        return true;
    } catch (error) {
        cameraSituation = false;
        handleError(error);
        toastr.error('دسترسی به دوربین یا میکروفن برقرار نشد. مجوز مرورگر و اتصال تجهیزات را بررسی کنید.');
        return false;
    }
}


// دوربین فقط یک‌بار و پس از کامل‌شدن DOM درخواست می‌شود.
window.addEventListener('load', function () {
    grabWebCamVideo();
}, { once: true });




// بستن دوربین
let isCameraMuted = true;
document.getElementById("muteCamera").addEventListener("click", () => {
    isCameraMuted = !isCameraMuted
    muteCamera();
});
function muteCamera() {
    if (localStream) {
        localStream.getTracks().forEach((track) => {
            if (track.kind === "video") {
                track.enabled = isCameraMuted;
            }
        })

    }
}

// بستن صدا
let isAudioMuted = true;
const audioMuteElement = document.getElementById("muteAudioWarning");
document.getElementById("muteAudio").addEventListener("click", () => {
    isAudioMuted = !isAudioMuted
    audioMuteElement.style.display = isAudioMuted ? "none" : 'block';
    muteAudio();
});
function muteAudio() {
    if (localStream) {
        localStream.getTracks().forEach((track) => {
            if (track.kind === "audio") {
                track.enabled = isAudioMuted;
            }
        })

    }
}


// ست کردن استریم و ویدئو کاربر در صفحه
function gotStream(stream) {
    cameraSituation = true;
    window.stream = stream;
    //recordButton.disabled = false;
    localVideo.srcObject = stream;
    tmpLocalStream = stream;
    localStream = stream;

    const codecPreferencesElement = document.getElementById('codecPreferences');
    if (codecPreferencesElement && codecPreferencesElement.options.length === 0) {
        getSupportedMimeTypes().forEach(mimeType => {
            const option = document.createElement('option');
            option.value = mimeType;
            option.innerText = option.value;
            codecPreferencesElement.appendChild(option);
        });
        codecPreferencesElement.disabled = false;
    }
    return navigator.mediaDevices.enumerateDevices();
}

/****************************************************************************
* مدیریت اتاق گفتگو
****************************************************************************/



$("#btnCreateRoom").click(function () {
    var name = txtRoomName.value;

    if (name) {

        connection.invoke("ansarCreateRoom", name).catch(function (err) {
            return console.error(err.toString());
        });

        polite = true;

    } else {
        toastr.error('پرکردن این قسمت ضروری است');
    }

});



$('#tblRooms tbody').on('click', 'button', function () {
    if (hasRoomJoined) {
        toastr.warning('شما در حال حاضر در این گفتگو حضور دارید');
        //getId('toggleTable').style.backgroundColor = 'green'
        //getId('toggleTable').innerText = 'شما جلسه باز دارید'
    }
    else {
        if (cameraSituation == false) {
            toastr.warning('لطفاً پیش از برقراری ارتباط دوربین خود را فعال کنید.');
        }
        else {
            var data = $(tblRooms).DataTable().row($(this).parents('tr')).data();
            connection.invoke("Join", data.RoomId).catch(function (err) {
                return console.error(err.toString());
            });

            isPicToPic = true;
        }
    }


    joinCall(roomId);
});


/****************************************************************************
*تنظیمات سیگنالینگ سرور
****************************************************************************/
//اتصال به سیگنالینگ سرور


toastr.options = {
    timeOut: 3000,
    progressBar: true,
    showMethod: "slideDown",
    hideMethod: "slideUp",
    showDuration: 200,
    hideDuration: 200
};




//function kickUser(id) {

//    connection.invoke("LeaveRoom", id).catch(function (err) {
//        return console.error(err.toString());
//    });


//    toastr.success(`کاربر با ایدی${id} حذف شد.`);



//    const usersItem = document.querySelectorAll(".userStatus")


//    usersItem.forEach((item) => {

//        const roomIdValue = item.children[2];

//    })



//    /*


//  `<div class="userStatus">
//                        <span class="online" style="width:10px;height:10px;border:2px solid #15df15; border-radius:50%;margin-left:6px"></span>
//                        <div id="handleName"></div>
//                        <p id="btnn" onclick="callUser(${item.RoomId})"
//                           style="color: #3b4eaf; font-weight: bold;margin-bottom:0px">
//                            ${item.Name}
//                         </p>
//<p onclick="kickUser('${item.RoomId}')" style="transform: rotate(45deg)"><i class="fa fa-plus"></i></p>

//                     </div>


//     * 
//     */

//}



connection.start().then(function () {
    // <p onclick="kickUser('${item.RoomId}')" style="transform: rotate(45deg)"><i class="fa fa-plus"></i></p>
    connection.on('updateRoom', function (data) {

        var obj = JSON.parse(data);
        const personalCodeNeedToBeOnline = [];

        obj.forEach((user) => {
            personalCodeNeedToBeOnline.push({ personalCode: user.PersonalCode, roomId: user.RoomId });
        })
        localStorage.setItem("onlineUsers", JSON.stringify(personalCodeNeedToBeOnline))

        setUserStatus(personalCodeNeedToBeOnline);

    });


    connection.on("ansarUpdateRoom", function (data) {

        if (!document.getElementById('ansarUsersContent')) {
            return;
        }

        var obj = JSON.parse(data);

        console.log(obj);

        // رندر کردن لیست اتاق های ملاقات انصار
        const result = obj.map((item) => {

            return (
                `<div class="userStatus" id="test">
                        <span class="online" style="width:10px;height:10px;border:2px solid #15df15; border-radius:50%;margin-left:6px"></span>
                        <div id="handleName"></div>
                        <p id="room-${item.RoomId}" onclick="callHandler(${item.RoomId}, '${item.Name}')"
                           style="color: #3b4eaf; font-weight: bold;margin-bottom:0px">
${item.Name}
                         </p>

                     </div>
`
            )
        })
        document.getElementById('ansarUsersContent').innerHTML = result.join("");
    })



    connection.on('created', function (RoomId) {
        isInitiator = true;
        localStorage.setItem("currentRoomId", RoomId);
        txtRoomName.disabled = true;
        btnCreateRoom.disabled = true;
        hasRoomJoined = false;
        // toastr.success('اتاق گفتگو شماره ' + RoomId + '.ایجاد شد در انتظار پیوستن مخاطب جلسه...');
        myRoomId = RoomId;




    });


    connection.on('ansarCreated', function (RoomId, name) {
        isInitiator = true;
        localStorage.setItem("currentRoomId", RoomId);

        // the owner of the room is joined to itself
        // we show the room title
        const elem = document.querySelector(".ansar_meeting_title")
        elem.innerText = `${name}`;
        elem.style.visibility = "visible";



        txtRoomName.disabled = true;
        btnCreateRoom.disabled = true;
        hasRoomJoined = false;
        // toastr.success('اتاق گفتگو شماره ' + RoomId + '.ایجاد شد در انتظار پیوستن مخاطب جلسه...');
        myRoomId = RoomId;
    });



    connection.on('joined', function (RoomId) {
        // toastr.success('کاربر به جلسه گفگو پیوست');
        isPicToPic = true;
        myRoomId = RoomId;
        isInitiator = false;
        getId('remotevideoContainer').style.display = 'inline-block'
        getId('remotevideoContainer').style.transition = '1s'
        document.querySelector(".localvideobox").classList.add('picToPicLocalVideo');
    });



    connection.on('ansarJoined', function (RoomId, name) {
        localStorage.setItem("currentRoomId", RoomId);
        // toastr.success('کاربر به جلسه گفگو پیوست');

        // show title for the joined user
        const elem = document.querySelector(".ansar_meeting_title")
        if (elem) {
            elem.style.visibility = "visible";
            elem.innerText = `${name}`;
        }



        isPicToPic = true;
        myRoomId = RoomId;
        isInitiator = false;
        getId('remotevideoContainer').style.display = 'inline-block'
        getId('remotevideoContainer').style.transition = '1s'
        document.querySelector(".localvideobox").classList.add('picToPicLocalVideo');
    });




    connection.on('error', function (message) {
        // alert(message);
    });

    connection.on('ready', async function (roomId) {
        localStorage.setItem("currentRoomId", roomId);

        txtRoomName.disabled = true;
        btnCreateRoom.disabled = true;
        hasRoomJoined = true;
        getId('remotevideoContainer').style.display = 'inline-block';
        document.querySelector(".localvideobox").classList.add('picToPicLocalVideo');
        document.querySelector(".localvideobox").classList.add("picToPicHover");

        const waitingText = document.getElementById('waitingText')
        if (waitingText) {
            waitingText.style.display = 'none';
        }
        createPeerConnection(isInitiator, configuration);
    });


    connection.on('ansarReady', async function (roomId) {
        localStorage.setItem("currentRoomId", roomId);

        txtRoomName.disabled = true;
        btnCreateRoom.disabled = true;
        hasRoomJoined = true;
        getId('remotevideoContainer').style.display = 'inline-block';
        document.querySelector(".localvideobox").classList.add('picToPicLocalVideo');
        document.querySelector(".localvideobox").classList.add("picToPicHover");

        // 1. پاک کردن روم فعلی از لیست تماس های انصار
        const element = document.getElementById(`room-${roomId}`);
        const parent1 = element.parentElement
        const container = parent1.parentElement;
        container.removeChild(parent1);
        try {
            createPeerConnection(isInitiator, configuration);
            // user's joined by now we show the meeting title here
        } catch (e) { }
    });

    // برای هندل کردن ارتباط تصویری و برقراری ارتباط استفاده میشود
    connection.on('message', async function (data) {
        const { description, candidate } = data;
        try {
            if (description) {
                const offerCollision = description.type == "offer" &&
                    (makingOffer || peerConn.signalingState != "stable");

                ignoreOffer = !polite && offerCollision;
                if (ignoreOffer) {
                    return;
                }
                if (offerCollision) {
                    await Promise.all([
                        peerConn.setLocalDescription({ type: "rollback" }),
                        peerConn.setRemoteDescription(description)
                    ]);
                } else {
                    await peerConn.setRemoteDescription(description);
                }
                if (description.type == "offer") {
                    await peerConn.setLocalDescription(await peerConn.createAnswer());
                    sendMessage({ description: peerConn.localDescription });
                }
            } else if (candidate) {
                try {
                    await peerConn.addIceCandidate(candidate);
                } catch (e) {
                    if (!ignoreOffer) console.error(e);
                }
            }
        } catch (e) {
            console.error(e);
        }
    });

    // زمانی صدا زده میشود که کاربر از اتاق گفتگو خارج شده باشد
    connection.on('bye', function () {
        localStorage.removeItem("currentRoomId");
        hasRoomJoined = false;
        hangup(false);
        toastr.info(`مخاطب شما گفتگو را ترک کرد. شناسه گفتگو: ${myRoomId}.`);
        // connectionStatusMessage.innerText = `مخاطب شما گفتگو را ترک کرد. شناسه گفتگو: ${myRoomId}.`;

        // remove ansar meeting title
        const elem = document.querySelector(".ansar_meeting_title")
        if (elem) {
            elem.style.visibility = "hidden";
            elem.innerText = ``;
        }
    });

    window.addEventListener('unload', function () {
        if (hasRoomJoined) {
            connection.invoke("LeaveRoom", myRoomId).catch(function (err) {
                return console.error(err.toString());
            });
        }
    });



    //Get room list.
    connection.invoke("GetRoomInfo").catch(function (err) {
        return console.error(err.toString());
    });

}).catch(function (err) {
    console.error(err.toString());
    scheduleSignalRReconnect();
});

let signalRReconnectTimer = null;
let signalRReconnectAttempt = 0;

function scheduleSignalRReconnect() {
    if (signalRReconnectTimer) return;

    const delays = [1000, 3000, 5000, 10000, 15000];
    const delay = delays[Math.min(signalRReconnectAttempt, delays.length - 1)];
    signalRReconnectTimer = window.setTimeout(async function () {
        signalRReconnectTimer = null;
        try {
            await connection.start();
            signalRReconnectAttempt = 0;
            await connection.invoke("GetRoomInfo");

            const activeMeetingId = parseInt(localStorage.getItem("active_meeting_id"), 10);
            if (activeMeetingId > 0) {
                await connection.invoke("JoinMeetingChat", activeMeetingId);
            }

            toastr.success('ارتباط با جلسه مجدداً برقرار شد.');
        } catch (error) {
            signalRReconnectAttempt++;
            scheduleSignalRReconnect();
        }
    }, delay);
}

connection.onclose(function () {
    hasRoomJoined = false;
    scheduleSignalRReconnect();
});

/**
* Send message to signaling server
*/
function sendMessage(message) {
    connection.invoke("SendMessage", myRoomId, message).catch(function (err) {
        console.error(err.toString());
    });
}

// ساخت یک کانکشن جدید web rtc
function createPeerConnection(isInitiator, config) {
    // 1. create new rtc connection when user status is ready
    peerConn = factoryPeerConnection();


    // 2. send local media to peer connection
    //sendStreamToRTCConnection();
    if (localStream) {
        localStream.getTracks().forEach(track => peerConn.addTrack(track, localStream));
    }

    // 3. handling incoming media from a remote source
    peerConn.ontrack = function (event) {
        remoteVideo.srcObject = event.streams[0];
    };

    /*
     *  ===================================
     *          web rtc negotiation
     *  ===================================
     */


    const webrtc_loading = document.getElementById("webrtc_loading");


    // 4. ارسال درخواست برای برقراری ارتباط تصویری
    peerConn.onnegotiationneeded = async () => {
       // webrtc_loading.style.display = "block";

        try {
            makingOffer = true;
            const offer = await peerConn.createOffer();
            if (peerConn.signalingState != "stable") return;
            await peerConn.setLocalDescription(offer);
            sendMessage({ description: peerConn.localDescription });
        } catch (e) {
            console.error(`ONN ${e}`);
        } finally {
            makingOffer = false;
        }
    }

    // 5. handling ice candidates
    peerConn.onicecandidate = ({ candidate }) => sendMessage({ candidate });

    // 6. در صورت وجود اررور آن ها را نشان بده
    peerConn.oniceconnectionstatechange = () => {
        if (peerConn.iceConnectionState === "failed") {
            peerConn.restartIce();
        }
       // webrtc_loading.style.display = "none";
    }
}

function onDataChannelCreated(channel) {

    channel.onopen = function () {
        connectionStatusMessage.innerText = 'ارتباط برقرار شد.';
        //fileInput.disabled = false;
        document.getElementById('hangup').style.opacity = '1';
    };

    channel.onclose = function () {
        connectionStatusMessage.innerText = 'خاتمه مکالمه.';

        hangup()
    }

    channel.onmessage = onReceiveMessageCallback();
}

function onReceiveMessageCallback() {
    let count;
    let fileSize, fileName;
    let receiveBuffer = [];

    return function onmessage(event) {
        if (typeof event.data === 'string') {
            const fileMetaInfo = event.data.split(',');
            fileSize = parseInt(fileMetaInfo[0]);
            fileName = fileMetaInfo[1];
            count = 0;
            return;
        }

        receiveBuffer.push(event.data);
        count += event.data.byteLength;

        if (fileSize === count) {
            // all data chunks have been received
            const received = new Blob(receiveBuffer);
            receiveBuffer = [];

            $(tblFiles).children('tbody').append('<tr><td><a></a></td></tr>');
            const downloadAnchor = $(tblFiles).find('a:last');
            downloadAnchor.attr('href', URL.createObjectURL(received));
            downloadAnchor.attr('download', fileName);
            downloadAnchor.text(`${fileName} (${fileSize} bytes)`);

        }
    };
}


/****************************************************************************
* مدیریت خطا
****************************************************************************/
function logError(err) {
    if (!err) return;
    if (typeof err === 'string') {
        console.warn(err);
    } else {
        console.warn(err.toString(), err);
    }
}
/****************************************************************************
 ****************************************************************************/


function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=h264,opus',
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=vp8,opus',
        'video/mp4;codecs=h264,aac',
    ];
    return possibleTypes.filter(mimeType => {
        return MediaRecorder.isTypeSupported(mimeType);
    });
}




window.addEventListener('load', function () {
    const videoElement = document.querySelector('.videoStyle');
    if (videoElement) videoElement.style.display = 'block';
}, { once: true })


////دریافت مدیا از کاربر
//document.querySelector('button#start').addEventListener('click', async () => {
//    if (cameraSituation == false) {
//        grabWebCamVideo();

//        document.querySelector('.videoStyle').style.display = 'block';

//    }

//    else {
//        const videoTracks = localStream.getVideoTracks();

//        videoTracks.forEach(videoTrack => {
//            videoTrack.stop();
//            localStream.removeTrack(videoTrack);

//        });
//        document.querySelector('.videoStyle').style.display = 'block';
//        //document.querySelector('.videoCover').style.display = 'flex';
//        //document.querySelector('.coverText').innerHTML = "یگان م فاوا";
//        cameraSituation = false;
//        //recordButton.disabled = true;
//    }
//});

/****************************************************************************
 ****************************************************************************/

//hangup قطع مکالمه


const hangupbtn = document.querySelector('button#hangup');
const ansarHangup = document.querySelector('#hangupansar');

if (hangupbtn) {
    hangupbtn.addEventListener('click', async () => {
        await hangup(true);
    });
}


if (ansarHangup) {
    ansarHangup.addEventListener('click', async () => {

        if (hasRoomJoined) {
            const currentRoomId = localStorage.getItem("currentRoomId");
            connection.invoke("ansarLeaveRoom", currentRoomId).catch(function (err) {
                console.error(err.toString());
            });
        }


        document.getElementById('hangupansar').style.opacity = '1';
    });
}



async function hangup(notifyRoom = true) {
    const roomId = myRoomId || localStorage.getItem("currentRoomId");

    if (notifyRoom && roomId && connection.state === signalR.HubConnectionState.Connected) {
        try {
            await connection.invoke("LeaveRoom", roomId);
        } catch (error) {
            console.error(error);
        }
    }

    if (peerConn) {
        peerConn.ontrack = null;
        peerConn.onicecandidate = null;
        peerConn.onnegotiationneeded = null;
        peerConn.close();
        peerConn = null;
    }

    if (remoteStream) {
        remoteStream.getTracks().forEach(track => track.stop());
    }

    remoteStream = null;
    if (remoteVideo) remoteVideo.srcObject = null;

    if (localStream) {
        localStream.getTracks().forEach(track => track.stop());
    }

    localStream = null;
    tmpLocalStream = null;
    window.stream = null;
    if (localVideo) localVideo.srcObject = null;

    cameraSituation = false;
    hasRoomJoined = false;
    isPicToPic = false;
    isInitiator = false;
    myRoomId = null;
    localStorage.removeItem("currentRoomId");
    hangupStyles();
}

// برای قطع ارتباط تصویری مورد استفاده قرار میگیرد
function hangupStyles() {
    // location.reload();
    // 1. ریست کردن متغیر های قطع تماس
    txtRoomName.disabled = false;
    btnCreateRoom.disabled = false;
    hasRoomJoined = false;

    // 2. پاک کردن استریم ریموت و ارتباط
    if (remoteStream != null) {
        remoteStream.getTracks().forEach(track => track.stop());
        remoteVideo.src = null;
    }
    remoteStream = null;
    remoteVideo.src = null;

    // let audioTrack = localStream.getTracks.find(track => track.kind === 'audio');
    // audioTrack.enabled = false

    // Stream و PeerConnection در تابع hangup به‌صورت کامل آزاد می‌شوند.



    // 5. استایل صفحه نمایش را از حالت پیک تو پیک خارج میکنیم
    getId('remotevideoContainer').style.display = 'none';
    document.querySelector(".localvideobox").classList.remove('picToPicLocalVideo');
}




$("#toggleOptions").click(function () {
    $("#options").toggle('slow');
    document.querySelector('body').style.overflow = 'scroll'

});



//تور

$(document).on('click', 'a.tour', function () {
    var enjoyhint_instance = new EnjoyHint({});

    enjoyhint_instance.set([
        {
            'next #start': 'ابتدا دوربین را از این قسمت روشن کنید',
        },

        {
            'next #toggleOptions': ' برای تغییر تنظیمات میکروفن ، دوربین ، اسپیکر این قسمت را انتخاب کنید',
        },
        {
            'next #toggleTable': ' برای مشاهده جلسات و یا پیوستن به جلسه بر روی این قسمت کلیک کنید',
        }, {
            'next #hangup': 'برای خروج از جلسه بر روی این قسمت کلیک کنید',
        },
    ]);
    enjoyhint_instance.run();

    return false;
});

//تصویر در تصویر

//picture in picture
// const picutreInPictureBtn1 = getId('pictureInPicture_user1');

//if ('pictureInPictureEnabled' in document) {

//    //picutreInPictureBtn1.style.display = null;
//    // picutreInPictureBtn2.style.display = null;

//    //picutreInPictureBtn1.addEventListener('click', () => {
//    //    localVideo.requestPictureInPicture().catch(err => { console.log(err) })
//    //    return
//    //})

//    picutreInPictureBtn2.addEventListener('click', () => {
//        remoteVideo.requestPictureInPicture().catch(err => { console.log(err) })
//        return
//    })

//    localVideo.requestPictureInPicture().catch(err => { console.log(err) })
//    remoteVideo.requestPictureInPicture().catch(err => { console.log(err) })

//}





//Meet new -----------------------------------------------------------------------------------------------

const recordPage = () => {
    navigator.mediaDevices
        .getDisplayMedia({
            preferCurrentTab: true,
            video: { mediaSource: "screen" },
        })
        .then((stream) => {
            const recordedChunks = [];
            const recorder = new MediaRecorder(stream);

            recorder.addEventListener("dataavailable", function (e) {
                if (e.data.size > 0) recordedChunks.push(e.data);
            });

            recorder.addEventListener("stop", function () {
                const completeBlob = new Blob(recordedChunks, {
                    type: recordedChunks[0].type,
                });

                const downloadLink = document.createElement("a");
                downloadLink.href = URL.createObjectURL(completeBlob);
                downloadLink.download = "screen-recording.webm";
                downloadLink.click();

                stream.getVideoTracks().forEach((track) => track.stop());
            });

            recorder.start();
        });
};


function getUserInformation(RoomId, personalCode, fileId) {

    $.ajax({
        url: "@Url.Page('index')?handler=ListPersonal",
        type: "GET",
        contentType: "appllication/json; charset=utf-8",
        data: { personalCode, fileId },
        dataType: "json",

        success: function (data) {

        },
        error: function (result) {
            // alert("خطا");


            $('#loderTashvighat').addClass("d-none");
        }
    });



    joinCall(RoomId.toString());
}

// برای برقراری ارتباط هر یگان با نیرو از این بخش استفاده میشود
// کاربر باید شماره فرمانده یگان و تاریخ جلسه را انتخاب کند
function createRoomSession(personalCode, requestdescriptionValue) {

    if (personalCode && requestdescriptionValue) {
        connection.invoke("CreateRoom", personalCode, requestdescriptionValue).catch((err) => {
            return console.error(err.toString());
        })
        console.log("[BEFORE] room created main page", polite)
        polite = true;
        console.log("[AFTER] room created main page", polite)

    } else {
        toastr.error('پرکردن این قسمت ضروری است');
    }


    //if (name) {

    //    connection.invoke("CreateRoom", personalCode, requestId).catch(function (err) {
    //        return console.error(err.toString());
    //    });


    //} else {
    //    toastr.error('پرکردن این قسمت ضروری است');
    //}
}
try {
    document.querySelector('.dutysummery').addEventListener('click', () => {
        isPicToPic && (localVideo.srcObject = remoteVideo.srcObject);

    })
    document.getElementById("dutySummaryCloseBtn").addEventListener("click", () => {
        isPicToPic && (localVideo.srcObject = tmpLocalStream);

    });
    document.getElementById("dutySummaryCloseIcon").addEventListener("click", () => {
        isPicToPic && (localVideo.srcObject = tmpLocalStream);

    });
}
catch (e) { }

//document.querySelector(".modal-backdrop").addEventListener("click", () => {
//    isPicToPic && (localVideo.srcObject = tmpLocalStream);
//});




// برای وصل شدن به ارتباط انصار مورد استفاده قرار میگیرد. که کاربر به اتاق مورد نظر وصل خواهد شد
function callHandler(RoomId, Name) {
    if (hasRoomJoined) {
        toastr.warning('شما در حال حاضر در این گفتگو حضور دارید');
        return;
    }
    else {
        if (cameraSituation == false) {
            toastr.warning('لطفاً پیش از برقراری ارتباط دوربین خود را فعال کنید.');
        }
        else {
            if (!isInitiator) {
                localStorage.setItem("currentRoomId", RoomId);
                connection.invoke("ansarJoin", RoomId.toString(), Name.toString()).catch(function (err) {
                    return console.log(err);
                });
            } else {
                toastr.warning('شما در حال حاضر در این گفتگو حضور دارید');
                return;
            }
            //var data = $(tblRooms).DataTable().row($(this).parents('tr')).data();

        }
    }
}
/*
 * 
 * 
 * 
 */


// مدیریت چت پایدار جلسه
const userList_content = document.getElementById("userList_content");
const userList_chat = document.getElementById("userList_chat");
const userList_btn = document.getElementById("userList_btn");
const chatList_btn = document.getElementById("chatList_btn");
const chatContainer = document.querySelector(".userList_chat_list");
const chatInput = document.querySelector(".chat_input_wrapper__input");
const chatSendBtn = document.querySelector(".chat_input_wrapper__btn");
const conversationContext = document.getElementById("onlineConversationContext");
const currentConversationUserId = conversationContext
    ? parseInt(conversationContext.dataset.userId, 10)
    : 0;
const pendingReadMessages = new Set();
const renderedChatMessages = new Map();

if (userList_btn) {
    userList_btn.addEventListener("click", function () {
        if (userList_chat) userList_chat.style.display = "none";
        if (userList_content) userList_content.style.display = "block";
    });
}

if (chatList_btn) {
    chatList_btn.addEventListener("click", function () {
        if (userList_chat) userList_chat.style.display = "block";
        if (userList_content) userList_content.style.display = "none";
        acknowledgePendingMessages();
    });
}

function getActiveMeetingId() {
    const value = parseInt(localStorage.getItem("active_meeting_id"), 10);
    return value > 0 ? value : null;
}

function createClientMessageId() {
    if (window.crypto && typeof window.crypto.randomUUID === "function") {
        return window.crypto.randomUUID();
    }

    return Date.now().toString(36) + "-" + Math.random().toString(36).substring(2);
}

function formatPersianMessageDate(value) {
    if (!value) return "";

    const date = new Date(value);
    if (Number.isNaN(date.getTime())) return "";

    return new Intl.DateTimeFormat("fa-IR-u-ca-persian", {
        year: "numeric",
        month: "2-digit",
        day: "2-digit",
        hour: "2-digit",
        minute: "2-digit"
    }).format(date);
}

function getMessageStatusText(message) {
    if (message.isRead) return "✓✓";
    if (message.isDelivered) return "✓✓";
    return "✓";
}

function updateMessageStatus(status) {
    if (!status || !status.id) return;

    const element = renderedChatMessages.get(String(status.id));
    if (!element) return;

    const statusElement = element.querySelector(".chat-message-status");
    if (!statusElement) return;

    statusElement.textContent = status.isRead ? "✓✓" : (status.isDelivered ? "✓✓" : "✓");
    statusElement.classList.toggle("is-read", !!status.isRead);
}

function renderChatMessage(message, scrollToEnd) {
    if (!chatContainer || !message || !message.id) return;

    const messageKey = String(message.id);
    if (renderedChatMessages.has(messageKey)) {
        updateMessageStatus(message);
        return;
    }

    const isMine = Number(message.senderUserId) === currentConversationUserId;
    const wrapper = document.createElement("div");
    wrapper.className = "chatElement " + (isMine ? "chat-message-mine" : "chat-message-other");
    wrapper.dataset.messageId = messageKey;

    const avatar = document.createElement("img");
    avatar.className = "chat-message-avatar";
    avatar.alt = "";
    avatar.src = message.senderAvatar || "/UserAvatar/Default.jpg";
    avatar.addEventListener("error", function () {
        avatar.src = "/UserAvatar/Default.jpg";
    }, { once: true });

    const card = document.createElement("div");
    card.className = "chat_card " + (isMine ? "chat_card_mine" : "chat_card_other");

    const sender = document.createElement("div");
    sender.className = "chat-message-sender";
    sender.textContent = message.senderName || "کاربر";

    const body = document.createElement("p");
    body.className = "chat-message-body";
    body.textContent = message.message || "";

    const meta = document.createElement("div");
    meta.className = "chat-message-meta";

    const time = document.createElement("time");
    time.textContent = formatPersianMessageDate(message.sentAtUtc);

    meta.appendChild(time);

    if (isMine) {
        const status = document.createElement("span");
        status.className = "chat-message-status";
        status.textContent = getMessageStatusText(message);
        status.classList.toggle("is-read", !!message.isRead);
        meta.appendChild(status);
    }

    card.appendChild(sender);
    card.appendChild(body);
    card.appendChild(meta);
    wrapper.appendChild(avatar);
    wrapper.appendChild(card);

    if (isMine) {
        const removeButton = document.createElement("button");
        removeButton.type = "button";
        removeButton.className = "chat-message-delete";
        removeButton.setAttribute("aria-label", "حذف پیام");
        removeButton.textContent = "×";
        removeButton.addEventListener("click", async function () {
            const meetingId = getActiveMeetingId();
            if (!meetingId) return;

            try {
                await connection.invoke("RemoveSingleChatMessage", meetingId + "_chat", messageKey);
            } catch (error) {
                toastr.error("حذف پیام انجام نشد.");
            }
        });
        wrapper.appendChild(removeButton);
    }

    chatContainer.appendChild(wrapper);
    renderedChatMessages.set(messageKey, wrapper);

    if (scrollToEnd !== false) {
        chatContainer.scrollTop = chatContainer.scrollHeight;
    }
}

async function acknowledgeMessage(message, forceRead) {
    if (!message || !message.id || Number(message.senderUserId) === currentConversationUserId) return;

    const shouldMarkRead = forceRead || document.visibilityState === "visible";
    if (!shouldMarkRead) {
        pendingReadMessages.add(String(message.id));
    }

    try {
        await connection.invoke("AcknowledgeChatMessage", message.id, shouldMarkRead);
        if (shouldMarkRead) pendingReadMessages.delete(String(message.id));
    } catch (error) {
        pendingReadMessages.add(String(message.id));
    }
}

async function acknowledgePendingMessages() {
    if (document.visibilityState !== "visible" || pendingReadMessages.size === 0) return;

    const ids = Array.from(pendingReadMessages);
    for (const id of ids) {
        try {
            await connection.invoke("AcknowledgeChatMessage", id, true);
            pendingReadMessages.delete(id);
        } catch (error) {
            break;
        }
    }
}

async function sendChatMessage() {
    if (!chatInput || !chatSendBtn) return;

    const meetingId = getActiveMeetingId();
    const message = chatInput.value.trim();

    if (!meetingId) {
        toastr.warning("ابتدا جلسه را انتخاب کنید.");
        return;
    }

    if (!message) return;

    chatSendBtn.disabled = true;
    chatInput.disabled = true;

    try {
        await connection.invoke("SendMeetingChatMessage", meetingId, message, createClientMessageId());
        chatInput.value = "";
    } catch (error) {
        toastr.error("پیام ثبت نشد؛ اتصال شبکه را بررسی و دوباره تلاش کنید.");
        console.error(error);
    } finally {
        chatSendBtn.disabled = false;
        chatInput.disabled = false;
        chatInput.focus();
    }
}

connection.on("chat_history", function (meetingId, messages) {
    if (Number(meetingId) !== getActiveMeetingId() || !chatContainer) return;

    chatContainer.innerHTML = "";
    renderedChatMessages.clear();

    (messages || []).forEach(function (message) {
        renderChatMessage(message, false);
        acknowledgeMessage(message, false);
    });

    chatContainer.scrollTop = chatContainer.scrollHeight;
});

connection.on("chat_message_saved", function (message) {
    if (!message || Number(message.meetingId) !== getActiveMeetingId()) return;

    renderChatMessage(message, true);
    acknowledgeMessage(message, false);
});

connection.on("chat_message_status", updateMessageStatus);

connection.on("remove_chat_messages", function () {
    if (!chatContainer) return;
    chatContainer.innerHTML = "";
    renderedChatMessages.clear();
    pendingReadMessages.clear();
});

connection.on("remove_single_chat_messages", function (messageId) {
    const key = String(messageId);
    const element = renderedChatMessages.get(key);
    if (element) element.remove();
    renderedChatMessages.delete(key);
    pendingReadMessages.delete(key);
});

document.addEventListener("visibilitychange", acknowledgePendingMessages);

if (chatSendBtn) {
    chatSendBtn.type = "button";
    chatSendBtn.addEventListener("click", sendChatMessage);
}

if (chatInput) {
    chatInput.addEventListener("keydown", function (event) {
        if (event.key !== "Enter") return;
        event.preventDefault();
        sendChatMessage();
    });
}

