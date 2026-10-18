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
    const testButton = document.getElementById('testInspectionMedia');
    const mediaTest = document.getElementById('inspectionMediaTest');
    const mediaTestVideo = document.getElementById('inspectionMediaTestVideo');
    const confirmMediaButton = document.getElementById('confirmInspectionMedia');
    const cancelMediaButton = document.getElementById('cancelInspectionMedia');
    const audioMeterFill = document.getElementById('inspectionAudioMeterFill');
    const callMessage = document.getElementById('inspectionCallMessage');
    const fullscreenButton = document.getElementById('toggleInspectionFullscreen');
    const videoStage = document.getElementById('inspectionVideoStage');
    const cameraSelect = document.getElementById('inspectionCameraSelect');
    const microphoneSelect = document.getElementById('inspectionMicrophoneSelect');
    const refreshDevicesButton = document.getElementById('refreshInspectionDevices');

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
    let audioContext = null;
    let audioMeterFrame = null;

    const isSignalRConnected = () => connection.state === signalR.HubConnectionState.Connected;
    const notify = (message, level) => {
        if (window.toastr && typeof toastr[level || 'info'] === 'function') toastr[level || 'info'](message);
        else window.alert(message);
    };
    const setCallMessage = (message, level) => {
        callMessage.textContent = message;
        callMessage.className = `inspection-call-message is-${level || 'info'}`;
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

    function selectedDeviceConstraints() {
        return {
            video: cameraSelect.value ? { deviceId: { exact: cameraSelect.value } } : true,
            audio: microphoneSelect.value ? { deviceId: { exact: microphoneSelect.value } } : true
        };
    }

    async function loadMediaDevices() {
        if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) return;
        const devices = await navigator.mediaDevices.enumerateDevices();
        const previousCamera = cameraSelect.value || sessionStorage.getItem('inspection-camera-id') || '';
        const previousMicrophone = microphoneSelect.value || sessionStorage.getItem('inspection-microphone-id') || '';
        const fill = (select, items, prefix, selectedId) => {
            select.innerHTML = '';
            items.forEach((device, index) => {
                const option = document.createElement('option');
                option.value = device.deviceId;
                option.textContent = device.label || `${prefix} ${index + 1}`;
                option.selected = !!selectedId && device.deviceId === selectedId;
                select.appendChild(option);
            });
            if (!items.length) {
                const option = document.createElement('option');
                option.textContent = `${prefix} یافت نشد`;
                option.value = '';
                select.appendChild(option);
            }
        };
        fill(cameraSelect, devices.filter(device => device.kind === 'videoinput'), 'دوربین', previousCamera);
        fill(microphoneSelect, devices.filter(device => device.kind === 'audioinput'), 'میکروفن', previousMicrophone);
    }

    async function ensureMedia(forceNewStream) {
        if (localStream && localStream.active && !forceNewStream) return true;
        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            notify('مرورگر یا بستر فعلی امکان دسترسی به دوربین و میکروفن را ندارد. صفحه را با HTTPS باز کنید.', 'error');
            return false;
        }

        try {
            if (!cameraSelect.options.length || !microphoneSelect.options.length) await loadMediaDevices();
            if (forceNewStream) stopLocalMedia();
            localStream = await navigator.mediaDevices.getUserMedia(selectedDeviceConstraints());
            localVideo.srcObject = localStream;
            await localVideo.play().catch(() => {});
            sessionStorage.setItem('inspection-camera-id', cameraSelect.value || '');
            sessionStorage.setItem('inspection-microphone-id', microphoneSelect.value || '');
            await loadMediaDevices();
            placeholder.hidden = true;
            micButton.disabled = false;
            cameraButton.disabled = false;
            setCallMessage('دوربین و میکروفن آماده‌اند.', 'success');
            return true;
        } catch (error) {
            const denied = error && (error.name === 'NotAllowedError' || error.name === 'PermissionDeniedError');
            const busy = error && (error.name === 'NotReadableError' || error.name === 'TrackStartError');
            const message = denied ? 'دسترسی دوربین و میکروفن رد شده است. مجوز مرورگر را فعال کنید.'
                : busy ? 'دستگاه انتخاب‌شده در مرورگر دیگری در حال استفاده است؛ دوربین دوم را از فهرست انتخاب کنید.'
                : 'دوربین یا میکروفن انتخاب‌شده در دسترس نیست.';
            notify(message, 'error');
            setCallMessage(message, 'danger');
            return false;
        }
    }

    function stopAudioMeter() {
        if (audioMeterFrame) cancelAnimationFrame(audioMeterFrame);
        audioMeterFrame = null;
        if (audioContext) audioContext.close().catch(() => {});
        audioContext = null;
        audioMeterFill.style.width = '0%';
    }

    function startAudioMeter() {
        stopAudioMeter();
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        const audioTrack = localStream && localStream.getAudioTracks()[0];
        if (!AudioContext || !audioTrack) return;
        audioContext = new AudioContext();
        const analyser = audioContext.createAnalyser();
        analyser.fftSize = 256;
        audioContext.createMediaStreamSource(new MediaStream([audioTrack])).connect(analyser);
        const values = new Uint8Array(analyser.frequencyBinCount);
        const draw = () => {
            analyser.getByteFrequencyData(values);
            const average = values.reduce((sum, value) => sum + value, 0) / values.length;
            audioMeterFill.style.width = `${Math.min(100, average * 1.6)}%`;
            audioMeterFrame = requestAnimationFrame(draw);
        };
        draw();
    }

    async function openMediaTest() {
        testButton.disabled = true;
        mediaTest.hidden = false;
        setCallMessage('در حال دریافت مجوز دوربین و میکروفن...', 'info');
        try {
            await loadMediaDevices();
            if (!await ensureMedia()) return;
            mediaTestVideo.srcObject = localStream;
            await mediaTestVideo.play().catch(() => {});
            startAudioMeter();
        } catch {
            setCallMessage('خواندن فهرست دوربین‌ها و میکروفن‌ها انجام نشد.', 'danger');
        } finally { testButton.disabled = false; }
    }

    async function switchMediaDevice() {
        setCallMessage('در حال تغییر دستگاه...', 'info');
        if (!await ensureMedia(true)) return;
        mediaTestVideo.srcObject = localStream;
        await mediaTestVideo.play().catch(() => {});
        startAudioMeter();
        setCallMessage('دستگاه جدید آماده است.', 'success');
    }

    function closeMediaTest(stopTracks) {
        mediaTest.hidden = true;
        mediaTestVideo.srcObject = null;
        stopAudioMeter();
        if (stopTracks && !roomId) {
            stopLocalMedia();
            micButton.disabled = true;
            cameraButton.disabled = true;
            placeholder.hidden = false;
            setCallMessage('تست تجهیزات بسته شد.', 'info');
        } else if (localStream) setCallMessage('تجهیزات تأیید شدند؛ اکنون وارد جلسه شوید.', 'success');
    }

    function stopLocalMedia() {
        stopAudioMeter();
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
        fullscreenButton.disabled = true;
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
        closeMediaTest(false);
        setCallMessage('تماس پایان یافت. برای ورود مجدد جلسه را انتخاب کنید.', 'info');
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
                fullscreenButton.disabled = false;
                setCallMessage('ارتباط تصویری برقرار شد.', 'success');
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
            setCallMessage('جلسه انتخاب شد؛ می‌توانید تجهیزات را تست یا وارد جلسه شوید.', 'success');
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
        setCallMessage('در حال ایجاد اتاق و ورود به جلسه...', 'info');
        try {
            if (!await ensureMedia()) return;
            isInitiator = true;
            await connection.invoke('CreateInspectionRoom', Number(selectedMeetingId));
        } catch (error) {
            resetCallUi();
            setCallMessage('ایجاد اتاق انجام نشد؛ اتصال سرور و دسترسی جلسه را بررسی کنید.', 'danger');
            notify('ایجاد اتاق جلسه انجام نشد. دوباره تلاش کنید.', 'error');
        } finally {
            if (!roomId) setStartBusy(false);
        }
    });

    testButton.addEventListener('click', openMediaTest);
    confirmMediaButton.addEventListener('click', () => closeMediaTest(false));
    cancelMediaButton.addEventListener('click', () => closeMediaTest(true));
    cameraSelect.addEventListener('change', switchMediaDevice);
    microphoneSelect.addEventListener('change', switchMediaDevice);
    refreshDevicesButton.addEventListener('click', async () => {
        refreshDevicesButton.disabled = true;
        try { await loadMediaDevices(); }
        finally { refreshDevicesButton.disabled = false; }
    });
    fullscreenButton.addEventListener('click', async () => {
        try {
            if (document.fullscreenElement) await document.exitFullscreen();
            else await videoStage.requestFullscreen();
        } catch { notify('نمایش تمام‌صفحه در این مرورگر در دسترس نیست.', 'warning'); }
    });
    remoteVideo.addEventListener('dblclick', () => fullscreenButton.click());

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
        setCallMessage('اتاق ایجاد شد؛ در انتظار ورود طرف مقابل...', 'success');
    });
    connection.on('joined', id => {
        roomId = String(id);
        isInitiator = false;
        leaveButton.disabled = false;
        startButton.disabled = true;
        setCallMessage('وارد اتاق شدید؛ در حال برقراری ارتباط تصویری...', 'success');
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
