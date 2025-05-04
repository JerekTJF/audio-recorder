using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace audio_recorder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private FileWriter FileWriter = new FileWriter();
    private Recorder Recorder = new Recorder();
    private Stopwatch RecordingDuration = new Stopwatch();
    private Regex regex = new Regex("^[0-9]+$");
    private bool FileNameIsValid = false;
    private int RecordingTimeLimit = 5;

    private enum RecordStatus {Record, NoRecord, PauseRecord}    
    RecordStatus Record_Status = RecordStatus.NoRecord;

    public MainWindow()
    {
        InitializeComponent();        
        
        //need timer to update UI recording duration label
        DispatcherTimer dt = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100), DispatcherPriority.Background,
            dispatchTimer_Tick, Dispatcher.CurrentDispatcher); 
        dt.IsEnabled = true;
    }

    //Check "events" from stopwatch
    private void dispatchTimer_Tick(object? sender, EventArgs e)
    {        
        //update UI 
        Duration_Label.Content = $"{RecordingDuration.Elapsed:hh\\:mm\\:ss}";

        //need to take hour and add to minutes
        int totalMinutes = RecordingDuration.Elapsed.Minutes + (RecordingDuration.Elapsed.Hours * 60);

        //Recording Time Limit reached
        if (totalMinutes > RecordingTimeLimit)
        {
            StopRecording();
        }
    }

    public static void ShowErrorMessage(System.Exception e)
    {
        MessageBox.Show(e.Message);
    }

    //ready for recording again
    private void ResetEverything()
    {
        Record_Status = RecordStatus.NoRecord;
        RecordOrPause_Button.Content = "Record";
        FileName_TextBox.IsEnabled = true;
        FileName_TextBox.Text = "";
        RecordingDuration.Reset();
    }

    private void RecordOrPause_Click(object sender, RoutedEventArgs e)
    {
        switch (Record_Status)
        {
            case RecordStatus.NoRecord: //start record
            {
                if (FileNameIsValid)
                {
                    Record_Status = RecordStatus.Record;
                    RecordOrPause_Button.Content = "Pause";
                    FileName_TextBox.IsEnabled = false;
                    
                    //handle bad file names
                    try
                    {
                        Recorder.Record(FileWriter.MusicFolderPath + @"\" + FileName_TextBox.Text + ".wav");
                        RecordingDuration.Reset();
                        RecordingDuration.Start();
                    }
                    catch (System.Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                        ResetEverything(); 
                    }
                }             
                
                break;
            }
            case RecordStatus.Record: //pause record
            {
                Record_Status = RecordStatus.PauseRecord;
                RecordOrPause_Button.Content = "Resume";
                Recorder.Pause();     
                RecordingDuration.Stop();         
                break;
            }
            case RecordStatus.PauseRecord: //resume record
            {
                Record_Status = RecordStatus.Record;
                RecordOrPause_Button.Content = "Pause";
                Recorder.Resume();   
                RecordingDuration.Start();       
                break;
            }            
            default: break;
        }
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        if (Record_Status == RecordStatus.Record || Record_Status == RecordStatus.PauseRecord)
        {
            StopRecording();
        }
    }

    private void StopRecording()
    {
        Record_Status = RecordStatus.NoRecord;
        RecordOrPause_Button.Content = "Record";
        FileName_TextBox.IsEnabled = true;
        FileName_TextBox.Text = "";
        Recorder.StopRecording();    
        RecordingDuration.Stop();  
    }

    //check for file name validity on text change
    private void FileNameTextBox_TextChange(object sender, EventArgs e)    
    {
        //have a new file name, check if it already exists
        if (FileWriter.FileNameAlreadyInUse(FileName_TextBox.Text))
        {            
            FileAlreadyInUse_Label.Content = "File already exists.";
            FileName_TextBox.Background = Brushes.LightPink;
            FileNameIsValid = false;
        }
        else
        {            
            FileAlreadyInUse_Label.Content = "";
            FileName_TextBox.Background = Brushes.White;
            FileNameIsValid = true;
        }
    }

    //validate record time text
    private void RecordTimeLimit_LostFocus(object sender, EventArgs e)
    {   
        if (regex.IsMatch(RecordTimeLimit_Textbox.Text))
        {
            RecordingTimeLimit = int.Parse(RecordTimeLimit_Textbox.Text);
        }
        else //bad value, set default
        {
            RecordTimeLimit_Textbox.Text = "5";
            RecordingTimeLimit = 5;
        }
    }
    
    private void RecordVolume_LostFocus(object sender, EventArgs e)
    {
        //TODO
    }

    private void AppendSilence_LostFocus(object sender, EventArgs e)
    {
        //TODO
    }
}