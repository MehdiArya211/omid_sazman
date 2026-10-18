(function () {
    'use strict';

    const root = document.querySelector('.inspection-meet');
    if (!root) return;

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

    if (typeof signalR === 'undefined') {
        state.textContent = 'کتابخانه ارتباطی بارگذاری نشد';
        state.classList.add('is-offline');
        return;
    }

    const connection = new signalR.HubConnectionBuilder().withUrl('/NezRTCHub').build();
    const rtcConfiguration = { iceServers: [] };
    let selectedMeetingId = null;
    let roomId = null;
    let localStream = null;
    let peer = null;
    let isInitiator = false;
    let isStartingRoom = false;
    let reconnectTimer = null;
    let pendingCandidates = [];

    const isSignalRConnected = () => connection.state === signalR.HubConnectionState.Connected;
    const notify = (message, level) => {
        if (window.toastr && typeof toastr[level || 'info'] === 'function') toastr[level || 'info'](message);
        else window.alert(message);
    };

    function setConnectionState(connected, message) {
        state.textContent = message || (connected ? 'متصل' : 'قطع ارتباط');
        state.classList.toggle('is-online', connected);
        state.classList.toggle('is-offline', !connected);
        startButton.disabled = !connected || !selectedMeetingId || isStartingRoom || !!roomId;
    }

    function setStartBusy(busy) {
        isStartingRoom = busy;
        startButton.innerHTML = busy
            ? '<span class="spinner-border spinner-border-sm" aria-hidden="true"></span> در حال ورود...'
            : '<i class="ti-video-camera"></i> ایجاد/ورود به جلسه';
        startButton.disabled = busy || !isSignalRConnected() || !selectedMeetingId || !!roomId;
    }

    async function sendSignal(payload) {
        if (!roomId || !isSignalRConnected()) throw new Error('SignalR connection is not ready.');
        await connection.invoke('SendMessage', roomId, payload);
    }

    async function ensureMedia() {
        if (localStream && localStream.active) return true;
        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            notify('مرورگر یا بستر فعلی امکان دسترسی به دوربین و میکروفن را ندارد. صفحه را با HTTPS باز کنید.', 'error');
            return false;
        }

        try {
            localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });
            localVideo.srcObject = localStream;
            await localVideo.play().catch(() => {});
            placeholder.hidden = true;
            micButton.disabled = false;
            cameraButton.disabled = false;
            return true;
        } catch (error) {
            const denied = error && (error.name === 'NotAllowedError' || error.name === 'PermissionDeniedError');
            notify(denied
                ? 'دسترسی دوربین و میکروفن رد شده است. دسترسی مرورگر را فعال و دوباره تلاش کنید.'
                : 'دوربین یا میکروفن در دسترس نیست. اتصال دستگاه و استفاده نشدن آن در برنامه دیگر را بررسی کنید.', 'error');
            return false;
        }
    }

    function stopLocalMedia() {
        if (localStream) localStream.getTracks().forEach(track => track.stop());
        localStream = null;
        localVideo.srcObject = null;
    }

    function closePeer() {
        pendingCandidates = [];
        if (!peer) return;
        peer.ontrack = null;
        peer.onicecandidate = null;
        peer.onconnectionstatechange = null;
        peer.close();
        peer = null;
        remoteVideo.srcObject = null;
    }

    function resetCallUi() {
        roomId = null;
        isInitiator = false;
        closePeer();
        stopLocalMedia();
        leaveButton.disabled = true;
        micButton.disabled = true;
        cameraButton.disabled = true;
        micButton.classList.remove('is-muted');
        cameraButton.classList.remove('is-muted');
        placeholder.hidden = false;
        setStartBusy(false);
    }

    async function closeCall(notifyServer) {
        const leavingRoomId = roomId;
        resetCallUi();
        if (notifyServer && leavingRoomId && isSignalRConnected()) {
            await connection.invoke('LeaveRoom', leavingRoomId).catch(() => {
                notify('خروج محلی انجام شد، اما اعلام خروج به طرف مقابل ارسال نشد.', 'warning');
            });
        }
        if (isSignalRConnected()) await refreshRooms();
    }

    function createPeer() {
        closePeer();
        peer = new RTCPeerConnection(rtcConfiguration);
        localStream.getTracks().forEach(track => peer.addTrack(track, localStream));
        peer.ontrack = event => {
            const stream = event.streams && event.streams[0];
            if (stream) {
                remoteVideo.srcObject = stream;
                remoteVideo.play().catch(() => {});
            }
        };
        peer.onicecandidate = event => {
            if (event.candidate) sendSignal({ candidate: event.candidate }).catch(() => {
                notify('ارسال اطلاعات برقراری تماس انجام نشد.', 'warning');
            });
        };
        peer.onconnectionstatechange = () => {
            if (peer && peer.connectionState === 'failed')
                notify('ارتباط تصویری برقرار نشد. وضعیت شبکه دو طرف را بررسی و دوباره وارد جلسه شوید.', 'error');
        };
    }

    async function makeOffer() {
        createPeer();
        const offer = await peer.createOffer();
        await peer.setLocalDescription(offer);
        await sendSignal({ description: peer.localDescription });
    }

    async function applySignal(data) {
        if (!data || !roomId) return;
        if (!localStream && !await ensureMedia()) return;
        if (!peer) createPeer();

        if (data.description) {
            await peer.setRemoteDescription(data.description);
            while (pendingCandidates.length) await peer.addIceCandidate(pendingCandidates.shift());
            if (data.description.type === 'offer') {
                const answer = await peer.createAnswer();
                await peer.setLocalDescription(answer);
                await sendSignal({ description: peer.localDescription });
            }
        } else if (data.candidate) {
            if (peer.remoteDescription) await peer.addIceCandidate(data.candidate);
            else pendingCandidates.push(data.candidate);
        }
    }

    async function validateMeeting(meetingId) {
        const response = await fetch(`?handler=ValidateMeeting&meetingId=${encodeURIComponent(meetingId)}`, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });
        if (!response.ok) throw new Error(`Meeting validation failed: ${response.status}`);
        return response.json();
    }

    async function refreshRooms() {
        if (isSignalRConnected()) await connection.invoke('GetRoomInfo').catch(() => {});
    }

    document.querySelectorAll('.inspection-meeting-item').forEach(button => button.addEventListener('click', async () => {
        if (roomId) {
            notify('ابتدا از تماس فعلی خارج شوید، سپس جلسه دیگری را انتخاب کنید.', 'warning');
            return;
        }

        const requestedMeetingId = button.dataset.meetingId;
        startButton.disabled = true;
        try {
            const result = await validateMeeting(requestedMeetingId);
            if (!result.isAllowed) {
                selectedMeetingId = null;
                button.classList.remove('active');
                notify('این جلسه برای یگان شما قابل ورود نیست یا دیگر منتشرشده نیست.', 'warning');
                return;
            }

            document.querySelectorAll('.inspection-meeting-item').forEach(item => item.classList.remove('active'));
            button.classList.add('active');
            selectedMeetingId = requestedMeetingId;
            setConnectionState(isSignalRConnected());
            await refreshRooms();
        } catch {
            selectedMeetingId = null;
            button.classList.remove('active');
            notify('اعتبارسنجی جلسه انجام نشد. اتصال شبکه را بررسی و دوباره تلاش کنید.', 'error');
        } finally {
            startButton.disabled = !isSignalRConnected() || !selectedMeetingId || isStartingRoom || !!roomId;
        }
    }));

    startButton.addEventListener('click', async () => {
        if (!selectedMeetingId || isStartingRoom || roomId) return;
        if (!isSignalRConnected()) {
            notify('ارتباط با سرور برقرار نیست. چند لحظه صبر کنید.', 'warning');
            return;
        }

        setStartBusy(true);
        try {
            if (!await ensureMedia()) return;
            isInitiator = true;
            await connection.invoke('CreateInspectionRoom', Number(selectedMeetingId));
        } catch {
            resetCallUi();
            notify('ایجاد اتاق جلسه انجام نشد. دوباره تلاش کنید.', 'error');
        } finally {
            if (!roomId) setStartBusy(false);
        }
    });

    leaveButton.addEventListener('click', () => closeCall(true));
    micButton.addEventListener('click', () => {
        const track = localStream && localStream.getAudioTracks()[0];
        if (!track) return;
        track.enabled = !track.enabled;
        micButton.classList.toggle('is-muted', !track.enabled);
    });
    cameraButton.addEventListener('click', () => {
        const track = localStream && localStream.getVideoTracks()[0];
        if (!track) return;
        track.enabled = !track.enabled;
        cameraButton.classList.toggle('is-muted', !track.enabled);
    });

    connection.on('created', id => {
        roomId = String(id);
        isInitiator = true;
        setStartBusy(false);
        startButton.disabled = true;
        leaveButton.disabled = false;
    });
    connection.on('joined', id => {
        roomId = String(id);
        isInitiator = false;
        leaveButton.disabled = false;
        startButton.disabled = true;
    });
    connection.on('ready', async id => {
        if (String(id) !== String(roomId) || !isInitiator) return;
        try { await makeOffer(); }
        catch { notify('شروع ارتباط تصویری انجام نشد. دوباره وارد جلسه شوید.', 'error'); }
    });
    connection.on('message', data => applySignal(data).catch(() => {
        notify('پردازش اطلاعات ارتباط تصویری با خطا روبه‌رو شد.', 'error');
    }));
    connection.on('bye', async () => {
        const wasHost = isInitiator;
        if (wasHost) await closeCall(true);
        else resetCallUi();
        notify('طرف مقابل از تماس خارج شد.', 'info');
        await refreshRooms();
    });
    connection.on('error', message => {
        resetCallUi();
        notify(message || 'ایجاد اتاق با خطا روبه‌رو شد.', 'error');
    });
    connection.on('updateRoom', data => {
        if (!selectedMeetingId || roomId) return;
        let rooms;
        try { rooms = typeof data === 'string' ? JSON.parse(data) : data; }
        catch { rooms = []; }
        if (!Array.isArray(rooms)) rooms = [];

        const meetingKey = `inspection-${selectedMeetingId}`;
        roomList.innerHTML = '';
        rooms.filter(item => String(item.MeetId || item.meetId) === meetingKey).forEach(room => {
            const ownerUnit = String(room.PersonalCode || room.personalCode || '');
            if (ownerUnit === String(unitCode)) return;
            const targetRoomId = String(room.RoomId || room.roomId || '');
            if (!targetRoomId) return;

            const button = document.createElement('button');
            button.type = 'button';
            button.className = 'btn btn-outline-info inspection-room-button';
            button.textContent = `ورود به اتاق یگان ${ownerUnit}`;
            button.addEventListener('click', async () => {
                if (roomId || isStartingRoom) return;
                setStartBusy(true);
                try {
                    if (!await ensureMedia()) return;
                    isInitiator = false;
                    roomId = targetRoomId;
                    await connection.invoke('JoinInspectionRoom', roomId, Number(selectedMeetingId));
                } catch {
                    resetCallUi();
                    notify('ورود به اتاق انجام نشد؛ ممکن است میزبان از جلسه خارج شده باشد.', 'error');
                    await refreshRooms();
                } finally {
                    if (!roomId) setStartBusy(false);
                }
            });
            roomList.appendChild(button);
        });
    });

    connection.onclose(() => {
        resetCallUi();
        setConnectionState(false, 'در حال اتصال مجدد...');
        window.clearTimeout(reconnectTimer);
        reconnectTimer = window.setTimeout(startConnection, 3000);
    });

    async function startConnection() {
        window.clearTimeout(reconnectTimer);
        if (connection.state !== signalR.HubConnectionState.Disconnected) return;
        try {
            setConnectionState(false, 'در حال اتصال...');
            await connection.start();
            setConnectionState(true);
            await refreshRooms();
        } catch {
            setConnectionState(false, 'اتصال برقرار نشد');
            reconnectTimer = window.setTimeout(startConnection, 3000);
        }
    }

    window.addEventListener('beforeunload', () => {
        closePeer();
        stopLocalMedia();
    });

    startConnection();
})();
