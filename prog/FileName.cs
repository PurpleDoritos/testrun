
using System.IO;

public class FileName
{
    public void StoreFile(string filePath, byte[] fileData)
    {
        File.WriteAllBytes(filePath, fileData);
    }
}
