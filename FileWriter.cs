using System.IO;
using System.Windows;

namespace audio_recorder;

class FileWriter
{
    public string MusicFolderPath { get; set; }

    public FileWriter()
    {
        CreateMusicFolder();
    }

    public void CreateMusicFolder()
    {
        try
        {
            MusicFolderPath = Directory.GetCurrentDirectory() + @"\music files";

            //Create folder if doesn't exist
            if (!(Directory.Exists(MusicFolderPath)))
            {
                Directory.CreateDirectory(MusicFolderPath);                
            }
        }
        catch (System.Exception e)
        {
            MessageBox.Show(e.Message, "Problem creating music folder.");            
        }
    }

    public bool FileNameAlreadyInUse(string fileName)
    {
        string NewFilePath = MusicFolderPath + @"\" + fileName + ".wav";

        return File.Exists(NewFilePath);
    }
}