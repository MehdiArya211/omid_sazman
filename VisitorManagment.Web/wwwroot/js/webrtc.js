"use strict";

//تعریف کانکشن به سیگنال آر
//===================================================================================
var connection = new signalR.HubConnectionBuilder().withUrl("/WebRTCHub").build();
//===================================================================================
//تنظیمات اولیه
//===================================================================================
const configuration = {
    'iceServers': [
        {//ip server turn between two browser
            'urls': 'turn:10.128.155.120:3478',
            'username': 'root',
            'credential': 'qwer@123123'
        },
        {
            //stun=> erja
            'urls': 'stun:10.128.155.120:3478'
        }
    ]
};
const peerConn = new RTCPeerConnection(configuration);
//===================================================================================

//دریافت المان های فرم مکالمه تصویری
//===================================================================================
const txtRoomName = document.getElementById('txtRoomName');
const txtUnitName = document.getElementById('txtUnitName');

const codecPreferences = document.getElementById('codecPreferences');
const chkEchoCancellation = document.getElementById('chkEchoCancellation');

const btnCreateRoom = document.getElementById('btnCreateRoom');
const btnExitRoom = document.getElementById('btnExitRoom');
const btnOpenCamera = document.getElementById('btnOpenCamera');

//const btnStartCamera = document.getElementById('btnStartCamera');
const btnRecord = document.getElementById('btnRecord');
const btnPlay = document.getElementById('btnPlay');
const btnDownlaod = document.getElementById('btnDownlaod');
//const btnStopCamera = document.getElementById('btnStopCamera');

const roomTable = document.getElementById('roomTable');
const connectionStatusMessage = document.getElementById('connectionStatusMessage');

const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');
//===================================================================================

//متغیر های سراسری
//===================================================================================
//شناسه اتاق جلسه نفر
let myRoomId;

//رشته دریافتی از مدیا نفر
let localStream;

//رشته دریافتی از مدیا جهت ارسال به طرف مقابل
let remoteStream;

//بررسی وضعیت دوربین
let IsCameraClosed = false;

//آغازگر جلسه
let isInitiator = false;

//وضعیت وضعیت حضور در اتاق جلسه
let hasRoomJoined = false;
//===================================================================================

//عملیات دوربین
//============================================================
//وصل دوربین
//btnStartCamera.addEventListener('click', async () => {
//    btnStartCamera.disabled = true;
//    btnStopCamera.disabled = false;
//    btnRecord.disabled = false;
//    btnPlay.disabled = true;
//    btnDownlaod.disabled = true;
//    chkEchoCancellation.disabled = false;
//    codecPreferences.disabled = false;
//    const hasEchoCancellation = chkEchoCancellation.checked;
//    const constraints = {
//        audio: {
//            echoCancellation: { exact: hasEchoCancellation }
//        },
//        video: {
//            width: 1280, height: 720
//        }
//    };
//    console.log('Using media constraints:', constraints);
//    await grabWebCamVideo();
//});

//قطع دوربین
//btnStopCamera.addEventListener('click', async () => {
//    btnStopCamera.disabled = true;
//    btnStartCamera.disabled = false;
//    btnRecord.disabled = true;
//    btnPlay.disabled = true;
//    btnDownlaod.disabled = true;
//    chkEchoCancellation.disabled = true;
//    codecPreferences.disabled = true;
//    const mediaStream = localVideo.srcObject;
//    await mediaStream.getTracks().forEach(track => track.stop());
//    localVideo.srcObject = null;
//});

function grabWebCamVideo() {
    console.log('Getting user media (video) ...');
    const hasEchoCancellation = chkEchoCancellation.checked;
    navigator.mediaDevices.getUserMedia({

        audio: { echoCancellation: { exact: hasEchoCancellation } },
        video: true
    })
        .then(gotStream)
        .catch(function (e) {
            alert('getUserMedia() error: ' + e.name);
        });
}

function gotStream(stream) {
    console.log('getUserMedia video stream URL:', stream);
    localStream = stream;
    peerConn.addStream(localStream);
    localVideo.srcObject = stream;
    console.log('Open Camera');
    //added
    getSupportedMimeTypes().forEach(mimeType => {
        const option = document.createElement('option');
        option.value = mimeType;
        option.innerText = option.value;
        codecPreferences.appendChild(option);
    });
    codecPreferences.disabled = false;
}


//============================================================
//ضبط و دانلود مکالمه
//============================================================
let mediaRecorder;
let recordedBlobs;


