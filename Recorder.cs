using NAudio.Wave;

namespace audio_recorder;

class Recorder
{
    private WasapiLoopbackCapture Capture;
    private WaveFileWriter Writer;    

    private bool Paused = false;

    public void Record(string musicFilePath)
    {
        Capture = new WasapiLoopbackCapture();       
        Writer = new WaveFileWriter(musicFilePath, Capture.WaveFormat);

        Capture.DataAvailable += (object? sender, WaveInEventArgs args) =>
        {
            if (!Paused)
            {
                Writer.Write(args.Buffer, 0, args.BytesRecorded);                
            }
        };

        Capture.StartRecording();
    }

    public void Pause()
    {
        Paused = true;
    }

    public void Resume()
    {
        Paused = false;
    }

    public void StopRecording()
    {
        Capture.RecordingStopped += (object? sender, StoppedEventArgs args) =>        
        {
            Writer.Dispose();
            Writer = null;
            Capture.Dispose();
        };

        Capture.StopRecording();
    }
}