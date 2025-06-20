class AudioRecorder {
    constructor() {
        this.mediaRecorder = null;
        this.recordedChunks = [];
        this.isRecording = false;
        this.stream = null;
    }

    async initialize() {
        try {
            this.stream = await navigator.mediaDevices.getUserMedia({ audio: true });
            this.mediaRecorder = new MediaRecorder(this.stream);
            
            this.mediaRecorder.ondataavailable = (event) => {
                if (event.data.size > 0) {
                    this.recordedChunks.push(event.data);
                }
            };

            this.mediaRecorder.onstop = () => {
                this.onRecordingComplete();
            };

            return true;
        } catch (error) {
            console.error('Error accessing microphone:', error);
            alert('Please allow microphone access to record pronunciation.');
            return false;
        }
    }

    startRecording() {
        if (this.mediaRecorder && this.mediaRecorder.state === 'inactive') {
            this.recordedChunks = [];
            this.mediaRecorder.start();
            this.isRecording = true;
            
            // Update UI
            this.updateRecordingUI(true);
        }
    }

    stopRecording() {
        if (this.mediaRecorder && this.mediaRecorder.state === 'recording') {
            this.mediaRecorder.stop();
            this.isRecording = false;
            
            // Update UI
            this.updateRecordingUI(false);
        }
    }

    onRecordingComplete() {
        const blob = new Blob(this.recordedChunks, { type: 'audio/wav' });
        const audioUrl = URL.createObjectURL(blob);
        
        // Show preview
        this.showPreview(audioUrl, blob);
    }

    showPreview(audioUrl, blob) {
        const previewContainer = document.getElementById('pronunciation-preview');
        previewContainer.innerHTML = `
            <div class="audio-preview">
                <audio controls>
                    <source src="${audioUrl}" type="audio/wav">
                    Your browser does not support the audio element.
                </audio>
                <div class="mt-2">
                    <button type="button" class="btn btn-success btn-sm" onclick="uploadPronunciation()">Save Pronunciation</button>
                    <button type="button" class="btn btn-secondary btn-sm" onclick="discardRecording()">Discard</button>
                </div>
            </div>
        `;
        
        // Store blob for upload
        window.currentRecordingBlob = blob;
    }

    updateRecordingUI(isRecording) {
        const recordBtn = document.getElementById('record-btn');
        const stopBtn = document.getElementById('stop-btn');
        const recordingIndicator = document.getElementById('recording-indicator');

        if (isRecording) {
            recordBtn.style.display = 'none';
            stopBtn.style.display = 'inline-block';
            recordingIndicator.style.display = 'inline-block';
        } else {
            recordBtn.style.display = 'inline-block';
            stopBtn.style.display = 'none';
            recordingIndicator.style.display = 'none';
        }
    }

    cleanup() {
        if (this.stream) {
            this.stream.getTracks().forEach(track => track.stop());
        }
    }
}

// Global recorder instance
let audioRecorder = null;

async function initializeRecorder() {
    audioRecorder = new AudioRecorder();
    const initialized = await audioRecorder.initialize();
    
    if (initialized) {
        document.getElementById('recorder-controls').style.display = 'block';
    }
}

function startRecording() {
    if (audioRecorder) {
        audioRecorder.startRecording();
    }
}

function stopRecording() {
    if (audioRecorder) {
        audioRecorder.stopRecording();
    }
}

async function uploadPronunciation() {
    if (!window.currentRecordingBlob) {
        alert('No recording to upload');
        return;
    }

    const wordId = document.getElementById('word-id').value;
    const formData = new FormData();
    formData.append('audioFile', window.currentRecordingBlob, `pronunciation_${wordId}.wav`);
    formData.append('wordId', wordId);

    try {
        const response = await fetch('/Words/UploadPronunciation', {
            method: 'POST',
            body: formData
        });

        const result = await response.json();
        
        if (result.success) {
            // Update the pronunciation display
            updatePronunciationDisplay(result.pronunciationUrl);
            
            // Clear preview
            document.getElementById('pronunciation-preview').innerHTML = '';
            window.currentRecordingBlob = null;
            
            alert('Pronunciation saved successfully!');
        } else {
            alert('Error saving pronunciation: ' + result.message);
        }
    } catch (error) {
        console.error('Error uploading pronunciation:', error);
        alert('Error uploading pronunciation');
    }
}

function discardRecording() {
    document.getElementById('pronunciation-preview').innerHTML = '';
    window.currentRecordingBlob = null;
}

async function deletePronunciation() {
    if (!confirm('Are you sure you want to delete the pronunciation?')) {
        return;
    }

    const wordId = document.getElementById('word-id').value;

    try {
        const response = await fetch('/Words/DeletePronunciation', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ wordId: wordId })
        });

        const result = await response.json();
        
        if (result.success) {
            updatePronunciationDisplay(null);
            alert('Pronunciation deleted successfully!');
        } else {
            alert('Error deleting pronunciation: ' + result.message);
        }
    } catch (error) {
        console.error('Error deleting pronunciation:', error);
        alert('Error deleting pronunciation');
    }
}

function updatePronunciationDisplay(pronunciationUrl) {
    const container = document.getElementById('current-pronunciation');
    
    if (pronunciationUrl) {
        container.innerHTML = `
            <div class="current-audio">
                <h5>Current Pronunciation:</h5>
                <audio controls class="mb-2">
                    <source src="${pronunciationUrl}" type="audio/wav">
                    Your browser does not support the audio element.
                </audio>
                <br>
                <button type="button" class="btn btn-danger btn-sm" onclick="deletePronunciation()">Delete Pronunciation</button>
            </div>
        `;
    } else {
        container.innerHTML = '<p class="text-muted">No pronunciation recorded</p>';
    }
}

// Initialize when page loads
document.addEventListener('DOMContentLoaded', function() {
    // Auto-initialize if on word details page
    if (document.getElementById('word-id')) {
        initializeRecorder();
    }
});