btnRecord.addEventListener('click', () => {
    if (btnRecord.textContent === 'ضبط مکالمه') {
        startRecording();
        console
    } else {
        stopRecording();
        btnRecord.textContent = 'ضبط مکالمه';
        btnPlay.disabled = false;
        btnDownlaod.disabled = false;
        codecPreferences.disabled = false;
    }
});

//کلید پخش مکالمه
btnPlay.addEventListener('click', () => {
    const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value.split(';', 1)[0];
    const superBuffer = new Blob(recordedBlobs, { type: mimeType });
    localVideo.src = null;
    localVideo.srcObject = null;
    localVideo.src = window.URL.createObjectURL(superBuffer);
    localVideo.controls = true;
    localVideo.play();
});
//کلید دانلود مکالمه
btnDownlaod.addEventListener('click', () => {
    const blob = new Blob(recordedBlobs, { type: 'video/webm' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.style.display = 'none';
    a.href = url;
    a.download = 'test.webm';
    document.body.appendChild(a);
    a.click();
    PostBlob(blob);
    setTimeout(() => {
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
    }, 100);

});



function handleDataAvailable(event) {
    console.log('handleDataAvailable', event);
    if (event.data && event.data.size > 0) {
        recordedBlobs.push(event.data);
    }
}

function getSupportedMimeTypes() {
    const possibleTypes = [
        'video/webm;codecs=vp9,opus',
        'video/webm;codecs=vp8,opus',
        'video/webm;codecs=h264,opus',
        'video/mp4;codecs=h264,aac',
    ];
    return possibleTypes.filter(mimeType => {
        return MediaRecorder.isTypeSupported(mimeType);
    });
}


function startRecording() {
    recordedBlobs = [];
    const mimeType = codecPreferences.options[codecPreferences.selectedIndex].value;
    const options = { mimeType };

    try {
        console.log(window.stream);
        mediaRecorder = new MediaRecorder(window.stream, options);
    } catch (e) {
        console.error('Exception while creating MediaRecorder:', e);
        errorMsgElement.innerHTML = `Exception while creating MediaRecorder: ${JSON.stringify(e)}`;
        return;
    }

    console.log('Created MediaRecorder', mediaRecorder, 'with options', options);
    btnRecord.textContent = 'خاتمه ضبط';
    btnPlay.disabled = true;
    btnDownlaod.disabled = true;
    codecPreferences.disabled = true;
    mediaRecorder.onstop = (event) => {
        console.log('Recorder stopped: ', event);
        console.log('Recorded Blobs: ', recordedBlobs);
    };
    mediaRecorder.ondataavailable = handleDataAvailable;
    mediaRecorder.start();
    console.log('MediaRecorder started', mediaRecorder);
}

function stopRecording() {
    mediaRecorder.stop();
}


//برای ذخیره استریم دوربین
function PostBlob(blob) {
    // FormData
    var formData = new FormData();
    formData.append('video-blob', blob);

    // progress-bar
    //var hr = document.createElement('hr');
    //container.appendChild(hr);
    //var strong = document.createElement('strong');
    //strong.id = 'percentage';
    //strong.innerHTML = 'Video upload progress: ';
    //container.appendChild(strong);
    //var progress = document.createElement('progress');
    //container.appendChild(progress);

    // POST the Blob  
    $.ajax({
        type: 'POST',
        url: "Video/SaveRecoredFile",
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result) {
                console.log('Success');
            }
        },
        error: function (result) {
            console.log(result);
        }
    })
}


//============================================================

//ارسال فایل
//============================================================
//let fileReader;
//const fileInput = document.getElementById('fileInput');
//const btnSendFile = document.getElementById('btnSendFile');
//const fileTable = document.getElementById('fileTable');
//fileInput.disabled = true;
//btnSendFile.disabled = true;
//============================================================
$(roomTable).DataTable({
    columns: [
        { data: 'RoomId', "width": "10%" },
        { data: 'Name', "width": "30%" },
        { data: 'Unit', "width": "30%" },
        { data: 'Button', "width": "30%" }
    ],
    "lengthChange": false,
    "searching": false,
    "language": {
        "emptyTable": "لیست مخاطبین خالی می باشد..."
    }
});

//setup my video here.
grabWebCamVideo();
/****************************************************************************
* Signaling server
****************************************************************************/

