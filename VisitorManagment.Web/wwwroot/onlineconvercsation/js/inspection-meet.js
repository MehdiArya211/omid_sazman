(function () {
    'use strict';
    const root = document.querySelector('.inspection-meet');
    if (!root || typeof signalR === 'undefined') return;
    const unitCode = root.dataset.unitCode;
    const state = document.getElementById('connectionState');
    const startButton = document.getElementById('startInspectionRoom');
    const leaveButton = document.getElementById('leaveInspectionRoom');
    const micButton = document.getElementById('toggleInspectionMic');
    const cameraButton = document.getElementById('toggleInspectionCamera');
    const localVideo = document.getElementById('localInspectionVideo');
    const remoteVideo = document.getElementById('remoteInspectionVideo');
    const roomList = document.getElementById('roomList');
    const placeholder = document.getElementById('callPlaceholder');
    const connection = new signalR.HubConnectionBuilder().withUrl('/NezRTCHub').build();
    const rtcConfiguration = { iceServers: [] };
    let selectedMeetingId = null, roomId = null, localStream = null, peer = null, isInitiator = false;

    const notify = message => window.toastr ? toastr.info(message) : alert(message);
    const setConnected = connected => { state.textContent = connected ? 'متصل' : 'قطع ارتباط'; state.classList.toggle('is-online', connected); state.classList.toggle('is-offline', !connected); startButton.disabled = !connected || !selectedMeetingId; };
    const sendSignal = payload => connection.invoke('SendMessage', roomId, payload).catch(console.error);

    async function ensureMedia() {
        if (localStream) return true;
        try { localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true }); localVideo.srcObject = localStream; placeholder.hidden = true; micButton.disabled = cameraButton.disabled = false; return true; }
        catch { notify('دسترسی دوربین و میکروفن تأیید نشد. تنظیمات مرورگر را بررسی کنید.'); return false; }
    }
    function createPeer() {
        if (peer) peer.close();
        peer = new RTCPeerConnection(rtcConfiguration);
        localStream.getTracks().forEach(track => peer.addTrack(track, localStream));
        peer.ontrack = event => { remoteVideo.srcObject = event.streams[0]; };
        peer.onicecandidate = event => { if (event.candidate) sendSignal({ candidate: event.candidate }); };
    }
    async function makeOffer() { createPeer(); const offer = await peer.createOffer(); await peer.setLocalDescription(offer); sendSignal({ description: peer.localDescription }); }
    async function closeCall(notifyServer) {
        if (notifyServer && roomId) await connection.invoke('LeaveRoom', roomId).catch(() => {});
        if (peer) { peer.ontrack = null; peer.close(); peer = null; }
        if (localStream) { localStream.getTracks().forEach(track => track.stop()); localStream = null; }
        localVideo.srcObject = remoteVideo.srcObject = null; roomId = null; leaveButton.disabled = micButton.disabled = cameraButton.disabled = true; startButton.disabled = !selectedMeetingId; placeholder.hidden = false;
    }

    document.querySelectorAll('.inspection-meeting-item').forEach(button => button.addEventListener('click', async () => {
        document.querySelectorAll('.inspection-meeting-item').forEach(item => item.classList.remove('active')); button.classList.add('active');
        selectedMeetingId = button.dataset.meetingId;
        const response = await fetch(`?handler=ValidateMeeting&meetingId=${encodeURIComponent(selectedMeetingId)}`); const result = await response.json();
        if (!result.isAllowed) { selectedMeetingId = null; notify('این جلسه برای یگان شما قابل ورود نیست.'); }
        startButton.disabled = connection.state !== signalR.HubConnectionState.Connected || !selectedMeetingId;
        if (selectedMeetingId) connection.invoke('GetRoomInfo').catch(console.error);
    }));
    async function registerAttendance() {
        const result = await connection.invoke('RegisterInspectionAttendance', Number(selectedMeetingId));
        if (!result || !result.isSuccess) { notify(result?.message || 'ثبت حضور در جلسه انجام نشد.'); return false; }
        return true;
    }
    startButton.addEventListener('click', async () => { if (!selectedMeetingId || !await ensureMedia() || !await registerAttendance()) return; isInitiator = true; await connection.invoke('CreateRoom', unitCode, `inspection-${selectedMeetingId}`); startButton.disabled = true; leaveButton.disabled = false; });
    leaveButton.addEventListener('click', async () => { await connection.invoke('EndInspectionAttendance').catch(() => {}); await closeCall(true); });
    micButton.addEventListener('click', () => { const track=localStream?.getAudioTracks()[0]; if(track){track.enabled=!track.enabled;micButton.classList.toggle('is-muted',!track.enabled);} });
    cameraButton.addEventListener('click', () => { const track=localStream?.getVideoTracks()[0]; if(track){track.enabled=!track.enabled;cameraButton.classList.toggle('is-muted',!track.enabled);} });
    connection.on('created', id => { roomId=id; connection.invoke('ChatJoin', `inspection-${selectedMeetingId}-chat`).catch(console.error); });
    connection.on('joined', id => { roomId=id; leaveButton.disabled=false; });
    connection.on('ready', async id => { if(id===roomId && isInitiator) await makeOffer(); });
    connection.on('message', async data => { if(!localStream && !await ensureMedia()) return; if(!peer) createPeer(); if(data.description){await peer.setRemoteDescription(data.description);if(data.description.type==='offer'){const answer=await peer.createAnswer();await peer.setLocalDescription(answer);sendSignal({description:peer.localDescription});}} else if(data.candidate){await peer.addIceCandidate(data.candidate);} });
    connection.on('bye', () => closeCall(false));
    connection.on('updateRoom', data => { if(!selectedMeetingId) return; const rooms=JSON.parse(data).filter(x=>String(x.MeetId)===`inspection-${selectedMeetingId}`); roomList.innerHTML=''; rooms.forEach(room=>{if(String(room.PersonalCode)===String(unitCode))return;const button=document.createElement('button');button.type='button';button.className='btn btn-outline-info';button.textContent=`ورود به اتاق یگان ${room.PersonalCode}`;button.onclick=async()=>{if(!await ensureMedia()||!await registerAttendance())return;isInitiator=false;roomId=String(room.RoomId);await connection.invoke('Join',roomId);leaveButton.disabled=false;};roomList.appendChild(button);}); });
    connection.onclose(() => { setConnected(false); setTimeout(startConnection, 3000); });
    async function startConnection(){try{if(connection.state===signalR.HubConnectionState.Disconnected)await connection.start();setConnected(true);await connection.invoke('GetRoomInfo');}catch{setConnected(false);setTimeout(startConnection,3000);}}
    window.addEventListener('beforeunload',()=>{if(localStream)localStream.getTracks().forEach(track=>track.stop());});
    startConnection();
})();
