// See https://aka.ms/new-console-template for more information

void ReadDataIn(string folderPath)
{
    string[] fileNames = Directory.GetFiles(folderPath);
    foreach (string fileName in fileNames) {
        Console.WriteLine(fileName);
    }
}

void ProcessData()
{
    
}


const string textFiles = @".\textFiles";
ReadDataIn(textFiles);