//Connect to the signaling server
connection.start().then(function () {
    connection.on('updateRoom', function (data) {
        var obj = JSON.parse(data);
        $(roomTable).DataTable().clear().rows.add(obj).draw();
    });

    connection.on('created', function (roomId) {
        console.log('Created room', roomId);
        txtRoomName.disabled = true;
        txtUnitName.disabled = true;
        btnCreateRoom.disabled = true;
        btnExitRoom.disabled = false;
        hasRoomJoined = true;
        connectionStatusMessage.innerText = 'جلسه با شناسه ' + roomId + ' برای شما ایجاد شد. در انتظار پیوستن مخاطب...';
        myRoomId = roomId;
        isInitiator = true;
    });

    connection.on('joined', function (roomId) {
        console.log('This peer has joined room', roomId);
        myRoomId = roomId;
        isInitiator = false;
    });

    connection.on('error', function (message) {
        alert(message);
    });

    connection.on('ready', function () {
        console.log('Socket is ready');
        txtRoomName.disabled = true;
        txtUnitName.disabled = true;
        btnCreateRoom.disabled = true;
        hasRoomJoined = true;
        connectionStatusMessage.innerText = 'در حال برقراری تماس...';
        createPeerConnection(isInitiator, configuration);
    });

    connection.on('message', function (message) {
        console.log('Client received message:', message);
        signalingMessageCallback(message);
    });

    connection.on('bye', function () {
        console.log(`Peer leaving room.`);
        // If peer did not create the room, re-enter to be creator.
        connectionStatusMessage.innerText = `مخاطب شما جلسه شماره  ${myRoomId} را ترک کرد.`;
    });

    window.addEventListener('unload', function () {
        if (hasRoomJoined) {
            console.log(`Unloading window. Notifying peers in ${myRoomId}.`);
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
    return console.error(err.toString());
});

/**
* Send message to signaling server
*/
function sendMessage(message) {
    console.log('Client sending message: ', message);
    connection.invoke("SendMessage", myRoomId, message).catch(function (err) {
        return console.error(err.toString());
    });
}

/****************************************************************************
* Room management
****************************************************************************/

$(btnCreateRoom).click(function () {
    grabWebCamVideo();
    if (txtRoomName.value == '' || txtUnitName.value == '')
        alert('نام و نشان یا یگان وارد نشده است.');
    else {

        var name = txtRoomName.value;
        var unit = txtUnitName.value;
        connection.invoke("CreateRoom", name, unit).catch(function (err) {
            return console.error(err.toString());
        });
        hasRoomJoined = true;
        btnExitRoom.disabled = false;
        //if (IsCameraClosed) {

        //    console.log('Camera Status:', IsCameraClosed);
        //    grabWebCamVideo();
        //}
    }
});

$(btnExitRoom).click(async function () {
    connection.invoke("LeaveRoom", myRoomId).catch(function (err) {
        return console.error(err.toString());
    });
    if (hasRoomJoined == false)
        alert('گفتگوی فعال وجود ندارد.');
    else {
        alert('شما از اتاق گفتگو خارج شدید.');
        btnCreateRoom.disabled = false;
        btnExitRoom.disabled = true;
        txtRoomName.disabled = false;
        txtUnitName.disabled = false;
        hasRoomJoined = false;


        const localmediaStream = localVideo.srcObject;
        await localmediaStream.getTracks().forEach(track => track.stop());

        const remotemediaStream = remoteVideo.srcObject;
        await remotemediaStream.getTracks().forEach(track => track.stop());

        remoteStream = null;
        localStream = null;
        localVideo.srcObject = '';
        remoteVideo.srcObject = '';
        IsCameraClosed = true;

    }

});


$('#roomTable tbody').on('click', 'button', function () {
    if (hasRoomJoined) {
        alert('شما در این اتاق گفتگو حضور دارید.');
    } else {
        grabWebCamVideo();
        var data = $(roomTable).DataTable().row($(this).parents('tr')).data();
        connection.invoke("Join", data.RoomId).catch(function (err) {
            return console.error(err.toString());
        });
    }
});

//$(fileInput).change(function () {
//    let file = fileInput.files[0];
//    if (file) {
//        btnSendFile.disabled = false;
//    } else {
//        btnSendFile.disabled = true;
//    }
//});

//$(btnSendFile).click(function () {
//    btnSendFile.disabled = true;
//    sendFile();
//});




/****************************************************************************
* WebRTC peer connection and data channel
****************************************************************************/

var dataChannel;

function signalingMessageCallback(message) {
    if (message.type === 'offer') {
        console.log('Got offer. Sending answer to peer.');
        peerConn.setRemoteDescription(new RTCSessionDescription(message), function () { },
            logError);
        peerConn.createAnswer(onLocalSessionCreated, logError);
    } else if (message.type === 'answer') {
        console.log('Got answer.');
        peerConn.setRemoteDescription(new RTCSessionDescription(message), function () { },
            logError);
    } else if (message.type === 'candidate') {
        peerConn.addIceCandidate(new RTCIceCandidate({
            candidate: message.candidate
        }));
    }
}

function createPeerConnection(isInitiator, config) {
    console.log('Creating Peer connection as initiator?', isInitiator, 'config:',
        config);

    // send any ice candidates to the other peer
    peerConn.onicecandidate = function (event) {
        console.log('icecandidate event:', event);
        if (event.candidate) {
            // Trickle ICE
            //sendMessage({
            //    type: 'candidate',
            //    label: event.candidate.sdpMLineIndex,
            //    id: event.candidate.sdpMid,
            //    candidate: event.candidate.candidate
            //});
        } else {
            console.log('End of candidates.');
            // Vanilla ICE
            sendMessage(peerConn.localDescription);
        }
    };

    peerConn.ontrack = function (event) {
        console.log('icecandidate ontrack event:', event);
        //alert(event.streams[0]);
        //remoteVideo.srcObject = event.streams[0];

        if (IsCameraClosed) {
            remoteVideo.srcObject = null;
        }
        else {
            remoteStream = event.streams[0];
            remoteVideo.srcObject = event.streams[0];
        }
    };

    if (isInitiator) {
        console.log('Creating Data Channel');
        dataChannel = peerConn.createDataChannel('sendDataChannel');
        onDataChannelCreated(dataChannel);

        console.log('Creating an offer');
        peerConn.createOffer(onLocalSessionCreated, logError);
    } else {
        peerConn.ondatachannel = function (event) {
            console.log('ondatachannel:', event.channel);
            dataChannel = event.channel;
            onDataChannelCreated(dataChannel);
        };
    }
}

function onLocalSessionCreated(desc) {
    console.log('local session created:', desc);
    peerConn.setLocalDescription(desc, function () {
        // Trickle ICE
        //console.log('sending local desc:', peerConn.localDescription);
        //sendMessage(peerConn.localDescription);
    }, logError);
}

function onDataChannelCreated(channel) {
    console.log('onDataChannelCreated:', channel);

    channel.onopen = function () {
        console.log('Channel opened!!!');
        connectionStatusMessage.innerText = 'مکالمه بر قرار شد.';
        btnExitRoom.disabled = false;
        //fileInput.disabled = false;
    };

    channel.onclose = function () {
        console.log('Channel closed.');
        connectionStatusMessage.innerText = 'خاتمه مکالمه.';
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

            $(fileTable).children('tbody').append('<tr><td><a></a></td></tr>');
            const downloadAnchor = $(fileTable).find('a:last');
            downloadAnchor.attr('href', URL.createObjectURL(received));
            downloadAnchor.attr('download', fileName);
            downloadAnchor.text(`${fileName} (${fileSize} bytes)`);
        }
    };
}

function sendFile() {
    const file = fileInput.files[0];
    console.log(`File is ${[file.name, file.size, file.type, file.lastModified].join(' ')}`);

    if (file.size === 0) {
        alert('File is empty, please select a non-empty file.');
        return;
    }

    //send file size and file name as comma separated value.
    dataChannel.send(file.size + ',' + file.name);

    const chunkSize = 16384;
    fileReader = new FileReader();
    let offset = 0;
    fileReader.addEventListener('error', error => console.error('Error reading file:', error));
    fileReader.addEventListener('abort', event => console.log('File reading aborted:', event));
    fileReader.addEventListener('load', e => {
        console.log('FileRead.onload ', e);
        dataChannel.send(e.target.result);
        offset += e.target.result.byteLength;
        if (offset < file.size) {
            readSlice(offset);
        } else {
            alert(`${file.name} has been sent successfully.`);
            btnSendFile.disabled = false;
        }
    });
    const readSlice = o => {
        console.log('readSlice ', o);
        const slice = file.slice(offset, o + chunkSize);
        fileReader.readAsArrayBuffer(slice);
    };
    readSlice(0);
}

/****************************************************************************
* Auxiliary functions
****************************************************************************/

function logError(err) {
    if (!err) return;
    if (typeof err === 'string') {
        console.warn(err);
    } else {
        console.warn(err.toString(), err);
    }
}
