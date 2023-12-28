// See https://aka.ms/new-console-template for more information

void ReadDataIn(string folderPath)
{
    string[] fileNames = Directory.GetFiles(folderPath);
    foreach (string fileName in fileNames) {
        using (StreamReader reader = new StreamReader(fileName))
        {
            string line = "";
            while ((line = reader.ReadLine()) != null) {
                Console.WriteLine(line);
            }
        }
    }
}

void ProcessData()
{
    
}


const string textFiles = @".\textFiles";
ReadDataIn(textFiles);